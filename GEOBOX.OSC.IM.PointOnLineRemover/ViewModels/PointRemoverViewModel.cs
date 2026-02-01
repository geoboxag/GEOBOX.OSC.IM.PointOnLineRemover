using Autodesk.Map.IM.Data;
using Autodesk.Map.IM.Forms;
using GEOBOX.OSC.Common.Logging;
using GEOBOX.OSC.IM.PointOnLineRemover.Controllers;
using GEOBOX.OSC.IM.PointOnLineRemover.Coordinates;
using GEOBOX.OSC.IM.PointOnLineRemover.Domain;
using GEOBOX.OSC.IM.PointOnLineRemover.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Forms;

namespace GEOBOX.OSC.IM.PointOnLineRemover.ViewModels
{
    public class PointRemoverViewModel : INotifyPropertyChanged
    {
        private Document document;

        public ObservableCollection<PointCoordinateDetail> PointCoordinateDetails { get; set; }

        public ObservableCollection<FeatureClassDetail> FeatureClassDetails { get; set; }
        

        private string coordinateFilePath;
        public string CoordinateFilePath
        {
            set
            {
                coordinateFilePath = value;
                OnPropertyChanged("CoordinateFilePath");
            }
            get
            {
                return coordinateFilePath;
            }
        }

        private string logFilePath;
        public string LogFilePath
        {
            set
            {
                logFilePath = value;
                OnPropertyChanged("LogFilePath");
            }
            get
            {
                return logFilePath;
            }
        }

        private bool drawCADObjects = true;
        public bool DrawCADObjects
        {
            set
            {
                drawCADObjects = value;
                OnPropertyChanged("DrawCADObjects");
            }
            get
            {
                return drawCADObjects;
            }
        }

        // ToDo : add for activate/deactivate remove Button...
        private bool isReadyForRemove = false;
        public bool IsReadyForRemove 
        {
            set
            {
                isReadyForRemove = value;
                OnPropertyChanged("IsReadyForRemove");
            }
            get
            {
                return isReadyForRemove;
            }
        }

        public PointRemoverViewModel(Document document)
        {
            this.document = document;

            PointCoordinateDetails = new ObservableCollection<PointCoordinateDetail>();
            FeatureClassDetails = new ObservableCollection<FeatureClassDetail>();

            LoadFeatureClasses();

            // only for testing - create sample json file for testing or debug
            //CreateSampleJson("C:\\Temp\\test.json");
        }

        public bool RemovePoints()
        {
            // 1. Wenn drawCADObjects = true und AutoCAD Map 3D offen >> Kreis mit Radius von 10m auf Layer "GEOBOX RemovePoints" erstellen, für jede Koordinate...
            // >> Später wird eine Option für das Setzten der Option auf dem UI erstellt.
            // >> Später wird eine Funktion erstellt, die nur die Punkte zeichnet > für die Prüfung durch den Benutzenden


            // 1. Logger erstellen
            var logger = CreateNewLogger();
            logger.WriteInformation($"{Resources.LogMessage_SelectedCoodianteFile}: [{CoordinateFilePath}]");

            // 2. Wenn drawCADObjects = true und AutoCAD Map 3D offen >> Kreis mit Radius von 10m auf Layer "GEOBOX RemovePoints" erstellen, für jede Koordinate...
            // >> Später wird eine Option für das Setzten der Option auf dem UI erstellt.
            // >> Später wird eine Funktion erstellt, die nur die Punkte zeichnet > für die Prüfung durch den Benutzenden

            // 3. Punkte in den entsprechenden LINIEN Objektklassen entfernen
            // Linien Objektklassen => Koordinaten als Stützpunkte entfernen (wenn Linienanfangspunkt oder Linienendpunkt, dann nichts machen)
            logger.WriteInformation(Resources.LogMessage_StartRemoveFromLineClasses);

            var selectedLineFeatureClasses = GetSelectedLineFeatureClasses();
            if (selectedLineFeatureClasses != null & selectedLineFeatureClasses.Count() > 0)
            {
                var linePointsController = new LinePointsController(PointCoordinateDetails.ToList(), logger);

                foreach (var selectedFeatureClass in selectedLineFeatureClasses)
                {
                    linePointsController.RemovePointsFromLines(selectedFeatureClass);
                }
            }
            else
            {
                logger.WriteInformation(Resources.LogMessage_NoLineClassesSelected);
            }

            // 4. Punkte in den entsprechenden PUNKT Objektklassen entfernen
            // Punkt Objektklassen => Punkte entfernen (es werden keine Linien gesucht und geändert, nur der Punkt gelöscht)
            //logger.WriteInformation(Resources.LogMessage_StartRemoveFromPointClasses);

            //var selectedPointFeatureClasses = GetSelectedLineFeatureClasses();
            //if (selectedPointFeatureClasses != null & selectedPointFeatureClasses.Count() > 0)
            //{
            //    var pointPointsController = new PointPointsController(PointCoordinateDetails.ToList(), logger);

            //    foreach (var selectedFeatureClass in GetSelectedLineFeatureClasses())
            //    {
            //        pointPointsController.RemovePointsFromClasses(selectedFeatureClass);
            //    }
            //}
            //else
            //{
            //    logger.WriteInformation(Resources.LogMessage_NoPointClassesSelected);
            //}

            return true;
        }

