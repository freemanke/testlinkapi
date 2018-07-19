using System;
using NUnit.Framework;

namespace TestLinkApi.Tests
{
    [TestFixture]
    public class TestProjectTests : BaseTest
    {
        [Test]
        public void GetAllTestProjects()
        {
            var result = proxy.GetProjects();
            Assert.IsNotEmpty(result);
            foreach (var tp in result)
            {
                Console.WriteLine("{0}:{1}", tp.id, tp.name);
                Console.WriteLine(" Automation={0}, Priority={1}, Requirements={2}, Prefix='{3}', TcCounter={4}", tp.option_automation, tp.option_priority, tp.option_reqs, tp.prefix, tp.tc_counter);
            }
        }

        [Test]
        public void GetTestProjectByName()
        {
            var tp = proxy.GetProject(ProjectName);
            Assert.IsNotNull(tp);
            Assert.AreEqual(ProjectName, tp.name);
            Assert.AreEqual(ProjectId, tp.id);
            Assert.IsTrue(tp.option_automation);
            Assert.IsTrue(tp.option_priority);
            Assert.IsTrue(tp.option_reqs);
            Assert.IsTrue(tp.option_inventory);
            Assert.AreEqual(Prefix, tp.prefix);
        }

        [Test]
        public void ShouldThrowTestLinkExceptionWhenGetTestProjectByUnexistName()
        {
            Assert.Catch<TestLinkException>(() => proxy.GetProject("project that does not exist"));
        }

        [Test]
        public void UploadTestProjectAttachment()
        {
            var content = new byte[4];
            content[0] = 48;
            content[1] = 49;
            content[2] = 50;
            content[3] = 51;

            var r = proxy.UploadTestProjectAttachment(ProjectId, "fileX.txt", "text/plain", content, "some result", "a description");
            Assert.AreEqual(r.foreignKeyId, ProjectId);
            Console.WriteLine("Response id:{0}, table '{1}', title:'{2}' size:{3}", r.foreignKeyId, r.linkedTableName, r.title, r.size);
        }
    }
}