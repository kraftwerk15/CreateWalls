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

				CreateBuilding createBuilding = new CreateBuilding();
                createBuilding.CreateBuildingElements(filepathJson, filepathXML, doc);

				
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
