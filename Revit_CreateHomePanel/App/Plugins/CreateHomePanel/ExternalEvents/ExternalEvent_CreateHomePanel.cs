using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Revit_CreateHomePanel.App.Plugins.CreateHomePanel.UserControls.Models;
using Revit_CreateHomePanel.App.Plugins.CreateHomePanel.UserControls.Models.ViewItems;
using Revit_CreateHomePanel.App.Views;
using System.Collections.Generic;
using System.Windows;


namespace Revit_CreateHomePanel.App.Plugins.CreateHomePanel.ExternalEvents
{
    [Transaction(TransactionMode.Manual)]
    public class ExternalEvent_CreateHomePanel : IExternalEventHandler
    {
        private RevitLinkInstance _selectedLinkInstance = null;
        private FamilySymbol _panelType = null;
        private PlacementType _placementType = PlacementType.manual;
        private ConfigurationType _configurationType = ConfigurationType.twoPanels;

        private int _floorHeight;
        private int _floorOffset;
        private int _twoPanelsDistance;
        private int _panelHeight;
        private int _wallDepth;

        public ExternalEvent_CreateHomePanel(
            RevitLinkInstance selectedLinkInstance,
            FamilySymbol panelType,
            PlacementType placementType,
            ConfigurationType configurationType,
            int floorHeight,
            int floorOffset,
            int twoPanelsDistance,
            int panelHeight,
            int wallDepth)
        {
            _selectedLinkInstance = selectedLinkInstance;
            _panelType = panelType;
            _placementType = placementType;
            _configurationType = configurationType;
            _floorHeight = floorHeight;
            _floorOffset = floorOffset;
            _twoPanelsDistance = twoPanelsDistance;
            _panelHeight = panelHeight;
            _wallDepth = wallDepth;
        }

        public void Execute(UIApplication uiApplication)
        {
            Document document = uiApplication.ActiveUIDocument.Document;
            //UIDocument uIDocument = new UIDocument(document);

            string report = "";

            if (document.IsFamilyDocument)
            {
                MessageBox.Show("Плагин работает только с моделями");
                return;
            }

            List<FamilyInstance> doors = new List<FamilyInstance>();

            if (_placementType == PlacementType.manual)
                doors = ElementCollector.PickLinkedDoors(document, _selectedLinkInstance.GetLinkDocument());

            if (_placementType == PlacementType.inModel)
                doors = ElementCollector.CollectAllLinkedDoors(_selectedLinkInstance.GetLinkDocument());

            //

            Transaction transaction = new Transaction(document, "Revit_CreateHomePanel - Разместить ЩК");
            transaction.Start();


            try
            {
                if (!_panelType.IsActive) _panelType.Activate();

                if (_configurationType == ConfigurationType.twoPanels)
                {
                    (_, report) = ElementCreation.PlaceHomePanel_TwoFamilies(
                        document,
                        _selectedLinkInstance,
                        doors,
                        _panelType,
                        _floorHeight,
                        _floorOffset,
                        _twoPanelsDistance,
                        _panelHeight,
                        _wallDepth);
                }

                if (_configurationType == ConfigurationType.onePanel)
                {
                    (_, report) = ElementCreation.PlaceHomePanel_OneFamily(
                        document,
                        _selectedLinkInstance,
                        doors,
                        _panelType,
                        _floorHeight,
                        _floorOffset,
                        _panelHeight,
                        _wallDepth);
                }

            }
            catch (System.Exception e)
            {
                MessageBox.Show($"{e}");
            }

            transaction.Commit();

            //

            DebugWindow debugWindow = new DebugWindow(report);
            debugWindow.ShowDialog();
        }


        public string GetName()
        {
            return "ExternalEvent_CreateHomePanel";
        }
    }
}
