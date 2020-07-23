using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Newtonsoft.Json;

namespace CreateWallsDesignAutomation
{
    internal class ReactJson
    {
        //[JsonProperty(PropertyName = "Walls")]
        //public IList<WallLine> Walls { get; set; }
        //[JsonProperty(PropertyName = "Floors")]
        //public IList<IList<Point>> Floors { get; set; }
        //[JsonProperty(PropertyName = "Levels")]
        //public IList<Level> Levels { get; set; }

        [JsonProperty("ProjectInformation")]
        public ProjectInformation _ProjectInformation { get; set; }

        [JsonProperty("Floors")]
        public Floors FloorsList { get; set; }

        [JsonProperty("Walls")]
        public Walls WallsList { get; set; }


        public static ReactJson Parse(string jsonPath)
        {
            try
            {
                if (!File.Exists(jsonPath))
                    return new ReactJson();

                string jsonContents = File.ReadAllText(jsonPath);
                return JsonConvert.DeserializeObject<ReactJson>(jsonContents);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception happens when parsing the json file: " + ex);
                return null;
            }
        }

        internal class Point
        {
            [JsonProperty(PropertyName = "X")]
            public double X { get; set; } = 0.0;
            [JsonProperty(PropertyName = "Y")]
            public double Y { get; set; } = 0.0;
            [JsonProperty(PropertyName = "Z")]
            public double Z { get; set; } = 0.0;
        }
        internal class WallLine
        {
            [JsonProperty(PropertyName = "StartPoint")]
            public Point Start { get; set; }
            [JsonProperty(PropertyName = "EndPoint")]
            public Point End { get; set; }
            [JsonProperty(PropertyName = "Height")]
            public double Height { get; set; }
        }

        public class Units
        {
            [JsonProperty("IsMetric")]
            public string IsMetric { get; set; }
        }

        public class Rotation
        {
            [JsonProperty("Point")]
            public IList<Point> Point { get; set; }

            [JsonProperty("Radians")]
            public string Radians { get; set; }
        }

        public class OriginPointFromMaster
        {
            [JsonProperty("Point")]
            public IList<Point> Point { get; set; }

            [JsonProperty("Rotation")]
            public Rotation Rotation { get; set; }
        }

        public class CoordinateSystem
        {
            [JsonProperty("Id")]
            public string Id { get; set; }

            [JsonProperty("Name")]
            public string Name { get; set; }

            [JsonProperty("OriginPointFromMaster")]
            public OriginPointFromMaster OriginPointFromMaster { get; set; }
        }

        public class Grid
        {
            [JsonProperty("Name")]
            public string Name { get; set; }

            [JsonProperty("Point")]
            public IList<Point> Point { get; set; }
        }

        public class GridSystem
        {
            [JsonProperty("Id")]
            public string Id { get; set; }

            [JsonProperty("Name")]
            public string Name { get; set; }

            [JsonProperty("Grid")]
            public Grid Grid { get; set; }
        }

        public class Level
        {
            [JsonProperty("Elevation")]
            public string Elevation { get; set; }

            [JsonProperty("Name")]
            public string Name { get; set; }
        }

        public class Levels
        {
            [JsonProperty("Level")]
            public IList<Level> Level { get; set; }
        }

        public class ProjectInformation
        {
            [JsonProperty("Units")]
            public Units Units { get; set; }

            [JsonProperty("CoordinateSystem")]
            public CoordinateSystem CoordinateSystem { get; set; }

            [JsonProperty("GridSystem")]
            public GridSystem GridSystem { get; set; }

            [JsonProperty("Levels")]
            public Levels Levels { get; set; }
        }

        public class BoundryPoints
        {
            [JsonProperty("Point")]
            public IList<Point> Point { get; set; }
        }

        public class Floor
        {
            [JsonProperty("ComponentName")]
            public string ComponentName { get; set; }

            [JsonProperty("BoundryPoints")]
            public BoundryPoints BoundryPoints { get; set; }
        }

        public class Floors
        {
            [JsonProperty("Floor")]
            public IList<Floor> Floor { get; set; }
        }

        public class StartPoint
        {
            [JsonProperty("X")]
            public string X { get; set; }

            [JsonProperty("Y")]
            public string Y { get; set; }

            [JsonProperty("Z")]
            public string Z { get; set; }
        }

        public class EndPoint
        {
            [JsonProperty("X")]
            public string X { get; set; }

            [JsonProperty("Y")]
            public string Y { get; set; }

            [JsonProperty("Z")]
            public string Z { get; set; }
        }

        public class Wall
        {
            [JsonProperty("ComponentName")]
            public string ComponentName { get; set; }

            [JsonProperty("StartPoint")]
            public StartPoint StartPoint { get; set; }

            [JsonProperty("EndPoint")]
            public EndPoint EndPoint { get; set; }

            [JsonProperty("Height")]
            public string Height { get; set; }
        }

        public class Walls
        {
            [JsonProperty("Wall")]
            public IList<Wall> Wall { get; set; }
        }

        public class React
        {
            
        }
    }
}
