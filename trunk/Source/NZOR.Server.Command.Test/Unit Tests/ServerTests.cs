using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using NZOR.Server;

namespace NZOR.Command.Test.Unit_Tests
{
    [TestFixture]
    public class ServerTests
    {
        [TestCase]
        public void TestCopyFiles()
        {
            DirectoryInfo source = new DirectoryInfo(@"C:\Development\NZOR\Source\NZOR.Web.Service\App_Data");
            DirectoryInfo target = new DirectoryInfo(@"\\testserver05\wwwroot\nzor.web.service\app_data");
            DirectoryInfo backupDir = new DirectoryInfo(@"\\testserver05\wwwroot\nzor.web.service\indexes_backup");

            Process.CopyWebFiles(source, target, backupDir);            
        }

    }
}
