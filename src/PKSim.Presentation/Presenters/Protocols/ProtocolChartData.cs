using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Protocols;
using OSPSuite.Core.Domain;
using static OSPSuite.Core.Domain.Constants;
using OSPSuite.Core.Domain.UnitSystem;

namespace PKSim.Presentation.Presenters.Protocols
{
   public class EventChartPoint
   {
      public string GroupingName { get; init; }
      public double Time { get; init; }
      public SchemaItemDTO SchemaItem { get; init; }
   }

   public interface IProtocolChartData
   {
      DataTable DataTable { get; }
      string GroupingName { get; }
      string XValue { get; }
      string XValue2 { get; }
      string YValue { get; }
      string SchemaItemName { get; }
      double XMin { get; }
      double XMax { get; }
      Unit YAxisUnit { get; }
      Unit Y2AxisUnit { get; }
      bool NeedsMultipleAxis { get; }
      IReadOnlyList<EventChartPoint> EventPoints { get; }

      bool SeriesShouldBeOnSecondAxis(string seriesName);
      SchemaItemDTO SchemaItemFor(object tag);
   }

   public class ProtocolChartData : IProtocolChartData
   {
      private readonly double _endTimeInMin;
      private double _maxEndTimeInDisplayUnit;

      private readonly ICache<Compound, IReadOnlyList<SchemaItemDTO>> _allSchemaItemDTO;
      private readonly List<EventChartPoint> _eventPoints = new();
      public IDimension TimeDimension { get; }
      public Unit TimeUnit { get; }
      public Unit YAxisUnit { get; private set; }
      public Unit Y2AxisUnit { get; private set; }
      public IReadOnlyList<EventChartPoint> EventPoints => _eventPoints;

      private const string GROUPING_NAME = "GroupingName";
      private const string TIME = "Time";
      private const string END_TIME = "EndTime";
      private const string DOSE = "Dose";
      private const string UNIT = "Unit";
      private const string SCHEMA_ITEM = "SchemaItem";

      public bool NeedsMultipleAxis => YAxisUnit != Y2AxisUnit;

      public bool SeriesShouldBeOnSecondAxis(string seriesName)
      {
         return (from DataRow dataRow in DataTable.Rows
            where Equals(dataRow[GroupingName], seriesName)
            select Equals(dataRow[UnitName], Y2AxisUnit)).FirstOrDefault();
      }

      public SchemaItemDTO SchemaItemFor(object tag)
      {
         if (tag is SchemaItemDTO schemaItemDTO)
            return schemaItemDTO;

         var dataRow = tag as DataRowView;
         return dataRow?[SchemaItemName].DowncastTo<SchemaItemDTO>();
      }

      public DataTable DataTable { get; }

      public ProtocolChartData(ICache<Compound, IReadOnlyList<SchemaItemDTO>> allSchemaItemDTO, IDimension timeDimension, Unit timeUnit, double endTimeInMin)
      {
         _endTimeInMin = endTimeInMin;
         _allSchemaItemDTO = allSchemaItemDTO;
         DataTable = new DataTable("Protocol");
         TimeDimension = timeDimension;
         TimeUnit = timeUnit;
         DataTable.AddColumn(GROUPING_NAME);
         DataTable.AddColumn<double>(TIME);
         DataTable.AddColumn<double>(END_TIME);
         DataTable.AddColumn<double>(DOSE);
         DataTable.AddColumn<Unit>(UNIT);
         DataTable.AddColumn<SchemaItemDTO>(SCHEMA_ITEM);
         createDataToPlot();
      }

      public string GroupingName => GROUPING_NAME;

      public string XValue => TIME;

      public string XValue2 => END_TIME;

      public string YValue => DOSE;

      public string UnitName => UNIT;

      public string SchemaItemName => SCHEMA_ITEM;

      //events may be offset (possibly negatively) from their administration, so the axis has to extend
      //left of 0 to keep a negative-time event (e.g. a meal before the first dose) visible
      public double XMin => _eventPoints.Count == 0 ? 0 : Math.Min(0, _eventPoints.Min(p => p.Time));

      public double XMax => Math.Max(timeValueInDisplayUnit(_endTimeInMin), _maxEndTimeInDisplayUnit);

