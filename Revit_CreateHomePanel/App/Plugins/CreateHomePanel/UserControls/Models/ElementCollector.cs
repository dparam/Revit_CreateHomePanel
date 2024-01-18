using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Revit_CreateHomePanel.App.Plugins.CreateHomePanel.Filters;
using Revit_CreateHomePanel.App.Plugins.CreateHomePanel.UserControls.Models.ViewItems;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;


namespace Revit_CreateHomePanel.App.Plugins.CreateHomePanel.UserControls.Models
{
    public static class ElementCollector
    {
        // LinkItems

        public static List<LinkItem> CollectLinks(Document document)
        {
            List<LinkItem> resultList = new List<LinkItem>();

            List<RevitLinkInstance> linkList = new FilteredElementCollector(document)
                .OfCategory(BuiltInCategory.OST_RvtLinks)
                .WhereElementIsNotElementType().Cast<RevitLinkInstance>().ToList();

            resultList.AddRange(CreateLinkItems(linkList));

            return resultList;
        }


        private static List<LinkItem> CreateLinkItems(IList<RevitLinkInstance> revitLinkInstanceList)
        {
            List<LinkItem> resultList = new List<LinkItem>();

            foreach (RevitLinkInstance revitLinkInstance in revitLinkInstanceList)
            {
                if (revitLinkInstance.GetLinkDocument() == null) continue;

                string linkName = Regex.Split(revitLinkInstance.Name, ".rvt")[0];

                resultList.Add(new LinkItem
                    (
                    linkElement: revitLinkInstance,
                    linkName: linkName
                    ));
            }

            return resultList;
        }


        // PanelFamilyItems

        public static List<PanelTypeItem> CollectPanelFamilySymbols(Document document)
        {
            List<FamilySymbol> familySymbolList = new List<FamilySymbol>();
            List<PanelTypeItem> resultList = new List<PanelTypeItem>();

            FilteredElementCollector collector =
                new FilteredElementCollector(document)
                .OfCategory(BuiltInCategory.OST_ElectricalEquipment)
                .WhereElementIsElementType();

            foreach (FamilySymbol familySymbol in collector)
            {
                if (familySymbol.FamilyName.ToLower().Contains("щрв-п"))
                {
                    familySymbolList.Add(familySymbol);
                }
            }

            resultList = CreatePanelTypeItem(familySymbolList);

            return resultList;
        }


        private static List<PanelTypeItem> CreatePanelTypeItem(IList<FamilySymbol> familySymbolList)
        {
            List<PanelTypeItem> resultList = new List<PanelTypeItem>();

            foreach (FamilySymbol familySymbol in familySymbolList)
            {
                string itemName = $"{familySymbol.FamilyName}: {familySymbol.Name}";
                resultList.Add(new PanelTypeItem
                    (
                    panelFamilySymbol: familySymbol,
                    panelFamilySymbolName: itemName
                    ));
            }

            return resultList;
        }


        // Linked Doors

        public static List<FamilyInstance> CollectAllLinkedDoors(Document linkDocument, View currentView = null)
        {
            List<FamilyInstance> doorInstances = new List<FamilyInstance>();

            FilteredElementCollector collector;

            if (currentView == null)
            {
                collector = new FilteredElementCollector(linkDocument)
                .OfCategory(BuiltInCategory.OST_Doors)
                .WhereElementIsNotElementType();
            }
            else
            {
                collector = new FilteredElementCollector(linkDocument, currentView.Id)
                .OfCategory(BuiltInCategory.OST_Doors)
                .WhereElementIsNotElementType();
            }

            foreach (FamilyInstance door in collector)
            {
                if (door.Name.ToLower().Contains("квартирная наружная"))
                    doorInstances.Add(door);
            }
            return doorInstances;
        }


        public static List<FamilyInstance> PickLinkedDoors(Document document, Document linkDocument)
        {
            List<FamilyInstance> doorInstances = new List<FamilyInstance>();
            List<ElementId> doorElementIds = new List<ElementId>();

            UIDocument uIDocument = new UIDocument(document);

            MessageBox.Show($"Выберите квартирные двери, над которыми будут размещены ЩК");

            IList<Reference> doors = uIDocument.Selection.PickObjects(
                ObjectType.LinkedElement,
                new SelectionFilter_DoorsHome(document),
                "Выберите квартирные двери в связи");

            foreach (Reference door in doors)
            {
                FamilyInstance doorInstance = linkDocument.GetElement(door.LinkedElementId) as FamilyInstance;

                if (doorElementIds.Contains(door.LinkedElementId)) continue;

                doorInstances.Add(doorInstance);
                doorElementIds.Add(door.LinkedElementId);
            }

            return doorInstances;
        }


        // Other

        public static FamilySymbol GetFamilySymbolByName(Document document, BuiltInCategory category, string familyTypeName)
        {

            FilteredElementCollector collector =
                new FilteredElementCollector(document)
                .OfCategory(category)
                .WhereElementIsElementType();

            foreach (FamilySymbol familySymbol in collector)
            {
                if (familySymbol.Name == familyTypeName)
                {
                    return familySymbol;
                }
            }

            return null;
        }
    }
}
