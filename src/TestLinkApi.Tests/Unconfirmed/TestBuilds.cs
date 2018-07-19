using System;
using NUnit.Framework;

namespace TestLinkApi.Tests
{
    /// <summary>
    /// Contains unit tests to test the API.
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
    public class TestBuilds : Testbase
    {
        [SetUp]
        protected void Setup()
        {
            LoadProjectIds();
            Assert.IsNotNull(PlanCalledAutomatedTesting, "Setup failure - couldn't get test plan");
        }

        [Test]
        [Category("Changes Database")]
        public void CreateABuild()
        {
            var result = proxy.CreateBuild(PlanCalledAutomatedTesting.id, string.Format("build {0}", DateTime.Now),
                "an auto created build");
            Assert.IsNotNull(result, "Returned a null result");
            Assert.IsTrue(result.status, "CreateBuild returned failure: {0}", result.message);
        }

        [Test]
        private void getLatestBuild()
        {
            var latest = proxy.GetLatestBuildForTestPlan(PlanCalledAutomatedTesting.id);
            Assert.IsNotNull(latest, "couldn't find a latest build");
        }

        [Test]
        public void ListBuilds()
        {
            var builds = proxy.GetBuildsForTestPlan(PlanCalledAutomatedTesting.id);
            Assert.IsNotEmpty(builds, "Got empty list of builds for plan");
            foreach (var b in builds)
                Console.WriteLine("  Build #{0}: {1}. IsOpen={2}", b.id, b.name, b.is_open);
        }

        [Test]
        public void ShouldGetEmptyList()
        {
            var tp = GetTestPlan(emptyTestplanName);
            Assert.IsNotNull(tp, "Test can't proceed, testplan '{0}' couldn't be found");
            var builds = proxy.GetBuildsForTestPlan(tp.id);
            Assert.IsEmpty(builds);
        }

        [Test]
        public void ShouldGetNoBuild()
        {
            // need a test project with no builds
            var tp = GetTestPlan(emptyTestplanName);
            Assert.IsNotNull(tp, "Test can't proceed, testplan '{0}' couldn't be found");
            var build = proxy.GetLatestBuildForTestPlan(tp.id);
            Assert.IsNull(build, "{0} should have no builds", emptyTestplanName);
        }
    }
}