using System.Collections.Generic;

namespace GEOBOX.OSC.IM.PointOnLineRemover.Coordinates
{
    public class CoordinateCollection
    {
        private List<Coordinate> coordinateCollection = new List<Coordinate>();

        public CoordinateCollection()
        {
        }

        public void AddCoordinate(Coordinate coordinate)
        {
            coordinateCollection.Add(coordinate);
        }

        public void AddCoordianteList(List<Coordinate> coordinateList)
        {
            coordinateCollection.AddRange(coordinateList);
        }
    }
}