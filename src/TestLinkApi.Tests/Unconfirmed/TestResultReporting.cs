using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace TestLinkApi.Tests
{
    /// <summary>
    /// tests the API for functions on storing test run results
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
    public class TestResultReporting : Testbase
    {
        [SetUp]
        protected void Setup()
        {
            TestPlan plan = GetTestPlan(theTestPlanName);
            tPlanId = plan.id;

            List<TestCaseId> list = proxy.GetTestCaseIDByName(testCasetoHaveResults);
            Assert.IsNotEmpty(list, "Failure in Setup. Couldn't find test case");
            tcIdToHaveResults = list[0].id;
            platformId = Platforms[0].id;
        }

        //   TestLink proxy;
        private int tcIdToHaveResults;
        private int tPlanId;
        private int platformId;

        protected int BuildIdUsedForTesting
        {
            get
            {
                List<Build> list = proxy.GetBuildsForTestPlan(tPlanId);
                Assert.AreNotEqual(0, list.Count, "Setup TestCaseStatus.Failed to find builds  for testproject '{1}", testProjectName);
                foreach (Build build in list)
                    if (build.name == buildUsedForTestingName)
                        return build.id;
                Assert.Fail("Setup TestCaseStatus.Failed to find build named {0} for testproject '{1}", buildUsedForTestingName, testProjectName);
                return 0;
            }
        }

        [Test]
        [Category("Changes Database")]
        public void ReportATestCaseExecution()
        {
            GeneralResult result = proxy.ReportTCResult(tcIdToHaveResults, tPlanId, TestCaseStatus.Blocked, platformId);
            Console.WriteLine("Status: '{0}', message: '{1}'", result.status, result.message);
            Assert.AreEqual(true, result.status);
        }

        [Test]
        [Category("Changes Database")]
        public void ReportTestCaseAgainstOlderBuild()
        {
            List<Build> builds = proxy.GetBuildsForTestPlan(PlanCalledAutomatedTesting.id);
            Assert.IsNotEmpty(builds, "Can't proceed. Got empty list of builds for plan");
            // remove inactive builds
            for (int i = builds.Count - 1; i >= 0; i--)
                if (builds[i].is_open == false)
                    builds.Remove(builds[i]);

            Assert.IsTrue(builds.Count > 1, "Can't proceed. Need at least two active builds");
            // select oldest build (lowest id)
            Build target = builds[0];
            foreach (Build b in builds)
                if (target.id > b.id)
                    target = b;
            Console.WriteLine("Test case id:{0} against test build {1}:{2} recorded as TestCaseStatus.Failed",
                tcIdToHaveResults, target.id, target.name);
            GeneralResult result = proxy.ReportTCResult(tcIdToHaveResults, tPlanId, TestCaseStatus.Failed, platformId,
                buildid: target.id,
                notes: "test case assigned to older build");
            Console.WriteLine("Status: '{0}', message: '{1}'", result.status, result.message);
            Assert.AreEqual(true, result.status);
        }

        [Test]
        [Category("Changes Database")]
        public void ReportTestCaseExecutionAsFailed()
        {
            Console.WriteLine("tcId:{0}, tpId:{1}, buildId:{2}", tcIdToHaveResults, tPlanId, BuildIdUsedForTesting);
            GeneralResult result = proxy.ReportTCResult(tcIdToHaveResults, tPlanId, TestCaseStatus.Failed, platformId);
            Console.WriteLine("Status: '{0}', message: '{1}'", result.status, result.message);
            Assert.AreEqual(true, result.status);
        }

        [Test]
        [Category("Changes Database")]
        public void ReportTestCaseExecutionAsPassed()
        {
            Console.WriteLine("tcId:{0}, tpId:{1}", tcIdToHaveResults, tPlanId);
            GeneralResult result = proxy.ReportTCResult(tcIdToHaveResults, tPlanId, TestCaseStatus.Passed, platformId);
            Console.WriteLine("Status: '{0}', message: '{1}'", result.status, result.message);
            Assert.AreEqual(true, result.status);
        }


        [Test]
        [Category("Changes Database")]
        public void ReportTestCaseExecutionAsPassed_buildId_notes()
        {
            Console.WriteLine("tcId:{0}, tpId:{1}, buildId:{2}", tcIdToHaveResults, tPlanId, BuildIdUsedForTesting);

            GeneralResult result = proxy.ReportTCResult(tcIdToHaveResults, tPlanId, TestCaseStatus.Passed, platformId,
                buildid: BuildIdUsedForTesting,
                notes: "Test case named : ReportTestCaseExecutionAsPassed_buildId_notes");
            Console.WriteLine("Status: '{0}', message: '{1}'", result.status, result.message);
            Assert.AreEqual(true, result.status);
        }

        [Test]
        [Category("Changes Database")]
        public void ReportTestCaseExecutionAsPassed_buildId_notes_guessFalse()
        {
            Console.WriteLine("tcId:{0}, tpId:{1}, buildId:{2}", tcIdToHaveResults, tPlanId, BuildIdUsedForTesting);
            GeneralResult result = proxy.ReportTCResult(tcIdToHaveResults, tPlanId, TestCaseStatus.Passed, platformId,
                buildid: BuildIdUsedForTesting,
                notes: "Test case named : ReportTestCaseExecutionAsPassed_buildId_notes_guessFalse",
                guess: false);
            Console.WriteLine("Status: '{0}', message: '{1}'", result.status, result.message);
            Assert.AreEqual(true, result.status);
        }

        [Test]
        [Category("Changes Database")]
        public void ReportTestCaseExecutionAsPassed_buildId_notes_guessFalse_bugid()
        {
            Console.WriteLine("tcId:{0}, tpId:{1}, buildId:{2}", tcIdToHaveResults, tPlanId, BuildIdUsedForTesting);
            GeneralResult result = proxy.ReportTCResult(tcIdToHaveResults, tPlanId, TestCaseStatus.Failed, platformId,
                buildid: BuildIdUsedForTesting,
                notes: "ReportTestCaseExecutionAsPassed_buildId_notes_guessFalse_bugid. This one TestCaseStatus.Failed with bug id 237",
                guess: false,
                bugid: 237);
            Console.WriteLine("Status: '{0}', message: '{1}'", result.status, result.message);
            Assert.AreEqual(true, result.status);
        }

        [Test]
        [Category("Changes Database")]
        public void ReportTestCaseExecutionAsPassed_buildId_notes_guessFalse_bugid_ExistingplatformName()
        {
            Console.WriteLine("tcId:{0}, tpId:{1}, buildId:{2}", tcIdToHaveResults, tPlanId, BuildIdUsedForTesting);

            TestPlatform platform = Platforms[0];

            GeneralResult result = proxy.ReportTCResult(tcIdToHaveResults, tPlanId, TestCaseStatus.Failed,
                buildid: BuildIdUsedForTesting,
                notes: "Test case named : ReportTestCaseExecutionAsPassed_buildId_notes_guessFalse_bugid_ExistingplatformName. This one TestCaseStatus.Failed with bug id 239 and build platform named 'platformX'",
                guess: false,
                bugid: 239,
                platformName: platform.name);
            Console.WriteLine("Status: '{0}', message: '{1}'", result.status, result.message);
            Assert.AreEqual(true, result.status);
        }

        [Test]
        [Category("Changes Database")]
        //[ExpectedException(typeof(TestLinkException))]
        public void ReportTestCaseExecutionAsPassed_buildId_notes_guessFalse_bugid_wrongplatformId()
        {
            Console.WriteLine("tcId:{0}, tpId:{1}, buildId:{2}", tcIdToHaveResults, tPlanId, BuildIdUsedForTesting);
            GeneralResult result = proxy.ReportTCResult(tcIdToHaveResults, tPlanId, TestCaseStatus.Failed, 9999,
                buildid: BuildIdUsedForTesting,
                notes: "Test case named : ReportTestCaseExecutionAsPassed_buildId_notes_guessFalse_bugid_wrongplatformId. This one TestCaseStatus.Failed with bug id 238 and build platform id 1",
                guess: false,
                bugid: 238);
            Console.WriteLine("Status: '{0}', message: '{1}'", result.status, result.message);
            Assert.AreEqual(true, result.status);
        }

        [Test]
        //[ExpectedException(typeof(TestLinkException))]
        [Category("Changes Database")]
        public void ReportTestCaseExecutionAsPassed_buildId_notes_guessFalse_bugid_WrongplatformName()
        {
            Console.WriteLine("tcId:{0}, tpId:{1}, buildId:{2}", tcIdToHaveResults, tPlanId, BuildIdUsedForTesting);
            GeneralResult result = proxy.ReportTCResult(tcIdToHaveResults, tPlanId, TestCaseStatus.Failed,
                platformName: "hfshjdks",
                buildid: BuildIdUsedForTesting,
                notes: "Test case named : ReportTestCaseExecutionAsPassed_buildId_notes_guessFalse_bugid_platformName. This one TestCaseStatus.Failed with bug id 239 and build platform named 'platformX'",
                guess: false,
                bugid: 239);
            Console.WriteLine("Status: '{0}', message: '{1}'", result.status, result.message);
            Assert.AreEqual(true, result.status);
        }

        [Test]
        [Category("Changes Database")]
        public void ReportTestCaseExecutionAsPassed_buildId_notes_guessTrue()
        {
            Console.WriteLine("tcId:{0}, tpId:{1}, buildId:{2}", tcIdToHaveResults, tPlanId, BuildIdUsedForTesting);
            GeneralResult result = proxy.ReportTCResult(tcIdToHaveResults, tPlanId, TestCaseStatus.Passed, platformId,
                buildid: BuildIdUsedForTesting,
                notes: "Test case named : ReportTestCaseExecutionAsPassed_buildId_notes_guessTrue",
                guess: true);
            Console.WriteLine("Status: '{0}', message: '{1}'", result.status, result.message);
            Assert.AreEqual(true, result.status);
        }

        [Test]
        [Category("Changes Database")]
        public void ReportTestCaseExecutionAsPassed_buildId_notes_guessTrue_bugid()
        {
            Console.WriteLine("tcId:{0}, tpId:{1}, buildId:{2}", tcIdToHaveResults, tPlanId, BuildIdUsedForTesting);
            TestPlatform platform = Platforms[0];
            GeneralResult result = proxy.ReportTCResult(tcIdToHaveResults, tPlanId, TestCaseStatus.Failed, platformId,
                buildid: BuildIdUsedForTesting,
                notes: "Test case named : ReportTestCaseExecutionAsPassed_buildId_notes_guessFalse_bugid. This one TestCaseStatus.Failed with bug id 238",
                guess: true,
                bugid: 238);
            Console.WriteLine("Status: '{0}', message: '{1}'", result.status, result.message);
            Assert.AreEqual(true, result.status);
        }

        [Test]
        [Category("Changes Database")]
        public void ReportTestCaseExecutionAsPassed_notes()
        {
            Console.WriteLine("tcId:{0}, tpId:{1}", tcIdToHaveResults, tPlanId);
            GeneralResult result = proxy.ReportTCResult(tcIdToHaveResults, tPlanId, TestCaseStatus.Passed, platformId, notes: "Test case named : ReportTestCaseExecutionAsPassed_notes");
            Console.WriteLine("Status: '{0}', message: '{1}'", result.status, result.message);
            Assert.AreEqual(true, result.status);
        }

        [Test]
        [Category("Changes Database")]
        public void ReportTestCaseExecutionUsingPlatformName()
        {
            Console.WriteLine("tcId:{0}, tpId:{1}", tcIdToHaveResults, tPlanId);
            GeneralResult result = proxy.ReportTCResult(tcIdToHaveResults, tPlanId, TestCaseStatus.Passed, platformName: "OS/X");
            Console.WriteLine("Status: '{0}', message: '{1}'", result.status, result.message);
            Assert.AreEqual(true, result.status);
        }


        [Test]
        public void ShouldFailWithInvalidTestCaseId()
        {
            Assert.Catch<TestLinkException>(() => proxy.ReportTCResult(1, tPlanId, TestCaseStatus.Blocked, platformId));
        }

        [Test]
        [Category("Changes Database")]
        public void testAttachmentUploadToExecution()
        {
            int testPlanId = PlanCalledAutomatedTesting.id;
            ;
            int testCaseId = TestCaseToHaveResultsDeleted;

            List<TestPlatform> platforms = proxy.GetTestPlanPlatforms(testPlanId);

            GeneralResult gr = proxy.ReportTCResult(testCaseId, testPlanId, TestCaseStatus.Failed, platforms[0].id);

            byte[] content = new byte[4];
            content[0] = 48;
            content[1] = 49;
            content[2] = 50;
            content[3] = 51;

            AttachmentRequestResponse r = proxy.UploadExecutionAttachment(gr.id, "fileX.txt", "text/plain", content, "some result", "a description");
            Assert.AreEqual(r.foreignKeyId, gr.id);
            Console.WriteLine("Response id:{0}, table '{1}', title:'{2}' size:{3}", r.foreignKeyId, r.linkedTableName, r.title, r.size);
            proxy.DeleteExecution(gr.id);
        }

        [Test]
        //[ExpectedException(typeof(TestLinkException))]
        public void testAttachmentUploadToExecution_wrongTcId()
        {
            int testPlanId = PlanCalledAutomatedTesting.id;
            ;
            int testCaseId = TestCaseToHaveResultsDeleted;

            List<TestPlatform> platforms = proxy.GetTestPlanPlatforms(testPlanId);


            byte[] content = new byte[4];
            content[0] = 48;
            content[1] = 49;
            content[2] = 50;
            content[3] = 51;
            //this should throw an exception as id = 0 is not valid
            AttachmentRequestResponse r = proxy.UploadExecutionAttachment(0, "fileX.txt", "text/plain", content, "some result", "a description");
        }

        [Test]
        [Category("Changes Database")]
        public void testDeleteExecutionExecution()
        {
            int testPlanId = PlanCalledAutomatedTesting.id;
            ;
            int testCaseId = TestCaseToHaveResultsDeleted;

            List<TestPlatform> platforms = proxy.GetTestPlanPlatforms(testPlanId);

            GeneralResult gr = proxy.ReportTCResult(testCaseId, testPlanId, TestCaseStatus.Failed, platforms[0].id);
            Assert.IsNotNull(gr, "did not get a return value from ReportTCResult");
            Assert.AreEqual(true, gr.status, "ReportTCResult did not return a true status");

            GeneralResult grdel = proxy.DeleteExecution(gr.id);
            Assert.IsNotNull(grdel, "did not get a return value from ReportTCResult");
            Assert.AreEqual(true, grdel.status, "ReportTCResult did not return a true status");
            Assert.AreEqual(grdel.id, gr.id, "did not get same id in return from delete execution as from reportTCResult");
        }
    }
}