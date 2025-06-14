﻿using Common.Utilities;
using Common.Helpers.Extentions;
using IocConfig.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace IocConfig.Theming
{
    public class ThemeableRazorViewEngine : ThemeableVirtualPathProviderViewEngine
    {
        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        public ThemeableRazorViewEngine()
        {
            var areaBasePathsSetting = CommonHelper.GetAppSetting<string>("am:AreaBasePaths", "~/Plugins/");
            var areaBasePaths = areaBasePathsSetting.Split(',').Select(x => x.Trim().EnsureEndsWith("/")).ToArray();

            // 0: view, 1: controller, 2: area
            var areaFormats = new string[] { "{2}/Views/{1}/{0}.cshtml", "{2}/Views/Shared/{0}.cshtml" };
            var areaViewLocationFormats = areaBasePaths.SelectMany(x => areaFormats.Select(f => x + f));

            AreaViewLocationFormats = areaViewLocationFormats.ToArray();
            AreaMasterLocationFormats = areaViewLocationFormats.ToArray();
            AreaPartialViewLocationFormats = areaViewLocationFormats.ToArray();

            // 0: view, 1: controller, 2: theme
            ViewLocationFormats = new[]
            {
                "~/Themes/{2}/Views/{1}/{0}.cshtml", 
				"~/Views/{1}/{0}.cshtml", 
                "~/Themes/{2}/Views/Shared/{0}.cshtml",
				"~/Views/Shared/{0}.cshtml"
            };

            // 0: view, 1: controller, 2: theme
            MasterLocationFormats = new[]
            {
                "~/Themes/{2}/Views/{1}/{0}.cshtml", 
				"~/Views/{1}/{0}.cshtml",
                "~/Themes/{2}/Views/Shared/{0}.cshtml", 
                "~/Views/Shared/{0}.cshtml"
            };

            // 0: view, 1: controller, 2: theme
            PartialViewLocationFormats = new[]
            {
				"~/Themes/{2}/Views/{1}/{0}.cshtml",
 				"~/Views/{1}/{0}.cshtml",  
				"~/Themes/{2}/Views/Shared/{0}.cshtml", 
				"~/Views/Shared/{0}.cshtml" 
            };

            FileExtensions = new[] { "cshtml" };
        }

        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
        {
            return new RazorView(controllerContext, partialPath, null, false, base.FileExtensions, base.ViewPageActivator);
        }

        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            return new RazorView(controllerContext, viewPath, masterPath, true, base.FileExtensions, base.ViewPageActivator);
        }
    }
}
