using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows.Media.Imaging;

namespace CreateWalls
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]

    class App : IExternalApplication
    {
        public static string ExecutingAssemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

        public Result OnStartup(UIControlledApplication application)
        {
            // Crear Tab 1 Dynoscript
            string tabName = "REACT-BIM";
            application.CreateRibbonTab(tabName);

            // Crear Panel 1
            RibbonPanel panel11 = application.CreateRibbonPanel(tabName, "Mini House Creation");


            // agregar un boton
            PushButton button11 = panel11.AddItem(new PushButtonData("CreateWallsButton", "CREATE", ExecutingAssemblyPath, "CreateWalls.ThisApplication")) as PushButton;


            // agregar la imagen al button1
            //button11.LargeImage = new BitmapImage(new Uri("pack://application:,,,/CreateWalls;component/Resources/split-(2).png"));

            button11.ToolTip = "..";
            button11.LongDescription = "..";

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {

            return Result.Succeeded;
        }


    }
}
