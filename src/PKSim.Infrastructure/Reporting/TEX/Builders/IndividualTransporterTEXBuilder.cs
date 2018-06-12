using System.Text;
using OSPSuite.Core.Domain;
using OSPSuite.Infrastructure.Reporting;
using OSPSuite.TeXReporting.Builder;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Infrastructure.Reporting.TeX.Builders
{
   public class IndividualTransporterTeXBuilder : IndividualMoleculeTEXBuilder<IndividualTransporter>
   {
      public IndividualTransporterTeXBuilder(ITeXBuilderRepository builderRepository, IRepresentationInfoRepository representationInfoRepository)
         : base(builderRepository, representationInfoRepository)
      {
      }

      protected override void AddMoleculeSpecificReportPart(IndividualTransporter transporter, OSPSuiteTracker buildTracker)
      {
         var sb = new StringBuilder();
         sb.AddIs(PKSimConstants.UI.TransporterType, transporter.TransportType);
         _builderRepository.Report(sb, buildTracker);
      }

      protected override string ExpressionContainerDisplayNameFor(IParameter parameter)
      {
         var container = parameter.ParentContainer.DowncastTo<TransporterExpressionContainer>();
         var displayName = _representationInfoRepository.DisplayNameFor(container);
         if (container.HasPolarizedMembrane)
            displayName = $"{displayName} ({container.MembraneLocation})";
         return displayName;
      }
   }
}