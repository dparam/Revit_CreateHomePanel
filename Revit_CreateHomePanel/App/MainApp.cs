using Autodesk.Revit.UI;
using Revit_CreateHomePanel.App.Ribbons;
using System.Collections.Generic;


namespace Revit_CreateHomePanel.App
{
    public class MainApp : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication uiControlledApp)
        {
            string tabName = "Revit_CreateHomePanel";

            RibbonPanel ribbonPanel = uiControlledApp.CreateRibbonPanel(tabName);
            CreateRibbonPanel(uiControlledApp, ribbonPanel);
            return Result.Succeeded;
        }


        public Result OnShutdown(UIControlledApplication uiControlledApp)
        {
            return Result.Succeeded;
        }


        //


        public void CreateRibbonPanel(UIControlledApplication uiControlledApp, RibbonPanel ribbonPanel)
        {
            var b21 = RibbonPanelHelpers.CreateButton(
                "BTN_CreateHomePanel",
                "Расставить\nЩК + ЩК-СС",
                "Автоматическая расстановка ЩК и ЩК-СС над дверными проёмами квартир",
                "Revit_CreateHomePanel.App.Plugins.CreateHomePanel.Commands.Command_CreateHomePanel_ShowWindow",
                "icon_CreateHomePanel.ico");

            RibbonPanelHelpers.CreateSplitButtonGroup(ribbonPanel, new List<PushButtonData> { b21 }, "Расстановка элементов");
        }
    }
}
