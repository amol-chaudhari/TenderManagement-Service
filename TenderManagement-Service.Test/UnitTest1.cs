using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TenderManagement_Service.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string[] filePaths = Directory.GetFiles(@"E:\Study Material\Certifications", "*.pdf",
                                         SearchOption.AllDirectories);
            foreach (var file in filePaths)
            {
                var x = new DirectoryInfo(file);
                var y = Path.GetFileNameWithoutExtension(file);
                var paths = file.Split('\\');
                var fileName = paths[paths.Length - 1];
            }
        }

        [TestMethod]
        public void TestExtensionMethod()
        {
            var x = StringExtensionMethods.Encrypt("campaign@2019");
        }
    }
}
