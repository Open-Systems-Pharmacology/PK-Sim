using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Container;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ProjectConverter.v5_4;
using PKSim.IntegrationTests;

namespace PKSim.ProjectConverter.v5_4
{
   public class When_converting_the_PopulationAnalysis_531_project : ContextWithLoadedProject<Converter532To541>
   {
      private PopulationSimulation _simulation;
      private IDimensionRepository _dimensionRepository;

      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("PopulationAnalysis_531");
         _simulation = First<PopulationSimulation>();
         _dimensionRepository = IoC.Resolve<IDimensionRepository>();
      }

      [Observation]
      public void should_have_set_the_default_display_unit_for_the_time_profile_anlysis()
      {
         foreach (var timeProfileAnalysis in _simulation.Analyses.OfType<TimeProfileAnalysisChart>())
         {
            timeProfileAnalysis.PopulationAnalysis.TimeUnit.ShouldBeEqualTo(_dimensionRepository.Time.DefaultUnit);
         }
      }
   }
}