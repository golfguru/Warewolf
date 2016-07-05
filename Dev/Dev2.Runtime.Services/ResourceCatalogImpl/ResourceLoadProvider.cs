﻿using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Dev2.Common;
using Dev2.Common.Common;
using Dev2.Common.Interfaces;
using Dev2.Common.Interfaces.Core.DynamicServices;
using Dev2.Common.Interfaces.Data;
using Dev2.Common.Interfaces.Monitoring;
using Dev2.Common.Wrappers;
using Dev2.Data.ServiceModel;
using Dev2.DynamicServices;
using Dev2.DynamicServices.Objects.Base;
using Dev2.Runtime.Hosting;
using Dev2.Runtime.Interfaces;
using Dev2.Runtime.ServiceModel.Data;
using ServiceStack.Common.Extensions;
using Warewolf.Resource.Errors;

namespace Dev2.Runtime.ResourceCatalogImpl
{
    public class ResourceLoadProvider: IResourceLoadProvider

    {
        public ConcurrentDictionary<Guid, List<IResource>> WorkspaceResources { get; } = new ConcurrentDictionary<Guid, List<IResource>>();
        public ConcurrentDictionary<string, List<DynamicServiceObjectBase>> FrequentlyUsedServices { get; } = new ConcurrentDictionary<string, List<DynamicServiceObjectBase>>();
        public ConcurrentDictionary<Guid, ManagementServiceResource> ManagementServices { get; } = new ConcurrentDictionary<Guid, ManagementServiceResource>();
        public ConcurrentDictionary<Guid, object> WorkspaceLocks { get; } = new ConcurrentDictionary<Guid, object>();
        readonly object _loadLock = new object();
        readonly object _workspaceLock = new object();
        private readonly FileWrapper _dev2FileWrapper;
        readonly IPerformanceCounter _perfCounter;


