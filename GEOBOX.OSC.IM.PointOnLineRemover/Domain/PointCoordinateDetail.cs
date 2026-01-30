using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GEOBOX.OSC.IM.PointOnLineRemover.Domain
{
    public class PointCoordinateDetail : INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// Unique object identifier. Because the point identifaction (ID) is not unique.
        /// </summary>
        public string UUID { get; init; }

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

        private string id;
        /// <summary>
        /// Point identifaction
        /// </summary>
        public string ID
        {
            set
            {
                id = value;
                OnPropertyChanged("ID");
            }
            get
            {
                return id;
            }
        }

        private double east;
        /// <summary>
        /// East - E/Y Wert (LV03 600'000; LV95 2'600'000)
        /// </summary>
        public double East
        {
            set
            {
                east = value;
                OnPropertyChanged("East");
            }
            get
            {
                return east;
            }
        }

        private double north;
        /// <summary>
        /// North - N/X Wert (LV03 200'000; LV95 1'200'000)
        /// </summary>
        public double North
        {
            set
            {
                north = value;
                OnPropertyChanged("North");
            }
            get
            {
                return north;
            }
        }

        public PointCoordinateDetail(string id, double east, double north)
        {
            UUID = Guid.NewGuid().ToString();
            IsSelected = true;
            ID = id;
            East = east;
            North = north;
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