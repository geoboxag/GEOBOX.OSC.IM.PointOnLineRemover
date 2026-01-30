using Autodesk.Map.IM.Forms;
using Autodesk.Map.IM.Forms.Events;
using GEOBOX.OSC.IM.PointOnLineRemover.Properties;
using GEOBOX.OSC.IM.PointOnLineRemover.Views;

namespace GEOBOX.OSC.IM.PointOnLineRemover.DocumentPlugins
{
    /// <summary>
    /// Summary description for DocumentPlugIn for Export KML File.
    /// </summary>
    public class PointOnLineRemoverDocumentPlugin : DocumentPlugIn
    {
        #region Init Modul
        /// <summary>
        /// Initializes a new instance of the <see cref="ExportKmlData"/> class.
        /// </summary>
        public PointOnLineRemoverDocumentPlugin()
        {
        }

        /// <summary>
        /// Checks whether a license is available for the current module.
        /// </summary>
        /// <returns>True, if the license is available; otherwise, false.</returns>
        public override bool LicenseAvailable()
        {
            return true;
        }

        public override void OnInitMenus(object sender, MenusEventArgs e)
        {
            // create new Menu for the Document "Fachschale"
            Menu menu = e.Menus.Item(DocumentContextMenuType.Document);
            MenuItem pointToolRootMenue = new Autodesk.Map.IM.Forms.MenuItem();
            pointToolRootMenue.Text = Resources.MapContextMenue_PointToolsRoot;
            menu.MenuItems.Add(pointToolRootMenue);

            // create new Menu Entry for Remove Points with List
            MenuItem removePointsWithListMenuItem = new Autodesk.Map.IM.Forms.MenuItem();
            removePointsWithListMenuItem.Text = Resources.MapContextMenue_RemovePointsWithList;
            removePointsWithListMenuItem.Click += new MenuItemClickEventHandler(RemovePointsWithListMenuItem_Click);

            // Add Menuitems to Menu-Entry
            pointToolRootMenue.MenuItems.Add(removePointsWithListMenuItem);
        }
        #endregion

        #region Remove Points
        private void RemovePointsWithListMenuItem_Click(object sender, MenuItemClickEventArgs e)
        {
            Document currentDocument = this.Application.Documents.Active;

            var pointRemoverView = new PointRemoverView(currentDocument);
            pointRemoverView.ShowDialog();
        }
        #endregion
    }
}