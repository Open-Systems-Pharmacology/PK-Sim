using System.Collections.Generic;
using OSPSuite.Infrastructure.Reporting;
using OSPSuite.TeXReporting.Builder;
using PKSim.Core.Model;
using PKSim.Core.Reporting;
using PKSim.Core.Services;

namespace PKSim.Infrastructure.Reporting.TeX.Builders
{
   public class RandomPopulationTeXBuilder : BuildingBlockTeXBuilder<RandomPopulation>
   {
      public RandomPopulationTeXBuilder(ITeXBuilderRepository builderRepository, IReportGenerator reportGenerator, ILazyLoadTask lazyLoadTask) : base(builderRepository, reportGenerator, lazyLoadTask)
      {
      }

      protected override IEnumerable<object> BuildingBlockReport(RandomPopulation population, OSPSuiteTracker tracker)
      {
         return new List<object>
                   {
                      population.Settings,
                      new Items.SelectedDistribution(population)
                   };
      }
   }
}