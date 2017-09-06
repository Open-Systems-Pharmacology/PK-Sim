using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Reporting;
using OSPSuite.Core.Domain;

namespace PKSim.Infrastructure.Reporting.Summary
{
   public class CompoundReportBuilder : ReportBuilder<Compound>
   {
      private readonly IReportGenerator _reportGenerator;

      public CompoundReportBuilder(IReportGenerator reportGenerator)
      {
         _reportGenerator = reportGenerator;
      }

      protected override void FillUpReport(Compound compound, ReportPart reportPart)
      {

         reportPart.AddPart(_reportGenerator.ReportFor(compound.ParameterAlternativeGroup(CoreConstants.Groups.COMPOUND_LIPOPHILICITY)));
         reportPart.AddToContent(_reportGenerator.ReportFor(compound.Parameter(Constants.Parameters.MOL_WEIGHT)));
         reportPart.AddToContent(_reportGenerator.ReportFor(compound.Parameter(CoreConstants.Parameter.EFFECTIVE_MOLECULAR_WEIGHT)));
         reportPart.AddToContent(_reportGenerator.ReportFor(compound.Parameter(CoreConstants.Parameter.IS_SMALL_MOLECULE)));

         if(compound.IsNeutral)
            reportPart.AddToContent(PKSimConstants.UI.CompoundTypeNeutral);
         else
         {
            addCompoundTypePart(reportPart, compound, CoreConstants.Parameter.PARAMETER_PKA1, CoreConstants.Parameter.COMPOUND_TYPE1);
            addCompoundTypePart(reportPart, compound, CoreConstants.Parameter.PARAMETER_PKA2, CoreConstants.Parameter.COMPOUND_TYPE2);
            addCompoundTypePart(reportPart, compound, CoreConstants.Parameter.PARAMETER_PKA3, CoreConstants.Parameter.COMPOUND_TYPE3);
         }
         var fractionUnboundReport = _reportGenerator.ReportFor(compound.ParameterAlternativeGroup(CoreConstants.Groups.COMPOUND_FRACTION_UNBOUND));
         fractionUnboundReport.AddToContent(_reportGenerator.ReportFor(compound.Parameter(CoreConstants.Parameter.PLASMA_PROTEIN_BINDING_PARTNER)));
         reportPart.AddPart(fractionUnboundReport);
      }

      private void addCompoundTypePart(ReportPart reportPart, Compound compound, string parameterPka1, string parameterCompoundType1)
      {
         var compoundType = compound.Parameter(parameterCompoundType1).Value;

         if (compoundType == CoreConstants.Compound.COMPOUND_TYPE_NEUTRAL) return;
         var pka = compound.Parameter(parameterPka1).Value;

         string compoundTypeDisplay;
         if (compoundType == CoreConstants.Compound.COMPOUND_TYPE_ACID)
            compoundTypeDisplay = PKSimConstants.UI.CompoundTypeAcid;
         else
            compoundTypeDisplay = PKSimConstants.UI.CompoundTypeBase;

         reportPart.AddToContent("{0} ({1})", pka, compoundTypeDisplay);
      }
   }
}