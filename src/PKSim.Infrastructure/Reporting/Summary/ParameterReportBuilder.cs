using System.Linq;
using OSPSuite.Core.Domain;
using PKSim.Assets;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Reporting;
using PKSim.Core.Repositories;

namespace PKSim.Infrastructure.Reporting.Summary
{
   public class ParameterReportBuilder : ReportBuilder<IParameter>
   {
      private readonly IRepresentationInfoRepository _representationInfoRepository;
      private readonly IParameterListOfValuesRetriever _parameterListOfValuesRetriever;

      public ParameterReportBuilder(IRepresentationInfoRepository representationInfoRepository,
         IParameterListOfValuesRetriever parameterListOfValuesRetriever)
      {
         _representationInfoRepository = representationInfoRepository;
         _parameterListOfValuesRetriever = parameterListOfValuesRetriever;
      }

      protected override void FillUpReport(IParameter parameter, ReportPart reportPart)
      {
         var displayName = _representationInfoRepository.DisplayNameFor(parameter);
         var displayValue = ParameterMessages.DisplayValueFor(parameter);
         var listOfValues = _parameterListOfValuesRetriever.ListOfValuesFor(parameter);
         if (listOfValues.Any())
            displayValue = listOfValues[parameter.Value];

         reportPart.AddToContent(PKSimConstants.UI.ReportIs(displayName, displayValue));
      }
   }
}