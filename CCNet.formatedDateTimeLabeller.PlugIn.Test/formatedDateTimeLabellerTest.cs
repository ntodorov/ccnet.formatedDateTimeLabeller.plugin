using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThoughtWorks.CruiseControl.Core;
using System;

namespace ccnet.formatedDateTimeLabeller.plugin.Test
{
    /// <summary>
    ///This is a test class for tfsRevisionLabellerTest and is intended
    ///to contain all tfsRevisionLabellerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class formatedDateTimeLabellerTest
    {
        private TestContext testContextInstance;

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

        #region Additional test attributes

        //
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //

        #endregion Additional test attributes



        /// <summary>
        ///A test for Generate
        ///</summary>
        [TestMethod()]
        public void GenerateVersion_With_Year()
        {
            formatedDateTimeLabeller target = new formatedDateTimeLabeller(); // TODO: Initialize to an appropriate value
            IIntegrationResult integrationResult = new IntegrationResult(); // TODO: Initialize to an appropriate value

            target.YearFormat = "yyyy";
            string expected = DateTime.Now.ToString("yyyy.MM.dd.001");
            string actual = target.Generate(integrationResult);
            Assert.AreEqual(expected, actual);
            TestContext.WriteLine("Actual Build Numberb: " + actual);
        }

        /// <summary>
        ///A test for Generate
        ///</summary>
        [TestMethod()]
        public void GenerateVersion_With_MMdd()
        {
            formatedDateTimeLabeller target = new formatedDateTimeLabeller(); // TODO: Initialize to an appropriate value
            IIntegrationResult integrationResult = new IntegrationResult(); // TODO: Initialize to an appropriate value

            target.MonthFormat = "M";
            target.DayFormat = "d";
            string expected = DateTime.Now.ToString("yy.M.d.001");
            string actual = target.Generate(integrationResult);
            Assert.AreEqual(expected, actual);
            TestContext.WriteLine("Actual Build Numberb: " + actual);
        }
    }
}