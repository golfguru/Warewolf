﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by coded UI test builder.
//      Version: 11.0.0.0
//
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------

using Dev2.CodedUI.Tests;

namespace Dev2.Studio.UI.Tests.UIMaps.NewServerUIMapClasses
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Text.RegularExpressions;
    using System.Windows.Input;
    using Microsoft.VisualStudio.TestTools.UITest.Extension;
    using Microsoft.VisualStudio.TestTools.UITesting;
    using Microsoft.VisualStudio.TestTools.UITesting.WpfControls;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Keyboard = Microsoft.VisualStudio.TestTools.UITesting.Keyboard;
    using Mouse = Microsoft.VisualStudio.TestTools.UITesting.Mouse;
    using MouseButtons = System.Windows.Forms.MouseButtons;
    
    
    [GeneratedCode("Coded UITest Builder", "11.0.51106.1")]
    public partial class NewServerUIMap
    {
        
        /// <summary>
        /// AssertMethod1
        /// </summary>
        public WpfWindow GetNewServerWindow()
        {
            WpfWindow uINewServerWindow = this.UINewServerWindow;
            //uINewServerWindow.Find();
            return uINewServerWindow;
        }

        #region Properties
        public UINewServerWindow UINewServerWindow
        {
            get
            {
                if ((this.mUINewServerWindow == null))
                {
                    this.mUINewServerWindow = new UINewServerWindow();
                }
                return this.mUINewServerWindow;
            }
        }

        public UIBusinessDesignStudioWindow UIBusinessDesignStudioWindow
        {
            get
            {
                if ((this.mUIBusinessDesignStudioWindow == null))
                {
                    this.mUIBusinessDesignStudioWindow = new UIBusinessDesignStudioWindow();
                }
                return this.mUIBusinessDesignStudioWindow;
            }
        }

        #endregion
        
        #region Fields
        private UIBusinessDesignStudioWindow mUIBusinessDesignStudioWindow;
        private UINewServerWindow mUINewServerWindow;
        #endregion
    }
    
    [GeneratedCode("Coded UITest Builder", "11.0.51106.1")]
    public class UINewServerWindow : WpfWindow
    {
        
        public UINewServerWindow()
        {
            #region Search Criteria
            this.SearchProperties[WpfWindow.PropertyNames.Name] = "New Server";
            this.SearchProperties.Add(new PropertyExpression(WpfWindow.PropertyNames.ClassName, "HwndWrapper", PropertyExpressionOperator.Contains));
            this.WindowTitles.Add("New Server");
            #endregion
        }
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
    }
}
