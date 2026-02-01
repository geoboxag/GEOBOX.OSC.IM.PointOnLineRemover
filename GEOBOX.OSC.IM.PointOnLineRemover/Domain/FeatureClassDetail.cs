using Autodesk.Map.IM.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GEOBOX.OSC.IM.PointOnLineRemover.Domain
{
    public class FeatureClassDetail : INotifyPropertyChanged, IDisposable
    {
        private bool isSelected = false;
        /// <summary>
        /// Feature Class is selected for remove points
        /// </summary>
        public bool IsSelected
        {
            set
            {
                isSelected = value;
                OnPropertyChanged("IsSelected");
            }
            get
            {
                return isSelected;
            }
        }

        private bool isLineFeatureClass = false;
        /// <summary>
        /// Feature Class - Line geometry
        /// </summary>
        public bool IsLineFeatureClass
        {
            set { isLineFeatureClass = value; }
            get { return isLineFeatureClass; }
        }

        private bool isPointFeatureClass = false;
        /// <summary>
        /// Feature Class - Point geometry
        /// </summary>
        public bool IsPointFeatureClass
        {
            set { isPointFeatureClass = value; }
            get { return isPointFeatureClass; }
        }

        private string displayName = "unbekannt (unknow)";
        /// <summary>
        /// Name for view in list on UI
        /// </summary>
        public string DisplayName
        {
            set
            {
                displayName = value;
                OnPropertyChanged("Selected");
            }
            get
            {
                return displayName;
            }
        }

        private FeatureClass mapFeatureClass;
        /// <summary>
        /// MAP.API instance for FeatureClass
        /// </summary>
        public FeatureClass MapFeatureClass {
            set
            {
                mapFeatureClass = value;
                if (mapFeatureClass != null)
                {
                    if (mapFeatureClass.Type == FeatureClassType.Point) { IsPointFeatureClass = true; }
                    else if (mapFeatureClass.Type == FeatureClassType.LineString) { IsLineFeatureClass = true; }
                }
            }
            get
            {
                return mapFeatureClass;
            }
        }


        /// <summary>
        /// Create new Entry
        /// </summary>
        /// <param name="displayText"></param>
        /// <param name="featureClass"></param>
        public FeatureClassDetail(string displayText, FeatureClass featureClass)
        {
            IsSelected = true;
            DisplayName = displayText;
            MapFeatureClass = featureClass;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects).
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

        #region Property Changed Handler
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}