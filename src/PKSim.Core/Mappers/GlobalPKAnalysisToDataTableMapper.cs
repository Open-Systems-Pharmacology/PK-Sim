using System.Data;
using PKSim.Assets;
using OSPSuite.Utility.Validation;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using OSPSuite.Core.Extensions;

namespace PKSim.Core.Mappers
{
   public interface IGlobalPKAnalysisToDataTableMapper
   {
      DataTable MapFrom(GlobalPKAnalysis globalPKAnalysis, bool addMetaData = false);
   }

   public class GlobalPKAnalysisToDataTableMapper : IGlobalPKAnalysisToDataTableMapper
   {
      private readonly IRepresentationInfoRepository _representationInfoRepository;

      public GlobalPKAnalysisToDataTableMapper(IRepresentationInfoRepository representationInfoRepository)
      {
         _representationInfoRepository = representationInfoRepository;
      }

      public DataTable MapFrom(GlobalPKAnalysis globalPKAnalysis, bool addMetaData = false)
      {
         var dataTable = new DataTable(PKSimConstants.UI.GlobalPKAnalyses);
         dataTable.AddColumn(PKSimConstants.PKAnalysis.ParameterDisplayName);
         dataTable.AddColumn(PKSimConstants.PKAnalysis.Compound);
         dataTable.AddColumn<double>(PKSimConstants.PKAnalysis.Value);
         dataTable.AddColumn(PKSimConstants.PKAnalysis.Unit);

         if (addMetaData)
         {
            dataTable.AddColumn(PKSimConstants.PKAnalysis.ParameterName);
            dataTable.AddColumn(PKSimConstants.PKAnalysis.Description);
            dataTable.AddColumn(PKSimConstants.PKAnalysis.Warning);
         }

         foreach (var parameterName in globalPKAnalysis.AllPKParameterNames)
         {
            foreach (var compoundName in globalPKAnalysis.CompoundNames)
            {
               var parameter = globalPKAnalysis.PKParameter(compoundName, parameterName);
               if (parameter == null)
                  continue;

               var info = _representationInfoRepository.InfoFor(parameter);
               var row = dataTable.NewRow();

               row[PKSimConstants.PKAnalysis.ParameterDisplayName] = info.DisplayName;
               row[PKSimConstants.PKAnalysis.Compound] = compoundName;
               row[PKSimConstants.PKAnalysis.Value] = parameter.ValueInDisplayUnit;
               row[PKSimConstants.PKAnalysis.Unit] = parameter.DisplayUnit;

               if (addMetaData)
               {
                  row[PKSimConstants.PKAnalysis.ParameterName] = parameter.Name;
                  row[PKSimConstants.PKAnalysis.Warning] = parameter.Validate().Message;
                  row[PKSimConstants.PKAnalysis.Description] = info.Description;
               }

               dataTable.Rows.Add(row);
            }
         }

         return dataTable;
      }
   }
}