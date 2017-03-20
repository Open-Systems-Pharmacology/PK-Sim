using System.Collections.Generic;
using System.Data;
using System.Linq;
using PKSim.Assets;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain.PKAnalyses;

namespace PKSim.Core.Mappers
{
   public interface IIndividualPKAnalysisToDataTableMapper
   {
      DataTable MapFrom(IReadOnlyList<IndividualPKAnalysis> allPKanalyses, IEnumerable<ICurve> curves, bool addMetaData = false);
   }

   public class IndividualPKAnalysisToDataTableMapper : BasePKAnalysisToDataTableMapper, IIndividualPKAnalysisToDataTableMapper
   {
      public IndividualPKAnalysisToDataTableMapper(IPKParameterRepository pkParameterRepository, IRepresentationInfoRepository representationInfoRepository) : base(pkParameterRepository, representationInfoRepository)
      {
      }

      public DataTable MapFrom(IReadOnlyList<IndividualPKAnalysis> allPKanalyses, IEnumerable<ICurve> curves, bool addMetaData = false)
      {
         var dataTable = new DataTable(PKSimConstants.UI.PKAnalyses);
         CreateTable(dataTable, addMetaData);
         allPKanalyses.Each(analysis => addAnalysisToTable(analysis, curves, dataTable, addMetaData));
         return dataTable;
      }

      private void addAnalysisToTable(IndividualPKAnalysis analysis, IEnumerable<ICurve> curves, DataTable dataTable, bool addMetaData)
      {
         var curveName = getCurveName(analysis, curves);
         AddPKParametersToDataTable(analysis.PKAnalysis, dataTable, curveName, addMetaData);
      }

      private static string getCurveName(IndividualPKAnalysis analysis, IEnumerable<ICurve> curves)
      {
         return curves.First(c => c.yData == analysis.DataColumn).Name;
      }
   }
}