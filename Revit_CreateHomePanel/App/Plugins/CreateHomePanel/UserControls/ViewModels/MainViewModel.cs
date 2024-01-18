using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Revit_CreateHomePanel.App.Helpers;
using Revit_CreateHomePanel.App.Plugins.CreateHomePanel.ExternalEvents;
using Revit_CreateHomePanel.App.Plugins.CreateHomePanel.UserControls.Models;
using Revit_CreateHomePanel.App.Plugins.CreateHomePanel.UserControls.Models.ViewItems;
using Revit_CreateHomePanel.App.Plugins.CreateHomePanel.UserControls.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;


namespace Revit_CreateHomePanel.App.Plugins.CreateHomePanel.UserControls.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private Window_CreateHomePanel _window;
        private Document _document = null;

        private int floorHeight = 110;
        private int floorOffset = 2250;
        private int twoPanelsDistance = 500;
        private int wallDepth = 240;
        private int panelHeight = 233;

        public int FloorHeight
        {
            get => floorHeight;
            set { floorHeight = value; OnPropertyChanged(nameof(FloorHeight)); }
        }

        public int FloorOffset
        {
            get => floorOffset;
            set { floorOffset = value; OnPropertyChanged(nameof(FloorOffset)); }
        }

        public int TwoPanelsDistance
        {
            get => twoPanelsDistance;
            set { twoPanelsDistance = value; OnPropertyChanged(nameof(TwoPanelsDistance)); }
        }

        public int WallDepth
        {
            get => wallDepth;
            set { wallDepth = value; OnPropertyChanged(nameof(WallDepth)); }
        }

        public int PanelHeight
        {
            get => panelHeight;
            set { panelHeight = value; OnPropertyChanged(nameof(PanelHeight)); }
        }


        public bool Visibility_TwoPanelProperties { get; set; }


        public SmartCollection<LinkItem> Collection_Links { get; set; }
        public ICollectionView CollectionView_Links { get; set; }


        public SmartCollection<PanelTypeItem> Collection_PanelFamilySymbols { get; set; }
        public ICollectionView CollectionView_PanelFamilySymbols { get; set; }


        public SmartCollection<ConfigurationTypeItem> Collection_Configurations { get; set; }
        public ICollectionView CollectionView_Configurations { get; set; }


        public SmartCollection<PlacementTypeItem> Collection_PlacementTypes { get; set; }
        public ICollectionView CollectionView_PlacementTypes { get; set; }


        // PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }


        // Constructor

        public MainViewModel(Document document, Window_CreateHomePanel window)
        {
            this._window = window;
            this._document = document;

            Collection_Links = new SmartCollection<LinkItem>();
            CollectionView_Links = CollectionViewSource.GetDefaultView(Collection_Links);

            Collection_PanelFamilySymbols = new SmartCollection<PanelTypeItem>();
            CollectionView_PanelFamilySymbols = CollectionViewSource.GetDefaultView(Collection_PanelFamilySymbols);

            Collection_Configurations = new SmartCollection<ConfigurationTypeItem>();
            CollectionView_Configurations = CollectionViewSource.GetDefaultView(Collection_Configurations);

            Collection_PlacementTypes = new SmartCollection<PlacementTypeItem>();
            CollectionView_PlacementTypes = CollectionViewSource.GetDefaultView(Collection_PlacementTypes);

            UpdateCollections();
        }


        // Updates

        public void UpdateCollections()
        {
            if (_document == null) return;
            if (!_document.IsValidObject) return;

            Collection_Links.Reset(ElementCollector.CollectLinks(_document));
            Collection_PanelFamilySymbols.Reset(ElementCollector.CollectPanelFamilySymbols(_document));

            Collection_Configurations.Reset(new List<ConfigurationTypeItem> {
                new ConfigurationTypeItem("ЩК + ЩК-СС", ConfigurationType.twoPanels),
                new ConfigurationTypeItem("ЩК", ConfigurationType.onePanel)
            });

            Collection_PlacementTypes.Reset(new List<PlacementTypeItem> {
                new PlacementTypeItem("Выбрать двери вручную / рамкой", PlacementType.manual),
                new PlacementTypeItem("Во всей модели", PlacementType.inModel)
            });

            CollectionView_PanelFamilySymbols.MoveCurrentToFirst();
            CollectionView_Configurations.MoveCurrentToFirst();
            CollectionView_PlacementTypes.MoveCurrentToFirst();

            TryGetPluginConfiguration();
        }


        // Events

        public void OnStart(object sender, RoutedEventArgs e)
        {
            LinkItem linkItem = CollectionView_Links.CurrentItem as LinkItem;

            if (linkItem == null)
            {
                MessageBox.Show("Выберите связанный файл из списка");
                return;
            }

            PanelTypeItem panelTypeItem = CollectionView_PanelFamilySymbols.CurrentItem as PanelTypeItem;

            if (panelTypeItem == null)
            {
                MessageBox.Show("Выберите семейство ЩК");
                return;
            }

            PlacementTypeItem placementTypeItem = CollectionView_PlacementTypes.CurrentItem as PlacementTypeItem;
            ConfigurationTypeItem configurationTypeItem = CollectionView_Configurations.CurrentItem as ConfigurationTypeItem;

            IExternalEventHandler createHomePanel_Handler = new ExternalEvent_CreateHomePanel(
                linkItem.linkElement,
                panelTypeItem.panelFamilySymbol,
                placementTypeItem.placementType,
                configurationTypeItem.configurationType,
                FloorHeight,
                FloorOffset,
                TwoPanelsDistance,
                PanelHeight,
                WallDepth);

            ExternalEvent createHomePanel_Event = ExternalEvent.Create(createHomePanel_Handler);

            if (createHomePanel_Event != null)
                createHomePanel_Event.Raise();

            TrySavePluginConfiguration();
            _window.Close();
        }


        public void OnSetDefaultValues(object sender, RoutedEventArgs e)
        {
            FloorHeight = 110;
            FloorOffset = 2250;
            TwoPanelsDistance = 500;
            WallDepth = 240;
            PanelHeight = 233;
        }


        public void OnCancel(object sender, RoutedEventArgs e)
        {
            TrySavePluginConfiguration();
            _window.Close();
        }


        // Configuration

        public void TrySavePluginConfiguration()
        {
            try
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();

                dict[nameof(FloorHeight)] = FloorHeight.ToString();
                dict[nameof(FloorOffset)] = FloorOffset.ToString();
                dict[nameof(TwoPanelsDistance)] = TwoPanelsDistance.ToString();
                dict[nameof(WallDepth)] = WallDepth.ToString();
                dict[nameof(PanelHeight)] = PanelHeight.ToString();

                if (CollectionView_Configurations.CurrentItem != null)
                    dict[nameof(Collection_Configurations)] = (CollectionView_Configurations.CurrentItem as ConfigurationTypeItem).configurationName;

                if (CollectionView_PanelFamilySymbols.CurrentItem != null)
                    dict[nameof(Collection_PanelFamilySymbols)] = (CollectionView_PanelFamilySymbols.CurrentItem as PanelTypeItem).panelFamilySymbolName;

                if (CollectionView_PlacementTypes.CurrentItem != null)
                    dict[nameof(Collection_PlacementTypes)] = (CollectionView_PlacementTypes.CurrentItem as PlacementTypeItem).placementTypeName;

                if (CollectionView_Links.CurrentItem != null)
                    dict[nameof(Collection_Links)] = (CollectionView_Links.CurrentItem as LinkItem).linkName;

                ConfigurationManager.SavePluginConfiguration(dict, "CreateHomePanel");
            }
            catch (Exception e)
            {
                MessageBox.Show($"{e}");
            }
        }


        public void TryGetPluginConfiguration()
        {
            try
            {
                Dictionary<string, string> dict = ConfigurationManager.LoadPluginConfiguration("CreateHomePanel");
                if (dict == null) return;

                FloorHeight = Int32.Parse(dict[nameof(FloorHeight)]);
                FloorOffset = Int32.Parse(dict[nameof(FloorOffset)]);
                TwoPanelsDistance = Int32.Parse(dict[nameof(TwoPanelsDistance)]);
                WallDepth = Int32.Parse(dict[nameof(WallDepth)]);
                PanelHeight = Int32.Parse(dict[nameof(PanelHeight)]);

                string value_Collection_PlacementTypes = dict[nameof(Collection_PlacementTypes)];

                var item1 = Collection_Configurations.FirstOrDefault(conf => conf.configurationName == dict[nameof(Collection_Configurations)]);
                if (item1 != null) { CollectionView_Configurations.MoveCurrentTo(item1); }
                else CollectionView_Configurations.MoveCurrentToFirst();

                var item2 = Collection_PanelFamilySymbols.FirstOrDefault(conf => conf.panelFamilySymbolName == dict[nameof(Collection_PanelFamilySymbols)]);
                if (item2 != null) { CollectionView_PanelFamilySymbols.MoveCurrentTo(item2); }
                else CollectionView_PanelFamilySymbols.MoveCurrentToFirst();

                var item3 = Collection_PlacementTypes.FirstOrDefault(conf => conf.placementTypeName == dict[nameof(Collection_PlacementTypes)]);
                if (item3 != null) { CollectionView_PlacementTypes.MoveCurrentTo(item3); }
                else CollectionView_PlacementTypes.MoveCurrentToFirst();

                var item4 = Collection_Links.FirstOrDefault(conf => conf.linkName == dict[nameof(Collection_Links)]);
                if (item4 != null) { CollectionView_Links.MoveCurrentTo(item4); }
            }
            catch (Exception) { }
        }
    }
}
