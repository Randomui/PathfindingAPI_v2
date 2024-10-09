namespace PathfindingAPI_v2.Models
{
    public class PathRequest
    {
        public Coordinate Start { get; set; }
        public Coordinate End { get; set; }
        public List<Coordinate> Obstacles { get; set; }
    }

    public class Coordinate
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
}
