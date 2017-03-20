using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ProjectConverter.v5_5;
using PKSim.IntegrationTests;
using OSPSuite.Core.Domain;

namespace PKSim.ProjectConverter.v5_5
{
   public class When_converting_the_SimulationSettings_54_project : ContextWithLoadedProject<Converter54To551>
   {
      private Simulation _individualSimulation;
      private Simulation _populationSimulation;
      private IDimensionRepository _dimensionRepository;

      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("SimulationSettings_54");
         _individualSimulation = First<IndividualSimulation>();
         _populationSimulation = First<PopulationSimulation>();
         _dimensionRepository = IoC.Resolve<IDimensionRepository>();
      }

      [Observation]
      public void should_have_converted_the_selected_outputs()
      {
         _individualSimulation.OutputSelections.Any().ShouldBeTrue();
         _populationSimulation.OutputSelections.Any().ShouldBeTrue();
      }


      [Observation]
      public void all_molecule_amount_should_have_the_dimension_amount()
      {
         _individualSimulation.All<IMoleculeAmount>().Each(m => m.Dimension.ShouldBeEqualTo(_dimensionRepository.Amount));
      }
   }
}