      private double timeValueInDisplayUnit(double timeValueInBaseUnit)
      {
         return TimeDimension.BaseUnitValueToUnitValue(TimeUnit, timeValueInBaseUnit);
      }
      private void createDataToPlot()
      {
         if (_allSchemaItemDTO.Count == 0)
            return;

         var allDoseUnits = allUsedDoseUnits();
         bool hasOnlyOneDoseUnit = allDoseUnits.Count <= 1;

         if (allDoseUnits.Count > 0)
         {
            YAxisUnit = allDoseUnits[0];
            Y2AxisUnit = allDoseUnits[hasOnlyOneDoseUnit ? 0 : 1];
         }

         foreach (var schemaItemsDTOForCompound in _allSchemaItemDTO.KeyValues)
         {
            var compound = schemaItemsDTOForCompound.Key;

            foreach (var schemaItemDTOGroup in schemaItemsDTOForCompound.Value
               .Where(x => !x.IsEvent)
               .GroupBy(schemaItemAggregation))
            {
               var row = DataTable.NewRow();
               var schemaItemDTO = schemaItemDTOGroup.First();
               var endTime = endTimeValueInDisplayUnit(schemaItemDTO);
               _maxEndTimeInDisplayUnit = Math.Max(_maxEndTimeInDisplayUnit, endTime);
               row[GROUPING_NAME] = createDataGroupingNameFor(hasOnlyOneDoseUnit, compound, schemaItemDTO);
               row[TIME] = timeValueInDisplayUnit(schemaItemDTO.StartTimeParameter.KernelValue);
               row[END_TIME] = endTime;
               row[DOSE] = schemaItemDTOGroup.Sum(x => x.Dose);
               row[UNIT] = schemaItemDTO.DoseParameter.DisplayUnit;
               row[SCHEMA_ITEM] = schemaItemDTO;
               DataTable.Rows.Add(row);
            }

            foreach (var eventDTO in schemaItemsDTOForCompound.Value.Where(x => x.IsEvent))
            {
               _eventPoints.Add(new EventChartPoint
               {
                  GroupingName = createEventGroupingNameFor(compound, eventDTO),
                  Time = timeValueInDisplayUnit(eventDTO.StartTimeParameter.KernelValue),
                  SchemaItem = eventDTO
               });
            }
         }
      }

      private string schemaItemAggregation(SchemaItemDTO dto)
      {
         var key = $"{dto.ApplicationType}|{dto.StartTime}|{dto.DoseParameter.DisplayUnit}";
         if (dto.NeedsFormulation)
            key = $"{key}|{dto.FormulationKey}";

         //infusions of different durations should not be aggregated even when they share a start time and dose unit
         if (isInfusion(dto))
            key = $"{key}|{dto.InfusionTimeParameter.KernelValue}";

         return key;
      }

      private double endTimeValueInDisplayUnit(SchemaItemDTO schemaItemDTO)
      {
         var startTimeInBaseUnit = schemaItemDTO.StartTimeParameter.KernelValue;
         if (isInfusion(schemaItemDTO))
            return timeValueInDisplayUnit(startTimeInBaseUnit + schemaItemDTO.InfusionTimeParameter.KernelValue);

         return timeValueInDisplayUnit(startTimeInBaseUnit);
      }

      private static bool isInfusion(SchemaItemDTO schemaItemDTO) => schemaItemDTO.InfusionTimeParameter != null && schemaItemDTO.InfusionTimeParameter.KernelValue > 0;

      private static string createDataGroupingNameFor(bool hasOnlyOneDoseUnit, Compound compound, SchemaItemDTO schemaItemDTO)
      {
         var baseName = schemaItemDTO.DisplayName;
         if (!string.IsNullOrEmpty(compound.Name))
            baseName = CompositeNameFor(baseName, compound.Name);

         return hasOnlyOneDoseUnit ? baseName : Constants.NameWithUnitFor(baseName, schemaItemDTO.DoseParameter.DisplayUnit);
      }

      private static string createEventGroupingNameFor(Compound compound, SchemaItemDTO schemaItemDTO)
      {
         var baseName = schemaItemDTO.DisplayName;
         if (!string.IsNullOrEmpty(compound.Name))
            baseName = CompositeNameFor(baseName, compound.Name);

         return baseName;
      }

      private IReadOnlyList<Unit> allUsedDoseUnits()
      {
         return _allSchemaItemDTO
            .SelectMany(x => x.Where(y => !y.IsEvent).Select(y => y.DoseParameter.DisplayUnit))
            .Where(u => u != null)
            .Distinct()
            .ToList();
      }
   }
}