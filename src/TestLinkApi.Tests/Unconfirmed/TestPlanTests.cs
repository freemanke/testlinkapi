using System;
using NUnit.Framework;

namespace TestLinkApi.Tests
{
    [TestFixture]
    public class TestPlanTests : BaseTest
    {
        private int planId;
        private readonly string planName = $"AutocreatedTestPlan at {DateTime.Now.ToUniversalTime()}";

        [OneTimeSetUp]
        protected void Setup()
        {
            var result = proxy.CreateTestPlan(planName, ProjectName, "These are some notes");
            Assert.AreEqual(true, result.status);
            planId = result.id;
            Assert.Greater(planId, 0);
            Assert.IsTrue(proxy.GetProjectTestPlans(ProjectId).Exists(a => a.name == planName && a.id == result.id));

            Assert.IsTrue(proxy.CreateBuild(planId, "V0", "V0 Build").status);
            Assert.IsTrue(proxy.CreateBuild(planId, "V1", "V1 Build").status);
        }

        [SetUp]
        protected void CreateTestPlan()
        {
            var name = Guid.NewGuid().ToString();
            var result = proxy.CreateTestPlan(name, ProjectName, "These are some notes");
            Assert.AreEqual(true, result.status);
            Assert.IsTrue(proxy.GetProjectTestPlans(ProjectId).Exists(a => a.name == name && a.id == result.id));
        }

        [Test]
        public void GetProjectTestPlans()
        {
            var plans = proxy.GetProjectTestPlans(ProjectId);
            Assert.IsNotEmpty(plans);
            Assert.Catch<TestLinkException>(() => proxy.GetProjectTestPlans(999999));
        }

        [Test]
        public void GetTestPlanByName()
        {
            var plan = proxy.getTestPlanByName(ProjectName, planName);
            Assert.IsNotNull(plan);

            Assert.Catch<TestLinkException>(() => proxy.getTestPlanByName(ProjectName, "No Such Plan"));
            Assert.Catch<TestLinkException>(() => proxy.getTestPlanByName("No such project", planName));
        }

        [Test]
        public void GetTestPlanPlatforms()
        {
            var platforms = proxy.GetTestPlanPlatforms(planId);
            Assert.IsNotEmpty(platforms);
        }

        [Test]
        public void TestPlanTotalsReporting()
        {
            var plan = proxy.getTestPlanByName(ProjectName, planName);
            Assert.IsNotNull(plan, "can't proceed. No plan found");
            Console.WriteLine("Getting totals for plan {0}:{1}", plan.id, plan.name);
            var list = proxy.GetTotalsForTestPlan(plan.id);
            Assert.IsNotEmpty(list, "Expected to get results");
            foreach (var tpt in list)
            {
                Console.WriteLine("Name='{0}' Type='{1}'  Total:{2}", tpt.Name, tpt.Type, tpt.Total_tc);
                var det = tpt.Details;
                foreach (var key in det.Keys)
                {
                    Console.WriteLine("  '{0}': '{1}'", key, det[key]);
                }
            }
        }

        [Test]
        public void GetTotalsForTestPlan()
        {
            var list = proxy.GetTotalsForTestPlan(planId);
            Assert.AreEqual(1, list.Count, "Expected to get 1 result which has all zeros");
            var tpt = list[0];
            Assert.AreEqual(0, tpt.Total_tc);
        }
    }
}