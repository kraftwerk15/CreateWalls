using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CreateWallsDesignAutomation;
using System.IO;
using System.Xml;

// ReSharper disable once CheckNamespace
namespace CreateWallsCommon
{
    internal class Construct
    {
        internal void CreateLevels(Document doc, Dictionary<string, double> levels)
        {
            
            
            using (Transaction levelTrans = new Transaction(doc, "Create some levels"))
            {
                levelTrans.Start();

                foreach (KeyValuePair<string, double> level in levels)
                {
                    Level l = Level.Create(doc, level.Value);
                    l.Name = level.Key;
                }

                levelTrans.Commit();
            }
		}
        
        internal void CreateWalls(List<Line> jsonReact, Document newDoc)
		{
			FilteredElementCollector levelCollector = new FilteredElementCollector(newDoc);
			levelCollector.OfClass(typeof(Level));
			ElementId someLevelId = levelCollector.FirstElementId();
			if (someLevelId == null || someLevelId.IntegerValue < 0) throw new System.IO.InvalidDataException("ElementID is invalid.");

			List<Line> curves = jsonReact;
			//foreach (ReactJson.WallLine lines in jsonReact)
			//{
			//	XYZ start = new XYZ(lines.Start.X, lines.Start.Y, lines.Start.Z);
			//	XYZ end = new XYZ(lines.End.X, lines.End.Y, lines.End.Z);
			//	curves.Add(Line.CreateBound(start, end));
			//}

			using (Transaction wallTrans = new Transaction(newDoc, "Create some walls"))
			{
				wallTrans.Start();

				foreach (Line oneCurve in curves)
				{
					Wall.Create(newDoc, oneCurve, someLevelId, false);
				}

				wallTrans.Commit();
			}
		}

		internal void CreateFloors(List<List<Point>> jsonReact, Document newDoc)
		{
			foreach (List<Point> floorPoints in jsonReact)
			{
				CurveArray floor = new CurveArray();
				int lastPointOnFloor = floorPoints.Count - 1;
				
				for (int pointNum = 0; pointNum <= lastPointOnFloor; pointNum++)
				{
					XYZ startPoint = new XYZ(floorPoints[pointNum].Coord.X, floorPoints[pointNum].Coord.Y, floorPoints[pointNum].Coord.Z);
					XYZ endPoint;

					if (pointNum == lastPointOnFloor)
					{
						endPoint = new XYZ(floorPoints[0].Coord.X, floorPoints[0].Coord.Y, floorPoints[0].Coord.Z);
					}
					else
					{
						endPoint = new XYZ(floorPoints[pointNum + 1].Coord.X, floorPoints[pointNum + 1].Coord.Y, floorPoints[pointNum + 1].Coord.Z);
					}

					Curve partOfFloor = Line.CreateBound(startPoint, endPoint);
					floor.Append(partOfFloor);
				}

				using (Transaction floorTrans = new Transaction(newDoc, "Create a floor"))
				{
					floorTrans.Start();
					newDoc.Create.NewFloor(floor, false);
					floorTrans.Commit();
				}
			}
		}

		public void place_WallsDoorsWindows(Document doc)
		{

			//<<----------------------------------------------------INPUTS---------------------------------------------------------->>

			//level
			Level level = GetLevel(doc, "Nivel 1"); // 1.

			// coordenadas XYZ(x,y,z)
			XYZ stPoint = new XYZ(0, 0, 0); // 2.
			XYZ endPoint = new XYZ(0, 0, 0); // 3.

			// Dimensions
			// type of House Shapes :  square, rectangle, triangle
			string n = "rectangle"; // 4.

			// Doors in the RIGHT
			bool door_RightWall = false;
			bool wind_RigthWall = true; // 5.


			// Ventanas in the LEFT
			bool door_LeftWall = true; // 6.
			bool wind_LeftWall = false;

			double _heigth_ = 2880 / 304.8; // 7.
			double h_window = 800 / 304.8; // 8.

			//<<------------------------------------------------------INPUTS-------------------------------------------------------->>


			// Create House in differents Shapes :  Square, Rectangle, Triangle
			List<List<Wall>> walls = Create_House_Shapes(doc, n, door_RightWall, wind_RigthWall, door_LeftWall, wind_LeftWall, _heigth_);

			List<Wall> walls_Rigth = walls.First();
			List<Wall> walls_Left = walls.Last();

			//			TaskDialog.Show("REACT-BIM", aa_Rigth.Count().ToString() + " Walls in the Rigth" + Environment.NewLine + 
			//			                									aa_Left.Count().ToString() + " Walls in the Left");

			if (door_LeftWall)
			{
				foreach (Wall wa in walls_Left)
				{
					CreateDoor(doc, wa);
				}
			}
			if (wind_RigthWall)
			{
				foreach (Wall wa in walls_Rigth)
				{
					CreateWindow(doc, wa, h_window);
				}
			}

		}



