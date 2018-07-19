using System;
using NUnit.Framework;

namespace TestLinkApi.Tests
{
    /// <summary>
    /// Unit Test for creation of test cases 
    /// </summary>
    /// <remarks>
    /// See class testbase for basic configuration.
    /// 
    /// This unit test has been marked up with a testlinkfixture attribute. 
    /// This fixture uses the Gallio Testlink Adapter to export results of this test 
    ///  fixture to Testlink. If you do not use the adapter, you can ignore this attribute.
    ///  if you DO use the adapter then you need to make sure that the parameters below are setup corectly
    ///  
    /// Please remember that this tests the API, not testlink.
    /// </remarks>
    [TestFixture]
    public class TestCaseCreation : Testbase
    {
        [SetUp]
        protected void Setup()
        {
            Assert.IsNotNull(AllProjects);
            platformId = Platforms[0].id;
        }

        private int platformId;

        [Test]
        [Category("Changes Database")]
        public void AddTestCaseToTestPlan()
        {
            var tcName = "externally created test case";
            var newTestResult = proxy.CreateTestCase(userName, BusinessRulesTestSuiteId,
                tcName, ApiTestProjectId,
                "This is a summary for an externally created test case",
                "auto,positive", 0, true, ActionOnDuplicatedName.GenerateNew, 2, 2);

            var prefix = ApiTestProject.prefix;
            var extid = string.Format("{0}-{1}", prefix, newTestResult.additionalInfo.external_id);
            var plan = GetTestPlan(theTestPlanName);

            var result = proxy.addTestCaseToTestPlan(ApiTestProject.id, plan.id, extid, 1, platformId);
            Assert.AreNotEqual(0, result);
        }

        [Test]
        [Category("Changes Database")]
        public void createANewVersion()
        {
            var tcName = "externally created test case";
            var result = proxy.CreateTestCase(userName, BusinessRulesTestSuiteId,
                tcName, ApiTestProjectId, "A summary",
                "auto,positive", 0, true,
                ActionOnDuplicatedName.CreateNewVersion,
                2, 2);
            Assert.AreEqual(true, result.status);
            Assert.AreEqual("Success!", result.message);
            Assert.AreEqual("createTestCase", result.operation);
//            Assert.AreEqual(tcName, result.additionalInfo.new_name);
            Assert.AreEqual(true, result.additionalInfo.status_ok);
            Assert.True(result.additionalInfo.msg.StartsWith("Created new version"));
            Assert.AreNotEqual(-1, result.additionalInfo.version_number); // that's a bug in testlink which has now a fix
            Assert.AreEqual(true, result.additionalInfo.has_duplicate);
        }

        [Test]
        public void createAUniqueTestCase()
        {
            var uniqueName = string.Format("unitTest created at {0}", DateTime.Now);

            var steps = new TestStep[3];
            steps[0] = new TestStep(1, "<p>Step 1</p>", "<p>result 1</p>", true, 1);
            steps[1] = new TestStep(2, "<p>Step 2</p>", "<p>result 2</p>", true, 1);
            steps[2] = new TestStep(3, "<p>Step 3</p>", "<p>result 3</p>", true, 1);

            var result = proxy.CreateTestCase(userName,
                BusinessRulesTestSuiteId, uniqueName, ApiTestProjectId,
                "This is a summary",
                steps,
                "auto,positive", 0, true,
                ActionOnDuplicatedName.CreateNewVersion, 2, 2);
            Assert.AreEqual(true, result.status);
            Assert.AreEqual("Success!", result.message);
            Assert.AreEqual("createTestCase", result.operation);
            Assert.AreEqual("", result.additionalInfo.new_name);
            Assert.AreEqual(true, result.additionalInfo.status_ok);
            Assert.AreEqual("ok", result.additionalInfo.msg);
            Assert.AreEqual(1, result.additionalInfo.version_number);
            Assert.AreEqual(false, result.additionalInfo.has_duplicate);
        }

        [Test]
        [Category("Changes Database")]
        public void createDuplicateNotNewVersion()
        {
            var tcName = "externally created test case";
            var result = proxy.CreateTestCase(userName, BusinessRulesTestSuiteId,
                tcName, ApiTestProjectId,
                "This is a summary for an externally created test case",
                "auto,positive", 0, true, ActionOnDuplicatedName.GenerateNew, 2, 2);
            Assert.AreEqual(true, result.status);
            Assert.AreEqual("Success!", result.message);
            Assert.AreEqual("createTestCase", result.operation);
            //Assert.AreEqual(tcName, result.additionalInfo.new_name); - no longer true the new name has a date prefixed
            Assert.AreEqual(true, result.additionalInfo.status_ok);
            Assert.True(result.additionalInfo.msg.StartsWith("Created with title"));
            Assert.AreNotEqual(-1, result.additionalInfo.id);
            Assert.AreEqual(1, result.additionalInfo.version_number);
            Assert.AreEqual(true, result.additionalInfo.has_duplicate);
        }

