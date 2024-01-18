using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System;
using System.Windows.Controls;


namespace Revit_CreateHomePanel.App.Plugins.CreateHomePanel.Filters
{
    public class SelectionFilter_DoorsHome : ISelectionFilter
    {
        private Document _document;

        public SelectionFilter_DoorsHome(Document document)
        {
            _document = document;
        }


        public bool AllowElement(Element elem)
        {
            return true;
        }


        public bool AllowReference(Reference reference, XYZ position)
        {
            RevitLinkInstance revitlinkinstance = _document.GetElement(reference) as RevitLinkInstance;
            Document docLink = revitlinkinstance.GetLinkDocument();
            Element element = docLink.GetElement(reference.LinkedElementId);

            FamilyInstance family = (FamilyInstance)element;

            if (family.SuperComponent != null)
                return false;

            if (element.Name.ToLower().Contains("квартирная наружная"))
                return true;

            return false;
        }
    }
}
