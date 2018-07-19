using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace TestLinkApi.Tests
{
    /// <summary>
    /// test test case related functions.
    /// Excludes test case creation which has its own test fixture
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
    public class TestCaseTests : Testbase
    {
        [SetUp]
        protected void Setup()
        {
            Setup();
            Assert.IsNotNull(AllProjects);
        }

        private int tcId = 16;
        private int buildId = 4;
        private int kwId = 1;

        private int assignedTo = 1;

        [Test]
        public void DealWithDuplicateTestCaseNames()
        {
            var idList = proxy.GetTestCaseIDByName("duplicate test case");
            Assert.AreEqual(2, idList.Count, "there should be two test cases");
            Console.WriteLine("Business Rules Test Suite Id = {0}", BusinessRulesTestSuiteId);

            foreach (var id in idList)
            {
                Console.WriteLine("TC-id {0}, parent Id = {1}", id.tc_external_id, id.parent_id);
            }
        }

        [Test]
        public void GetNonExistentTc()
        {
            var idList = proxy.GetTestCaseIDByName("Does Not Exist");
            Assert.IsEmpty(idList);
        }

        /// <summary>
        /// prerequisite: a test case called 'TestCase with Attachments' that has two attachments
        /// </summary>
        [Test]
        public void getTestcaseAttachments()
        {
            var list = proxy.GetTestCaseIDByName("TestCase with Attachments");

            var result = proxy.GetTestCaseAttachments(list[0].id);
            foreach (var a in result)
                Console.WriteLine("{0}:'{1}'. File Type:{2}. date added:{3}", a.id, a.name, a.file_type, a.date_added);
            Assert.AreEqual(2, result.Count, "Expected two attachments for test case called 'TestCase with Attachments'");
        }

        [Test]
        public void getTestCaseById()
        {
            var tcid = TestCaseIdWithVersions;
            TestCase tc = proxy.GetTestCase(tcid);
            Assert.IsNotNull(tc, "should have a test case");
            Console.WriteLine("Found {0}:{1} Version:{2}", tc.id, tc.name, tc.version);
        }

        [Test]
        public void getTestCaseById_nonexistingId()
        {
            var tcid = -12;
            Assert.Catch<TestLinkException>(() => proxy.GetTestCase(tcid));
        }

        [Test]
        public void getTestcaseByName_noResult()
        {
            var testcases = proxy.GetTestCaseIDByName("xxxxxxxxxx", BusinessRulesTestSuiteId);
            Assert.IsEmpty(testcases, "there should be a no test case named 'xxxxxxxxxx' in the business rules test suite");
        }

        [Test]
        public void getTestcaseByName1()
        {
            var testcases = proxy.GetTestCaseIDByName("TestCase with no results", "business rules");
            Assert.IsNotEmpty(testcases, "there should be a test case named 'TestCase with no results' in the business rules test suite");
        }

        [Test]
        public void getTestcaseByName2()
        {
            var testcases = proxy.GetTestCaseIDByName("TestCase with no results", BusinessRulesTestSuiteId);
            Assert.IsNotEmpty(testcases, "there should be a test case named 'TestCase with no results' in the business rules test suite");
        }

        [Test]
        public void getTestCaseIdsForTestSuite_Deep()
        {
            var plan = GetTestPlan(theTestPlanName);

            List<TestSuite> list = proxy.GetTestSuitesForTestPlan(plan.id);

            var items = proxy.GetTestCaseIdsForTestSuite(list[0].id, true);
            Assert.IsNotEmpty(items);
        }

        [Test]
        public void getTestCaseIdsForTestSuite_Shallow()
        {
            var plan = GetTestPlan(theTestPlanName);

            List<TestSuite> list = proxy.GetTestSuitesForTestPlan(plan.id);

            var items = proxy.GetTestCaseIdsForTestSuite(list[0].id, false);
            Assert.IsNotEmpty(items);
        }

        [Test]
        public void GetTestCasesForTestPlan()
        {
            var idList = proxy.GetTestCasesForTestPlan(PlanCalledAutomatedTesting.id);


            foreach (var tc in idList)
            {
                Console.Write("TC-id {0} :'{1}. testsuiteId='{2}' ", tc.tc_id, tc.name, tc.testsuite_id);
                Console.WriteLine("Platform: {0}, Status:{1}", tc.platform_name, tc.status);
            }

            Assert.IsTrue(idList.Count > 0, "there should be multiple test cases");
        }

        [Test]
        public void GetTestCasesForTestPlan_tc()
        {
            var idList = proxy.GetTestCasesForTestPlan(PlanCalledAutomatedTesting.id, 16);


            foreach (var tc in idList)
            {
                Console.Write("TC-id {0} :'{1}' ", tc.tc_id, tc.name);
                Console.WriteLine("Platform: {0}, Status:{1}", tc.platform_name, tc.status);
            }

            Assert.IsTrue(idList.Count > 0, "there should be multiple test cases");
        }

        [Test]
        public void GetTestCasesForTestPlan_tc_build()
        {
            var idList = proxy.GetTestCasesForTestPlan(PlanCalledAutomatedTesting.id, tcId, buildId);

            foreach (var tc in idList)
            {
                Console.Write("TC-id {0} :'{1}' ", tc.tc_id, tc.name);
                Console.WriteLine("Platform: {0}, Status:{1}", tc.platform_name, tc.status);
            }

            Assert.IsTrue(idList.Count > 0, "there should be multiple test cases");
        }

        [Test]
        public void GetTestCasesForTestPlan_tc_build_executedTrue()
        {
            var idList = proxy.GetTestCasesForTestPlan(PlanCalledAutomatedTesting.id, tcId, buildId, kwId, true);

            foreach (var tc in idList)
            {
                Console.WriteLine("TC-id {0} :'{1}'", tc.tc_id, tc.name);
                Console.WriteLine("Platform: {0}, Status:{1}", tc.platform_name, tc.status);
            }

            Assert.IsTrue(idList.Count == 0, "there should be no test cases");
        }

        [Test]
        public void GetTestCasesForTestPlan_tc_build_kw_executed_assignedTo_status_notexecuted()
        {
            var idList = proxy.GetTestCasesForTestPlan(PlanCalledAutomatedTesting.id, tcId, buildId,
                kwId, true, assignedTo, "p");

            foreach (var tc in idList)
            {
                Console.WriteLine("TC-id {0} :'{1}' Version {2}", tc.tc_id, tc.name, tc.version);
                Console.WriteLine("Platform: {0}, Status:{1}", tc.platform_name, tc.status);
            }

            Assert.IsTrue(idList.Count == 0, "there should be no test cases");
        }

        [Test]
        public void GetTestCasesForTestPlan_tc_build_kw_executed_assignedToWrongId()
        {
            var idList = proxy.GetTestCasesForTestPlan(PlanCalledAutomatedTesting.id, tcId, buildId, kwId, true, assignedTo);

            foreach (var tc in idList)
            {
                Console.WriteLine("TC-id {0} :'{1}'", tc.tc_id, tc.name);
                Console.WriteLine("Platform: {0}, Status:{1}", tc.platform_name, tc.status);
            }

            Assert.IsTrue(idList.Count == 0, "there should be no test cases");
        }

        [Test]
        public void GetTestCasesForTestPlan_tc_build_kw_nonexistent()
        {
            var idList = proxy.GetTestCasesForTestPlan(PlanCalledAutomatedTesting.id, tcId, buildId, kwId);

            foreach (var tc in idList)
            {
                Console.WriteLine("TC-id {0} :'{1}'", tc.tc_id, tc.name);
                Console.WriteLine("Platform: {0}, Status:{1}", tc.platform_name, tc.status);
            }

            Assert.AreEqual(0, idList.Count, "There should be no test cases");
        }

        [Test]
        public void getTestCasesForTestSuite_Deep()
        {
            var plan = GetTestPlan(theTestPlanName);

            List<TestSuite> list = proxy.GetTestSuitesForTestPlan(plan.id);
            Assert.IsNotEmpty(list);
            List<TestCaseFromTestSuite> items = proxy.GetTestCasesForTestSuite(list[0].id, true);
            Assert.IsNotEmpty(items);
        }

        [Test]
        public void getTestCasesForTestSuite_Shallow()
        {
            var plan = GetTestPlan(theTestPlanName);

            List<TestSuite> list = proxy.GetTestSuitesForTestPlan(plan.id);

            List<TestCaseFromTestSuite> items = proxy.GetTestCasesForTestSuite(list[0].id, false);
            Assert.IsNotEmpty(items);
            foreach (TestCaseFromTestSuite item in items)
            {
                Console.WriteLine("id: {0}", item.id);
                Console.WriteLine("Name          : {0}", item.name);
                Console.WriteLine("Author Id     : {0}", item.author_id);
                Console.WriteLine("Created       : {0}", item.creation_ts);
                Console.WriteLine("Status        : {0}", item.status);
                Console.WriteLine("Test Case Version: {0}", item.tcversion_id);
                Console.WriteLine("Test Suite Id : {0}", item.testSuite_id);
                Console.WriteLine("Version       : {0}", item.version);
                Console.WriteLine("Summary       : {0}", item.summary);
                Console.WriteLine("Details       : {0}", item.details);
                Console.WriteLine("Preconditions : {0}", item.preconditions);
                Console.WriteLine("Update Id     : {0}", item.updater_id);
                Console.WriteLine("Modified on   : {0}", item.modification_ts);
                Console.WriteLine("Active        : {0}", item.active);
                Console.WriteLine("Is Open       : {0}", item.is_open);
                Console.WriteLine("Execution Type: {0}", item.execution_type);
                Console.WriteLine("External Id   : {0}", item.external_id);
                Console.WriteLine("Importance    : {0}", item.importance);
                Console.WriteLine("Layout        : {0}", item.layout);
                Console.WriteLine("Node Order    : {0}", item.node_order);
                Console.WriteLine("Node table    : {0}", item.node_table);
                Console.WriteLine("Node type id  : {0}", item.node_type_id);
                Console.WriteLine("====================================");
            }
        }

        /// <summary>
        /// pre-requisite:
        /// a test case named 'Testcase with Steps'
        /// it must have 4 steps
        /// steps 1,2 and 4 are manual
        /// step 3 is automatic.
        /// </summary>
        [Test]
        public void getTestCaseWithSteps()
        {
            var tcId = TestCaseIdWithSteps;

            TestCase tc = proxy.GetTestCase(tcId);
            Assert.IsNotNull(tc);
            Assert.AreEqual(4, tc.steps.Count, " there should be 4 steps in tc named {0}. Check the Testlink DB", tc.name);
            for (var idx = 0; idx < 4; idx++)
            {
                TestStep ts = tc.steps[idx];
                Assert.AreEqual(idx + 1, ts.step_number);
                if (idx == 2)
                    Assert.AreEqual(2, ts.execution_type);
                else
                    Assert.AreEqual(1, ts.execution_type);
                Assert.IsNotEmpty(ts.actions);
                Assert.IsNotEmpty(ts.expected_results);
            }
        }

        [Test]
        public void TestCaseAttachmentCreation()
        {
            var tcid = TestCaseToUseWithAttachments;
            TestCase tc = proxy.GetTestCase(tcid);

            var content = new byte[4];
            content[0] = 48;
            content[1] = 49;
            content[2] = 50;
            content[3] = 51;

            var r = proxy.UploadTestCaseAttachment(tcid, "fileX.txt", "text/plain", content, "some result", "a description");
            Assert.AreEqual(r.foreignKeyId, tcid);
            Console.WriteLine("Upload Response id:{0}, table '{1}', title:'{2}' size:{3}", r.foreignKeyId, r.linkedTableName, r.title, r.size);
        }
    }
}