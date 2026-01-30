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
            // ToDo : Prüfen ist ein Log-Datei ausgewählt, wenn nicht wird eine Datei 
            if (string.IsNullOrEmpty(CoordinateFilePath))
            {
                //document.ApplicationObject.MessageBox("Es wurde kein Ordner ausgewählt!", "Ordner wählen!", System.Windows.Forms.MessageBoxIcon.Information);
                return false;
            }

            // ToDo : Prüfen ob Fachschale Job enabled ist oder nicht.
            // >> Wenn ja und kein Job in Bearbeitung ist - Meldung als MesssageBox anzeigen.

            // 1. Wenn drawCADObjects = true und AutoCAD Map 3D offen >> Kreis mit Radius von 10m auf Layer "GEOBOX RemovePoints" erstellen, für jede Koordinate...
            // >> Später wird eine Option für das Setzten der Option auf dem UI erstellt.
            // >> Später wird eine Funktion erstellt, die nur die Punkte zeichnet > für die Prüfung durch den Benutzenden

            // 2. Punkte in den entsprechenden Objektklassen entfernen
            // Prio 1
            // >> Linien Objektklassen => Koordinaten als Stützpunkte entfernen (wenn Linienanfangspunkt oder Linienendpunkt, dann nichts machen und eine Logmeldung ausgeben:
            // Meldung: WRN: Koordinate [X/Y] ist auf der Linie [FID der Linie] in der Objektklasse [FeatureClass-Caption] ein Anfangs- oder Endpunkt. Die Linie wurde nicht geändert.
            // Bei erfoglreichem entfernen: INF: Koordinate [X/Y] wurde auf der Linie [FID der Linie] in der Objektklasse [FeatureClass-Caption] als Stützpukt entfernt.
            // Prio 2
            // >> Punkt Objektklassen => Punkte entfernen (es werden keine Linien gesucht und geändert, nur der Punkt gelöscht)
            // Bei erfoglreichem entfernen: INF: Koordinate [X/Y] wurde als Punkt [FID des Punktes] in der Objektklasse [FeatureClass-Caption] entfernt.

            var selectedFeatureClasses = FeatureClassDetails.Where(elem => elem.IsSelected).Select(elem => elem.MapFeatureClass);
            if(selectedFeatureClasses.Count() == 0)
            {
                document.ApplicationObject.MessageBox("Es wurden Objekt-Klassen ausgewählt!", "Objekt-Klassen wählen!", MessageBoxIcon.Information);
                return false;
            }

            var linePointsController = new LinePointsController(PointCoordinateDetails.ToList(), CreateNewLogger());

            foreach (var selectedFeatureClass in selectedFeatureClasses)
            {
                linePointsController.RemovePointsFromLines(selectedFeatureClass);
            }

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
            ChangeAllChechedOnFeatureClasses(true);
        }

        public void DeselectAllCoodinates()
        {
            ChangeAllChechedOnFeatureClasses(false);
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
        private void CheckIsReadyForRemove()
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