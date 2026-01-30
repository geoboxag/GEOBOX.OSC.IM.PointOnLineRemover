using Autodesk.Map.IM.Forms;
using GEOBOX.OSC.IM.PointOnLineRemover.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace GEOBOX.OSC.IM.PointOnLineRemover.Views
{
    public partial class PointRemoverView : Window
    {
        private PointRemoverViewModel pointRemoverViewModel;

        public PointRemoverView(Document document)
        {
            InitializeComponent();
            pointRemoverViewModel = new PointRemoverViewModel(document);
            DataContext = pointRemoverViewModel;
        }

        #region Header-Grid Functions

        private void HeaderGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void MainWindow_Minimize(object sender, RoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void MainWindow_Close(object sender, RoutedEventArgs e)
        {
            base.OnClosed(e);
            this.Close();
        }

        #endregion Header-Grid Functions


        private void RemovePointsButton_Click(object sender, RoutedEventArgs e)
        {
            if (!pointRemoverViewModel.RemovePoints()) return;

            DialogResult = true;
            MainWindow_Close(sender, e);
        }

        private void ChooseCoordinateFilePathButton_Click(object sender, RoutedEventArgs e)
        {
            pointRemoverViewModel.ChooseCoordinateFile();
        }

        private void ChooseLogFilePathButton_Click(object sender, RoutedEventArgs e)
        {
            pointRemoverViewModel.ChooseLLogFileSavePath();
        }

        private void SelectAllFeatureClassesButton_Click(object sender, RoutedEventArgs e)
        {
            // ToDo: not implemented
        }

        private void DeselectAlFeatureClasseslButton_Click(object sender, RoutedEventArgs e)
        {
            // ToDo: not implemented
        }
    }
}