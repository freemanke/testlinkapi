using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace TestLinkApi.Tests
{
    [TestFixture]
    public class TestSuites : BaseTest
    {
        private int testsuiteId;
        private string testsuiteName = $"test-suite-{Guid.NewGuid().ToString()}";

        [Test]
        public void CreateTestSuite()
        {
            var createResult = proxy.CreateTestSuite(ProjectId, testsuiteName, "details");
            testsuiteId = createResult.id;
            Assert.Greater(testsuiteId, 0);
            Assert.AreEqual(true, createResult.status);

            var result = proxy.GetFirstLevelTestSuitesForTestProject(ProjectId);
            Assert.IsTrue(result.Exists(a => a.name == testsuiteName));
        }

        [Test]
        public void CreateTestSuiteOneLevelDown()
        {
            var testSuiteId = proxy.CreateTestSuite(ProjectId, testsuiteName, "details").id;
            var parentSuite = proxy.GetFirstLevelTestSuitesForTestProject(ProjectId).First(a => a.id == testSuiteId);
            var name = Guid.NewGuid().ToString();
            var createResult = proxy.CreateTestSuite(ProjectId, name, "details", parentSuite.id);
            Assert.IsTrue(createResult.status);
        }

        [Test]
        public void getChildTestSuites()
        {
            var parentId = proxy.CreateTestSuite(ProjectId, Guid.NewGuid().ToString(), "details").id;
            proxy.CreateTestSuite(ProjectId, Guid.NewGuid().ToString(), "details", parentId);
            proxy.CreateTestSuite(ProjectId, Guid.NewGuid().ToString(), "details", parentId);

            var children = proxy.GetTestSuitesForTestSuite(parentId);
            Assert.AreEqual(2, children.Count);
        }

        [Test]
        public void getChildTestSuitesShouldGetNone()
        {
            var suites = proxy.GetFirstLevelTestSuitesForTestProject(ProjectId);
            proxy.CreateTestSuite(ProjectId, "", "");
        }

        [Test]
        public void GetTestSuiteById_InvalidId()
        {
            var testSuite = proxy.GetTestSuiteById(999999);
            Assert.IsNull(testSuite);
        }

        [Test]
        public void GetTestSuiteById_validId()
        {
            var name = Guid.NewGuid().ToString();
            var id = proxy.CreateTestSuite(ProjectId, name, "details").id;
            var testSuite = proxy.GetTestSuiteById(id);
            Assert.IsNotNull(testSuite);
            Assert.AreEqual(name, testSuite.name);
        }

        [Test]
        public void GetTestSuitesForEmptyTestProject()
        {
            var list = proxy.GetFirstLevelTestSuitesForTestProject(-1);
            Assert.IsEmpty(list);
        }


        [Test]
        public void GetTestSuitesForTestPlan()
        {
            // TestPlan plan = GetTestPlan(theTestPlanName);

            //  var list = proxy.GetTestSuitesForTestPlan(plan.id);
            //  Assert.IsNotEmpty(list);
            //  foreach (var ts in list)
            //      Console.WriteLine("{0}:{1}", ts.id, ts.name);
        }


        [Test]
        public void GetTestSuitesForTestProject()
        {
            var list = proxy.GetFirstLevelTestSuitesForTestProject(ProjectId);
            Assert.IsNotEmpty(list);
        }

        [Test]
        [Category("Changes Database")]
        public void TestSuiteAttachmentCreation()
        {
            var id = proxy.CreateTestSuite(ProjectId, Guid.NewGuid().ToString(), "details").id;
            var content = new byte[] {48, 49, 50, 51};
            var response = proxy.UploadTestSuiteAttachment(id, "fileX.txt", "text/plain", content, "some result", "a description");
            Assert.AreEqual(response.foreignKeyId, id);
        }
    }
}