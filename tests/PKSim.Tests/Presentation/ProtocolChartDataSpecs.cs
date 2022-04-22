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
}