using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;
using System.Windows;


namespace Revit_CreateHomePanel.App.Plugins.CreateHomePanel.UserControls.Models
{
    public class ElementCreation
    {
        public static (Dictionary<ElementId, XYZ>, string) PlaceHomePanel_TwoFamilies(
            Document document,
            RevitLinkInstance revitLinkInstance,
            List<FamilyInstance> doorList,
            FamilySymbol panelSymbol,
            int floorHeight,
            int floorOffset,
            int twoPanelsDistance,
            int panelHeight,
            int wallDepth
            )
        {
            int count_Success = 0;
            int count_Fail = 0;
            string reportString = "";

            Dictionary<ElementId, XYZ> resultDict = new Dictionary<ElementId, XYZ>();

            double levelOffset = UnitUtils.ConvertToInternalUnits(floorOffset + floorHeight + panelHeight, UnitTypeId.Millimeters);
            double locationOffset_front = UnitUtils.ConvertToInternalUnits(-(wallDepth / 2), UnitTypeId.Millimeters);
            double locationOffset_side = UnitUtils.ConvertToInternalUnits(twoPanelsDistance / 2, UnitTypeId.Millimeters);

            foreach (FamilyInstance door in doorList)
            {
                XYZ doorPoint = GetPointFromLinkedElement(revitLinkInstance, door);
                XYZ facingOrientation = door.FacingOrientation;

                // get level
                Level level = GetLevelFromLinkedElement(document, door);
                if (level == null)
                {
                    count_Fail += 1;
                    continue;
                }

                // create instances
                FamilyInstance instance_left = document.Create.NewFamilyInstance(doorPoint, panelSymbol, level, StructuralType.NonStructural);
                FamilyInstance instance_right = document.Create.NewFamilyInstance(doorPoint, panelSymbol, level, StructuralType.NonStructural);

                resultDict[instance_left.Id] = facingOrientation.Negate();
                resultDict[instance_right.Id] = facingOrientation.Negate();

                // set parameters
                instance_left.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM).Set(levelOffset);
                instance_right.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM).Set(levelOffset);

                instance_left.get_Parameter(BuiltInParameter.RBS_ELEC_PANEL_NAME).Set("ЩК-СС");
                instance_right.get_Parameter(BuiltInParameter.RBS_ELEC_PANEL_NAME).Set("ЩК");

                // add front offset
                MoveElement_Front(document, instance_left, locationOffset_front, facingOrientation);
                MoveElement_Front(document, instance_right, locationOffset_front, facingOrientation);

                // add side offset
                MoveElement_Side(document, instance_left, locationOffset_side, facingOrientation);
                MoveElement_Side(document, instance_right, -locationOffset_side, facingOrientation);
            }

            //

            foreach (KeyValuePair<ElementId, XYZ> pair in resultDict)
            {
                FamilyInstance instance = document.GetElement(pair.Key) as FamilyInstance;
                XYZ facingOrientation = pair.Value;

                // rotate new instances
                RotateElement_ToFacing(document, revitLinkInstance, instance, facingOrientation);

                count_Success += 1;
            }

            reportString += $"Результат:\n\n";
            reportString += $"Размещено элементов: {count_Success}\n";
            reportString += $"Пропущено квартир: {count_Fail}";

