using Autodesk.Revit.DB;


namespace Revit_CreateHomePanel.App.Plugins.CreateHomePanel.UserControls.Models.ViewItems
{
    public class PanelTypeItem
    {
        public FamilySymbol panelFamilySymbol { get; set; }
        public string panelFamilySymbolName { get; set; }


        public PanelTypeItem(FamilySymbol panelFamilySymbol, string panelFamilySymbolName)
        {
            this.panelFamilySymbol = panelFamilySymbol;
            this.panelFamilySymbolName = panelFamilySymbolName;
        }
    }
}
