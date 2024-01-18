using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Revit_CreateHomePanel.App.Helpers;
using Revit_CreateHomePanel.App.Plugins.CreateHomePanel.UserControls.Views;
using System.Windows;
using System.Xml;


namespace Revit_CreateHomePanel.App.Plugins.CreateHomePanel.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class Command_CreateHomePanel_ShowWindow : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApplication = commandData.Application;
            Document document = uiApplication.ActiveUIDocument.Document;

            if (document.IsFamilyDocument)
            {
                MessageBox.Show("Плагин работает только с моделями");
                return Result.Failed;
            }

            //

            Window_CreateHomePanel window = new Window_CreateHomePanel(document);
            window.ShowDialog();

            //

            return Result.Succeeded;
        }
    }
}
