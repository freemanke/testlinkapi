using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace TestLinkApi.Tests
{
    public class Testbase
    {
        protected const string apiKey = "061b7297baa1098156a177e94f4e10b8";
        protected const string testlinkUrl = "https://localhost:63002/lib/api/xmlrpc/v1/xmlrpc.php";

        protected const string userName = "admin";
        protected const string testProjectName = "apiSandbox";
        protected const string emptyProjectName = "Empty TestProject"; // this test project needs to be empty

        /// <summary>
        /// the test plan used for most automated testing. they are all to be setup under the project named apiSandbox
        /// This test plan should have 2 platform: OS/X and Windows 95
        /// </summary>
        protected const string theTestPlanName = "Automated Testing"; // need to have one plan of this name

        protected const string platformName1 = "OS/X";
        protected const string platformName2 = "Windows 95";

        /// <summary>
        /// this test plan should have neither test cases nor test platforms associated no builds
        /// </summary>
        protected const string emptyTestplanName = "Empty Test plan";


        protected const string testSuiteName2 = "Function Requirements";
        protected const string testSuiteName1 = "business rules";
        protected const string emptyTestSuiteName = "Empty Test Suite";
        protected const string subTestSuiteName1 = "child test suite with test cases";
        protected const string subTestSuiteName2 = "empty Child Test Suite";
        protected const string testCasetoHaveResults = "Test Case with many results";
        protected const string testCaseWithVersions = "TestCaseWithVersions";
        protected const string buildUsedForTestingName = "Build to be used for testing";
        protected const string testsuitetohaveattachmentsaddedName = "Testsuite to have attachments added";
        protected const string TestCaseToUseWithAttachmentsName = "TestCase to use to test attachments";
        protected const string TestCaseWithAttachmentsName = "TestCase with Attachments";
        protected const string TestCaseWithStepsName = "Testcase with Steps";
        private List<TestProject> allProjects;
        private int apiTestProjectId;
        private int businessRulesTestSuiteId;
        private int emptyProjectId;

        private TestPlan plan;
        private TestPlan planCalledAutomatedTesting;
        protected TestLink proxy = new TestLink(apiKey, testlinkUrl, true);

        private int testSuiteWithSubtestSuitesId;

        /// <summary>
        /// the id of the project that is used for automated testing of the API
        /// </summary>
        protected int ApiTestProjectId
        {
            get
            {
                if (apiTestProjectId == 0)
                    LoadProjectIds();
                return apiTestProjectId;
            }
        }

        /// <summary>
        /// get the APITest project
        /// </summary>
        protected TestProject ApiTestProject
        {
            get
            {
                if (allProjects == null)
                    LoadProjectIds();
                foreach (var tp in allProjects)
                    if (tp.name == testProjectName)
                        return tp;
                return null; // not found
            }
        }

        /// <summary>
        /// the id of a project that is empty
        /// </summary>
        protected int EmptyProjectId
        {
            get
            {
                if (emptyProjectId == 0)
                    LoadProjectIds();
                return emptyProjectId;
            }
        }

        /// <summary>
        /// get all platforms for the standard test plan
        /// </summary>
        protected List<TestPlatform> Platforms => proxy.GetTestPlanPlatforms(PlanCalledAutomatedTesting.id);

        /// <summary>
        /// the id of the test suite where we can create test cases and record test results
        /// </summary>
        protected int BusinessRulesTestSuiteId
        {
            get
            {
                if (businessRulesTestSuiteId == 0)
                {
                    var allSuites = proxy.GetFirstLevelTestSuitesForTestProject(ApiTestProjectId);
                    foreach (var ts in allSuites)
                        if (ts.name == testSuiteName1)
                            businessRulesTestSuiteId = ts.id;
                }

                // Assert.AreNotEqual(0, businessRulesTestSuiteId, "Setup failed to find test suite named '{0}'", testSuiteName2);
                return businessRulesTestSuiteId;
            }
        }

        protected int TestSuiteWithSubTestSuites
        {
            get
            {
                if (testSuiteWithSubtestSuitesId == 0)
                {
                    var allSuites = proxy.GetFirstLevelTestSuitesForTestProject(ApiTestProjectId);
                    foreach (var ts in allSuites)
                        if (ts.name == testSuiteName2)
                            testSuiteWithSubtestSuitesId = ts.id;
                }

                // Assert.AreNotEqual(0, businessRulesTestSuiteId, "Setup failed to find test suite named '{0}'", testSuiteName2);
                return testSuiteWithSubtestSuitesId;
            }
        }


        protected TestSuite Testsuitetohaveattachmentsadded
        {
            get
            {
                var allSuites = proxy.GetFirstLevelTestSuitesForTestProject(ApiTestProjectId);
                foreach (var ts in allSuites)
                    if (ts.name == testsuitetohaveattachmentsaddedName)
                        return ts;
                Assert.Fail("Setup failed, could not get test suite named '{0}'. must be top level test suite", testsuitetohaveattachmentsaddedName);
                return null;
            }
        }

        /// <summary>
        /// a test case where new results can be added and then deleted
        /// </summary>
        protected int TestCaseToHaveResultsDeleted
        {
            get
            {
                var testcases = proxy.GetTestCaseIDByName("TestCaseToHaveResultsDeleted", testSuiteName1);
                Assert.IsNotEmpty(testcases, "could not find test cases called 'TestCaseToHaveResultsDeleted'. Can't proceed with test");
                return testcases[0].id;
            }
        }


        protected int TestCaseToUseWithAttachments
        {
            get
            {
                var testcases = proxy.GetTestCaseIDByName(TestCaseToUseWithAttachmentsName);
                Assert.IsNotEmpty(testcases, "Setup:Could not find any test cases with this name: '{0}'", testCaseWithVersions);
                return testcases[0].id;
            }
        }

        protected int TestCaseIdWithAttachments
        {
            get
            {
                var testcases = proxy.GetTestCaseIDByName(TestCaseWithAttachmentsName);
                Assert.IsNotEmpty(testcases, "Setup:Could not find any test cases with this name: '{0}'", testCaseWithVersions);
                return testcases[0].id;
            }
        }

        protected int TestCaseIdWithVersions
        {
            get
            {
                var testcases = proxy.GetTestCaseIDByName(testCaseWithVersions);
                Assert.IsNotEmpty(testcases, "Setup:Could not find any test cases with this name: '{0}'", testCaseWithVersions);
                return testcases[0].id;
            }
        }

        /// <summary>
        /// get the test case that has a number of steps in it
        /// </summary>
        protected int TestCaseIdWithSteps
        {
            get
            {
                var tcIdList = proxy.GetTestCaseIDByName(TestCaseWithStepsName);
                Assert.IsNotEmpty(tcIdList, "Setup: could not find testcase named {0}", TestCaseWithStepsName);
                return tcIdList[0].id;
            }
        }

        protected TestPlan PlanCalledAutomatedTesting => planCalledAutomatedTesting ?? (planCalledAutomatedTesting = GetTestPlan(theTestPlanName));

        /// <summary>
        /// get a list of all projects;
        /// </summary>
        protected List<TestProject> AllProjects => allProjects ?? (allProjects = proxy.GetProjects());

        protected TestPlan AutomatedTestingTestplan => GetTestPlan(theTestPlanName);

        /// <summary>
        /// in case we want to force a retrieve of all projects
        /// </summary>
        protected void clearAllProjects()
        {
            allProjects = null;
        }

        /// <summary>
        /// load a list of all projects
        /// </summary>
        protected void LoadProjectIds()
        {
            apiTestProjectId = -1;
            emptyProjectId = -1;

            foreach (var project in AllProjects)
            {
                switch (project.name)
                {
                    case testProjectName:
                        apiTestProjectId = project.id;
                        break;
                    case emptyProjectName:
                        emptyProjectId = project.id;
                        break;
                }
            }

            Assert.AreNotEqual(-1, apiTestProjectId, "Could not find project id for project {0}", testProjectName);
            Assert.AreNotEqual(-1, emptyProjectId, "Could not find project id for project {0}", emptyProjectName);
        }

        protected TestPlan GetTestPlan(string testPlanName)
        {
            plan = proxy.GetProjectTestPlans(ApiTestProjectId).FirstOrDefault(a => a.name == testPlanName);
            if (plan == null) Assert.Fail("Setup failed, could not find test plan named '{0}' in project '{1}'", testPlanName, testProjectName);

            return plan;
        }
    }
}