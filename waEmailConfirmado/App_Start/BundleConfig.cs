using System.Web.Optimization;

namespace waEmailConfirmado
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/reset.css",
                      "~/Content/normalize.css",
                      "~/Content/estilo.css",
                      "~/Content/grid.css"));
        }
    }
}
