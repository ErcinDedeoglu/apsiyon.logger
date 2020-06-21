using System;
using System.IO;
using NUnit.Framework;

namespace Apsiyon.Logger.Test
{
    [TestFixture]
    public class FileTest
    {
        [Test]
        public void CreatePathTest()
        {
            Helper.ConfigurationHelper.Configuration();

            var path = Logger.Helper.PathHelper.CreatePath();
            
            Assert.IsTrue(Directory.Exists(path));
        }

        [Test]
        public void CreateLogFileTest()
        {
            Helper.ConfigurationHelper.Configuration();

            var path = Logger.Helper.PathHelper.CreatePath();
                
            path = Logger.Helper.FileHelper.CreateLogFile(path, new Logger.Helper.LogQueueHelper.LogDto()
            {
                Object = DateTime.UtcNow,
                ObjectType = DateTime.UtcNow.GetType().FullName,
                CreateDate = DateTime.UtcNow,
                Message = "Apsiyon.Logger UNIT TEST"
            });

            Assert.IsTrue(File.Exists(path));
        }
    }
}