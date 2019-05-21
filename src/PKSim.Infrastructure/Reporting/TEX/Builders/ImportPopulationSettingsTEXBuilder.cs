using System.Collections.Generic;
using System.Data;
using OSPSuite.Infrastructure.Reporting;
using OSPSuite.TeXReporting.Builder;
using OSPSuite.TeXReporting.Items;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Infrastructure.Reporting.TeX.Builders
{
   internal class ImportPopulationSettingsTeXBuilder : OSPSuiteTeXBuilder<ImportPopulationSettings>
   {
      private readonly ITeXBuilderRepository _builderRepository;
      private readonly IBuildingBlockRepository _buildingBlockRepository;

      public ImportPopulationSettingsTeXBuilder(ITeXBuilderRepository builderRepository, IBuildingBlockRepository buildingBlockRepository)
      {
         _builderRepository = builderRepository;
         _buildingBlockRepository = buildingBlockRepository;
      }

      public override void Build(ImportPopulationSettings importSettings, OSPSuiteTracker tracker)
      {
         var objectToReports = new List<object>();

         objectToReports.Add(new SubSection(PKSimConstants.UI.PopulationProperties));

         objectToReports.Add(new Text("{0}: {1}",
                                      PKSimConstants.UI.BasedOnIndividual,
                                      importSettings.BaseIndividual.Name));

         objectToReports.Add(new SubSection("List of Imported Files"));
         objectToReports.Add(getTableFor(importSettings.AllFiles));

         _builderRepository.Report(objectToReports, tracker);
      }

      private DataTable getTableFor(IEnumerable<PopulationImportFile> importedFiles)
      {
         var dt = new DataTable("Imported Files");

         dt.Columns.Add(PKSimConstants.UI.FilePath, typeof (string));
         dt.Columns.Add(PKSimConstants.UI.NumberOfIndividuals, typeof (int));

         dt.BeginLoadData();
         foreach (var file in importedFiles)
         {
            var row = dt.NewRow();
            row[PKSimConstants.UI.FilePath] = file.FilePath;
            row[PKSimConstants.UI.NumberOfIndividuals] = file.NumberOfIndividuals;
            dt.Rows.Add(row);
         }
         dt.EndLoadData();

         return dt;
      }
   }
}