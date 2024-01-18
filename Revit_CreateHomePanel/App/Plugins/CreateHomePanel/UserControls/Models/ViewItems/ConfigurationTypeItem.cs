using System;


namespace Revit_CreateHomePanel.App.Plugins.CreateHomePanel.UserControls.Models.ViewItems
{
    public enum ConfigurationType
    {
        twoPanels,
        onePanel
    }


    public class ConfigurationTypeItem
    {
        public string configurationName { get; set; }
        public ConfigurationType configurationType { get; set; }


        public ConfigurationTypeItem(string configurationName, ConfigurationType configurationType)
        {
            this.configurationName = configurationName;
            this.configurationType = configurationType;
        }
    }
}