		// input: "Nivel 1"
		Level GetLevel(Document doc, string input)
		{

			View active = doc.ActiveView;


			// INPUT LEVEL
			FilteredElementCollector lvlCollector = new FilteredElementCollector(doc);
			ICollection<Element> lvlCollection = lvlCollector.OfClass(typeof(Level)).ToElements();

			string msg = "";

			List<Level> salida = new List<Level>();
			foreach (Element l in lvlCollection)
			{

				msg = msg + l.Name.ToString() + "\n" + Environment.NewLine;
				if (l.Name.ToString() == input)
				{
					salida.Add(l as Level);
				}

			}


			//TaskDialog.Show("test", salida.First().Name.ToString());

			return salida.First();

		}

		// inputs: n=shape 
		List<List<Wall>> Create_House_Shapes(Document doc, string n, bool a, bool b, bool c, bool d, double _heigth_)
		{


			//list walls output
			List<Wall> output_wall_Rigth = new List<Wall>();
			List<Wall> output_wall_Left = new List<Wall>();

			List<List<Wall>> output = new List<List<Wall>>();

			//inputs

			//level
			Level level = GetLevel(doc, "Nivel 1");

			// coord XYZ(x,y,z)
			XYZ stPoint = new XYZ(0, 0, 0);

			//Door & windows
			bool door_RightWall = a;
			bool wind_RigthWall = b;
			bool door_LeftWall = c;
			bool wind_LeftWall = d;

			Transaction trans = new Transaction(doc);

			trans.Start("Create House");

			if (n == "square")
			{

				#region square
				//TaskDialog.Show("ALERTA", "--------------------------------------------------------------------------");

				double heigth_double = _heigth_;

				// Crear Wall Primer

				XYZ endPoint = new XYZ(30, 0, 0);
				XYZ endPoint_2 = new XYZ(30, 30, 0);
				XYZ endPoint_3 = new XYZ(0, 30, 0);

				Line newLineN = Line.CreateBound(stPoint, endPoint);
				Line newLineN_2 = Line.CreateBound(endPoint, endPoint_2);
				Line newLineN_3 = Line.CreateBound(endPoint_2, endPoint_3);
				Line newLineN_4 = Line.CreateBound(endPoint_3, stPoint);

				Wall wall = Wall.Create(doc, newLineN, level.Id, false);
				Wall wall_2 = Wall.Create(doc, newLineN_2, level.Id, false);
				Wall wall_3 = Wall.Create(doc, newLineN_3, level.Id, false);
				Wall wall_4 = Wall.Create(doc, newLineN_4, level.Id, false);

				List<Wall> lista_walls = new List<Wall>();

				lista_walls.Add(wall);
				lista_walls.Add(wall_2);
				lista_walls.Add(wall_3);
				lista_walls.Add(wall_4);

				output_wall_Rigth.Add(wall_2);
				output_wall_Rigth.Add(wall_4);
				output_wall_Left.Add(wall);
				output_wall_Left.Add(wall_3);

				output.Add(output_wall_Rigth);
				output.Add(output_wall_Left);


				foreach (Wall e in lista_walls)
				{
					Parameter height = e.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
					if (!height.IsReadOnly)
					{
						height.Set(heigth_double);
					}

					if (WallUtils.IsWallJoinAllowedAtEnd(e, 1))
						WallUtils.DisallowWallJoinAtEnd(e, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(e, 0))
						WallUtils.DisallowWallJoinAtEnd(e, 0);


				}

				#endregion

			}
			else if (n == "rectangle")
			{

				#region rectangle

				double heigth_double = _heigth_;

				// Crear Wall Primer

				XYZ endPoint = new XYZ(20, 0, 0);
				XYZ endPoint_2 = new XYZ(20, 10, 0);
				XYZ endPoint_3 = new XYZ(0, 10, 0);

				Line newLineN = Line.CreateBound(stPoint, endPoint);
				Line newLineN_2 = Line.CreateBound(endPoint, endPoint_2);
				Line newLineN_3 = Line.CreateBound(endPoint_2, endPoint_3);
				Line newLineN_4 = Line.CreateBound(endPoint_3, stPoint);

				Wall wall = Wall.Create(doc, newLineN, level.Id, false);
				Wall wall_2 = Wall.Create(doc, newLineN_2, level.Id, false);
				Wall wall_3 = Wall.Create(doc, newLineN_3, level.Id, false);
				Wall wall_4 = Wall.Create(doc, newLineN_4, level.Id, false);

				List<Wall> lista_walls = new List<Wall>();

				lista_walls.Add(wall);
				lista_walls.Add(wall_2);
				lista_walls.Add(wall_3);
				lista_walls.Add(wall_4);

				output_wall_Rigth.Add(wall_2);
				output_wall_Rigth.Add(wall_4);
				output_wall_Left.Add(wall);
				output_wall_Left.Add(wall_3);

				output.Add(output_wall_Rigth);
				output.Add(output_wall_Left);

				foreach (Wall e in lista_walls)
				{
					Parameter height = e.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
					if (!height.IsReadOnly)
					{
						height.Set(heigth_double);
					}

					if (WallUtils.IsWallJoinAllowedAtEnd(e, 1))
						WallUtils.DisallowWallJoinAtEnd(e, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(e, 0))
						WallUtils.DisallowWallJoinAtEnd(e, 0);


				}

				#endregion

			}
			else if (n == "triangle")
			{

				#region triangle


				double heigth_double = _heigth_;


				// Crear Wall Primer

				XYZ endPoint = new XYZ(20, 0, 0);
				XYZ endPoint_2 = new XYZ(10, 10 * 1.7320508075, 0);

				Line newLineN = Line.CreateBound(stPoint, endPoint);
				Line newLineN_2 = Line.CreateBound(endPoint, endPoint_2);
				Line newLineN_3 = Line.CreateBound(endPoint_2, stPoint);


				Wall wall = Wall.Create(doc, newLineN, level.Id, false);
				Wall wall_2 = Wall.Create(doc, newLineN_2, level.Id, false);
				Wall wall_3 = Wall.Create(doc, newLineN_3, level.Id, false);


				List<Wall> lista_walls = new List<Wall>();

				lista_walls.Add(wall);
				lista_walls.Add(wall_2);
				lista_walls.Add(wall_3);

				output_wall_Rigth.Add(wall);
				output_wall_Rigth.Add(wall_2);
				output_wall_Rigth.Add(wall_3);

				output.Add(output_wall_Rigth);

				foreach (Wall e in lista_walls)
				{
					Parameter height = e.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
					if (!height.IsReadOnly)
					{
						height.Set(heigth_double);
					}

					if (WallUtils.IsWallJoinAllowedAtEnd(e, 1))
						WallUtils.DisallowWallJoinAtEnd(e, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(e, 0))
						WallUtils.DisallowWallJoinAtEnd(e, 0);
				}

				#endregion
			}

			trans.Commit();


			return output;

		}


