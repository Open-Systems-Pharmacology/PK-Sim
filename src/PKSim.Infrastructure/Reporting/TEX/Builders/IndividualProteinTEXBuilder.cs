using System.Text;
using OSPSuite.Core.Domain;
using OSPSuite.Infrastructure.Reporting;
using OSPSuite.TeXReporting.Builder;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Infrastructure.Reporting.TeX.Builders
{
   public class IndividualProteinTeXBuilder : IndividualMoleculeTEXBuilder<IndividualProtein>
   {
      public IndividualProteinTeXBuilder(ITeXBuilderRepository builderRepository, IRepresentationInfoRepository representationInfoRepository) : base(builderRepository, representationInfoRepository)
      {
      }

      protected override void AddMoleculeSpecificReportPart(IndividualProtein protein, OSPSuiteTracker buildTracker)
      {
         var sb = new StringBuilder();

         sb.AddIs(PKSimConstants.UI.LocalizationInTissue, protein.TissueLocation);

         if (protein.TissueLocation == TissueLocation.Intracellular)
            sb.AddIs(PKSimConstants.UI.IntracellularVascularEndoLocation, protein.IntracellularVascularEndoLocation);

         if (protein.TissueLocation == TissueLocation.ExtracellularMembrane)
            sb.AddIs(PKSimConstants.UI.LocationOnVascularEndo, protein.MembraneLocation);

         _builderRepository.Report(sb, buildTracker);
      }

      protected override string ExpressionContainerDisplayNameFor(IParameter parameter)
      {
         return _representationInfoRepository.DisplayNameFor(parameter.ParentContainer);
      }
   }
}