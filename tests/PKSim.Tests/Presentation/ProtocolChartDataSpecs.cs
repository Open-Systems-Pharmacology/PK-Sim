using System.Collections.Generic;
using System.Data;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using FakeItEasy;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Protocols;
using PKSim.Presentation.Presenters.Protocols;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;

namespace PKSim.Presentation
{
   public abstract class concern_for_ProtocolChartData : ContextSpecification<IProtocolChartData>
   {
      protected ICache<Compound, IReadOnlyList<SchemaItemDTO>> _allSchemaItemsDTOForCompoundCache;
      protected IDimension _timeDimension;
      protected Unit _timeDisplayUnit;
      protected double _endTimeMin;
      protected Unit _doseUnit;
      protected Unit _dosePerKgUnit;
      protected DataTable _data;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _timeDimension = A.Fake<IDimension>();
         _timeDisplayUnit = new Unit("min", 1, 0);
         _doseUnit = new Unit("mg", 1, 0);
         _dosePerKgUnit = new Unit("mg/kg", 1, 0);
         _endTimeMin = 60;
         _allSchemaItemsDTOForCompoundCache = new Cache<Compound, IReadOnlyList<SchemaItemDTO>>();
      }

      protected override void Because()
      {
         _data = sut.DataTable;
      }

      protected override void Context()
      {
         sut = new ProtocolChartData(_allSchemaItemsDTOForCompoundCache, _timeDimension, _timeDisplayUnit, _endTimeMin);
      }
   }

   public class When_creating_the_chart_data_for_compound_defined_with_name : concern_for_ProtocolChartData
   {
      protected override void Context()
      {
         var schemaItemDTO1 = DomainHelperForSpecs.SchemaItemDTO(ApplicationTypes.Intravenous, _doseUnit);
         var schemaItemDTO2 = DomainHelperForSpecs.SchemaItemDTO(ApplicationTypes.Oral, _doseUnit);
         _allSchemaItemsDTOForCompoundCache.Add(new Compound().WithName("DRUG"), new[] {schemaItemDTO1, schemaItemDTO2});
         base.Context();
      }

      [Observation]
      public void should_add_the_name_of_the_compound_in_the_resulting_table()
      {
         var allValues = _data.AllValuesInColumn<string>(sut.GroupingName);
         allValues.ShouldOnlyContain($"{ApplicationTypes.Intravenous}-DRUG", $"{ApplicationTypes.Oral}-DRUG");
      }
   }

   public class When_creating_the_chart_data_for_schema_items_having_the_same_application_type_and_the_same_start_time_and_dose_unit : concern_for_ProtocolChartData
   {
      protected override void Context()
      {
         var schemaItemDTO1 = DomainHelperForSpecs.SchemaItemDTO(ApplicationTypes.Intravenous, _doseUnit, 1);
         var schemaItemDTO2 = DomainHelperForSpecs.SchemaItemDTO(ApplicationTypes.Intravenous, _doseUnit, 2);
         _allSchemaItemsDTOForCompoundCache.Add(new Compound(), new[] {schemaItemDTO1, schemaItemDTO2});
         base.Context();
      }

      [Observation]
      public void should_aggregate_the_values_for_the_application_type()
      {
         _data.AllValuesInColumn<double>(sut.YValue).ShouldOnlyContain(3);
      }
   }

   public class When_creating_the_chart_data : concern_for_ProtocolChartData
   {
      protected override void Context()
      {
         var schemaItemDTO1 = DomainHelperForSpecs.SchemaItemDTO(ApplicationTypes.Intravenous, startTimeValue: 10); //min
         var schemaItemDTO2 = DomainHelperForSpecs.SchemaItemDTO(ApplicationTypes.Intravenous, startTimeValue: 20); //min
         _allSchemaItemsDTOForCompoundCache.Add(new Compound(), new[] {schemaItemDTO1, schemaItemDTO2});
         A.CallTo(() => _timeDimension.BaseUnitValueToUnitValue(_timeDisplayUnit, 10.0)).Returns(100);
         A.CallTo(() => _timeDimension.BaseUnitValueToUnitValue(_timeDisplayUnit, 20.0)).Returns(200);
         base.Context();
      }

      [Observation]
      public void should_create_the_time_value_in_the_display_unit_defined_by_the_user()
      {
         _data.AllValuesInColumn<double>(sut.XValue).ShouldOnlyContain(100, 200);
      }
   }

   public class When_creating_the_chart_data_for_schema_items_having_the_same_application_type_and_the_same_unit_but_different_start_time : concern_for_ProtocolChartData
   {
      protected override void Context()
      {
         var schemaItemDTO1 = DomainHelperForSpecs.SchemaItemDTO(ApplicationTypes.Intravenous, _doseUnit, 1, 1);
         var schemaItemDTO2 = DomainHelperForSpecs.SchemaItemDTO(ApplicationTypes.Intravenous, _doseUnit, 2, 2);
         _allSchemaItemsDTOForCompoundCache.Add(new Compound(), new[] {schemaItemDTO1, schemaItemDTO2});
         base.Context();
      }

      [Observation]
      public void should_not_aggreagate_the_value_for_dose()
      {
         _data.AllValuesInColumn<double>(sut.YValue).ShouldOnlyContain(1, 2);
      }
   }

   public class When_creating_the_chart_data_for_schema_items_having_the_same_application_type_and_the_start_time_but_different_does_unit : concern_for_ProtocolChartData
   {
      protected override void Context()
      {
         var schemaItemDTO1 = DomainHelperForSpecs.SchemaItemDTO(ApplicationTypes.Intravenous, _doseUnit, 1);
         var schemaItemDTO2 = DomainHelperForSpecs.SchemaItemDTO(ApplicationTypes.Intravenous, _dosePerKgUnit, 2);
         _allSchemaItemsDTOForCompoundCache.Add(new Compound(), new[] {schemaItemDTO1, schemaItemDTO2});
         base.Context();
      }

      [Observation]
      public void should_not_aggreagate_the_value_for_dose()
      {
         _data.AllValuesInColumn<double>(sut.YValue).ShouldOnlyContain(1, 2);
      }
   }

   public class When_creating_the_chart_data_for_schema_items_for_an_invalid_protocol : concern_for_ProtocolChartData
   {
      protected override void Context()
      {
         _allSchemaItemsDTOForCompoundCache.Add(new Compound(), new List<SchemaItemDTO>());
         base.Context();
      }

      [Observation]
      public void should_not_return_any_data_to_plot()
      {
         _data.Rows.Count.ShouldBeEqualTo(0);
      }
   }

   public class When_creating_the_chart_data_for_an_infusion_schema_item : concern_for_ProtocolChartData
   {
      protected override void Context()
      {
         var infusion = DomainHelperForSpecs.SchemaItemDTO(ApplicationTypes.Intravenous, _doseUnit, startTimeValue: 10, infusionTimeValue: 30); //min
         _allSchemaItemsDTOForCompoundCache.Add(new Compound(), new[] {infusion});
         A.CallTo(() => _timeDimension.BaseUnitValueToUnitValue(_timeDisplayUnit, 10.0)).Returns(10);
         A.CallTo(() => _timeDimension.BaseUnitValueToUnitValue(_timeDisplayUnit, 40.0)).Returns(40);
         base.Context();
      }

      [Observation]
      public void should_span_from_the_start_time_to_the_start_time_plus_the_infusion_time()
      {
         _data.AllValuesInColumn<double>(sut.XValue).ShouldOnlyContain(10);
         _data.AllValuesInColumn<double>(sut.XValue2).ShouldOnlyContain(40);
      }
   }

   public class When_creating_the_chart_data_for_an_infusion_that_ends_after_the_protocol_end_time : concern_for_ProtocolChartData
   {
      protected override void Context()
      {
         //protocol end time (_endTimeMin) is 60 min, but the infusion runs from 60 to 90 min
         var infusion = DomainHelperForSpecs.SchemaItemDTO(ApplicationTypes.Intravenous, _doseUnit, startTimeValue: 60, infusionTimeValue: 30);
         _allSchemaItemsDTOForCompoundCache.Add(new Compound(), new[] {infusion});
         A.CallTo(() => _timeDimension.BaseUnitValueToUnitValue(_timeDisplayUnit, 60.0)).Returns(60);
         A.CallTo(() => _timeDimension.BaseUnitValueToUnitValue(_timeDisplayUnit, 90.0)).Returns(90);
         base.Context();
      }

      [Observation]
      public void should_extend_the_x_axis_maximum_to_the_end_of_the_infusion()
      {
         sut.XMax.ShouldBeEqualTo(90);
      }
   }

   public class When_creating_the_chart_data_for_a_protocol_that_ends_after_the_last_administration : concern_for_ProtocolChartData
   {
      protected override void Context()
      {
         //protocol end time (_endTimeMin) is 60 min, but the only administration is a bolus at 10 min
         var bolus = DomainHelperForSpecs.SchemaItemDTO(ApplicationTypes.IntravenousBolus, _doseUnit, startTimeValue: 10);
         _allSchemaItemsDTOForCompoundCache.Add(new Compound(), new[] {bolus});
         A.CallTo(() => _timeDimension.BaseUnitValueToUnitValue(_timeDisplayUnit, 10.0)).Returns(10);
         A.CallTo(() => _timeDimension.BaseUnitValueToUnitValue(_timeDisplayUnit, 60.0)).Returns(60);
         base.Context();
      }

      [Observation]
      public void should_use_the_protocol_end_time_as_the_x_axis_maximum()
      {
         sut.XMax.ShouldBeEqualTo(60);
      }
   }

   public class When_creating_the_chart_data_for_a_non_infusion_schema_item : concern_for_ProtocolChartData
   {
      protected override void Context()
      {
         var bolus = DomainHelperForSpecs.SchemaItemDTO(ApplicationTypes.IntravenousBolus, _doseUnit, startTimeValue: 10); //min
         _allSchemaItemsDTOForCompoundCache.Add(new Compound(), new[] {bolus});
         A.CallTo(() => _timeDimension.BaseUnitValueToUnitValue(_timeDisplayUnit, 10.0)).Returns(10);
         base.Context();
      }

      [Observation]
      public void should_set_the_end_time_equal_to_the_start_time()
      {
         _data.AllValuesInColumn<double>(sut.XValue2).ShouldOnlyContain(10);
      }
   }

   public class When_creating_the_chart_data_for_two_infusions_with_the_same_start_time_but_different_durations : concern_for_ProtocolChartData
   {
      protected override void Context()
      {
         var infusion1 = DomainHelperForSpecs.SchemaItemDTO(ApplicationTypes.Intravenous, _doseUnit, doseValue: 1, startTimeValue: 0, infusionTimeValue: 30);
         var infusion2 = DomainHelperForSpecs.SchemaItemDTO(ApplicationTypes.Intravenous, _doseUnit, doseValue: 2, startTimeValue: 0, infusionTimeValue: 60);
         _allSchemaItemsDTOForCompoundCache.Add(new Compound(), new[] {infusion1, infusion2});
         base.Context();
      }

      [Observation]
      public void should_not_aggregate_infusions_that_differ_only_in_duration()
      {
         _data.Rows.Count.ShouldBeEqualTo(2);
      }
   }
}