		void CreateWindow(Document doc, Wall _wall_, double _height_)
		{


			string fsFamilyName = "M_VANO VENTANA";
			string fsName = "1220x1240";
			string levelName = "Nivel 1";

			Wall wall = _wall_ as Wall; // muro actual

			Curve wallCurve = ((LocationCurve)wall.Location).Curve;

			double stParam = wallCurve.GetEndParameter(0);
			double endParam = wallCurve.GetEndParameter(1);

			Parameter longi = wall.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH);
			double longi_double = longi.AsDouble(); // 1220

			double h_window = _height_;

			double midle_wall_longi = stParam + longi_double / 2;

			XYZ point_midle_wall_longi = wallCurve.Evaluate(midle_wall_longi, false);


			XYZ stPoint = wallCurve.Evaluate(stParam, false);
			XYZ endPoint = wallCurve.Evaluate(endParam, false);



			// LINQ to find the window's FamilySymbol by its type name.
			FamilySymbol familySymbol = (from fs in new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).Cast<FamilySymbol>()
										 where (fs.Family.Name == fsFamilyName && fs.Name == fsName)
										 select fs).First();

			// LINQ to find the level by its name.
			Level level = (from lvl in new FilteredElementCollector(doc).
						   OfClass(typeof(Level)).
						   Cast<Level>()
						   where (lvl.Name == levelName)
						   select lvl).First();




