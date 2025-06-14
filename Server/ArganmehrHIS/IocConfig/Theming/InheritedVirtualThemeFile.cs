﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace IocConfig.Theming
{
    internal class InheritedVirtualThemeFile : VirtualFile
    {
        private readonly InheritedThemeFileResult _resolveResult;

        public InheritedVirtualThemeFile(InheritedThemeFileResult resolveResult)
            : base(DetermineVirtualPath(resolveResult))
        {
            this._resolveResult = resolveResult;
        }

        public override Stream Open()
        {
            return new FileStream(_resolveResult.ResultPhysicalPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }

        private static string DetermineVirtualPath(InheritedThemeFileResult resolveResult)
        {
            if (resolveResult.RelativePath.EndsWith(".cshtml", StringComparison.OrdinalIgnoreCase))
            {
                // ASP.NET BuildManager requires the original path for razor views,
                // otherwise an exception is thrown
                return resolveResult.OriginalVirtualPath;
            }
            else
            {
                return resolveResult.ResultVirtualPath;
            }
        }

    }
}