        public ResourceLoadProvider(IEnumerable<DynamicService> managementServices = null)
            :this(new FileWrapper())
        {
            // MUST load management services BEFORE server workspace!!
            try
            {
                _perfCounter = CustomContainer.Get<IWarewolfPerformanceCounterLocater>().GetCounter("Count of requests for workflows which don’t exist");

            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch (Exception)
            {


            }
            if (managementServices != null)
            {
                foreach (var service in managementServices)
                {
                    var resource = new ManagementServiceResource(service);
                    ManagementServices.TryAdd(resource.ResourceID, resource);
                }
            }
            LoadFrequentlyUsedServices();
        }

        private ResourceLoadProvider(FileWrapper  fileWrapper)
        {
            _dev2FileWrapper = fileWrapper;
        }

        private void LoadFrequentlyUsedServices()
        {
            // do we really need this still - YES WE DO ELSE THERE ARE INSTALL ISSUES WHEN LOADING FROM FRESH ;)

            var serviceNames = new[]
            {
                "XXX"
            };

            foreach (var serviceName in serviceNames)
            {
                var resourceName = serviceName;
                var resource = GetResource(GlobalConstants.ServerWorkspaceID, resourceName);
                var objects = GenerateObjectGraph(resource);
                FrequentlyUsedServices.TryAdd(resourceName, objects);

            }

        }
        #region Implementation of IResourceLoadProvider

        public T GetResource<T>(Guid workspaceID, Guid serviceID) where T : Resource, new()
        {
            var resourceContents = ResourceContents<T>(workspaceID, serviceID);
            if (resourceContents == null || resourceContents.Length == 0)
            {
                var resource = GetResource(workspaceID, serviceID);
                var content = GetResourceContents(resource);
                return GetResource<T>(content);
            }
            return GetResource<T>(resourceContents);
        }

        public T GetResource<T>(Guid workspaceID, string resourceName) where T : Resource, new()
        {
            if (resourceName != null)
            {
                var resourceContents = ResourceContents<T>(workspaceID, resourceName);
                if (resourceContents == null || resourceContents.Length == 0) return null;
                return GetResource<T>(resourceContents);
            }
            return null;
        }

        public string GetResourcePath(Guid id)
        {
            return GetResource(Guid.Empty, id).ResourcePath;
        }
        public List<IResource> GetResources(Guid workspaceID)
        {
            try
            {
                var @lock = GetWorkspaceLock(workspaceID);
                lock (@lock)
                {
                    var resources = WorkspaceResources.GetOrAdd(workspaceID, LoadWorkspaceImpl);

                    return resources;
                }
            }
            catch (Exception e)
            {
                Dev2Logger.Error(ErrorResource.ErrorGettingResources, e);
                throw;
            }
        }

        public IResource GetResource(string resourceName, Guid workspaceId)
        {
            if (string.IsNullOrEmpty(resourceName))
            {
                return null;
            }
            var allResources = GetResources(workspaceId);
            IResource foundResource = null;
            if (allResources != null)
            {
                foundResource = allResources.FirstOrDefault(resource => resourceName.Equals(resource.ResourceName, StringComparison.OrdinalIgnoreCase));
                if (foundResource == null && workspaceId != Guid.Empty)
                {
                    allResources = GetResources(GlobalConstants.ServerWorkspaceID);
                    foundResource = allResources.FirstOrDefault(resource => resourceName.Equals(resource.ResourceName, StringComparison.OrdinalIgnoreCase));
                }
            }
            return foundResource;
        }

        /// <summary>
        /// Gets the contents of the resources with the given source type.
        /// </summary>
        /// <param name="workspaceID">The workspace ID to be queried.</param>
        /// <param name="sourceType">The type of the source to be queried.</param>
        /// <returns>The resource's contents or <code>string.Empty</code> if not found.</returns>
        public IEnumerable GetModels(Guid workspaceID, enSourceType sourceType)
        {
            var workspaceResources = GetResources(workspaceID);
            var resources = workspaceResources.FindAll(r => r.ResourceType == sourceType.ToString());
            if (sourceType == enSourceType.MySqlDatabase || sourceType == enSourceType.Oracle || sourceType == enSourceType.PostgreSql || sourceType == enSourceType.SqlDatabase)
            {
                resources = workspaceResources.FindAll(r => r.ResourceType.ToUpper() == "DbSource".ToUpper());
            }
            Dictionary<enSourceType, Func<IEnumerable>> commands = new Dictionary<enSourceType, Func<IEnumerable>>
            {
                { enSourceType.Dev2Server, ()=>BuildSourceList<Connection>(resources) },
                { enSourceType.EmailSource, ()=>BuildSourceList<EmailSource>(resources) },
                { enSourceType.SqlDatabase, ()=>BuildSourceList<DbSource>(resources) },
                { enSourceType.MySqlDatabase, ()=>BuildSourceList<DbSource>(resources) },
                { enSourceType.PostgreSql, ()=>BuildSourceList<DbSource>(resources) },
                { enSourceType.Oracle, ()=>BuildSourceList<DbSource>(resources) },
                { enSourceType.PluginSource, ()=>BuildSourceList<PluginSource>(resources) },
                { enSourceType.WebSource, ()=>BuildSourceList<WebSource>(resources) },
                { enSourceType.OauthSource, ()=>BuildSourceList<DropBoxSource>(resources) },
                { enSourceType.SharepointServerSource, ()=>BuildSourceList<SharepointSource>(resources) },
                { enSourceType.ExchangeSource, ()=>BuildSourceList<ExchangeSource>(resources) }
            };

            var result = commands.ContainsKey(sourceType) ? commands[sourceType].Invoke() : null;
            return result;
        }

        public List<TServiceType> GetDynamicObjects<TServiceType>(Guid workspaceID, Guid resourceID) where TServiceType : DynamicServiceObjectBase
        {
            if (resourceID == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(resourceID));
            }

            var resource = GetResource(workspaceID, resourceID);
            var results = resource == null ? new List<DynamicServiceObjectBase>() : GetDynamicObjects(resource);
            return results.OfType<TServiceType>().ToList();
        }

        public IList<IResource> GetResourceList<T>(Guid workspaceId) where T : Resource, new()
        {
            var workspaceResources = GetResources(workspaceId);
            var resourcesMatchingType = workspaceResources.Where(resource => typeof(T) == resource.GetType());
            return resourcesMatchingType.ToList();
        }


        public List<TServiceType> GetDynamicObjects<TServiceType>(Guid workspaceID, string resourceName, bool useContains = false) where TServiceType : DynamicServiceObjectBase
        {
            if (string.IsNullOrEmpty(resourceName))
            {
                throw new ArgumentNullException(nameof(resourceName));
            }

            List<DynamicServiceObjectBase> results;

            if (useContains)
            {
                var resources = GetResources(workspaceID);
                results = GetDynamicObjects(resources.Where(r => r.ResourceName.Contains(resourceName)));
            }
            else
            {
                var resource = GetResource(workspaceID, resourceName);
                results = resource == null ? new List<DynamicServiceObjectBase>() : GetDynamicObjects(resource);
            }
            return results.OfType<TServiceType>().ToList();
        }

        public List<DynamicServiceObjectBase> GetDynamicObjects(IResource resource)
        {
            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            var result = new List<DynamicServiceObjectBase>();
            AddResourceAsDynamicServiceObject(result, resource);
            return result;
        }

