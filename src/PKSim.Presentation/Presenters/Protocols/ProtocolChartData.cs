using System.Collections.Generic;
using System.Data;
using System.Linq;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Protocols;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;

namespace PKSim.Presentation.Presenters.Protocols
{
   public interface IProtocolChartData
   {
      DataTable DataTable { get; }
      string GroupingName { get; }
      string XValue { get; }
      string YValue { get; }
      double XMin { get; }
      double XMax { get; }
      IDimension TimeDimension { get; }
      Unit TimeUnit { get; }
      Unit YAxisUnit { get; }
      Unit Y2AxisUnit { get; }
      bool NeedsMultipleAxis { get; }

      bool SeriesShouldBeOnSecondAxis(string serieName);
      SchemaItemDTO SchemaItemFor(object tag);
   }

   public class ProtocolChartData : IProtocolChartData
   {
      private readonly double _endTimeInMin;

      private readonly ICache<Compound, IReadOnlyList<SchemaItemDTO>> _allSchemaItemDTO;
      public IDimension TimeDimension { get; private set; }
      public Unit TimeUnit { get; private set; }
      public Unit YAxisUnit { get; private set; }
      public Unit Y2AxisUnit { get; private set; }

      private const string GROUPING_NAME = "GroupingName";
      private const string TIME = "Time";
      private const string DOSE = "Dose";
      private const string UNIT = "Unit";
      private const string SCHEMA_ITEM = "SchemaItem";

      public bool NeedsMultipleAxis
      {
         get { return YAxisUnit != Y2AxisUnit; }
      }

      public bool SeriesShouldBeOnSecondAxis(string serieName)
      {
         return (from DataRow dataRow in DataTable.Rows
            where Equals(dataRow[GroupingName], serieName)
            select Equals(dataRow[UnitName], Y2AxisUnit)).FirstOrDefault();
      }

      public SchemaItemDTO SchemaItemFor(object tag)
      {
         var dataRow = tag as DataRowView;
         if (dataRow == null) return null;
         return dataRow[SchemaItemName].DowncastTo<SchemaItemDTO>();
      }

      public DataTable DataTable { get; private set; }

      public ProtocolChartData(ICache<Compound, IReadOnlyList<SchemaItemDTO>> allSchemaItemDTO, IDimension timeDimension, Unit timeUnit, double endTimeInMin)
      {
         _endTimeInMin = endTimeInMin;
         _allSchemaItemDTO = allSchemaItemDTO;
         DataTable = new DataTable("Protocol");
         TimeDimension = timeDimension;
         TimeUnit = timeUnit;
         DataTable.Columns.Add(new DataColumn(GROUPING_NAME, typeof (string)));
         DataTable.Columns.Add(new DataColumn(TIME, typeof (double)));
         DataTable.Columns.Add(new DataColumn(DOSE, typeof (double)));
         DataTable.Columns.Add(new DataColumn(UNIT, typeof (Unit)));
         DataTable.Columns.Add(new DataColumn(SCHEMA_ITEM, typeof(SchemaItemDTO)));
         createDataToPlot();
      }

      public string GroupingName
      {
         get { return GROUPING_NAME; }
      }

      public string XValue
      {
         get { return TIME; }
      }

      public string YValue
      {
         get { return DOSE; }
      }

      public string UnitName
      {
         get { return UNIT; }
      }

      public string SchemaItemName
      {
         get { return SCHEMA_ITEM; }
      }

      public double XMin
      {
         get { return 0; }
      }

      public double XMax
      {
         get { return timeValueInDisplayUnit(_endTimeInMin); }
      }

      private double timeValueInDisplayUnit(double timeValueInBaseUnit)
      {
         return TimeDimension.BaseUnitValueToUnitValue(TimeUnit, timeValueInBaseUnit);
      }
      private void createDataToPlot()
      {
         if (_allSchemaItemDTO.Count == 0) return;
         var allDoseUnits = allUsedDoseUnits();
         bool hasOnlyOneDoseUnit = allDoseUnits.Count == 1;
         YAxisUnit = allDoseUnits[0];
         Y2AxisUnit = allDoseUnits[hasOnlyOneDoseUnit ? 0 : 1];
         foreach (var schemaItemsDTOForCompound in _allSchemaItemDTO.KeyValues)
         {
            var compound = schemaItemsDTOForCompound.Key;
            foreach (var schemaItemDTOGroup in schemaItemsDTOForCompound.Value.GroupBy(schemaItemAggregation))
            {
               var row = DataTable.NewRow();
               var schemaItemDTO = schemaItemDTOGroup.First();
               row[GROUPING_NAME] = createDataGroupingNameFor(hasOnlyOneDoseUnit, compound, schemaItemDTO);
               row[TIME] = timeValueInDisplayUnit(schemaItemDTO.StartTimeParameter.KernelValue);
               row[DOSE] = schemaItemDTOGroup.Sum(x => x.Dose);
               row[UNIT] = schemaItemDTO.DoseParameter.DisplayUnit;
               row[SCHEMA_ITEM] = schemaItemDTO;
               DataTable.Rows.Add(row);
            }
         }
      }

      private string schemaItemAggregation(SchemaItemDTO dto)
      {
         var key = string.Format("{0}|{1}|{2}", dto.ApplicationType, dto.StartTime, dto.DoseParameter.DisplayUnit);
         if (dto.NeedsFormulation)
            key = string.Format("{0}|{1}", key, dto.FormulationKey);

         return key;
      }

      private static string createDataGroupingNameFor(bool hasOnlyOneDoseUnit, Compound compound, SchemaItemDTO schemaItemDTO)
      {
         var baseName = schemaItemDTO.DisplayName;
         if (!string.IsNullOrEmpty(compound.Name))
            baseName = CoreConstants.CompositeNameFor(baseName, compound.Name);

         return hasOnlyOneDoseUnit ? baseName : Constants.NameWithUnitFor(baseName, schemaItemDTO.DoseParameter.DisplayUnit);
      }

      private IReadOnlyList<Unit> allUsedDoseUnits()
      {
         return _allSchemaItemDTO.SelectMany(x => x.Select(y => y.DoseParameter.DisplayUnit)).Distinct().ToList();
      }
   }
}