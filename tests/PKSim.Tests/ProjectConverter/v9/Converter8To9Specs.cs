using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Infrastructure.ProjectConverter;
using PKSim.Infrastructure.ProjectConverter.v9;
using PKSim.IntegrationTests;

namespace PKSim.ProjectConverter.v9
{
   public class When_converting_the_simple_project_730_project_to_90 : ContextWithLoadedProject<Converter8To9>
   {
      private List<PopulationSimulation> _allSimulations;
      private List<Population> _allPopulations;

      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("SimplePop_73");
         _allSimulations = All<PopulationSimulation>().ToList();
         _allPopulations = All<Population>().ToList();
         _allSimulations.Each(Load);
         _allPopulations.Each(Load);
      }

      [Observation]
      public void should_be_able_to_load_the_project()
      {
         _project.ShouldNotBeNull();
      }

      [Observation]
      public void should_have_converted_all_population_building_blocks()
      {
         _allPopulations.Each(verifyIndividualValueCache);
      }

      private void verifyIndividualValueCache(Population population)
      {
         var numberOfIndividuals = 20;
         population.IndividualValuesCache.Count.ShouldBeEqualTo(numberOfIndividuals);
         population.AllCovariateNames.ShouldContain(Constants.Population.GENDER, Constants.Population.POPULATION);
         population.IndividualValuesCache.AllCovariateValuesFor(Constants.Population.GENDER).Count.ShouldBeEqualTo(numberOfIndividuals);
         population.IndividualValuesCache.AllCovariateValuesFor(Constants.Population.POPULATION).Count.ShouldBeEqualTo(numberOfIndividuals);
      }

      [Observation]
      public void should_have_converted_all_population_building_blocks_defined_in_simulations()
      {
         _allSimulations.Select(x => x.Population).Each(verifyIndividualValueCache);
      }
   }

   public class When_converting_the_population_analysis_74_project_to_90 : ContextWithLoadedProject<Converter8To9>
   {
      private List<PopulationSimulation> _allSimulations;

      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("PopulationAnalyses_740");
         _allSimulations = All<PopulationSimulation>().ToList();
         _allSimulations.Each(Load);
      }

      [Observation]
      public void should_have_converted_all_covariate_fields_referencing_race_to_use_population_instead()
      {
         foreach (var popSimulation in _allSimulations)
         {
            var covariateFields = popSimulation.AnalysesOfType<PopulationAnalysisChart>().Select(x => x.BasePopulationAnalysis)
               .SelectMany(x => x.All<PopulationAnalysisCovariateField>()).ToList();
            covariateFields.FindByName(ConverterConstants.Population.RACE).ShouldBeNull();
            var populationCovariateFields = covariateFields.FindByName(Constants.Population.POPULATION);
            populationCovariateFields.ShouldNotBeNull();
            populationCovariateFields.Covariate.ShouldBeEqualTo(Constants.Population.POPULATION);
         }
      }

      [Observation]
      public void should_have_converted_the_pk_parameter_in_the_pk_parameter_field()
      {
         foreach (var popSimulation in _allSimulations)
         {
            var pkParameterFields = popSimulation.AnalysesOfType<PopulationAnalysisChart>().Select(x => x.BasePopulationAnalysis)
               .SelectMany(x => x.All<PopulationAnalysisPKParameterField>()).ToList();
            pkParameterFields.Exists(x => x.PKParameter.Equals(Constants.PKParameters.AUC_tEnd)).ShouldBeTrue();
            pkParameterFields.Exists(x => x.PKParameter.Equals("AUC")).ShouldBeFalse();
         }
      }

      [Observation]
      public void should_have_converter_the_population_simulation_pk_analysis()
      {
         foreach (var popSimulation in _allSimulations)
         {
            var pkAnalyses = popSimulation.PKAnalyses;
            pkAnalyses.All().Any(x => x.Name.Equals(Constants.PKParameters.AUC_tEnd)).ShouldBeTrue();
            pkAnalyses.All().Any(x => x.Name.Equals("AUC")).ShouldBeFalse();
         }
      }
   }
}