using System;
using NUnit.Framework;

namespace TestLinkApi.Tests
{
    public class BaseTest
    {
        protected const string apiKey = "061b7297baa1098156a177e94f4e10b8";
        protected const string testlinkUrl = "https://localhost:63002/lib/api/xmlrpc/v1/xmlrpc.php";
        protected readonly string Prefix = $"{Guid.NewGuid().ToString().Substring(0, 16)}";
        protected readonly string ProjectName = $"{Guid.NewGuid().ToString()}";
        protected TestLink proxy = new TestLink(apiKey, testlinkUrl);

        protected int ProjectId { get; private set; }

        [OneTimeSetUp]
        public void SetUp()
        {
            var result = proxy.CreateProject(ProjectName, Prefix, "project notes");
            Assert.IsTrue(result.status);

            var project = proxy.GetProject(ProjectName);
            ProjectId = project.id;
            Assert.AreEqual(result.id, ProjectId);
            Assert.Greater(ProjectId, 0);
            Assert.AreEqual(result.id, ProjectId);
        }
    }
}