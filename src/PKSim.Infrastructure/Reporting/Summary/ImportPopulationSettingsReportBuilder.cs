using System.Globalization;
using System.Linq;
using PKSim.Assets;
using OSPSuite.Utility;
using PKSim.Core.Model;
using PKSim.Core.Reporting;

namespace PKSim.Infrastructure.Reporting.Summary
{
   public class ImportPopulationSettingsReportBuilder : ReportBuilder<ImportPopulationSettings>
   {
      protected override void FillUpReport(ImportPopulationSettings populationSettings, ReportPart reportPart)
      {
         var baseOnIndividualPart = new ReportPart().WithTitle(PKSimConstants.UI.BasedOnIndividual);
         baseOnIndividualPart.AddToContent(populationSettings.BaseIndividual.Name);

         var numberOfIndividuals = populationSettings.AllFiles.Sum(x => x.NumberOfIndividuals);
         var numberOfIndividualPart = new ReportPart().WithTitle(PKSimConstants.UI.NumberOfIndividuals);
         numberOfIndividualPart.AddToContent(numberOfIndividuals.ToString(CultureInfo.InvariantCulture));
         
         var part = new ReportPart().WithTitle(PKSimConstants.UI.CreatedUsingFiles);
   
         foreach (var populationFiles in populationSettings.AllFiles)
         {
            part.AddToContent(FileHelper.ShortenPathName(populationFiles.FilePath,50));
         }

         reportPart.AddPart(baseOnIndividualPart);
         reportPart.AddPart(numberOfIndividualPart);
         reportPart.AddPart(part);
      }
   }
}