            return (resultDict, reportString);
        }


        public static (Dictionary<ElementId, XYZ>, string) PlaceHomePanel_OneFamily(
            Document document,
            RevitLinkInstance revitLinkInstance,
            List<FamilyInstance> doorList,
            FamilySymbol panelSymbol,
            int floorHeight,
            int floorOffset,
            int panelHeight,
            int wallDepth
            )
        {
            int count_Success = 0;
            int count_Fail = 0;
            string reportString = "";

            Dictionary<ElementId, XYZ> resultDict = new Dictionary<ElementId, XYZ>();

            double levelOffset = UnitUtils.ConvertToInternalUnits(floorOffset + floorHeight + panelHeight, UnitTypeId.Millimeters);
            double locationOffset_front = UnitUtils.ConvertToInternalUnits(-(wallDepth / 2), UnitTypeId.Millimeters);

            foreach (FamilyInstance door in doorList)
            {
                XYZ doorPoint = GetPointFromLinkedElement(revitLinkInstance, door);
                XYZ facingOrientation = door.FacingOrientation;

                // get level
                Level level = GetLevelFromLinkedElement(document, door);
                if (level == null)
                {
                    count_Fail += 1;
                    continue;
                }

                // create instance
                FamilyInstance instance = document.Create.NewFamilyInstance(doorPoint, panelSymbol, level, StructuralType.NonStructural);

                resultDict[instance.Id] = facingOrientation.Negate();

                instance.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM).Set(levelOffset);
                instance.get_Parameter(BuiltInParameter.RBS_ELEC_PANEL_NAME).Set("ЩК");

                // add front offset
                MoveElement_Front(document, instance, locationOffset_front, facingOrientation);
            }

            //

            foreach (KeyValuePair<ElementId, XYZ> pair in resultDict)
            {
                FamilyInstance instance = document.GetElement(pair.Key) as FamilyInstance;
                XYZ facingOrientation = pair.Value;

                // rotate new instances
                RotateElement_ToFacing(document, revitLinkInstance, instance, facingOrientation);

                count_Success += 1;
            }

            reportString += $"Результат:\n\n";
            reportString += $"Размещено элементов: {count_Success}\n";
            reportString += $"Пропущено квартир: {count_Fail}";

            return (resultDict, reportString);
        }


        // Helpers

        private static Level GetLevelFromLinkedElement(Document document, FamilyInstance door)
        {
            Document linkedDocument = door.Document;
            Level linkedLevel = linkedDocument.GetElement(door.LevelId) as Level;
            string linkedLevelName = linkedLevel.Name;

            FilteredElementCollector collector = new FilteredElementCollector(document)
                .OfCategory(BuiltInCategory.OST_Levels)
                .WhereElementIsNotElementType();

            foreach (Level level in collector)
            {
                if (level.Name == linkedLevelName)
                {
                    //MessageBox.Show($"GetLevelFromLinkedElement {level.Name}");
                    return level;
                }
            }

            return null;
        }


        private static XYZ GetPointFromLinkedElement(RevitLinkInstance revitLinkInstance, FamilyInstance door)
        {
            XYZ transformedPoint;

            Transform linkTransform = revitLinkInstance.GetTotalTransform();
            LocationPoint locationPoint = door.Location as LocationPoint;

            transformedPoint = linkTransform.OfPoint(locationPoint.Point);

            return transformedPoint;
        }


        private static void RotateElement_ToFacing(Document document, RevitLinkInstance revitLinkInstance, FamilyInstance instance, XYZ facingOrientation)
        {
            Transform linkTransform = revitLinkInstance.GetTotalTransform();
            XYZ newFacingOrientation = linkTransform.OfVector(facingOrientation);

            var angle = newFacingOrientation.AngleTo(XYZ.BasisY);

            LocationPoint locationPoint = instance.Location as LocationPoint;
            XYZ instancePoint = locationPoint.Point;

            Line axisZ = Line.CreateBound(instancePoint, new XYZ(
            instancePoint.X,
            instancePoint.Y,
            instancePoint.Z + 1));

            if (newFacingOrientation.X > 0)
                angle = -angle;

            ElementTransformUtils.RotateElement(document, instance.Id, axisZ, angle);
        }


        private static void MoveElement_Front(Document document, FamilyInstance instance, double offset, XYZ facingOrientation)
        {
            XYZ doorPointFrontOffset = new XYZ(
                    facingOrientation.X * offset,
                    facingOrientation.Y * offset,
                    0);

            XYZ translaionPoint = doorPointFrontOffset;

            ElementTransformUtils.MoveElement(document, instance.Id, translaionPoint);
        }


        private static void MoveElement_Side(Document document, FamilyInstance instance, double offset, XYZ facingOrientation)
        {
            XYZ axis = XYZ.BasisZ;

            double angle = 90 * Math.PI / 180;

            Transform transform = Transform.CreateRotationAtPoint(axis, angle, facingOrientation);
            XYZ newFacingOrientation = transform.OfVector(facingOrientation);

            newFacingOrientation = new XYZ(
                    newFacingOrientation.X * offset,
                    newFacingOrientation.Y * offset,
                    0);

            ElementTransformUtils.MoveElement(document, instance.Id, newFacingOrientation);
        }
    }
}