        public List<DynamicServiceObjectBase> GetDynamicObjects(IEnumerable<IResource> resources)
        {
            if (resources == null)
            {
                throw new ArgumentNullException(nameof(resources));
            }

            var result = new List<DynamicServiceObjectBase>();
            foreach (var resource in resources)
            {
                AddResourceAsDynamicServiceObject(result, resource);
            }
            return result;
        }

        public List<Guid> GetDependants(Guid workspaceID, Guid? resourceId)
        {
            if (resourceId == null) throw new ArgumentNullException(nameof(resourceId), ErrorResource.NoResourceName);

            var resources = GetResources(workspaceID);
            var dependants = new List<Guid>();
            resources.ForEach(resource =>
            {
                if (resource.Dependencies == null) return;
                resource.Dependencies.ForEach(tree =>
                {
                    if (tree.ResourceID == resourceId)
                    {
                        dependants.Add(resource.ResourceID);
                    }
                });
            });
            return dependants.ToList();
        }

        public List<ResourceForTree> GetDependentsAsResourceForTrees(Guid workspaceID, Guid resourceId)
        {
            var resources = GetResources(workspaceID);
            var dependants = new List<ResourceForTree>();
            foreach (var resource in resources)
            {
                resource.Dependencies?.ForEach(tree =>
                {
                    if (tree.ResourceID == resourceId)
                    {
                        dependants.Add(CreateResourceForTree(resource, tree));
                    }
                });
            }
            return dependants.ToList();
        }

        public IList<IResource> GetResourceList(Guid workspaceId)
        {
            var workspaceResources = GetResources(workspaceId);


            return workspaceResources.ToList();
        }
      
        public IList<Resource> GetResourceList(Guid workspaceId, Dictionary<string, string> filterParams)
        {
            string resourceName;
            filterParams.TryGetValue("resourceName", out resourceName);
            string type;
            filterParams.TryGetValue("type", out type);
            string guidCsv;
            filterParams.TryGetValue("guidCsv", out guidCsv);
            var workspaceResources = GetResources(workspaceId);

            if (!string.IsNullOrEmpty(guidCsv) || filterParams.ContainsKey(nameof(guidCsv)))
            {
                if (guidCsv == null)
                {
                    guidCsv = string.Empty;
                }
                var guids = SplitGuidsByComma(guidCsv);
                var resources = GetResourcesBasedOnType(type, workspaceResources, r => guids.Contains(r.ResourceID));
                return resources.Cast<Resource>().ToList();
            }
            else
            {
                if (string.IsNullOrEmpty(resourceName) && string.IsNullOrEmpty(type))
                {
                    throw new InvalidDataContractException(ErrorResource.ResourceNameAndTypeMissing);
                }
                if (string.IsNullOrEmpty(resourceName) || resourceName == "*")
                {
                    resourceName = string.Empty;
                }
                var resources = GetResourcesBasedOnType(type, workspaceResources, r => r.ResourcePath.Contains(resourceName));
                return resources.Cast<Resource>().ToList();

            }
        }

        public int GetResourceCount(Guid workspaceID)
        {
            return GetResources(workspaceID).Count;
        }

        public void RemoveWorkspace(Guid workspaceID)
        {
            object @lock;
            lock (_loadLock)
            {
                if (!WorkspaceLocks.TryRemove(workspaceID, out @lock))
                {
                    @lock = new object();
                }
            }
            lock (@lock)
            {
                List<IResource> resources;
                WorkspaceResources.TryRemove(workspaceID, out resources);
            }
        }

        public IResource GetResource(Guid workspaceID, string resourceName, string resourceType = "Unknown", string version = null)
        {
            while (true)
            {
                if (string.IsNullOrEmpty(resourceName))
                {
                    throw new ArgumentNullException(nameof(resourceName));
                }
                var resourceNameToSearchFor = resourceName.Replace("/", "\\");
                var resourcePath = resourceNameToSearchFor;
                var endOfResourcePath = resourceNameToSearchFor.LastIndexOf('\\');
                if (endOfResourcePath >= 0)
                {
                    resourceNameToSearchFor = resourceNameToSearchFor.Substring(endOfResourcePath + 1);
                }
                var resources = GetResources(workspaceID);
                if (resources != null)
                {
                    var foundResource = resources.FirstOrDefault(r =>
                    {
                        if (r == null)
                        {
                            return false;
                        }
                        return string.Equals(r.ResourcePath ?? "", resourcePath, StringComparison.InvariantCultureIgnoreCase) && string.Equals(r.ResourceName, resourceNameToSearchFor, StringComparison.InvariantCultureIgnoreCase) && (resourceType == "Unknown" || r.ResourceType == resourceType.ToString());
                    });
                    if (foundResource == null && workspaceID != GlobalConstants.ServerWorkspaceID)
                    {
                        workspaceID = GlobalConstants.ServerWorkspaceID;
                        continue;
                    }
                    return foundResource;
                }
            }
        }

