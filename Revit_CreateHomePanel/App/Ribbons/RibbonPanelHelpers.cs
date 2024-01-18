using Autodesk.Revit.UI;
using Revit_CreateHomePanel.App.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration.Assemblies;
using System.Reflection;
using System.Windows.Media.Imaging;


namespace Revit_CreateHomePanel.App.Ribbons
{
    public static class RibbonPanelHelpers
    {
        private static string _location = Assembly.GetExecutingAssembly().Location;

        public static PushButtonData CreateButton(
            string buttonName,
            string buttonText,
            string buttonToolTip,
            string buttonCommand,
            string buttonIconFileName)
        {
            PushButtonData pushButtonData = new PushButtonData(buttonName, buttonText, _location, buttonCommand);

            BitmapImage iconImage = new BitmapImage(new Uri($"{AppData.RESOURCES}{buttonIconFileName}"));
            pushButtonData.LargeImage = iconImage;
            pushButtonData.Image = iconImage;
            pushButtonData.ToolTip = buttonToolTip;

            return pushButtonData;
        }


        public static SplitButtonData CreateSplitButtonGroup(RibbonPanel panel, List<PushButtonData> buttonsDataList, string name)
        {
            SplitButtonData groupData = new SplitButtonData(name, "Split Group 1");
            SplitButton group = panel.AddItem(groupData) as SplitButton;
            PushButton firstButton = null;

            foreach (PushButtonData data in buttonsDataList)
            {
                PushButton button = group.AddPushButton(data);

                if (firstButton == null)
                    firstButton = button;
            }

            group.CurrentButton = firstButton;

            return groupData;
        }
    }
}