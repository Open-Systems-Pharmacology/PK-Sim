using System.Globalization;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Reporting;

namespace PKSim.Infrastructure.Reporting.Summary
{
   public class PopulationSettingsReportBuilder : ReportBuilder<RandomPopulationSettings>
   {
      private readonly IReportGenerator _reportGenerator;

      public PopulationSettingsReportBuilder(IReportGenerator reportGenerator)
      {
         _reportGenerator = reportGenerator;
      }

      protected override void FillUpReport(RandomPopulationSettings populationSettings, ReportPart reportPart)
      {
         var baseOnIndividualPart = new ReportPart().WithTitle(PKSimConstants.UI.BasedOnIndividual);
         baseOnIndividualPart.AddToContent(populationSettings.BaseIndividual.Name);

         var numberOfIndividualPart = new ReportPart().WithTitle(PKSimConstants.UI.NumberOfIndividuals);
         numberOfIndividualPart.AddToContent(populationSettings.NumberOfIndividuals.ToString(CultureInfo.InvariantCulture));

         var genderRatioPart = new ReportPart().WithTitle(PKSimConstants.UI.GenderRatio);
         foreach (var genderRatio in populationSettings.GenderRatios)
         {
            genderRatioPart.AddToContent(_reportGenerator.ReportFor(genderRatio).Content);
         }

         var parameterRangePart = new ReportPart().WithTitle(PKSimConstants.UI.PopulationParameterRanges);
         foreach (var parameterRange in populationSettings.ParameterRanges)
         {
            parameterRangePart.AddToContent(_reportGenerator.ReportFor(parameterRange).Content);
         }

         reportPart.AddPart(baseOnIndividualPart);
         reportPart.AddPart(numberOfIndividualPart);
         reportPart.AddPart(genderRatioPart);
         reportPart.AddPart(parameterRangePart);
      }
   }
}