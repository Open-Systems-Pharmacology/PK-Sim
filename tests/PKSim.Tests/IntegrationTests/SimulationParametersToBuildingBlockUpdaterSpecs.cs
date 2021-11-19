using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure;
using static PKSim.Core.CoreConstants.Compartment;
using static PKSim.Core.CoreConstants.Organ;
using static PKSim.Core.CoreConstants.Parameters;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_SimulationParametersToBuildingBlockUpdater : ContextForSimulationIntegration<ISimulationParametersToBuildingBlockUpdater>
   {
      protected Individual _templateIndividual;
      protected ExpressionProfile _templateExpressionProfileCYP3A4;
      protected Individual _templateIndividualUsingSameProfile;
      protected Individual _templateIndividualUsingAnotherProfile;
      protected ExpressionProfile _templateExpressionProfileCYP2D6;

      public override void GlobalContext()
      {
         base.GlobalContext();
         var moleculeExpressionTask = IoC.Resolve<IMoleculeExpressionTask<Individual>>();

         _templateIndividual = DomainFactoryForSpecs.CreateStandardIndividual();
         _templateExpressionProfileCYP3A4 = DomainFactoryForSpecs.CreateExpressionProfile<IndividualEnzyme>("CYP3A4");
         moleculeExpressionTask.AddExpressionProfile(_templateIndividual, _templateExpressionProfileCYP3A4);

         _templateIndividualUsingSameProfile = DomainFactoryForSpecs.CreateStandardIndividual().WithName("IND2");
         moleculeExpressionTask.AddExpressionProfile(_templateIndividualUsingSameProfile, _templateExpressionProfileCYP3A4);

         _templateIndividualUsingAnotherProfile = DomainFactoryForSpecs.CreateStandardIndividual().WithName("IND3");
         _templateExpressionProfileCYP2D6 = DomainFactoryForSpecs.CreateExpressionProfile<IndividualEnzyme>("CYP2D6");
         moleculeExpressionTask.AddExpressionProfile(_templateIndividualUsingAnotherProfile, _templateExpressionProfileCYP2D6);

         var compound = DomainFactoryForSpecs.CreateStandardCompound();
         var protocol = DomainFactoryForSpecs.CreateStandardIVBolusProtocol();
         
         _simulation = DomainFactoryForSpecs.CreateSimulationWith(_templateIndividual, compound, protocol) as IndividualSimulation;

         var project = new PKSimProject();
         project.AddBuildingBlock(compound);
         project.AddBuildingBlock(protocol);
         project.AddBuildingBlock(_simulation);
         project.AddBuildingBlock(_templateIndividual);
         project.AddBuildingBlock(_templateIndividualUsingSameProfile);
         project.AddBuildingBlock(_templateIndividualUsingAnotherProfile);
         project.AddBuildingBlock(_templateExpressionProfileCYP3A4);
         project.AddBuildingBlock(_templateExpressionProfileCYP2D6);
         var workspace = IoC.Resolve<ICoreWorkspace>();
         workspace.Project = project;
      }
   }

   public class When_updating_the_parameter_values_from_a_simulation_building_block_in_the_template_building_block : concern_for_SimulationParametersToBuildingBlockUpdater
   {
      private ICommand _result;
      private IParameter _templateParameter;
      private IParameter _templateRelExpParameter;
      private IParameter _templateExpressionProfileParameter;
      private string[] _relExpPathCYP3A4;
      private string[] _relExpPathCYP2D6;

      public override void GlobalContext()
      {
         base.GlobalContext();
         //Update one individual parameter not in expression profile
         var parameterInSimulation = _simulation.Individual.Organism.Organ(LIVER).Parameter(BLOOD_FLOW);
         _templateParameter = _templateIndividual.Organism.Organ(LIVER).Parameter(BLOOD_FLOW);

         parameterInSimulation.Value = 3;

         //Update one individual parameter also defined as expression profile
         _relExpPathCYP3A4 = new[] {BRAIN, INTRACELLULAR, _templateExpressionProfileCYP3A4.MoleculeName, REL_EXP};
         _relExpPathCYP2D6= new[] {BRAIN, INTRACELLULAR, _templateExpressionProfileCYP2D6.MoleculeName, REL_EXP};
         parameterInSimulation = _simulation.Individual.Organism.EntityAt<IParameter>(_relExpPathCYP3A4);
         _templateRelExpParameter = _templateIndividual.Organism.EntityAt<IParameter>(_relExpPathCYP3A4);
         _templateExpressionProfileParameter = _templateExpressionProfileCYP3A4.Individual.Organism.EntityAt<IParameter>(_relExpPathCYP3A4);

         parameterInSimulation.Value = 0.9;
      }

      protected override void Because()
      {
         _result = sut.UpdateParametersFromSimulationInBuildingBlock(_simulation, _templateIndividual);
      }

      [Observation]
      public void should_have_updated_the_parameter_values_in_the_building_block_according_to_the_value_in_the_simulation()
      {
         _templateParameter.Value.ShouldBeEqualTo(3);
         _templateRelExpParameter.Value.ShouldBeEqualTo(0.9);
      }

      [Observation]
      public void should_have_updated_the_parameter_values_in_the_expression_profile()
      {
         _templateExpressionProfileParameter.Value.ShouldBeEqualTo(0.9);
      }

      [Observation]
      public void should_have_updated_the_value_of_the_rel_exp_parameter_in_all_other_individual_using_this_profile()
      {
         var param = _templateIndividualUsingSameProfile.Organism.EntityAt<IParameter>(_relExpPathCYP3A4);
         param.Value.ShouldBeEqualTo(0.9);
      }

      [Observation]
      public void should_not_updated_the_value_of_the_rel_exp_parameter_in_all_other_individual_using_another_profile()
      {
         var param = _templateIndividualUsingAnotherProfile.Organism.EntityAt<IParameter>(_relExpPathCYP2D6);
         param.Value.ShouldNotBeEqualTo(0.9);
      }

      [Observation]
      public void should_not_update_the_parameter_id_of_the_template_parameter()
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