        public IResource GetResource(Guid workspaceID, Guid resourceID)
        {
            IResource foundResource = null;
            try
            {
                lock (_workspaceLock)
                {
                    List<IResource> resources;
                    if (WorkspaceResources.TryGetValue(workspaceID, out resources))
                    {
                        foundResource = resources.FirstOrDefault(resource => resource.ResourceID == resourceID);
                    }

                    if (foundResource == null && workspaceID != GlobalConstants.ServerWorkspaceID)
                    {
                        if (WorkspaceResources.TryGetValue(GlobalConstants.ServerWorkspaceID, out resources))
                        {
                            foundResource = resources.FirstOrDefault(resource => resource.ResourceID == resourceID);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Dev2Logger.Error(ErrorResource.ErrorGettingResources, e);
            }
            if (foundResource == null)
            {
                _perfCounter?.Increment();
            }
            return foundResource;
        }

        public StringBuilder GetResourceContents(Guid workspaceID, Guid resourceID)
        {
            IResource foundResource = null;

            lock (_workspaceLock)
            {
                List<IResource> resources;
                if (WorkspaceResources.TryGetValue(workspaceID, out resources))
                {
                    foundResource = resources.FirstOrDefault(resource => resource.ResourceID == resourceID);
                }

                if (foundResource == null && workspaceID != GlobalConstants.ServerWorkspaceID)
                {
                    if (WorkspaceResources.TryGetValue(GlobalConstants.ServerWorkspaceID, out resources))
                    {
                        foundResource = resources.FirstOrDefault(resource => resource.ResourceID == resourceID);
                    }
                }
            }
            return GetResourceContents(foundResource);
        }

        public StringBuilder GetResourceContents(IResource resource)
        {
            StringBuilder contents = new StringBuilder();

            if (string.IsNullOrEmpty(resource?.FilePath) || !_dev2FileWrapper.Exists(resource.FilePath))
            {
                return contents;
            }

            // Open the file with the file share option of read. This will ensure that if the file is opened for write while this read operation
            // is happening the wite will fail.
            using (FileStream fs = new FileStream(resource.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    while (!sr.EndOfStream)
                    {
                        var readLine = sr.ReadLine();
                        if (!string.IsNullOrEmpty(readLine))
                        {
                            contents.Append(readLine);
                            contents.Append(Environment.NewLine);
                        }
                    }
                }

            }
            return contents;
        }

        #endregion

        #region Private methods
        private StringBuilder ResourceContents<T>(Guid workspaceID, string resourceName) where T : Resource, new()
        {
            var resource = GetResource(workspaceID, resourceName);
            var resourceContents = GetResourceContents(resource);
            return CheckType<T>(resource) ? resourceContents : null;
        }

        private StringBuilder ResourceContents<T>(Guid workspaceID, Guid resourceID) where T : Resource, new()
        {
            var resource = GetResource(workspaceID, resourceID);
            var resourceContents = GetResourceContents(resource);
            return CheckType<T>(resource) ? resourceContents : null;
        }

        private static bool CheckType<T>(IResource resource) where T : Resource, new()
        {
            if (resource != null)
            {
                //This is for migration from pre-v1 to V1. Remove once V1 is released.
                if (typeof(T) == typeof(DbService) && resource.ResourceType == "DbService")
                {
                    return true;
                }
                if (typeof(T) == typeof(PluginService) && resource.ResourceType == "PluginService")
                {
                    return true;
                }
                if (typeof(T) == typeof(WebService) && resource.ResourceType == "WebService")
                {
                    return true;
                }

                if (typeof(T) == typeof(Workflow) && resource.IsService)
                {
                    return true;
                }
                if (typeof(IResourceSource).IsAssignableFrom(typeof(T)) && resource.IsSource)
                {
                    return true;
                }
            }
            return false;
        }

        private static T GetResource<T>(StringBuilder resourceContents) where T : Resource, new()
        {
            var elm = resourceContents.ToXElement();
            object[] args = { elm };
            return (T)Activator.CreateInstance(typeof(T), args);
        }

        private void AddResourceAsDynamicServiceObject(List<DynamicServiceObjectBase> result, IResource resource)
        {
            if (resource.ResourceType == "ReservedService")
            {
                var managementResource = resource as ManagementServiceResource;
                if (managementResource != null)
                {
                    result.Add(managementResource.Service);
                }
            }
            else
            {
                List<DynamicServiceObjectBase> objects;
                if (!FrequentlyUsedServices.TryGetValue(resource.ResourceName, out objects))
                {
                    objects = GenerateObjectGraph(resource);
                    //_frequentlyUsedServices.TryAdd(resource.ResourceName, objects);
                }
                else
                {
                    Dev2Logger.Debug($"{resource.ResourceName} -> Resource Catalog Cache HIT");
                }
                if (objects != null)
                {
                    result.AddRange(objects);
                }
            }
        }
        private List<Guid> SplitGuidsByComma(string guidCsv)
        {
            var guids = new List<Guid>();
            var guidStrs = guidCsv.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var guidStr in guidStrs)
            {
                Guid guid;
                if (Guid.TryParse(guidStr, out guid))
                {
                    guids.Add(guid);
                }
            }
            return guids;
        }
        private static IEnumerable<IResource> GetResourcesBasedOnType(string type, List<IResource> workspaceResources, Func<IResource, bool> func)
        {
            List<IResource> resources;
            if (string.IsNullOrEmpty(type))
            {
                resources = workspaceResources.FindAll(func.Invoke);
            }
            else
            {
                Dictionary<string, List<IResource>> commands = new Dictionary<string, List<IResource>>()
                {
                    {"WorkflowService", workspaceResources.FindAll(r => func.Invoke(r) && r.IsService)},
                    {"Source", workspaceResources.FindAll(r => func.Invoke(r) && r.IsSource)},
                    {"ReservedService", workspaceResources.FindAll(r => func.Invoke(r) && r.IsReservedService)},
                };

                resources = commands.ContainsKey(type) ? commands[type] : workspaceResources.FindAll(func.Invoke);
            }
            return resources;
        }
        private List<IResource> LoadWorkspaceImpl(Guid workspaceID)
        {
            var workspacePath = workspaceID == GlobalConstants.ServerWorkspaceID ? EnvironmentVariables.ResourcePath : EnvironmentVariables.GetWorkspacePath(workspaceID);
            IList<IResource> userServices = new List<IResource>();
            if (Directory.Exists(workspacePath))
            {
                var folders = Directory.EnumerateDirectories(workspacePath, "*", SearchOption.AllDirectories);
                var allFolders = folders.ToList();
                allFolders.Add(workspacePath);
                userServices = LoadWorkspaceViaBuilder(workspacePath, allFolders.ToArray());
            }
            var result = userServices.Union(ManagementServices.Values);
            var resources = result.ToList();

            return resources;
        }

        public IList<IResource> LoadWorkspaceViaBuilder(string workspacePath, params string[] folders)
        {
            ResourceCatalogBuilder builder = new ResourceCatalogBuilder();

            builder.BuildCatalogFromWorkspace(workspacePath, folders);


            var resources = builder.ResourceList;
            return resources;
        }
        private object GetWorkspaceLock(Guid workspaceID)
        {
            lock (_loadLock)
            {
                return WorkspaceLocks.GetOrAdd(workspaceID, guid => new object());
            }
        }
        private static ResourceForTree CreateResourceForTree(IResource resource, IResourceForTree tree)
        {
            return new ResourceForTree
            {
                UniqueID = tree.UniqueID,
                ResourceID = resource.ResourceID,
                ResourceName = resource.ResourceName,
                ResourceType = resource.ResourceType
            };
        }

        private List<DynamicServiceObjectBase> GenerateObjectGraph(IResource resource)
        {
            var xml = GetResourceContents(resource);
            if (xml == null || xml.Length > 0)
            {
                return new ServiceDefinitionLoader().GenerateServiceGraph(xml);
            }

            return null;
        }
        private IEnumerable BuildSourceList<T>(IEnumerable<IResource> resources) where T : Resource, new()
        {
            var objects = resources.Select(r => GetResource<T>(ToPayload(r))).ToList();
            return objects;
        }
        public StringBuilder ToPayload(IResource resource)
        {
            var result = new StringBuilder();

            if (resource.ResourceType == "ReservedService")
            {
                result.AppendFormat("<Service Name=\"{0}\" ResourceType=\"{1}\" />", resource.ResourceName, resource.ResourceType);
            }
            else
            {
                var contents = GetResourceContents(resource);
                if (contents != null)
                {
                    contents = contents.Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>", "");
                    result.Append(contents);
                }
            }

            return result;
        }
        #endregion

    }
}