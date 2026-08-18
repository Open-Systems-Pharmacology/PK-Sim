using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_BuildingBlockParametersToSimulationUpdater : ContextForSimulationIntegration<IBuildingBlockParametersToSimulationUpdater>
   {
      protected Individual _templateIndividual;
      private ICoreWorkspace _workspace;
      protected ExpressionProfile _templateExpressionProfile;
      protected Compound _templateCompound;
      private IMoleculeExpressionTask<Individual> _moleculeExpressionTask;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _moleculeExpressionTask = IoC.Resolve<IMoleculeExpressionTask<Individual>>();

         _templateIndividual = DomainFactoryForSpecs.CreateStandardIndividual();
         _templateExpressionProfile = DomainFactoryForSpecs.CreateExpressionProfile<IndividualEnzyme>();
         _moleculeExpressionTask.AddExpressionProfile(_templateIndividual, _templateExpressionProfile);


         var compound = _templateCompound = DomainFactoryForSpecs.CreateStandardCompound();
         var protocol = DomainFactoryForSpecs.CreateStandardIVBolusProtocol();
         _simulation = DomainFactoryForSpecs.CreateSimulationWith(_templateIndividual, compound, protocol) as IndividualSimulation;
         _workspace = IoC.Resolve<ICoreWorkspace>();
         var project = new PKSimProject();
         project.AddBuildingBlock(compound);
         project.AddBuildingBlock(protocol);
         project.AddBuildingBlock(_simulation);
         project.AddBuildingBlock(_templateIndividual);
         project.AddBuildingBlock(_templateExpressionProfile);
         _workspace.Project = project;
      }
   }

   public class When_updating_the_parameter_values_from_a_template_building_block_in_a_simulation_building_block : concern_for_BuildingBlockParametersToSimulationUpdater
   {
      private ICommand _result;

      public override void GlobalContext()
      {
         base.GlobalContext();
         var templateParameter = _templateIndividual.Organism.Organ(CoreConstants.Organ.LIVER).Parameter(CoreConstants.Parameters.ALLOMETRIC_SCALE_FACTOR);
         templateParameter.Value = 3;

         var templateExpressionProfileParameter = _templateExpressionProfile.Molecule.HalfLifeLiver;
         templateExpressionProfileParameter.Value = 5;
      }

      protected override void Because()
      {
         _result = sut.UpdateParametersFromBuildingBlockInSimulation(_templateIndividual, _simulation);
      }

      [Observation]
      public void should_have_updated_the_parameter_values_in_the_simulation_and_in_the_simulation_building_block_according_to_the_value_in_the_template()
      {
         var simIndividual = _simulation.Individual;
         var parameter = simIndividual.Organism.Organ(CoreConstants.Organ.LIVER).Parameter(CoreConstants.Parameters.ALLOMETRIC_SCALE_FACTOR);
         parameter.Value.ShouldBeEqualTo(3);

         //now parameter in simulation
         var simParameter = _simulation.All<IParameter>().First(x => string.Equals(x.Origin.ParameterId, parameter.Id));
         simParameter.Value.ShouldBeEqualTo(3);
         
      }

      [Observation]
      public void should_have_synchronized_value_in_the_expression_profile()
      {
         var simExpressionProfile = _simulation.BuildingBlockByTemplateId<ExpressionProfile>(_templateExpressionProfile.Id);
         var parameter = simExpressionProfile.Molecule.HalfLifeLiver;
         parameter.Value.ShouldBeEqualTo(5);
      }

      [Observation]
      public void should_return_a_command_containing_all_the_sub_commands_describing_the_update()
      {
         _result.IsEmpty().ShouldBeFalse();
      }
   }

   public class When_updating_the_compound_of_a_simulation_from_the_template_compound_defining_overwrite_parameter_sets : concern_for_BuildingBlockParametersToSimulationUpdater
   {
      private Compound _simulationCompound;
      private string _existingSetId;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _simulationCompound = _simulation.BuildingBlockByTemplateId<Compound>(_templateCompound.Id);

         var existingSetInSimulation = new OverwriteParameterSet { Name = "OPS 1", Id = "ExistingSetId" };
         existingSetInSimulation.Add(parameterValueFor(1));
         _simulationCompound.AddOverwriteParameterSet(existingSetInSimulation);
         _existingSetId = existingSetInSimulation.Id;

         var staleSetInSimulation = new OverwriteParameterSet { Name = "OPS stale", Id = "StaleSetId" };
         _simulationCompound.AddOverwriteParameterSet(staleSetInSimulation);

         var updatedSetInTemplate = new OverwriteParameterSet { Name = "OPS 1" };
         updatedSetInTemplate.Add(parameterValueFor(2));
         _templateCompound.AddOverwriteParameterSet(updatedSetInTemplate);

         var newSetInTemplate = new OverwriteParameterSet { Name = "OPS 2" };
         newSetInTemplate.Add(parameterValueFor(3));
         _templateCompound.AddOverwriteParameterSet(newSetInTemplate);
      }

      private ParameterValue parameterValueFor(double value) =>
         new() { Path = $"{_templateCompound.Name}|Lipophilicity".ToObjectPath(), Value = value };

      protected override void Because()
      {
         sut.UpdateParametersFromBuildingBlockInSimulation(_templateCompound, _simulation);
      }

      [Observation]
      public void should_have_added_the_set_only_defined_in_the_template_to_the_compound_of_the_simulation()
      {
         _simulationCompound.OverwriteParameterSets.FindByName("OPS 2").ShouldNotBeNull();
      }

      [Observation]
      public void should_have_updated_the_set_defined_in_both_compounds_without_replacing_it()
      {
         var updatedSet = _simulationCompound.OverwriteParameterSets.FindByName("OPS 1");
         updatedSet.Id.ShouldBeEqualTo(_existingSetId);
         updatedSet.ParameterValues.Single().Value.ShouldBeEqualTo(2);
      }

      [Observation]
      public void should_have_removed_the_set_no_longer_defined_in_the_template()
      {
         _simulationCompound.OverwriteParameterSets.FindByName("OPS stale").ShouldBeNull();
      }
   }
}