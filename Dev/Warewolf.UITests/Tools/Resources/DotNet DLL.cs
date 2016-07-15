﻿using System;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Warewolf.UITests.Tools.Resources
{
    [CodedUITest]
    public class DotNet_DLL
    {
        [TestMethod]
        public void DotNetDLLToolUITest()
        {
            //Scenario: Drag toolbox DotNet Dll Tool onto a new workflow creates base conversion tool with small view on the design surface
            Uimap.Drag_DotNet_DLL_Connector_Onto_DesignSurface();
            Uimap.Assert_DotNet_DLL_Connector_Exists_OnDesignSurface();

            //@NeedsDotNetDllToolLargeViewOnTheDesignSurface
            // Scenario: Double Clicking DotNet Dll Tool Large View on the Design Surface Collapses it to Small View
            Uimap.Open_DotNet_DLL_Connector_Tool_Small_View();
            Uimap.Assert_DotNet_DLL_Connector_Exists_OnDesignSurface();
        }

        #region Additional test attributes

        [TestInitialize]
        public void MyTestInitialize()
        {
            Uimap.SetGlobalPlaybackSettings();
            Uimap.WaitIfStudioDoesNotExist();
            Console.WriteLine("Test \"" + TestContext.TestName + "\" starting on " + System.Environment.MachineName);
            Uimap.InitializeABlankWorkflow();
        }

        [TestCleanup]
        public void MyTestCleanup()
        {
            Uimap.CleanupWorkflow();
        }

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        private TestContext testContextInstance;

        UIMap Uimap
        {
            get
            {
                if ((_uiMap == null))
                {
                    _uiMap = new UIMap();
                }

                return _uiMap;
            }
        }

        private UIMap _uiMap;

        #endregion
    }
}