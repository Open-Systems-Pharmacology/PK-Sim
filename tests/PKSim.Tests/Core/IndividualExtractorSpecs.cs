using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_IndividualExtractor : ContextSpecification<IIndividualExtractor>
   {
      protected IExecutionContext _executionContext;
      protected IEntityPathResolver _entityPathResolver;
      protected IIndividualTask _individualTask;
      protected IContainerTask _containerTask;
      protected IBuildingBlockRepository _buildingBlockRepository;
      protected Individual _templateIndividual1;

      protected override void Context()
      {
         _executionContext = A.Fake<IExecutionContext>();
         _entityPathResolver = A.Fake<IEntityPathResolver>();
         _individualTask = A.Fake<IIndividualTask>();
         _containerTask = new ContainerTaskForSpecs();
         _buildingBlockRepository = A.Fake<IBuildingBlockRepository>();

         sut = new IndividualExtractor(_executionContext, _entityPathResolver, _individualTask, _containerTask, _buildingBlockRepository);

         _templateIndividual1 = new Individual();
         A.CallTo(() => _buildingBlockRepository.All<Individual>()).Returns(new[] {_templateIndividual1});
      }
   }

   public class When_extracting_indiduals_by_id_for_a_population_that_was_created_from_MoBi : concern_for_IndividualExtractor
   {
      private Population _population;

      protected override void Context()
      {
         base.Context();
         _population = new MoBiPopulation();
      }

      [Observation]
      public void should_throw_an_exception_indicating_to_the_user_that_the_exctraction_feature_is_not_available_for_population_imported_from_MoBi()
      {
         The.Action(() => sut.ExtractIndividualsFrom(_population, 1));
      }
   }

   public class When_extracting_individuals_by_id_for_a_population_created_in_PKSim_with_a_valid_individual : concern_for_IndividualExtractor
   {
      private RandomPopulation _population;
      private Individual _baseIndividual;
      private Individual _cloneIndividual;
      private IParameter _constParam1;
      private IParameter _constParam2;
      private IParameter _forumlaParameter;
      private IParameter _indParam1;
      private IParameter _indParam2;
      private IParameter _indParam3;
      private IParameter _indParam4;
      private IPKSimCommand _addIndividualCommand;
      private ICommand _overallCommand;

      protected override void Context()
      {
         base.Context();
         _population = A.Fake<RandomPopulation>().WithName("POP");
         A.CallTo(() => _population.NumberOfItems).Returns(7);

         _baseIndividual = A.Fake<Individual>();
         _cloneIndividual = A.Fake<Individual>();
         var organism = new Organism();
         A.CallTo(() => _cloneIndividual.Organism).Returns(organism);
         organism.Add(DomainHelperForSpecs.ConstantParameterWithValue(40).WithName(CoreConstants.Parameters.WEIGHT));
         organism.Add(DomainHelperForSpecs.ConstantParameterWithValue(50).WithName(CoreConstants.Parameters.HEIGHT));
         organism.Add(DomainHelperForSpecs.ConstantParameterWithValue(60).WithName(CoreConstants.Parameters.BMI));
         organism.Add(DomainHelperForSpecs.ConstantParameterWithValue(70).WithName(CoreConstants.Parameters.AGE));
         organism.Add(DomainHelperForSpecs.ConstantParameterWithValue(80).WithName(Constants.Parameters.GESTATIONAL_AGE));

         _constParam1 = DomainHelperForSpecs.ConstantParameterWithValue(1).WithName("P1");
         _constParam2 = DomainHelperForSpecs.ConstantParameterWithValue(1).WithName("P2");

         //create a reference to another parameter to ensure that formula parameter are not overwritten if nothing has changed
         _forumlaParameter = new PKSimParameter().WithName("P3").WithFormula(new ExplicitFormula("P2*2"));
         _forumlaParameter.Formula.AddObjectPath(new FormulaUsablePath("..", "P2").WithAlias("P2"));
         var container1 = new Container {_constParam1, _constParam2, _forumlaParameter};

         _indParam1 = DomainHelperForSpecs.ConstantParameterWithValue(1).WithName("P1");
         _indParam2 = DomainHelperForSpecs.ConstantParameterWithValue(2).WithName("P2");
         _indParam3 = new PKSimParameter().WithName("P3").WithFormula(new ExplicitFormula("P2*2"));
         _indParam3.Formula.AddObjectPath(new FormulaUsablePath("..", "P2").WithAlias("P2"));
         _indParam4 = DomainHelperForSpecs.ConstantParameterWithValue(4).WithName("P4");
         var container2 = new Container {_indParam1, _indParam2, _indParam3, _indParam4};

         A.CallTo(() => _executionContext.Clone(_baseIndividual)).Returns(_cloneIndividual);
         A.CallTo(() => _population.FirstIndividual).Returns(_baseIndividual);
         //put explicit formula first
         A.CallTo(() => _population.AllVectorialParameters(_entityPathResolver)).Returns(new[] {_forumlaParameter, _constParam1, _constParam2});
         A.CallTo(() => _cloneIndividual.GetAllChildren<IParameter>()).Returns(new[] {_indParam1, _indParam2, _indParam3, _indParam4});

         A.CallTo(() => _entityPathResolver.PathFor(_constParam1)).Returns("PATH1");
         A.CallTo(() => _entityPathResolver.PathFor(_constParam2)).Returns("PATH2");
         A.CallTo(() => _entityPathResolver.PathFor(_forumlaParameter)).Returns("PATH3");

         A.CallTo(() => _entityPathResolver.PathFor(_indParam1)).Returns("PATH1");
         A.CallTo(() => _entityPathResolver.PathFor(_indParam2)).Returns("PATH2");
         A.CallTo(() => _entityPathResolver.PathFor(_indParam3)).Returns("PATH3");
         A.CallTo(() => _entityPathResolver.PathFor(_indParam4)).Returns("PATH4");

         A.CallTo(() => _population.AllValuesFor("PATH1")).Returns(new[] {10d, 11, 12, 13, 14, 15, 16});
         A.CallTo(() => _population.AllValuesFor("PATH2")).Returns(new[] {20d, 21, 22, 23, 24, 25, 26});
         A.CallTo(() => _population.AllValuesFor("PATH3")).Returns(new[] {40d, 42, 44, 46, 48, 50, 52});

         _templateIndividual1.Name = "POP-3";

         _addIndividualCommand= A.Fake<IPKSimCommand>();
         A.CallTo(() => _individualTask.AddToProject(_cloneIndividual, false, false)).Returns(_addIndividualCommand);

         A.CallTo(() => _executionContext.AddToHistory(A<ICommand>._))
            .Invokes(x => _overallCommand = x.GetArgument<ICommand>(0));

      }

      protected override void Because()
      {
         sut.ExtractIndividualsFrom(_population, 3);
      }

      [Observation]
      public void should_create_a_clone_of_the_base_individual()
      {
         A.CallTo(() => _executionContext.Clone(_baseIndividual)).MustHaveHappened();
      }

      [Observation]
      public void should_have_updated_the_clone_with_the_parameters_of_the_individual_by_id()
      {
         _indParam1.Value.ShouldBeEqualTo(13);
         _indParam2.Value.ShouldBeEqualTo(23);
      }

      [Observation]
      public void should_have_kept_the_formula_of_parameter_with_unchanged_formula()
      {
         _indParam3.Value.ShouldBeEqualTo(23 * 2);
         _indParam3.IsFixedValue.ShouldBeFalse();
      }

      [Observation]
      public void should_have_left_based_parameter_untouched()
      {
         _indParam4.Value.ShouldBeEqualTo(4);
      }

      [Observation]
      public void should_name_the_added_individual_uniquely_according_to_available_individual_building_block_using_the_name_of_the_population()
      {
         _cloneIndividual.Name.ShouldBeEqualTo("POP-3 1");
      }

      [Observation]
      public void should_add_the_individual_to_the_project_using_a_unique_name()
      {
         A.CallTo(() => _individualTask.AddToProject(_cloneIndividual, false, false)).MustHaveHappened();
      }

      [Observation]
      public void should_add_the_command_resulting_from_the_extraction_to_the_history_as_a_unique_macro_command()
      {
         _overallCommand.ShouldBeAnInstanceOf<PKSimMacroCommand>();
         var macroCommand = _overallCommand.DowncastTo<PKSimMacroCommand>();
         macroCommand.All().ShouldContain(_addIndividualCommand);
      }

      [Observation]
      public void should_update_the_origin_data_with_the_value_of_all_organism_parameters()
      {
         checkOriginDataParameter(_cloneIndividual.OriginData.Age, CoreConstants.Parameters.AGE);
         checkOriginDataParameter(_cloneIndividual.OriginData.GestationalAge, Constants.Parameters.GESTATIONAL_AGE);
         checkOriginDataParameter(_cloneIndividual.OriginData.BMI, CoreConstants.Parameters.BMI);
         checkOriginDataParameter(_cloneIndividual.OriginData.Height, CoreConstants.Parameters.HEIGHT);
         checkOriginDataParameter(_cloneIndividual.OriginData.Weight, CoreConstants.Parameters.WEIGHT);
      }

      private void checkOriginDataParameter(double? originDataParameter, string parameterName)
      {
         originDataParameter.ShouldBeEqualTo(_cloneIndividual.Organism.Parameter(parameterName).Value);
      }
   }
}