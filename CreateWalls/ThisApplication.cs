using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using CreateWallsDesignAutomation;
using Application = Autodesk.Revit.ApplicationServices.Application;

namespace CreateWallsCommon
{
    [TransactionAttribute(TransactionMode.Manual)]
	[RegenerationAttribute(RegenerationOption.Manual)]
	class ThisApplication : IExternalCommand
	{
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			//Get application and document objects
			UIApplication uiApp = commandData.Application;
			UIDocument uidoc = uiApp.ActiveUIDocument;
			Document doc = uiApp.ActiveUIDocument.Document;
			Application app = uiApp.Application;
			try
			{

				string filepathJson = @"A:\Users\mgray\OneDrive - ARCO\Desktop\tempJson.json";

				string filePath = "sketchIt.rvt";
				//string filepathJson = "SketchItInput.json";
				string filepathXML = @"A:\Users\mgray\OneDrive - ARCO\Desktop\xmlDocument.xml";

				Dictionary<string, double> levelsList = new Dictionary<string, double>();
				List<List<Point>> floorsList = new List<List<Point>>();
				List<Line> wallsList = new List<Line>();

				ReactJson json = new ReactJson();
				if (File.Exists(filepathJson))
				{
					json = ReactJson.Parse(filepathJson);
				}
				if (File.Exists(filepathXML))
				{
					XmlDocument xmlDocument = new XmlDocument();
					xmlDocument.Load(filepathXML);
					XmlNodeList levelsXmlList = xmlDocument.SelectNodes("//building/ProjectInformation/Levels/Level");
					foreach (XmlNode levelsXml in levelsXmlList)
					{
                        if (levelsXml.HasChildNodes)
                        {
                            XmlNode name = levelsXml.SelectSingleNode("Name");
                            XmlNode height = levelsXml.SelectSingleNode("Elevation");
                            if ((name != null) && (height != null))
								levelsList.Add(name.InnerText, Convert.ToDouble(height.InnerText));
                        }
					}
					XmlNodeList nodeList = xmlDocument.SelectNodes("//building/Floors/Floor");
					foreach (XmlNode floorsXml in nodeList)
					{
                        if (floorsXml.HasChildNodes)
                        {
                            XmlNode points = floorsXml.SelectSingleNode("BoundryPoints");
                            XmlNodeList pointList = points.ChildNodes;
                            List<Point> fl = new List<Point>();
							foreach (XmlNode node in pointList)
                            {
                                XmlNode x = node.SelectSingleNode("X");
                                XmlNode y = node.SelectSingleNode("Y");
                                XmlNode z = node.SelectSingleNode("Z");

                                if (x != null && y != null && z != null)
                                {
                                    XYZ pt = new XYZ(Convert.ToDouble(x.InnerText), Convert.ToDouble(y.InnerText), Convert.ToDouble(z.InnerText));
                                    Point r = Point.Create(pt);
                                    fl.Add(r);
								}
									
							}
                            floorsList.Add(fl);
						}
					}
					XmlNodeList wallList = xmlDocument.SelectNodes("//building/Walls/Wall");
					foreach (XmlNode wallsXml in wallList)
                    {
                        XmlNode start = wallsXml.SelectSingleNode("StartPoint");
                        XmlNode end = wallsXml.SelectSingleNode("EndPoint");
                        XmlNode heightNode = wallsXml.SelectSingleNode("Height");
                        if (start != null && end != null && heightNode != null)
                        {
                            XmlNode startX = start.SelectSingleNode("X");
                            XmlNode startY = start.SelectSingleNode("Y");
                            XmlNode startZ = start.SelectSingleNode("Z");
                            XmlNode endX = end.SelectSingleNode("X");
                            XmlNode endY = end.SelectSingleNode("Y");
                            XmlNode endZ = end.SelectSingleNode("Z");

                            if (startX != null && endX != null)
                            {
                                XYZ startPoint = new XYZ(Convert.ToDouble(startX.InnerText), Convert.ToDouble(startY.InnerText), Convert.ToDouble(startZ.InnerText));
                                XYZ endPoint = new XYZ(Convert.ToDouble(endX.InnerText), Convert.ToDouble(endY.InnerText), Convert.ToDouble(endZ.InnerText));
                                Line line = Line.CreateBound(startPoint, endPoint);
                                wallsList.Add(line);
							}
						}
					}
				}

				foreach (List<ReactJson.Point> floor in json.Floors)
				{
					List<Point> fl = new List<Point>();
					foreach (var k in floor)
					{
						XYZ pt = new XYZ(k.X, k.Y, k.Z);
						Point r = Point.Create(pt);
						fl.Add(r);
					}
					floorsList.Add(fl);
				}

				foreach (ReactJson.WallLine walls in json.Walls)
				{
					XYZ start = new XYZ(walls.Start.X, walls.Start.Y, walls.Start.Z);
					XYZ end = new XYZ(walls.End.X, walls.End.Y, walls.End.Z);
					Line line = Line.CreateBound(start, end);
					wallsList.Add(line);
				}

				foreach (ReactJson.Level level in json.Levels)
				{
					levelsList.Add(level.Name, level.Elevation);
				}

				CreateWallsCommon.Construct c = new CreateWallsCommon.Construct();
				c.CreateLevels(doc, levelsList);
				c.CreateFloors(floorsList, doc);
				c.CreateWalls(wallsList, doc);
			}
			catch (Exception e)
			{
				//TaskDialog.Show("Error", e.Message.ToString());
				throw;
			}
			return Result.Succeeded;
		}
	}
}
