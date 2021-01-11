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
      public IndividualProteinTeXBuilder(ITeXBuilderRepository builderRepository, IRepresentationInfoRepository representationInfoRepository) : base(
         builderRepository, representationInfoRepository)
      {
      }

      protected override void AddMoleculeSpecificReportPart(IndividualProtein protein, OSPSuiteTracker buildTracker)
      {
         var sb = new StringBuilder();

         sb.AddIs(PKSimConstants.UI.Localization, protein.Localization);

         _builderRepository.Report(sb, buildTracker);
      }

      protected override string ExpressionContainerDisplayNameFor(IParameter parameter)
      {
         return _representationInfoRepository.DisplayNameFor(parameter.ParentContainer);
      }
   }
}