        #region Coodinate Points
        public void ChooseCoordinateFile()
        {
            using (var openFileDialog = 
                new OpenFileDialog
                {
                    Multiselect = false,
                    CheckFileExists = true,
                    Filter = Resources.CoordinateFile_FileDialogFilter,
                    DefaultExt = Resources.CoordinateFile_FileDialogDefaultExt
                })
            {
                DialogResult result = openFileDialog.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(openFileDialog.FileName))
                {
                    CoordinateFilePath = openFileDialog.FileName;
                    ReadCoordinateFile();
                }
            }

            CheckIsReadyForRemove();
        }

        private bool ReadCoordinateFile()
        {
            if (string.IsNullOrWhiteSpace(CoordinateFilePath)) return false;

            CoordinateCollection list = null;
            try
            {
                var text = File.ReadAllText(CoordinateFilePath);
                list = JsonSerializer.Deserialize<CoordinateCollection>(text);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Resources.CoordnateFile_WrongContentErrorMessage} ({ex.Message})");
                return false;
            }

            PointCoordinateDetails.Clear();

            foreach (Coordinate jsonCoord in list)
            {
                PointCoordinateDetails.Add(new PointCoordinateDetail(jsonCoord.ID, jsonCoord.East, jsonCoord.North));
            }

            return true;
        }

        public void SelectAllCoodinates()
        {
            ChangeAllChechedOnCoodinates(true);
        }

        public void DeselectAllCoodinates()
        {
            ChangeAllChechedOnCoodinates(false);
        }

        private void ChangeAllChechedOnCoodinates(bool selected)
        {
            foreach (var coordPoint in PointCoordinateDetails)
            {
                coordPoint.IsSelected = selected;
            }

            CheckIsReadyForRemove();
        }
        #endregion Coodinate Points

        #region Logger
        public void ChooseLLogFileSavePath()
        {
            using (var saveFileDialog = 
                new SaveFileDialog
                {
                    Filter = Resources.LogFile_FileDialogFilter,
                    FilterIndex = 1,
                    DefaultExt = Resources.LogFile_FileDialogDefaultExt
                })
            {
                DialogResult result = saveFileDialog.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(saveFileDialog.FileName))
                {
                    LogFilePath = saveFileDialog.FileName;
                }
            }

            CheckIsReadyForRemove();
        }

        private ILogger CreateNewLogger()
        {
            ILogger logger = new CustomerFriendlyLogger(FileLogger.Create(logFilePath), true);
            ((CustomerFriendlyLogger)logger).WriteHeader(Resources.genModulName, Resources.LoggerComment);

            return logger;
        }
        #endregion Logger

        #region Feature Classes
        private void LoadFeatureClasses()
        {
            var featureClassController = new FeatureClassController(document);

            FeatureClassDetails = new ObservableCollection<FeatureClassDetail>();

            foreach (FeatureClassDetail featureClassDetail in featureClassController.GetFeatureClasses())
            {
                FeatureClassDetails.Add(featureClassDetail);
            }
        }

        private IEnumerable<FeatureClass> GetSelectedLineFeatureClasses()
        {
            var selectedFeatureClasses = FeatureClassDetails.Where(elem => elem.IsSelected & elem.IsLineFeatureClass).Select(elem => elem.MapFeatureClass);
            if (selectedFeatureClasses.Count() == 0)
            {
                return null;
            }

            return selectedFeatureClasses;
        }

        public void SelectAllFeatureClasses()
        {
            ChangeAllChechedOnFeatureClasses(true);
        }

        public void DeselectAllFeatureClasses()
        {
            ChangeAllChechedOnFeatureClasses(false);
        }

        private void ChangeAllChechedOnFeatureClasses(bool selected)
        {
            foreach (var fcDetail in FeatureClassDetails)
            {
                fcDetail.IsSelected = selected;
            }

            CheckIsReadyForRemove();
        }
        #endregion Feature Classes

        #region is ready
        /// <summary>
        /// Check are all inputs valid for run remove
        /// Activate or Deactivate RemovePointsButton
        /// </summary>
        public void CheckIsReadyForRemove()
        {
            if (!HasSelectedFeatureClasses())
            {
                IsReadyForRemove = false;
                return;
            }

            if (!HasSelectedPointCoordinates())
            {
                IsReadyForRemove = false;
                return;
            }

            if (string.IsNullOrEmpty(LogFilePath))
            {
                IsReadyForRemove = false;
                return;
            }

            IsReadyForRemove = true;
        }

        private bool HasSelectedFeatureClasses()
        {
            if (FeatureClassDetails == null) return false;

            foreach(var fcDetail in FeatureClassDetails)
            {
                if (fcDetail.IsSelected == true) return true;
            }

            return false;
        }

        private bool HasSelectedPointCoordinates()
        {
            if (PointCoordinateDetails == null) return false;

            foreach (var pointCoord in PointCoordinateDetails)
            {
                if (pointCoord.IsSelected == true) return true;
            }

            return false;
        }
        #endregion is ready

        #region Property Changed Handler

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Property Changed Handler


        #region Sample Data 
        /// <summary>
        /// Create Sample Data for debug and testing
        /// </summary>
        private void CreateSampleJson(string pathToJsonFile)
        {
            CoordinateCollection col = new CoordinateCollection();
            col.AddCoordinate(new Coordinate("260101", 2652300.094, 1260057.89));
            col.AddCoordinate(new Coordinate("260102", 2660250, 1252610.768));
            col.AddCoordinate(new Coordinate("260103", 2660250.232, 1251438.785));

            var content = JsonSerializer.Serialize<CoordinateCollection>(col);
            File.WriteAllText(pathToJsonFile, content);
        }
        #endregion
    }
}