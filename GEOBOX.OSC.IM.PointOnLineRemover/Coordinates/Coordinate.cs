namespace GEOBOX.OSC.IM.PointOnLineRemover.Coordinates
{
    /// <summary>
    /// Coordinate Object
    /// Representing in JSON file
    /// </summary>
    public class Coordinate
    {
        /// <summary>
        /// Point identifaction
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// East - E/Y Wert (LV03 600'000; LV95 2'600'000)
        /// </summary>
        public double East { get; set; }
        /// <summary>
        /// North - N/X Wert (LV03 200'000; LV95 1'200'000)
        /// </summary>
        public double North { get; set; }

        /// <summary>
        /// Empty Constructor - using only for JsonSerializer.Deserialize
        /// </summary>
        public Coordinate() { }

        /// <summary>
        /// Constructor for using in sample 
        /// </summary>
        /// <param name="id">Point ID (Info for Logging)</param>
        /// <param name="east">East - E/Y Wert</param>
        /// <param name="north">North - N/X Wert</param>
        public Coordinate(string id, double east, double north)
        {
            this.ID = id;
            this.East = east;
            this.North = north;
        }
    }
}