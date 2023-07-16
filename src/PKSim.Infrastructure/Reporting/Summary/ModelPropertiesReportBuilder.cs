using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Reporting;
using PKSim.Core.Repositories;

namespace PKSim.Infrastructure.Reporting.Summary
{
   public class ModelPropertiesReportBuilder : ReportBuilder<ModelProperties>
   {
      private readonly IRepresentationInfoRepository _representationInfoRepository;
      private readonly IReportGenerator _reportGenerator;

      public ModelPropertiesReportBuilder(IRepresentationInfoRepository representationInfoRepository, IReportGenerator reportGenerator)
      {
         _representationInfoRepository = representationInfoRepository;
         _reportGenerator = reportGenerator;
      }

      protected override void FillUpReport(ModelProperties modelProperties, ReportPart reportPart)
      {
         reportPart.Title = PKSimConstants.UI.SimulationModelConfiguration;
         if(modelProperties.ModelConfiguration!=null)
            reportPart.AddToContent(_representationInfoRepository.DisplayNameFor(RepresentationObjectType.MODEL, modelProperties.ModelConfiguration.ModelName));

         reportPart.AddPart(_reportGenerator.ReportFor(modelProperties.AllCalculationMethods()));
      }
   }
}