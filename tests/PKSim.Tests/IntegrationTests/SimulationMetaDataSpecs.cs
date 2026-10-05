using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Reporting;
using PKSim.Infrastructure;
using PKSim.Infrastructure.Serialization.ORM.Mappers;

namespace PKSim.IntegrationTests
{
   public class When_reading_a_saved_simulation_selecting_an_overwrite_parameter_set_without_loading_it : ContextForSimulationIntegration<ISimulationToSimulationMetaDataMapper>
   {
      private Simulation _savedSimulation;

      public override void GlobalContext()
      {
         base.GlobalContext();
         var compound = DomainFactoryForSpecs.CreateStandardCompound();
         _simulation = DomainFactoryForSpecs.CreateSimulationWith(DomainFactoryForSpecs.CreateStandardIndividual(), compound, DomainFactoryForSpecs.CreateStandardIVBolusProtocol()).DowncastTo<IndividualSimulation>();

         var overwriteParameterSet = new OverwriteParameterSet {Name = "Renal impairment"};
         _simulation.Compounds.FindByName(compound.Name).AddOverwriteParameterSet(overwriteParameterSet);
         _simulation.AddOverwriteParameterSetSelection(compound.Name, overwriteParameterSet);
      }

      protected override void Because()
      {
         var simulationMetaData = sut.MapFrom(_simulation);
         _savedSimulation = IoC.Resolve<ISimulationMetaDataToSimulationMapper>().MapFrom(simulationMetaData);
      }

      [Observation]
      public void should_show_the_selected_overwrite_parameter_set_in_the_tooltip_of_the_simulation()
      {
         _savedSimulation.IsLoaded.ShouldBeFalse();
         IoC.Resolve<IReportGenerator>().StringReportFor(_savedSimulation).Contains("Renal impairment").ShouldBeTrue();
      }
   }
}
