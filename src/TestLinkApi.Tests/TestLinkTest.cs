﻿using System;
using System.Linq;
using NUnit.Framework;

namespace TestLinkApi.Tests
{
    public class TestLinkTest
    {
        private const string apiKey = "79eb8aa0a0fb58902f081c857c66d63d";
        private const string testlinkUrl = "https://dev.testlink.tp.cmit.local/lib/api/xmlrpc/v1/xmlrpc.php";
        private TestLink testlink = new TestLink(apiKey, testlinkUrl);

        [Test]
        public void SayHelloTest()
        {
            Assert.IsNotNull(testlink.SayHello());
            Console.WriteLine(testlink.SayHello());
        }

        [Test]
        public void GetObjects()
        {
            Console.WriteLine(testlink.about());

            var projects = testlink.GetProjects();
            projects.ForEach(a => Console.WriteLine($"Project: {a.id}, {a.name}"));

            var project = projects.First(a => a.name == "CMGE");
            var testPlans = testlink.GetProjectTestPlans(project.id);
            testPlans.ForEach(a => Console.WriteLine($"Test Plans: {a.id}, {a.name}"));

            var testSuites = testlink.GetFirstLevelTestSuitesForTestProject(project.id);
            testSuites.ForEach(a => Console.WriteLine($"Test Suites: {a.id}, {a.name}"));

            var testSuite = testSuites.First();
            var testCases = testlink.GetTestCasesForTestSuite(testSuite.id, false);
            testCases.ForEach(a => Console.WriteLine($"Test Cases: {a.id}, {a.name}"));

            var testCase = testlink.GetTestCase(4510, 1);
            Console.WriteLine($"Test Case: {testCase.id}, {testCase.name}");
        }

        [Test]
        public void GetTestCasesForTestPlanTest()
        {
            Console.WriteLine(testlink.about());

            var project = testlink.GetProjects().First(a => a.name == "CMGE");
            var testPlan = testlink.GetProjectTestPlans(project.id).First(a => a.name == "V0-H.1020");

            var platforms = testlink.GetTestPlanPlatforms(testPlan.id);
            Console.WriteLine($"\r\n{testPlan.name} includes platforms:");
            platforms.ForEach(a => Console.WriteLine($"{a.name} (id: {a.id})"));

            var builds = testlink.GetBuildsForTestPlan(testPlan.id);
            Console.WriteLine($"\r\n{testPlan.name} includes builds:");
            builds.ForEach(a => Console.WriteLine($"{a.name} (id: {a.id})"));

            var cases = testlink.GetTestCasesForTestPlan(testPlan.id);
            var testCases = cases.GroupBy(a => a.external_id).Select(g => g.First()).ToList();
            Console.WriteLine($"\r\n{testPlan.name} includes test cases: {testCases.Count}");

            var platformTestCases = testCases.GroupBy(a => a.platform_name);
            Console.WriteLine("\r\nGroups:");
            var counter = 0;
            foreach (var group in platformTestCases)
            {
                counter += group.ToArray().Length;
                Console.WriteLine($"Platform: {group.Key} Test cases: {group.ToArray().Length}");
            }

            Console.WriteLine($"All platform total test cases: {counter}");
        }

        [Test]
        public void DoesUserExistTest()
        {
            Assert.IsTrue(testlink.DoesUserExist("user"));
        }

        [Test]
        public void GetCustomField()
        {
            var project = testlink.GetProject("CMGE");
            var firtRootTestSuite = testlink.GetFirstLevelTestSuitesForTestProject(project.id).First();
            var testSuites = testlink.GetTestSuitesForTestSuite(firtRootTestSuite.id);
            testSuites.ForEach(ts =>
            {
                Console.WriteLine($"Test Suite: {ts.id}, {ts.name}");
                var testCases = testlink.GetTestCasesForTestSuite(ts.id, false);
                testCases.ForEach(tc =>
                {
                    var testCase = testlink.GetTestCase(tc.id);
                    var product = testlink.GetCustomFileds(testCase.testcase_id, testCase.externalid, testCase.version, project.id, "Product");
                    var ef = testlink.GetCustomFileds(testCase.testcase_id, testCase.externalid, testCase.version, project.id, "ExecutionFramework");
                    Console.WriteLine($"Test Case: {tc.id}, {tc.name}, Product: {product}, ExecutionFramework: {ef}");
                });
            });
        }

        [Test]
        public void ReportTCResultTest()
        {
            var testPlan = testlink.getTestPlanByName("CMGE", "dev-nunit-test-plan");
            var testCases = testlink.GetTestCasesForTestPlan(testPlan.id);
            var testCase = testCases.Last();

            testlink.ReportTCResult(testCase.tc_id, testPlan.id, TestCaseStatus.Failed, testCase.platform_id, testCase.platform_name, true, true, "notes");
            var result = testlink.GetLastExecutionResult(testPlan.id, testCase.tc_id);
            Assert.AreEqual("f", result.status);

            testlink.ReportTCResult(testCase.tc_id, testPlan.id, TestCaseStatus.Passed, testCase.platform_id, testCase.platform_name, true, true, "notes");
            result = testlink.GetLastExecutionResult(testPlan.id, testCase.tc_id);
            Assert.AreEqual("p", result.status);
        }

        [Test]
        public void CreateTestCase()
        {
            var project = testlink.GetProject("CMGE");
            var testSuites = testlink.GetFirstLevelTestSuitesForTestProject(project.id);
            testSuites.ForEach(a => Console.WriteLine($"Test Suites: {a.id}, {a.name}"));

            var testSuite = testSuites.First();
            var created = testlink.CreateTestCase("user", testSuite.id, "新创建的测试用例", project.id, "summary",
                new[] {new TestStep(1, "<p>click(<img alt=\"\" src=\"http://dev-wiki-1.cmit.local/resources/assets/cmgos.png \" style=\"height:71px; width:139px\" />)</p>", "find", true, 2)},
                "", 1, false, ActionOnDuplicatedName.CreateNewVersion, 0, 0);

            Console.WriteLine($"{created.id}, {created.additionalInfo.has_duplicate}, {created.status}, {created.message}, {created.operation}");
            var testCases = testlink.GetTestCaseIDByName("新创建的测试用例");
            Assert.IsNotEmpty(testCases);
        }
    }
}