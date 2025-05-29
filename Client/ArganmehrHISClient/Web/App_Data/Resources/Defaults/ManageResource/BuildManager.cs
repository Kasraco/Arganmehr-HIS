using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resources.ManageResource
{
    public class BuildManager
    {
        public string ProjectPath { get; private set; }
        public BuildManager(string projectPath)
        {
            ProjectPath = projectPath;
        }
        public void Build()
        {
            var regKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\MSBuild\ToolsVersions\4.0");
            if (regKey == null) return;
            var msBuildExeFilePath = Path.Combine(regKey.GetValue("MSBuildToolsPath").ToString(), "MSBuild.exe");
            var startInfo = new ProcessStartInfo
            {
                FileName = msBuildExeFilePath,
                Arguments = ProjectPath,
                WindowStyle = ProcessWindowStyle.Hidden
            };
            var process = Process.Start(startInfo);
            process.WaitForExit();
        }
    }
}
