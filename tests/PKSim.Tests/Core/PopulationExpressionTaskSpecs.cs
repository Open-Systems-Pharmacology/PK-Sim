using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility.Extensions;
using FakeItEasy;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core
{
   public abstract class concern_for_PopulationExpressionTask : ContextSpecification<PopulationExpressionTask>
   {
      protected IEntityPathResolver _entityPathResolver;
      private IExecutionContext _executionContext;
      protected IMoleculeParameterVariabilityCreator _moleculeParameterVariabilityCreator;

      protected override void Context()
      {
         _entityPathResolver = A.Fake<IEntityPathResolver>();
         _executionContext = A.Fake<IExecutionContext>();
         _moleculeParameterVariabilityCreator = A.Fake<IMoleculeParameterVariabilityCreator>();


         sut = new PopulationExpressionTask(_executionContext, _moleculeParameterVariabilityCreator, _entityPathResolver);
      }
   }


   public class When_adding_a_molecule_to_a_population : concern_for_PopulationExpressionTask
   {
      private IndividualMolecule _molecule;
      private RandomPopulation _population;
      private ICommand _addVariabilityCommand;
      private PKSimMacroCommand _result;

      protected override void Context()
      {
         base.Context();
         _molecule = new IndividualEnzyme();
         _population = new RandomPopulation {Settings = new RandomPopulationSettings {BaseIndividual = new Individual()}};
         _addVariabilityCommand= A.Fake<IPKSimCommand>();
         A.CallTo(_moleculeParameterVariabilityCreator).WithReturnType<ICommand>().Returns(_addVariabilityCommand);
      }

      protected override void Because()
      {
         _result = sut.AddMoleculeTo(_molecule, _population) as PKSimMacroCommand;
      }

      [Observation]
      public void should_add_the_molecule_to_the_population()
      {
         _population.AllMolecules().ShouldContain(_molecule);
      }

      [Observation]
      public void should_add_the_know_variability_to_the_population_for_the_molecule_using_the_default_mean_value()
      {
         A.CallTo(() => _moleculeParameterVariabilityCreator.AddMoleculeVariability(_molecule, _population, true)).MustHaveHappened();
      }

      [Observation]
      public void should_return_a_macro_command_containing_the_add_variability_and_molecule_action()
      {
         _result.Count.ShouldBeEqualTo(2);
         _result.All().ElementAt(0).ShouldBeAnInstanceOf<AddMoleculeToPopulationCommand>();
         _result.All().ElementAt(1).ShouldBeEqualTo(_addVariabilityCommand);
      }
   }


   public class When_removing_a_molecule_from_a_population : concern_for_PopulationExpressionTask
   {
      private IndividualMolecule _molecule;
      private Population _population;
      private PKSimMacroCommand _result;
      private IParameter _moleculeParameterAsAdvanced;
      private AdvancedParameter _advancedParameter;

      protected override void Context()
      {
         base.Context();
         _moleculeParameterAsAdvanced = A.Fake<IParameter>();   
         _molecule = new IndividualEnzyme();
         _population = new RandomPopulation { Settings = new RandomPopulationSettings { BaseIndividual = new Individual() } };
         _population.Add(new AdvancedParameterCollection());
         _population.AddMolecule(_molecule);
         A.CallTo(() => _entityPathResolver.PathFor(_moleculeParameterAsAdvanced)).Returns("A|B");
         _molecule.Add(_moleculeParameterAsAdvanced);
         _advancedParameter = new AdvancedParameter {ParameterPath = "A|B", Name = "A|B"};
         _population.AddAdvancedParameter(_advancedParameter);
         _population.AddAdvancedParameter(new AdvancedParameter{ParameterPath = "ANOTHER PARAMETER", Name = "ANOTHER PARAMETER"});
      }

      protected override void Because()
      {
         _result = sut.RemoveMoleculeFrom(_molecule, _population) as PKSimMacroCommand;
      }

      [Observation]
      public void should_remove_the_molecule_from_the_population()
      {
         _population.AllMolecules().ShouldBeEmpty();
      }

      [Observation]
      public void should_also_remove_the_associated_advanced_parameters()
      {
         _population.AdvancedParameters.Contains(_advancedParameter).ShouldBeFalse();
         _population.AdvancedParameters.Count().ShouldBeEqualTo(1);
      }


      [Observation]
      public void should_return_a_macro_command_containing_the_remove_variability_and_molecule_action()
      {
         _result.Count.ShouldBeEqualTo(2);
         _result.All().ElementAt(0).ShouldBeAnInstanceOf<PKSimMacroCommand>();
         var removeAdvancedParmaeterCommand = _result.All().ElementAt(0).DowncastTo<PKSimMacroCommand>();
         removeAdvancedParmaeterCommand.All().First().ShouldBeAnInstanceOf<RemoveAdvancedParameterFromContainerCommand>();
         _result.All().ElementAt(1).ShouldBeAnInstanceOf<RemoveMoleculeFromPopulationCommand>();
      }
   }
}