using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Infrastructure.ProjectConverter;
using PKSim.Infrastructure.ProjectConverter.v11;
using PKSim.IntegrationTests;

namespace PKSim.ProjectConverter.v11
{
   public class When_converting_the_simple_project_730_project_to_11 : ContextWithLoadedProject<Converter10to11>
   {
      private List<PopulationSimulation> _allSimulations;
      private List<Population> _allPopulations;
      private List<Individual> _allIndividuals;

      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("SimplePop_73");
         _allSimulations = All<PopulationSimulation>();
         _allPopulations = All<Population>();
         _allIndividuals = All<Individual>();
         _allSimulations.Each(Load);
         _allPopulations.Each(Load);
         _allIndividuals.Each(Load);
      }

      [Observation]
      public void should_have_created_an_expression_profile_for_the_individual()
      {
         var ind = _allIndividuals.FindByName("Human");
         var cyp3A4 = ind.MoleculeByName<IndividualEnzyme>("CYP3A4");
         var expressionProfile = FindByName<ExpressionProfile>(CoreConstants.ContainerName.ExpressionProfileName(cyp3A4.Name, ind.Species, ind.Name));
         expressionProfile.ShouldNotBeNull();
      }

      [Observation]
      public void should_have_converted_the_origin_data_value_to_origin_data_parameters()
      {
         var ind = _allIndividuals.FindByName("Human");
         ind.OriginData.Age.ShouldNotBeNull();
         ind.OriginData.Height.ShouldNotBeNull();
         ind.OriginData.Weight.ShouldNotBeNull();
         ind.OriginData.BMI.ShouldNotBeNull();
         ind.OriginData.GestationalAge.ShouldNotBeNull();
      }

      [Observation]
      public void should_have_added_the_eGFR_parameter()
      {
         var ind = _allIndividuals.FindByName("Human");
         var parameter = ind.Organism.EntityAt<IParameter>(CoreConstants.Organ.KIDNEY, ConverterConstants.Parameters.E_GFR);
         parameter.ShouldNotBeNull();
      }

