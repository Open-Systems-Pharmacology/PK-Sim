using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure;
using PKSim.Presentation.Core;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_SimulationParametersToBuildingBlockUpdater : ContextForSimulationIntegration<ISimulationParametersToBuildingBlockUpdater>
   {
      protected Individual _templateIndividual;
      private ICoreWorkspace _workspace;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _templateIndividual = DomainFactoryForSpecs.CreateStandardIndividual();
         var compound = DomainFactoryForSpecs.CreateStandardCompound();
         var protocol = DomainFactoryForSpecs.CreateStandardIVBolusProtocol();
         _simulation = DomainFactoryForSpecs.CreateSimulationWith(_templateIndividual, compound, protocol) as IndividualSimulation;
         _workspace = IoC.Resolve<ICoreWorkspace>();
         var project = new PKSimProject();
         project.AddBuildingBlock(compound);
         project.AddBuildingBlock(protocol);
         project.AddBuildingBlock(_simulation);
         project.AddBuildingBlock(_templateIndividual);
         _workspace.Project = project;
      }
   }

   public class When_updating_the_parameter_values_from_a_simulation_building_block_in_the_template_building_block : concern_for_SimulationParametersToBuildingBlockUpdater
   {
      private ICommand _result;
      private IParameter _templateParameter;

      public override void GlobalContext()
      {
         base.GlobalContext();
         var parameterInSimulation = _simulation.Individual.Organism.Organ(CoreConstants.Organ.Liver).Parameter(CoreConstants.Parameters.BLOOD_FLOW);
         _templateParameter = _templateIndividual.Organism.Organ(CoreConstants.Organ.Liver).Parameter(CoreConstants.Parameters.BLOOD_FLOW);

         parameterInSimulation.Value = 3;
      }

      protected override void Because()
      {
         _result = sut.UpdateParametersFromSimulationInBuildingBlock(_simulation, _templateIndividual);
      }

      [Observation]
      public void should_have_updated_the_parameter_values_in_the_building_block_according_to_the_value_in_the_simulation()
      {
         _templateParameter.Value.ShouldBeEqualTo(3);
      }

      [Observation]
      public void should_not_update_the_parameter_id_of_the_template_parameter ()
      {
         string.IsNullOrEmpty(_templateParameter.Origin.ParameterId).ShouldBeTrue();
      }

      [Observation]
      public void should_return_a_command_containing_all_the_sub_commands_describing_the_update()
      {
         _result.IsEmpty().ShouldBeFalse();
      }
   }
}