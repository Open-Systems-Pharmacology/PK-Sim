using System.Collections.Generic;
using OSPSuite.Infrastructure.Reporting;
using OSPSuite.TeXReporting.Builder;
using PKSim.Core.Model;
using PKSim.Core.Reporting;
using PKSim.Core.Services;
using PKSim.Infrastructure.Reporting.TeX.Items;

namespace PKSim.Infrastructure.Reporting.TeX.Builders
{
   public class ImportPopulationTeXBuilder : BuildingBlockTeXBuilder<ImportPopulation>
   {
      public ImportPopulationTeXBuilder(ITeXBuilderRepository builderRepository, IReportGenerator reportGenerator, ILazyLoadTask lazyLoadTask)
         : base(builderRepository, reportGenerator, lazyLoadTask)
      {
      }

      protected override IEnumerable<object> BuildingBlockReport(ImportPopulation population, OSPSuiteTracker tracker)
      {
         return new List<object>
         {
            population.Settings,
            new SelectedDistribution(population)
         };
      }
   }
}