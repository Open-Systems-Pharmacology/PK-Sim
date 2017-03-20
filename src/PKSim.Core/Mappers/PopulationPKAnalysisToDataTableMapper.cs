using System.Collections.Generic;
using System.Data;
using PKSim.Assets;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using OSPSuite.Core.Domain.PKAnalyses;

namespace PKSim.Core.Mappers
{
   public interface IPopulationPKAnalysisToDataTableMapper
   {
      DataTable MapFrom(IReadOnlyList<PopulationPKAnalysis> allPKanalyses, bool addMetaData = false);
   }

   public class PopulationPKAnalysisToDataTableMapper : BasePKAnalysisToDataTableMapper, IPopulationPKAnalysisToDataTableMapper
   {
      public PopulationPKAnalysisToDataTableMapper(IPKParameterRepository pkParameterRepository, IRepresentationInfoRepository representationInfoRepository) : base(pkParameterRepository, representationInfoRepository)
      {
      }

      public DataTable MapFrom(IReadOnlyList<PopulationPKAnalysis> allPKanalyses, bool addMetaData = false)
      {
         var dataTable = new DataTable(PKSimConstants.UI.PKAnalyses);
         CreateTable(dataTable, addMetaData);

         allPKanalyses.Each(analysis => addAnalysisToTable(analysis, dataTable, addMetaData));

         return dataTable;
      }

      private void addAnalysisToTable(PopulationPKAnalysis analysis, DataTable dataTable, bool addMetaData)
      {
         var curveName = getCurveName(analysis);
         AddPKParametersToDataTable(analysis.PKAnalysis, dataTable, curveName, addMetaData);
      }

      private string getCurveName(PopulationPKAnalysis analysis)
      {
         return analysis.CurveData.Caption;
      }
   }
}