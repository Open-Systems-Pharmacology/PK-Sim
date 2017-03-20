using System.Data;
using PKSim.Assets;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Validation;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.PKAnalyses;
using OSPSuite.Core.Extensions;

namespace PKSim.Core.Mappers
{
   public abstract class BasePKAnalysisToDataTableMapper
   {
      private readonly IPKParameterRepository _pkParameterRepository;
      private readonly IRepresentationInfoRepository _representationInfoRepository;

      protected BasePKAnalysisToDataTableMapper(IPKParameterRepository pkParameterRepository, IRepresentationInfoRepository representationInfoRepository)
      {
         _pkParameterRepository = pkParameterRepository;
         _representationInfoRepository = representationInfoRepository;
      }

      protected void AddPKParametersToDataTable(PKAnalysis pkAnalysis, DataTable dataTable, string curveName, bool addMetaData)
      {
         pkAnalysis.AllPKParameters.Each(parameter => AddPKParameterToDataTable(pkAnalysis, dataTable, curveName, parameter, addMetaData));
      }

      protected void AddPKParameterToDataTable(PKAnalysis pkAnalysis, DataTable dataTable, string curveName, IParameter parameter, bool addMetaData)
      {
         var pkValue = parameter.ValueInDisplayUnit;

         var row = dataTable.NewRow();
         row[PKSimConstants.PKAnalysis.Value] = pkValue;
         row[PKSimConstants.PKAnalysis.CurveName] = curveName;

         if (addMetaData)
         {
            row[PKSimConstants.PKAnalysis.Compound] = parameter.ParentContainer.Name;
            row[PKSimConstants.PKAnalysis.ParameterName] = parameter.Name;
            row[PKSimConstants.PKAnalysis.Description] = descriptionFor(parameter);
            row[PKSimConstants.PKAnalysis.Warning] = parameter.Validate().Message;
         }

         row[PKSimConstants.PKAnalysis.ParameterDisplayName] = _pkParameterRepository.DisplayNameFor(parameter.Name);
         row[PKSimConstants.PKAnalysis.Unit] = parameter.DisplayUnit.Name;
         dataTable.Rows.Add(row);
      }

      private string descriptionFor(IParameter parameter)
      {
         var pkDescription = _pkParameterRepository.DescriptionFor(parameter.Name);
         return !string.IsNullOrEmpty(pkDescription) ? pkDescription : _representationInfoRepository.DescriptionFor(parameter);
      }

      protected static void CreateTable(DataTable dataTable, bool addMetaData)
      {
         dataTable.AddColumn(PKSimConstants.PKAnalysis.CurveName);

         if (addMetaData)
         {
            dataTable.AddColumn(PKSimConstants.PKAnalysis.Compound);
            dataTable.AddColumn(PKSimConstants.PKAnalysis.ParameterName);
            dataTable.AddColumn(PKSimConstants.PKAnalysis.Description);
            dataTable.AddColumn(PKSimConstants.PKAnalysis.Warning);
         }

         dataTable.AddColumn(PKSimConstants.PKAnalysis.ParameterDisplayName);
         dataTable.AddColumn<double>(PKSimConstants.PKAnalysis.Value);
         dataTable.AddColumn(PKSimConstants.PKAnalysis.Unit);
      }
   }
}