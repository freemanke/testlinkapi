using System;
using CookComputing.XmlRpc;
using NUnit.Framework;

namespace TestLinkApi.Tests
{
    [TestFixture]
    public class SmokeTests : Testbase
    {
        [Test]
        [TestCase(5, "10 G shock", "Handheld devices")]
        [TestCase(7, "Gamma Ray Storm", "Handheld devices")]
        public void GetTestCase(int id, string name, string testSuiteId)
        {
            var tcidList = proxy.GetTestCaseIDByName(name);
            Assert.AreEqual(1, tcidList.Count, "Call should return only 1 element");
            var tcid = tcidList[0];
            Assert.AreEqual(id, tcid.id);
            Assert.AreEqual(name, tcid.name);
            Assert.AreEqual(testSuiteId, tcid.tsuite_name);
        }

        [Test]
        public void SayHello()
        {
            var result = proxy.SayHello();
            Assert.AreEqual("Hello!", result, "Unexpected Server Response");
        }

        [Test]
        public void ShouldFailBecauseOfInvalidDevKey()
        {
            proxy = new TestLink("empty", testlinkUrl);
            Assert.Catch<TestLinkException>(() => proxy.GetTestCaseIDByName("10 G shock"));
        }

        [Test]
        public void ShouldFailBecauseOfInvalidURL()
        {
            proxy = new TestLink(apiKey, "http://localhost/testlink/api/xmlrpc.php");
            Assert.Catch<XmlRpcServerException>(() => proxy.SayHello());
            Assert.Fail("Did not cause an exception");
        }

        [Test]
        public void shouldFailBecauseOfNullDevKey()
        {
            proxy = new TestLink(null, testlinkUrl);
            var tcidList = proxy.GetTestCaseIDByName("10 G shock");
            Assert.Catch<XmlRpcServerException>(() => proxy.SayHello());
            Assert.Fail("Did not cause an exception");
        }

        [Test]
        public void ShouldGetListofProjects()
        {
            var result = proxy.GetProjects();
            Assert.IsNotNull(result, "should at least be an empty list");
            foreach (var tp in result)
                Console.WriteLine("{0}: {1}", tp.id, tp.name);
        }

        [Test]
        public void TestThatTestDBisProperlySetup()
        {
            Assert.IsNotNull(ApiTestProject, "Can't run tests. Project named {0} is not set up.", testProjectName);
            var empty = EmptyProjectId;
            Assert.AreNotEqual(0, empty, "Can't run tests. Empty Project named '{0}' is not set up.", emptyProjectName);
            Assert.AreNotEqual(0, BusinessRulesTestSuiteId, "Setup failed to find test suite named '{0}'", testSuiteName2);
            Assert.IsNotNull(PlanCalledAutomatedTesting, "Can't run tests. need to have at least one testplan named {1} defined for project '{0}' .", testProjectName, theTestPlanName);
            var platforms = Platforms;
            Assert.IsNotEmpty(platforms, "Can't run tests. need to have at least one platform defined for project '{0}' .", testProjectName);
        }

        [Test]
        [TestCase("joe", true)]
        [TestCase("admin", true)]
        [TestCase("Nonexistentuser", false)]
        public void testUserExists(string userName, bool shouldExist)
        {
            var result = proxy.DoesUserExist(userName);
            Assert.AreEqual(shouldExist, result, "tried user {0} and got result {1}", userName, shouldExist);
        }
    }
}