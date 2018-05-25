#region Using

using System.Web.Optimization;

#endregion

namespace Conciliacao
{
    public static class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/content/smartadmin/css").Include("~/content/css/bootstrap.css",
                "~/content/css/font-awesome.css",
                "~/content/css/invoice.css",
                "~/content/css/lockscreen.css",
                "~/content/css/site.css",
                "~/content/css/smartadminproductionplugins.css",
                "~/content/css/smartadminproduction.css",
                "~/content/css/smartadminskins.css",
                "~/content/css/sweetalert2.css"));

            bundles.Add(new ScriptBundle("~/scripts/smartadmin/js").Include(
               "~/scripts/app.config.js",
               "~/scripts/plugin/jquery-touch/jquery.ui.touch-punch.min.js",
               "~/scripts/bootstrap/bootstrap.min.js",
               "~/scripts/notification/SmartNotification.min.js",
               "~/scripts/sweetalert/sweetalert2.min.js",
               "~/scripts/smartwidgets/jarvis.widget.min.js",
               "~/scripts/plugin/jquery-validate/jquery.validate.min.js",
               "~/scripts/plugin/masked-input/jquery.maskedinput.min.js",
               "~/scripts/plugin/select2/select2.min.js",
               "~/scripts/plugin/bootstrap-slider/bootstrap-slider.min.js",
               "~/scripts/plugin/bootstrap-progressbar/bootstrap-progressbar.min.js",
               "~/scripts/plugin/msie-fix/jquery.mb.browser.min.js",
               "~/scripts/plugin/fastclick/fastclick.min.js",
               "~/scripts/app.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/signalr/js").Include(
                  "~/Scripts/jquery.signalR-{version}.js"));

            bundles.Add(new ScriptBundle("~/scripts/full-calendar/js").Include(
                "~/scripts/plugin/moment/moment.min.js",
                "~/scripts/plugin/fullcalendar/jquery.fullcalendar.min.js"
                ));

            bundles.Add(new ScriptBundle("~/scripts/charts/js").Include(
                "~/scripts/plugin/easy-pie-chart/jquery.easy-pie-chart.min.js",
                "~/scripts/plugin/sparkline/jquery.sparkline.min.js",
                "~/scripts/plugin/morris/morris.min.js",
                "~/scripts/plugin/morris/raphael.min.js",
                "~/scripts/plugin/flot/jquery.flot.cust.min.js",
                "~/scripts/plugin/flot/jquery.flot.resize.min.js",
                "~/scripts/plugin/flot/jquery.flot.time.min.js",
                "~/scripts/plugin/flot/jquery.flot.fillbetween.min.js",
                "~/scripts/plugin/flot/jquery.flot.orderBar.min.js",
                "~/scripts/plugin/flot/jquery.flot.pie.min.js",
                "~/scripts/plugin/flot/jquery.flot.tooltip.min.js",
                "~/scripts/plugin/dygraphs/dygraph-combined.min.js",
                "~/scripts/plugin/chartjs/chart.min.js"
                ));

            bundles.Add(new ScriptBundle("~/scripts/datatables/js").Include(
                "~/scripts/plugin/datatables/jquery.dataTables.min.js",
              /*  "~/scripts/plugin/datatables/dataTables.colVis.min.js",
                "~/scripts/plugin/datatables/dataTables.buttons.min.js",
                "~/scripts/plugin/datatables/buttons.bootstrap.min.js",
                "~/scripts/plugin/datatables/dataTables.tableTools.min.js", */
                "~/scripts/plugin/datatables/dataTables.bootstrap.min.js",
                "~/scripts/plugin/datatable-responsive/datatables.responsive.min.js"
                ));

            bundles.Add(new ScriptBundle("~/scripts/jq-grid/js").Include(

                "~/scripts/plugin/jqgrid/grid.locale-en.min.js",
                "~/scripts/plugin/jqgrid/jquery.jqGrid.min.js"

                ));

            bundles.Add(new ScriptBundle("~/scripts/forms/js").Include(
                "~/scripts/plugin/jquery-form/jquery-form.min.js"
                ));

            bundles.Add(new ScriptBundle("~/scripts/smart-chat/js").Include(
                "~/scripts/smart-chat-ui/smart.chat.ui.min.js",
                "~/scripts/smart-chat-ui/smart.chat.manager.min.js"
                ));

            bundles.Add(new ScriptBundle("~/scripts/vector-map/js").Include(
                "~/scripts/plugin/vectormap/jquery-jvectormap-1.2.2.min.js",
                "~/scripts/plugin/vectormap/jquery-jvectormap-world-mill-en.js"
                ));


            BundleTable.EnableOptimizations = true;
        }
    }
}