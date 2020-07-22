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

            CreateWallsCommon.CreateBuilding cwc = new CreateWallsCommon.CreateBuilding();
            cwc.CreateBuildingElements(filepathJson, filepathXML, newDoc);
            newDoc.SaveAs(filePath);
        }
    }

    
}