      [Observation]
      public void should_have_converted_the_population_in_origin_data()
      {
         var pop = _allPopulations.FindByName("Pop");
         var ind = pop.FirstIndividual;
         ind.OriginData.Population.ShouldNotBeNull();
      }
   }

   public class When_converting_the_expression_v9_project_to_11 : ContextWithLoadedProject<Converter10to11>
   {
      private List<PopulationSimulation> _allSimulations;
      private List<Population> _allPopulations;
      private List<Individual> _allIndividuals;

      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("expression_v9");
         _allSimulations = All<PopulationSimulation>();
         _allPopulations = All<Population>();
         _allIndividuals = All<Individual>();
         _allSimulations.Each(Load);
         _allPopulations.Each(Load);
         _allIndividuals.Each(Load);
      }

      [Observation]
      public void should_have_created_an_expression_profile_for_the_individual()
      {
         var ind = _allIndividuals.FindByName("IND");
         var cyp3A4 = ind.MoleculeByName<IndividualEnzyme>("CYP3A4");
         var expressionProfile = FindByName<ExpressionProfile>(CoreConstants.ContainerName.ExpressionProfileName(cyp3A4.Name, ind.Species, ind.Name));
         expressionProfile.ShouldNotBeNull();
      }

      [Observation]
      public void should_have_created_an_expression_profile_for_the_population()
      {
         var pop = _allPopulations.FindByName("POP");
         var cyp3A4 = pop.MoleculeByName<IndividualEnzyme>("CYP3A4");
         var expressionProfile = FindByName<ExpressionProfile>(CoreConstants.ContainerName.ExpressionProfileName(cyp3A4.Name, pop.Species, pop.Name));
         expressionProfile.ShouldNotBeNull();
      }

      [Observation]
      public void should_have_converted_the_origin_data_value_to_origin_data_parameters()
      {
         var pop = _allPopulations.FindByName("POP");
         var ind = pop.FirstIndividual;
         ind.OriginData.Age.ShouldNotBeNull();
         ind.OriginData.Height.ShouldNotBeNull();
         ind.OriginData.Weight.ShouldNotBeNull();
         ind.OriginData.BMI.ShouldNotBeNull();
         ind.OriginData.GestationalAge.ShouldNotBeNull();
      }

      [Observation]
      public void should_have_rendered_the_parameter_required_as_is_changed_by_created_individual()
      {
         var ind = _allIndividuals.FindByName("IND");
         ind.AgeParameter.IsChangedByCreateIndividual.ShouldBeTrue();
      }

      [Observation]
      public void should_have_set_the_fraction_of_blood_for_sampling_parameter_visible()
      {
         var ind = _allIndividuals.FindByName("IND");
         var parameters = ind.GetAllChildren<IParameter>(x => x.IsNamed(ConverterConstants.Parameters.FRACTION_OF_BLOOD_FOR_SAMPLING));
         parameters.Each(x =>
         {
            x.Visible.ShouldBeTrue();
            x.Info.ReadOnly.ShouldBeFalse();
            x.GroupName.ShouldBeEqualTo("FRACTION_OF_BLOOD_SAMPLING");
         });
      }
   }

   public class When_converting_the_expression_v10_project_to_11 : ContextWithLoadedProject<Converter10to11>
   {
      private List<Population> _allPopulations;
      private List<Individual> _allIndividuals;

      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("expression_v10");
         _allPopulations = All<Population>();
         _allIndividuals = All<Individual>();

         _allPopulations.Each(Load);
         _allIndividuals.Each(Load);
      }

      [Observation]
      public void should_be_able_to_read_the_population_of_all_individuals_before_loading_them()
      {
         _allIndividuals.Each(x => x.OriginData.Population.ShouldNotBeNull());
      }

      [Observation]
      public void should_have_created_an_expression_profile_for_the_population()
      {
         var pop = _allPopulations.FindByName("Pop");
         var cyp3A4 = pop.MoleculeByName<IndividualEnzyme>("CYP3A4");
         var expressionProfile = FindByName<ExpressionProfile>(CoreConstants.ContainerName.ExpressionProfileName(cyp3A4.Name, pop.Species, pop.Name));
         expressionProfile.ShouldNotBeNull();
      }

      [Observation]
      public void should_have_set_the_parameter_fraction_expressed_basolateral_that_are_hidden_to_unchanged()
      {
         var ind = _allIndividuals.FindByName("Ind");
         var trans = ind.MoleculeByName<IndividualTransporter>("TRANS");
         var expressionProfile = FindByName<ExpressionProfile>(CoreConstants.ContainerName.ExpressionProfileName(trans.Name, ind.Species, ind.Name));
         expressionProfile.ShouldNotBeNull();
         var allHiddenFixedParameters = expressionProfile.Individual.AllMoleculeParametersFor(expressionProfile.Molecule)
            .Where(x => !x.Visible)
            .Where(x => x.IsFixedValue).ToList();

         allHiddenFixedParameters.ShouldBeEmpty();
      }

      [Observation]
      public void should_have_set_the_initial_concentration_parameter_to_not_variable_in_population()
      {
         var pop = _allPopulations.FindByName("Pop");
         var allInitialConcentrationParameters = pop.FirstIndividual.GetAllChildren<IParameter>(x => x.IsNamed(CoreConstants.Parameters.INITIAL_CONCENTRATION));
         allInitialConcentrationParameters.Each(x => x.CanBeVariedInPopulation.ShouldBeFalse());
      }
   }

   public class When_converting_the_expression_v10_project_to_11_and_an_expression_profile_is_added_with_the_name_that_would_be_created_from_conversion : ContextWithLoadedProject<Converter10to11>
   {
      private Individual _ind;
      private ExpressionProfile _existingExpressionProfile;

      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("expression_v10");
         var individuals = All<Individual>();
         _ind = individuals.FindByName("Ind");
      }

      protected override void Because()
      {
         //add an expression profile with the name
         _existingExpressionProfile = DomainHelperForSpecs.CreateExpressionProfile<IndividualEnzyme>(_ind.Species.Name, "CYP3A4", _ind.Name);
         _project.AddBuildingBlock(_existingExpressionProfile);

         //now load to trigger conversion
         Load(_ind);
      }

      [Observation]
      public void should_have_created_an_expression_profile_named_differently()
      {
         _ind.Uses(_existingExpressionProfile).ShouldBeFalse();
         var expressionProfile = FindByName<ExpressionProfile>($"{_existingExpressionProfile.Name}_1");
         expressionProfile.ShouldNotBeNull();
         _ind.Uses(expressionProfile).ShouldBeTrue();
      }
   }
}