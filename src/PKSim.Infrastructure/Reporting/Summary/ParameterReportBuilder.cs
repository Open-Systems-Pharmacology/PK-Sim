using System.Linq;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Reporting;
using PKSim.Core.Repositories;
using PKSim.Presentation.DTO.Mappers;
using OSPSuite.Core.Domain;

namespace PKSim.Infrastructure.Reporting.Summary
{
   public class ParameterReportBuilder : ReportBuilder<IParameter>
   {
      private readonly IRepresentationInfoRepository _representationInfoRepository;
      private readonly IParameterToParameterDTOMapper _parameterMapper;

      public ParameterReportBuilder(IRepresentationInfoRepository representationInfoRepository, IParameterToParameterDTOMapper parameterMapper)
      {
         _representationInfoRepository = representationInfoRepository;
         _parameterMapper = parameterMapper;
      }

      protected override void FillUpReport(IParameter parameter, ReportPart reportPart)
      {
         var parameterDTO = _parameterMapper.MapFrom(parameter);
         var displayName= _representationInfoRepository.DisplayNameFor(parameter);
         var displayValue = ParameterMessages.DisplayValueFor(parameter);

         if (parameterDTO.ListOfValues.Any())
            displayValue = parameterDTO.ListOfValues[parameter.Value];

         reportPart.AddToContent(PKSimConstants.UI.ReportIs(displayName, displayValue));
      }
   }
}