using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using DesignAutomationFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace CreateWallsDesignAutomation
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]

    public class App : IExternalDBApplication
    {
        public static string ExecutingAssemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

        public ExternalDBApplicationResult OnStartup(ControlledApplication application)
        {
            DesignAutomationBridge.DesignAutomationReadyEvent += HandleDesignAutomationReadyEvent;
            return ExternalDBApplicationResult.Succeeded;
        }

        public ExternalDBApplicationResult OnShutdown(ControlledApplication application)
        {
            return ExternalDBApplicationResult.Succeeded;
        }

        public void HandleDesignAutomationReadyEvent(object sender, DesignAutomationReadyEventArgs e)
        {
            e.Succeeded = true;
            SketchItFunc(e.DesignAutomationData);
        }

        private static void SketchItFunc(DesignAutomationData data)
        {
            if (data == null)
                throw new InvalidDataException(nameof(data));

            Application rvtApp = data.RevitApp;
            if (rvtApp == null)
                throw new InvalidDataException(nameof(rvtApp));

            Document newDoc = rvtApp.NewProjectDocument(UnitSystem.Imperial);
            if (newDoc == null)
                throw new InvalidOperationException("Could not create new document.");
            string filePath = "sketchIt.rvt";
            string filepathJson = "SketchItInput.json";
            string filepathXML = "xmlDocument.xml";

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
                XmlNodeList levelsXmlList = xmlDocument.SelectNodes("//building/ProjectInformation/Levels");
                foreach (XmlNode levelsXml in levelsXmlList)
                {
                    //something here
                }
                XmlNodeList nodeList = xmlDocument.SelectNodes("//building/Floors");
                foreach (XmlNode floorsXml in nodeList)
                {
                    //something here
                }
                XmlNodeList wallList = xmlDocument.SelectNodes("//building/Walls");
                foreach (XmlNode wallsXml in wallList)
                {
                    //something here
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
            c.CreateLevels(newDoc, levelsList);
            c.CreateFloors(floorsList, newDoc);
            c.CreateWalls(wallsList, newDoc);

            newDoc.SaveAs(filePath);
        }
    }

    
}
