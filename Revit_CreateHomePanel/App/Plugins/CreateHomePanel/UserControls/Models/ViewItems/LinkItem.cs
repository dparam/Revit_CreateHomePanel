using Autodesk.Revit.DB;


namespace Revit_CreateHomePanel.App.Plugins.CreateHomePanel.UserControls.Models.ViewItems
{
    public class LinkItem
    {
        public RevitLinkInstance linkElement { get; set; }
        public string linkName { get; set; }


        public LinkItem(RevitLinkInstance linkElement, string linkName)
        {
            this.linkElement = linkElement;
            this.linkName = linkName;
        }
    }
}
