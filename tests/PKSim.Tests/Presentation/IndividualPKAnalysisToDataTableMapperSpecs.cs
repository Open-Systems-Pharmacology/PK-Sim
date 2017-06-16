using System.Collections.Generic;
using System.Data;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Mappers;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain.PKAnalyses;
using DataColumn = OSPSuite.Core.Domain.Data.DataColumn;

namespace PKSim.Presentation
{
   public abstract class concern_for_IndividualPKAnalysisToDataTableMapper : ContextSpecification<IndividualPKAnalysisToDataTableMapper>
   {
      private IPKParameterRepository _pkParameterRepository;
      private IRepresentationInfoRepository _representationInfoRepository;
      protected IReadOnlyList<IndividualPKAnalysis> _allPKAnalysis;
      protected DataTable _result;
      protected IList<Curve> _curves;

      protected override void Context()
      {
         _pkParameterRepository = A.Fake<IPKParameterRepository>();
         _representationInfoRepository = A.Fake<IRepresentationInfoRepository>();
         sut = new IndividualPKAnalysisToDataTableMapper(_pkParameterRepository, _representationInfoRepository);

         _curves = new List<Curve>();
         var curve1 = A.Fake<Curve>();
         var curve2 = A.Fake<Curve>();
         var dataColumn1 = generateDataColumn();
         var dataColumn2 = generateDataColumn();
         A.CallTo(() => curve1.yData).Returns(dataColumn1);
         A.CallTo(() => curve2.yData).Returns(dataColumn2);
         _curves.Add(curve1);
         _curves.Add(curve2);

         _allPKAnalysis = new List<IndividualPKAnalysis> {new IndividualPKAnalysis(dataColumn1, PKAnalysisHelperForSpecs.GenerateIndividualPKAnalysis()), new IndividualPKAnalysis(dataColumn2, PKAnalysisHelperForSpecs.GenerateIndividualPKAnalysis())};
      }

      private DataColumn generateDataColumn()
      {
         return new DataColumn();
      }
   }

   public class When_mapping_data_table_from_individual_analysis : concern_for_IndividualPKAnalysisToDataTableMapper
   {
      protected override void Because()
      {
         _result = sut.MapFrom(_allPKAnalysis, _curves);
      }

      [Observation]
      public void values_should_be_mapped_from_analysis_to_data_table()
      {
         var i = 0;
         foreach (DataRow row in _result.Rows)
         {
            // Checking that both analyses have mapped values but the values are all from 0-9 for both analyses
            // so the restart point is at 10.
            row[PKSimConstants.PKAnalysis.Value].ShouldBeEqualTo(i % 10);
            i++;
         }
      }

      [Observation]
      public void must_have_one_row_for_each_required_field_per_analysis()
      {
         _result.Rows.Count.ShouldBeEqualTo(20);
      }

      [Observation]
      public void should_not_add_four_extra_columns_for_the_internal_values()
      {
         _result.Columns[PKSimConstants.PKAnalysis.Compound].ShouldBeNull();
         _result.Columns[PKSimConstants.PKAnalysis.ParameterName].ShouldBeNull();
         _result.Columns[PKSimConstants.PKAnalysis.Warning].ShouldBeNull();
         _result.Columns[PKSimConstants.PKAnalysis.Description].ShouldBeNull();
      }
   }

   public class When_mapping_data_table_from_individual_analysis_with_meta_data : concern_for_IndividualPKAnalysisToDataTableMapper
   {
      protected override void Because()
      {
         _result = sut.MapFrom(_allPKAnalysis, _curves, addMetaData:true);
      }

      [Observation]
      public void values_should_be_mapped_from_analysis_to_data_table()
      {
         var i = 0;
         foreach (DataRow row in _result.Rows)
         {
            // Checking that both analyses have mapped values but the values are all from 0-9 for both analyses
            // so the restart point is at 10.
            row[PKSimConstants.PKAnalysis.Value].ShouldBeEqualTo(i % 10);
            i++;
         }
      }

      [Observation]
      public void must_have_one_row_for_each_required_field_per_analysis()
      {
         _result.Rows.Count.ShouldBeEqualTo(20);
      }

      [Observation]
      public void should_have_added_four_extra_columns_for_the_internal_values()
      {
         _result.Columns[PKSimConstants.PKAnalysis.Compound].ShouldNotBeNull();
         _result.Columns[PKSimConstants.PKAnalysis.ParameterName].ShouldNotBeNull();
         _result.Columns[PKSimConstants.PKAnalysis.Warning].ShouldNotBeNull();
         _result.Columns[PKSimConstants.PKAnalysis.Description].ShouldNotBeNull();
      }
   }
}