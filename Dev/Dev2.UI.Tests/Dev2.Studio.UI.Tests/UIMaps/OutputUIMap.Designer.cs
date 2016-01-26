
/*
*  Warewolf - The Easy Service Bus
*  Copyright 2014 by Warewolf Ltd <alpha@warewolf.io>
*  Licensed under GNU Affero General Public License 3.0 or later. 
*  Some rights reserved.
*  Visit our website for more information <http://warewolf.io/>
*  AUTHORS <http://warewolf.io/authors.php> , CONTRIBUTORS <http://warewolf.io/contributors.php>
*  @license GNU Affero General Public License <http://www.gnu.org/licenses/agpl-3.0.html>
*/


// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by coded UI test builder.
//      Version: 11.0.0.0
//
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------

using Dev2.CodedUI.Tests;
using Microsoft.VisualStudio.TestTools.UITest.Extension;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UITesting.WpfControls;
using System.CodeDom.Compiler;

namespace Dev2.Studio.UI.Tests.UIMaps
{
    [GeneratedCode("Coded UITest Builder", "11.0.51106.1")]
    public partial class OutputUIMap
    {
        #region Properties
        public virtual NewUIMapExpectedValues NewUIMapExpectedValues
        {
            get
            {
                if ((this.mNewUIMapExpectedValues == null))
                {
                    this.mNewUIMapExpectedValues = new NewUIMapExpectedValues();
                }
                return this.mNewUIMapExpectedValues;
            }
        }

        public static UIBusinessDesignStudioWindow UIBusinessDesignStudioWindow
        {
            get
            {
                if ((mUIBusinessDesignStudioWindow == null))
                {
                    mUIBusinessDesignStudioWindow = new UIBusinessDesignStudioWindow();
                }
                return mUIBusinessDesignStudioWindow;
            }
        }
        #endregion

        #region Fields
        private NewUIMapExpectedValues mNewUIMapExpectedValues;

        private static UIBusinessDesignStudioWindow mUIBusinessDesignStudioWindow;
        #endregion
    }

    /// <summary>
    /// Parameters to be passed into 'NewUIMap'
    /// </summary>
    [GeneratedCode("Coded UITest Builder", "11.0.51106.1")]
    public class NewUIMapExpectedValues
    {

        #region Fields
        /// <summary>
        /// Verify that the 'Header' property of 'DsfActivity' -> 'DsfActivity' -> 'Assign' tree item equals 'Dev2.Studio.ViewModels.Diagnostics.DebugStateTreeViewItemViewModel'
        /// </summary>
        public string UIAssignTreeItemHeader = "Dev2.Studio.ViewModels.Diagnostics.DebugStateTreeViewItemViewModel";
        #endregion
    }

    [GeneratedCode("Coded UITest Builder", "11.0.51106.1")]
    public class UIBusinessDesignStudioWindow : WpfWindow
    {

        public UIBusinessDesignStudioWindow()
        {
            #region Search Criteria
            this.SearchProperties.Add(new PropertyExpression(WpfWindow.PropertyNames.ClassName, "HwndWrapper", PropertyExpressionOperator.Contains));
            this.SearchProperties.Add(new PropertyExpression(WpfWindow.PropertyNames.Name, "Warewolf", PropertyExpressionOperator.Contains));
            #endregion
        }

        #region Properties
        public UIDebugOutputCustom UIDebugOutputCustom
        {
            get
            {
                if ((this.mUIDebugOutputCustom == null))
                {
                    this.mUIDebugOutputCustom = new UIDebugOutputCustom(this);
                }
                return this.mUIDebugOutputCustom;
            }
        }
        #endregion

        #region Fields
        private UIDebugOutputCustom mUIDebugOutputCustom;
        #endregion
    }

    [GeneratedCode("Coded UITest Builder", "11.0.51106.1")]
    public class UIDebugOutputCustom : WpfCustom
    {

