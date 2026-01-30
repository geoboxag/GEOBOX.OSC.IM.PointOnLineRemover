using System.Collections.Generic;

namespace GEOBOX.OSC.IM.PointOnLineRemover.Coordinates
{
    /// <summary>
    /// List with Coordinate Objects
    /// </summary>
    public class CoordinateCollection : List<Coordinate>
    {
        /// <summary>
        /// Empty Constructor - using only for JsonSerializer.Deserialize
        /// </summary>
        public CoordinateCollection() { }

        /// <summary>
        /// Add single coordinate to list
        /// </summary>
        /// <param name="coordinate"></param>
        public void AddCoordinate(Coordinate coordinate)
        {
            this.Add(coordinate);
        }

        /// <summary>
        /// Add multible coordiantes to list
        /// </summary>
        /// <param name="coordinateList"></param>
        public void AddCoordianteList(List<Coordinate> coordinateList)
        {
            this.AddRange(coordinateList);
        }
    }
}