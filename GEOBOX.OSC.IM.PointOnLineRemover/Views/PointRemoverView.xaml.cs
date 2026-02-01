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
            if (!pointRemoverViewModel.RemovePoints()) 
            {
                MessageBox.Show(Properties.Resources.PointRemoverViewModel_RemovePointsWithWarnings, Properties.Resources.genModulName, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show(Properties.Resources.PointRemoverViewModel_RemovePointsSuccess, Properties.Resources.genModulName, MessageBoxButton.OK, MessageBoxImage.Information);
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

        // Feature Classes
        private void FeatureClassesCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            pointRemoverViewModel.CheckIsReadyForRemove();
        }

        private void FeatureClassesCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            pointRemoverViewModel.CheckIsReadyForRemove();
        }

        private void SelectAllFeatureClassesButton_Click(object sender, RoutedEventArgs e)
        {
            pointRemoverViewModel.SelectAllFeatureClasses();
        }

        private void DeselectAllFeatureClassesButton_Click(object sender, RoutedEventArgs e)
        {
            pointRemoverViewModel.DeselectAllFeatureClasses();
        }


        // Coordiantes
        private void CoodinatesCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            pointRemoverViewModel.CheckIsReadyForRemove();
        }

        private void CoodinatesCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            pointRemoverViewModel.CheckIsReadyForRemove();
        }

        private void SelectAllCoodinatesButton_Click(object sender, RoutedEventArgs e)
        {
            pointRemoverViewModel.SelectAllCoodinates();
        }

        private void DeselectAllCoodinatesButton_Click(object sender, RoutedEventArgs e)
        {
            pointRemoverViewModel.DeselectAllCoodinates();
        }
    }
}