using System.Collections.Generic;
using System.Data;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain.PKAnalyses;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Chart;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

namespace PKSim.Presentation
{
   public abstract class concern_for_PopulationPKAnalysisToDataTableMapper : ContextSpecification<PopulationPKAnalysisToDataTableMapper>
   {
      private IPKParameterRepository _pkParameterRepository;
      private IRepresentationInfoRepository _representationInfoRepository;

      protected override void Context()
      {
         _pkParameterRepository = A.Fake<IPKParameterRepository>();
         _representationInfoRepository = A.Fake<IRepresentationInfoRepository>();
         sut = new PopulationPKAnalysisToDataTableMapper(_pkParameterRepository, _representationInfoRepository);
      }

      protected static PKAnalysis GeneratePKAnalysis()
      {
         return PKAnalysisHelperForSpecs.GeneratePopulationAnalysis();
      }
   }

   public class When_mapping_data_table_from_population_analysis : concern_for_PopulationPKAnalysisToDataTableMapper
   {
      private IReadOnlyList<PopulationPKAnalysis> _allPKAnalysis;
      private DataTable _result;

      protected override void Context()
      {
         base.Context();

         _allPKAnalysis = new List<PopulationPKAnalysis> {new PopulationPKAnalysis(generateCurveData(), GeneratePKAnalysis()), new PopulationPKAnalysis(generateCurveData(), GeneratePKAnalysis())};
      }

      private CurveData<TimeProfileXValue, TimeProfileYValue> generateCurveData()
      {
         return new CurveData<TimeProfileXValue, TimeProfileYValue>();
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_allPKAnalysis);
      }

      [Observation]
      public void values_should_be_mapped_from_analysis_to_data_table()
      {
         var i = 0;
         foreach (DataRow row in _result.Rows)
         {
            // Checking that both analyses have mapped values but the values are all from 0-6 for both analyses
            // so the restart point is at 10.
            row[PKSimConstants.PKAnalysis.Value].ShouldBeEqualTo(i % 7);
            i++;
         }
      }

      [Observation]
      public void must_have_one_row_for_each_required_field_per_analysis()
      {
         _result.Rows.Count.ShouldBeEqualTo(14);
      }
   }
}