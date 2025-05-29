using System.Collections.Generic;
using System.Web.Optimization;

namespace Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/libs/jquery-{version}.js",
                        "~/Scripts/libs/jquery.unobtrusive-ajax.min.js"));

            var jqueryVal = new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/libs/jqueryval-default.min.js")
                .Include("~/Scripts/libs/jquery.validate*"
                );
            jqueryVal.Orderer = new NonOrderingBundleOrderer();
            bundles.Add(jqueryVal);

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/libs/modernizr-*"
                       ));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/libs/bootstrap.min.js",
                "~/Scripts/libs/respond.min.js",
                "~/Scripts/plugins/fileinput.min.js",
                "~/Scripts/plugins/jquery.noty.packaged.min.js",
                "~/Scripts/libs/site.min.js"
                ));

            bundles.Add(new StyleBundle("~/Content/cssLTR").Include(
             "~/Content/bootstrap.css",
               "~/Content/plugins/font-awesome.min.css",
               "~/Content/plugins/fileinput.min.css",
               "~/Content/plugins/animate.min.css",
               "~/Content/site.min.css",
               "~/Content/KRBStyle.css",
               "~/Content/PagedList.min.css"
               ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
              "~/Content/customBootstrap.min.css",
                "~/Content/plugins/font-awesome.min.css",
                "~/Content/plugins/fileinput.min.css",
                "~/Content/plugins/animate.min.css",
                "~/Content/site.min.css",
                "~/Content/KRBStyle.css",
                "~/Content/PagedList.min.css"
                ));

            bundles.Add(new StyleBundle("~/Content/adminCssLtR").Include(
              "~/Content/bootstrap.css",
              "~/Content/plugins/font-awesome.min.css",
              "~/Content/ionicons.min.css",
              "~/Content/plugins/animate.min.css",
               "~/Content/site.min.css",
              "~/Content/plugins/fileinput.min.css"));

            bundles.Add(new StyleBundle("~/Content/adminCss").Include(
                "~/Content/customBootstrap.min.css",
                "~/Content/plugins/font-awesome.min.css",
                "~/Content/ionicons.min.css",
                "~/Content/plugins/animate.min.css",
                 "~/Content/site.min.css",
                "~/Content/plugins/fileinput.min.css"));

            bundles.Add(new ScriptBundle("~/bundles/adminJs").Include(
                "~/Scripts/libs/bootstrap.min.js",
                "~/Scripts/libs/respond.min.js",
                "~/Scripts/plugins/jquery.noty.packaged.min.js",
                "~/Scripts/plugins/fileinput.min.js",
                "~/Scripts/libs/site.min.js"
                ));


            BundleTable.EnableOptimizations = false;

        }

    }
    class NonOrderingBundleOrderer : IBundleOrderer
    {
        public IEnumerable<BundleFile> OrderFiles(BundleContext context, IEnumerable<BundleFile> files)
        {
            return files;
        }
    }
}