        public UIDebugOutputCustom(UITestControl searchLimitContainer) :
            base(searchLimitContainer)
        {
            #region Search Criteria
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.DebugOutputView";
            this.SearchProperties["AutomationId"] = "DebugOutput";
            this.WindowTitles.Add(TestBase.GetStudioWindowName());
            #endregion
        }

        #region Properties
        public UIItemTree UIItemTree
        {
            get
            {
                if ((this.mUIItemTree == null))
                {
                    this.mUIItemTree = new UIItemTree(this);
                }
                return this.mUIItemTree;
            }
        }


        #endregion

        #region Fields
        private UIItemTree mUIItemTree;
        #endregion
    }

    [GeneratedCode("Coded UITest Builder", "11.0.51106.1")]
    public class UIItemTree : WpfTree
    {

        public UIItemTree(UITestControl searchLimitContainer) :
            base(searchLimitContainer)
        {
            #region Search Criteria
            this.WindowTitles.Add(TestBase.GetStudioWindowName());
            #endregion
        }

        #region Properties
        public UIDsfActivityTreeItem UIDsfActivityTreeItem
        {
            get
            {
                if ((this.mUIDsfActivityTreeItem == null))
                {
                    this.mUIDsfActivityTreeItem = new UIDsfActivityTreeItem(this);
                }
                return this.mUIDsfActivityTreeItem;
            }
        }
        #endregion

        #region Fields
        private UIDsfActivityTreeItem mUIDsfActivityTreeItem;
        #endregion
    }

    [GeneratedCode("Coded UITest Builder", "11.0.51106.1")]
    public class UIDsfActivityTreeItem : WpfTreeItem
    {

        public UIDsfActivityTreeItem(UITestControl searchLimitContainer) :
            base(searchLimitContainer)
        {
            #region Search Criteria
            this.SearchProperties[WpfTreeItem.PropertyNames.AutomationId] = "DsfActivity";
            this.WindowTitles.Add(TestBase.GetStudioWindowName());
            #endregion
        }

        #region Properties
        public UIDsfActivityTreeItem1 UIDsfActivityTreeItem1
        {
            get
            {
                if ((this.mUIDsfActivityTreeItem1 == null))
                {
                    this.mUIDsfActivityTreeItem1 = new UIDsfActivityTreeItem1(this);
                }
                return this.mUIDsfActivityTreeItem1;
            }
        }
        #endregion

        #region Fields
        private UIDsfActivityTreeItem1 mUIDsfActivityTreeItem1;
        #endregion
    }

    [GeneratedCode("Coded UITest Builder", "11.0.51106.1")]
    public class UIDsfActivityTreeItem1 : WpfTreeItem
    {

        public UIDsfActivityTreeItem1(UITestControl searchLimitContainer) :
            base(searchLimitContainer)
        {
            #region Search Criteria
            this.SearchProperties[WpfTreeItem.PropertyNames.AutomationId] = "DsfActivity";
            this.SearchConfigurations.Add(SearchConfiguration.ExpandWhileSearching);
            this.SearchConfigurations.Add(SearchConfiguration.DisambiguateChild);
            this.WindowTitles.Add(TestBase.GetStudioWindowName());
            #endregion
        }

        #region Properties
        public WpfTreeItem UIAssignTreeItem
        {
            get
            {
                if ((this.mUIAssignTreeItem == null))
                {
                    this.mUIAssignTreeItem = new WpfTreeItem(this);
                    #region Search Criteria
                    this.mUIAssignTreeItem.SearchProperties[WpfTreeItem.PropertyNames.AutomationId] = "Assign";
                    this.mUIAssignTreeItem.SearchConfigurations.Add(SearchConfiguration.ExpandWhileSearching);
                    this.mUIAssignTreeItem.WindowTitles.Add(TestBase.GetStudioWindowName());
                    #endregion
                }
                return this.mUIAssignTreeItem;
            }
        }
        #endregion

        #region Fields
        private WpfTreeItem mUIAssignTreeItem;
        #endregion
    }
}