using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace TestLinkApi.Tests
{
    /// <summary>
    /// tests the API for functions on getting execution results.
    /// </summary>
    /// <remarks>
    /// See class TestBase for basic configuration.
    /// 
    /// This unit test has been marked up with a testlinkfixture attribute. 
    /// This fixture uses the Gallio Testlink Adapter to export results of this test 
    ///  fixture to Testlink. If you do not use the adapter, you can ignore this attribute.
    ///  if you DO use the adapter then you need to make sure that the parameters below are setup corectly
    ///  
    /// Please remember that this tests the API, not testlink.
    /// </remarks>
    [TestFixture]
    public class testGetLastExecutionResult : Testbase
    {
        [SetUp]
        protected void Setup()
        {
        }

        private int testPlanId = 11; // this needs to be the test plan that is currently active and has the two test cases assigned to it

        [Test]
        public void TestShouldHaveNoResults()
        {
            List<TestCaseId> testcases = proxy.GetTestCaseIDByName("TestCase with no results", "business rules");
            Assert.IsNotEmpty(testcases, "Setup failed - couldn't find test case");

            int id = testcases[0].id;

            ExecutionResult result = proxy.GetLastExecutionResult(testPlanId, id);
            //Console.WriteLine("Build {0}: status: '{1}'", result.build_id, result.status);
            Assert.IsNull(result, "Result should be null");
        }

        [Test]
        public void TestShouldHavePassedResult()
        {
            List<TestCaseId> testcases = proxy.GetTestCaseIDByName("passed test case", "business rules");
            Assert.IsNotEmpty(testcases, "Setup failed - couldn't find test case");
            int id = testcases[0].id;

            ExecutionResult result = proxy.GetLastExecutionResult(testPlanId, id);
            Assert.IsNotNull(result);
            Console.WriteLine("Build {0}: status: '{1}' tc_id:{2} tplan id {3}", result.build_id, result.status, result.tcversion_id, result.testplan_id);
            Assert.AreEqual(TestCaseStatus.Passed, result.status);
        }

        [Test]
        public void TestShouldHavePassedResult2()
        {
            List<TestCaseId> testcases = proxy.GetTestCaseIDByName("Test Case with many results", "child test suite with test cases");
            Assert.IsNotEmpty(testcases, "Setup failed - couldn't find test case");
            int id = testcases[0].id;

            ExecutionResult result = proxy.GetLastExecutionResult(testPlanId, id);
            Assert.IsNotNull(result);
            Console.WriteLine("Build {0}: status: '{1}'", result.build_id, result.status);
            Assert.AreEqual(TestCaseStatus.Passed, result.status);
        }
    }
}