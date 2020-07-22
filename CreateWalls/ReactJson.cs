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
        [JsonProperty(PropertyName = "walls")]
        public IList<WallLine> Walls { get; set; }
        [JsonProperty(PropertyName = "floors")]
        public IList<IList<Point>> Floors { get; set; }
        [JsonProperty(PropertyName = "Levels")]
        public IList<Level> Levels { get; set; }
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
        internal class Level
        {
            [JsonProperty(PropertyName = "Elevation")]
            public double Elevation { get; set; }
            [JsonProperty(PropertyName = "Name")]
            public string Name { get; set; }
        }
    }
}
