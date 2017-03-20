using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Reporting;
using PKSim.Core.Repositories;

namespace PKSim.Infrastructure.Reporting.Summary
{
   public class CompoundPropertiesReportBuilder : ReportBuilder<CompoundProperties>
   {
      private readonly IRepresentationInfoRepository _representationInfoRepository;
      private readonly IReportGenerator _reportGenerator;

      public CompoundPropertiesReportBuilder(IRepresentationInfoRepository representationInfoRepository, IReportGenerator reportGenerator)
      {
         _representationInfoRepository = representationInfoRepository;
         _reportGenerator = reportGenerator;
      }

      protected override void FillUpReport(CompoundProperties compoundProperties, ReportPart reportPart)
      {
         var processes = compoundProperties.Processes;
         var compoundConfig = new TablePart(PKSimConstants.UI.Parameter, PKSimConstants.UI.AlternativeInCompound)
            {
               Title = PKSimConstants.UI.SimulationCompoundsConfiguration
            };


         foreach (var alternativeSelection in compoundProperties.CompoundGroupSelections)
         {
            var parameterName = _representationInfoRepository.DisplayNameFor(RepresentationObjectType.GROUP, alternativeSelection.GroupName);
            compoundConfig.AddIs(parameterName, alternativeSelection.AlternativeName);
         }

         reportPart.AddPart(compoundConfig);
         reportPart.AddPart(_reportGenerator.ReportFor(processes.MetabolizationSelection).WithTitle(PKSimConstants.UI.SimulationMetabolism));
         reportPart.AddPart(_reportGenerator.ReportFor(processes.TransportAndExcretionSelection).WithTitle(PKSimConstants.UI.SimulationTransportAndExcretion));
         reportPart.AddPart(_reportGenerator.ReportFor(processes.SpecificBindingSelection).WithTitle(PKSimConstants.UI.SimulationSpecificBinding));
      }
   }
}