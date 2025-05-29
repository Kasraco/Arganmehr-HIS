using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Resources.ManageResource
{
    public class ResXResourceFileManager
    {
        public static readonly string BinPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase.Replace("file:///", ""));
        public static readonly string ResourcesPath = Path.Combine(BinPath, @"..\App_Data\Resources");
        public static readonly string ResourceProjectPath = Path.Combine(ResourcesPath, "Resources.csproj");
        public static readonly string DefaultsPath = Path.Combine(ResourcesPath, "Defaults");
        public static void CopyDlls()
        {
            File.Copy(Path.Combine(ResourcesPath, @"bin\debug\Resources.dll"), Path.Combine(BinPath, "Resources.dll"), true);
            File.Copy(Path.Combine(ResourcesPath, @"bin\debug\fa\Resources.resources.dll"), Path.Combine(BinPath, @"fa\Resources.resources.dll"), true);
            Directory.Delete(Path.Combine(ResourcesPath, "bin"), true);
            Directory.Delete(Path.Combine(ResourcesPath, "obj"), true);
        }
        public static void RestoreAll()
        {
            RestoreDlls();
            RestoreResourceFiles();
        }
        public static void RestoreDlls()
        {
            File.Copy(Path.Combine(DefaultsPath, @"bin\Resources.dll"), Path.Combine(BinPath, "Resources.dll"), true);
            File.Copy(Path.Combine(DefaultsPath, @"bin\fa\Resources.resources.dll"), Path.Combine(BinPath, @"fa\Resources.resources.dll"), true);
        }
        public static void RestoreResourceFiles(string resourceCategory)
        {
            RestoreFile(resourceCategory.Replace(".", "\\"));
        }
        public static void RestoreResourceFiles()
        {
            RestoreFile(@"Global\Configs");
            RestoreFile(@"Global\Exceptions");
            RestoreFile(@"Global\Paths");
            RestoreFile(@"Global\Texts");

            RestoreFile(@"ViewModels\Employees");
            RestoreFile(@"ViewModels\LogOn");
            RestoreFile(@"ViewModels\Settings");

            RestoreFile(@"Views\Employees");
            RestoreFile(@"Views\LogOn");
            RestoreFile(@"Views\Settings");
        }

        private static void RestoreFile(string subPath)
        {
            File.Copy(Path.Combine(DefaultsPath, subPath + ".resx"), Path.Combine(ResourcesPath, subPath + ".resx"), true);
            File.Copy(Path.Combine(DefaultsPath, subPath + ".fa.resx"), Path.Combine(ResourcesPath, subPath + ".fa.resx"), true);
        }
    }
}
