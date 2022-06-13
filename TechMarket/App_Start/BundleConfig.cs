using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Optimization;

namespace TechMarket {

    public class BundleConfig {

        public static void RegisterBundles(BundleCollection bundles) {

            bundles.Add(new StyleBundle("~/Content/bundle").Include(
                        "~/Content/bootstrap.css",
                        "~/Content/Site.css",
                        "~/Content/bootstrap-theme.css"));

            bundles.Add(new StyleBundle("~/Scripts/bundle").Include(
                        "~/Scripts/bootstrap.js",
                        "~/Scripts/bootstrap.bundle.js"));
            
            bundles.Add(new StyleBundle("~/Scripts/jquery").Include(
                        "~/Scripts/jquery-3.5.1.js"));

           
            //var scriptBundle = new ScriptBundle("~/Scripts/bundle");
            //var styleBundle = new StyleBundle("~/Content/bundle");

            //// jQuery
            //scriptBundle
            //    .Include("~/Scripts/jquery-3.5.1.js");

            //// Bootstrap
            //scriptBundle
            //    .Include("~/Scripts/bootstrap.js");

            //// Bootstrap
            //styleBundle
            //    .Include("~/Content/bootstrap.css");

            //// Custom site styles
            //styleBundle
            //    .Include("~/Content/Site.css");

            //bundles.Add(scriptBundle);
            //bundles.Add(styleBundle);


#if !DEBUG
            BundleTable.EnableOptimizations = true;
#endif
        }
    }
}