        // we are not testing individual data failures as we are not testing testLink but the 
        // .net interface and its error handling
        [Test]
        [Category("Changes Database")]
        public void ShouldRejectBecauseOfInvalidData()
        {
            var tcName = "externally created test case";
            Assert.Catch<TestLinkException>(() => proxy.CreateTestCase(userName, BusinessRulesTestSuiteId,
                tcName, 0,
                "This is a summary for an externally created test case",
                "auto,positive", 0, true, ActionOnDuplicatedName.GenerateNew, 2, 2));
        }

        [Test]
        [Category("Changes Database")]
        public void TestBlock()
        {
            var result = proxy.CreateTestCase(userName, BusinessRulesTestSuiteId,
                "externally created test case", ApiTestProjectId,
                "This is a summary for an externally created test case",
                "auto,positive", 0, true, ActionOnDuplicatedName.Block
                , 2, 2);
            Assert.AreEqual(true, result.status);
            Assert.AreEqual("Success!", result.message);
            Assert.AreEqual("createTestCase", result.operation);
            Assert.AreEqual("", result.additionalInfo.new_name);
            Assert.AreEqual(false, result.additionalInfo.status_ok);
            Assert.True(result.additionalInfo.msg.StartsWith("There's already a Test Case with this title"));
            Assert.AreEqual(-1, result.additionalInfo.id);
            //Assert.AreEqual(1, result.additionalInfo.version_number);  // should ignore this
            Assert.AreEqual(true, result.additionalInfo.has_duplicate);
        }

        [Test]
        [Ignore("bug")]
        [Category("Changes Database")]
        public void testExternalId()
        {
            var tcName = "externally created test case";
            var result = proxy.CreateTestCase(userName, BusinessRulesTestSuiteId,
                tcName, ApiTestProjectId,
                "This is a summary for an externally created test case",
                "auto,positive", 0, true, ActionOnDuplicatedName.GenerateNew,
                2, 2);

            var extId = string.Format("TAPI-{0}", result.additionalInfo.external_id);

            proxy.addTestCaseToTestPlan(ApiTestProjectId, PlanCalledAutomatedTesting.id, extId, 1);
            var tcList = proxy.GetTestCasesForTestPlan(PlanCalledAutomatedTesting.id);
            foreach (var tc in tcList)
            {
                Console.WriteLine("tc:{0}, feature_id:{1}, external_id:{2}, tcversion:{3}, tcversion_number:{4}",
                    tc.tc_id, tc.feature_id, tc.external_id, tc.version, tc.tcversion_id, tc.tcversion_number);
                if (tc.tc_id == result.id)
                    Assert.AreEqual(tc.external_id, extId);
            }
        }

        [Test]
        [Category("Changes Database")]
        public void VersionIncrementTest()
        {
            var tcName = "version Number test case";
            var result = proxy.CreateTestCase(userName, BusinessRulesTestSuiteId,
                tcName, ApiTestProjectId,
                "This is a summary for an externally created test case",
                "auto,positive", 0, true,
                ActionOnDuplicatedName.CreateNewVersion
                , 2, 2);


            var versionNumber = result.additionalInfo.version_number;
            Console.WriteLine("Version Number first pass: {0}", result.additionalInfo.version_number);

            var steps = new TestStep[3];
            steps[0] = new TestStep(1, "<p>Step 1</p>", "<p>result 1</p>", true, 1);
            steps[1] = new TestStep(2, "<p>Step 2</p>", "<p>result 2</p>", true, 2);
            steps[2] = new TestStep(3, "<p>Step 3</p>", "<p>result 3</p>", true, 1);

            result = proxy.CreateTestCase(userName, BusinessRulesTestSuiteId,
                tcName, ApiTestProjectId,
                "This is a different summary for an externally created test case", steps,
                "auto,positive", 0, true,
                ActionOnDuplicatedName.CreateNewVersion,
                2, 2);
            Console.WriteLine("Version Number second pass: {0}", result.additionalInfo.version_number);
            Assert.AreEqual(versionNumber + 1, result.additionalInfo.version_number, "Version number should have been incremented");
        }
    }
}