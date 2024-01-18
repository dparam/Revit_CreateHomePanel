using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revit_CreateHomePanel.App.Plugins.CreateHomePanel.UserControls.Models.ViewItems
{
    public enum PlacementType
    {
        manual,
        inModel
    }


    public class PlacementTypeItem
    {
        public string placementTypeName { get; set; }
        public PlacementType placementType { get; set; }


        public PlacementTypeItem(string placementTypeName, PlacementType placementType)
        {
            this.placementTypeName = placementTypeName;
            this.placementType = placementType;
        }
    }
}