			// Convert coordinates to double and create XYZ point.

			XYZ xyz = new XYZ(point_midle_wall_longi.X, point_midle_wall_longi.Y, level.Elevation + h_window);


			// Create window.
			using (Transaction t = new Transaction(doc, "Create window"))
			{
				t.Start();

				if (!familySymbol.IsActive)
				{
					// Ensure the family symbol is activated.
					familySymbol.Activate();
					doc.Regenerate();
				}

				// Create window
				// unliss you specified a host, Rebit will create the family instance as orphabt object.
				FamilyInstance window = doc.Create.NewFamilyInstance(xyz, familySymbol, wall, StructuralType.NonStructural);
				t.Commit();
			}
			//            string prompt = "The element was created!";
			//            TaskDialog.Show("Revit", prompt);
		}


		void CreateDoor(Document doc, Wall _wall_)
		{

			string fsFamilyName = "M_VANO PUERTA";
			string fsName = "750x2050";
			string levelName = "Nivel 1";

			Wall wall = _wall_ as Wall; // muro actual

			Curve wallCurve = ((LocationCurve)wall.Location).Curve;

			double stParam = wallCurve.GetEndParameter(0);
			double endParam = wallCurve.GetEndParameter(1);

			Parameter longi = wall.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH);
			double longi_double = longi.AsDouble(); // 1220

			double midle_wall_longi = stParam + longi_double / 2;

			XYZ point_midle_wall_longi = wallCurve.Evaluate(midle_wall_longi, false);


			XYZ stPoint = wallCurve.Evaluate(stParam, false);
			XYZ endPoint = wallCurve.Evaluate(endParam, false);



			// LINQ to find the window's FamilySymbol by its type name.
			FamilySymbol familySymbol = (from fs in new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).Cast<FamilySymbol>()
										 where (fs.Family.Name == fsFamilyName && fs.Name == fsName)
										 select fs).First();

			// LINQ to find the level by its name.
			Level level = (from lvl in new FilteredElementCollector(doc).
						   OfClass(typeof(Level)).
						   Cast<Level>()
						   where (lvl.Name == levelName)
						   select lvl).First();




			// Convert coordinates to double and create XYZ point.

			XYZ xyz = new XYZ(point_midle_wall_longi.X, point_midle_wall_longi.Y, level.Elevation);

			#region 

			#endregion

			// Create window.
			using (Transaction t = new Transaction(doc, "Create window"))
			{
				t.Start();

				if (!familySymbol.IsActive)
				{
					// Ensure the family symbol is activated.
					familySymbol.Activate();
					doc.Regenerate();
				}

				// Create window
				// unliss you specified a host, Rebit will create the family instance as orphabt object.
				FamilyInstance window = doc.Create.NewFamilyInstance(xyz, familySymbol, wall, StructuralType.NonStructural);
				t.Commit();
			}
			//            string prompt = "The element was created!";
			//            TaskDialog.Show("Revit", prompt);
		}
	}

	internal class CreateBuilding
	{
		public void CreateBuildingElements(string jsonFilePath, string xmlFilePath, Document doc)
		{
			Dictionary<string, double> levelsList = new Dictionary<string, double>();
			List<List<Point>> floorsList = new List<List<Point>>();
			List<Line> wallsList = new List<Line>();

			ReactJson json = new ReactJson();
			if (File.Exists(jsonFilePath))
			{
				json = ReactJson.Parse(jsonFilePath);
			}
			if (File.Exists(xmlFilePath))
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(xmlFilePath);
				XmlNodeList levelsXmlList = xmlDocument.SelectNodes("//Building/ProjectInformation/Levels/Level");
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

				XmlNodeList nodeList = xmlDocument.SelectNodes("//Building/Floors/Floor");
				if (nodeList != null)
				{
					foreach (XmlNode floorsXml in nodeList)
					{
						if (!floorsXml.HasChildNodes) continue;
						XmlNode points = floorsXml.SelectSingleNode("BoundryPoints");
						XmlNodeList pointList = points.ChildNodes;
						List<Point> fl = new List<Point>();
						foreach (XmlNode node in pointList)
						{
							XmlNode x = node.SelectSingleNode("X");
							XmlNode y = node.SelectSingleNode("Y");
							XmlNode z = node.SelectSingleNode("Z");

							if (x == null || y == null || z == null) continue;
							XYZ pt = new XYZ(Convert.ToDouble(x.InnerText), Convert.ToDouble(y.InnerText), Convert.ToDouble(z.InnerText));
							Point r = Point.Create(pt);
							fl.Add(r);

						}
						floorsList.Add(fl);
					}
				}

				XmlNodeList wallList = xmlDocument.SelectNodes("//Building/Walls/Wall");
				if (wallList != null)
				{
					foreach (XmlNode wallsXml in wallList)
					{
						if (!wallsXml.HasChildNodes) continue;
						XmlNode start = wallsXml.SelectSingleNode("StartPoint");
						XmlNode end = wallsXml.SelectSingleNode("EndPoint");
						XmlNode heightNode = wallsXml.SelectSingleNode("Height");

						if (start == null || end == null || heightNode == null) continue;
						XmlNode startX = start.SelectSingleNode("X");
						XmlNode startY = start.SelectSingleNode("Y");
						XmlNode startZ = start.SelectSingleNode("Z");
						XmlNode endX = end.SelectSingleNode("X");
						XmlNode endY = end.SelectSingleNode("Y");
						XmlNode endZ = end.SelectSingleNode("Z");

						if (startX == null || endX == null) continue;
						XYZ startPoint = new XYZ(Convert.ToDouble(startX.InnerText), Convert.ToDouble(startY.InnerText), Convert.ToDouble(startZ.InnerText));
						XYZ endPoint = new XYZ(Convert.ToDouble(endX.InnerText), Convert.ToDouble(endY.InnerText), Convert.ToDouble(endZ.InnerText));
						Line line = Line.CreateBound(startPoint, endPoint);
						wallsList.Add(line);
					}
				}
			}

			if (json.Floors.Count > 0)
			{
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
			}

			if (json.Walls.Count > 0)
			{
				foreach (ReactJson.WallLine walls in json.Walls)
				{
					XYZ start = new XYZ(walls.Start.X, walls.Start.Y, walls.Start.Z);
					XYZ end = new XYZ(walls.End.X, walls.End.Y, walls.End.Z);
					Line line = Line.CreateBound(start, end);
					wallsList.Add(line);
				}
			}

			if (json.Levels.Count > 0)
			{
				foreach (ReactJson.Level level in json.Levels)
				{
					levelsList.Add(level.Name, level.Elevation);
				}
			}

			CreateWallsCommon.Construct c = new CreateWallsCommon.Construct();
			c.CreateLevels(doc, levelsList);
			c.CreateFloors(floorsList, doc);
			c.CreateWalls(wallsList, doc);
		}
	}
}
