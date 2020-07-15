using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.ApplicationServices;
using DesignAutomationFramework;
using System.Windows.Media.Imaging;

namespace CreateWalls
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]

    public class App : IExternalDBApplication
    {
        public static string ExecutingAssemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

        public ExternalDBApplicationResult OnStartup(ControlledApplication application)
        {
            // Crear Tab 1 Dynoscript
            // string tabName = "REACT-BIM";
            //application.CreateRibbonTab(tabName);

            // Crear Panel 1
            //RibbonPanel panel11 = application.CreateRibbonPanel(tabName, "Mini House Creation");


            // agregar un boton
            //PushButton button11 = panel11.AddItem(new PushButtonData("CreateWallsButton", "CREATE", ExecutingAssemblyPath, "CreateWalls.ThisApplication")) as PushButton;


            // agregar la imagen al button1
            //button11.LargeImage = new BitmapImage(new Uri("pack://application:,,,/CreateWalls;component/Resources/split-(2).png"));

            //button11.ToolTip = "..";
            //button11.LongDescription = "..";
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
            //SketchItParams jsonDeserialized = SketchItParams.Parse(filepathJson);

            //CreateWalls(jsonDeserialized, newDoc);

            //CreateFloors(jsonDeserialized, newDoc);

            Construct c = new Construct();
            c.place_WallsDoorsWindows(newDoc);

            newDoc.SaveAs(filePath);
        }
    }
}
