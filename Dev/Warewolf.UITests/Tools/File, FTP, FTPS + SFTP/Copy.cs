﻿using System;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Warewolf.UITests.Tools
{
    [CodedUITest]
    public class Copy
    {
        [TestMethod]
        public void CopyToolUITest()
        {
            //Scenario: Drag toolbox Copy Path onto a new workflow should create small view on the design surface
            Uimap.Drag_Toolbox_Copy_Onto_DesignSurface();
            Uimap.Assert_Copy_Exists_OnDesignSurface();

            //@NeedsCopy_PathToolSmallViewOnTheDesignSurface
            // Scenario: Double Clicking Copy_Path Tool Small View on the Design Surface Opens Large View
            Uimap.Open_Copy_Tool_Large_View();
            Uimap.Assert_Copy_Large_View_Exists_OnDesignSurface();
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