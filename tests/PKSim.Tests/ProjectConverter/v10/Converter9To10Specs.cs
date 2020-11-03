using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Infrastructure.ProjectConverter.v10;
using PKSim.IntegrationTests;

namespace PKSim.ProjectConverter.v10
{
   public class When_converting_the_simple_project_730_project_to_10 : ContextWithLoadedProject<Converter9To10>
   {
      private List<PopulationSimulation> _allSimulations;
      private List<Population> _allPopulations;
      private List<Individual> _allIndividuals;

      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("SimplePop_73");
         _allSimulations = All<PopulationSimulation>().ToList();
         _allPopulations = All<Population>().ToList();
         _allIndividuals = All<Individual>().ToList();
         _allSimulations.Each(Load);
         _allPopulations.Each(Load);
         _allIndividuals.Each(Load);
      }

      [Observation]
      public void should_have_converted_the_individual_enzyme_and_protein_to_use_the_new_localization_concept()
      {
         verifyIndividuals(_allSimulations.Select(x => x.BuildingBlock<Individual>()));
         verifyIndividuals(_allIndividuals);
         verifyIndividuals(_allPopulations.Select(x => x.FirstIndividual));

         var ind = _allIndividuals.FindByName("Human");
         var cyp3A4 = ind.MoleculeByName<IndividualEnzyme>("CYP3A4");
         var allExpressionParameters = ind.AllExpressionParametersFor(cyp3A4);
         allExpressionParameters["Bone"].Value.ShouldBeEqualTo(0.04749, 1e-2);
         allExpressionParameters["Duodenum"].Value.ShouldBeEqualTo(0.3999, 1e-2);
      }

      private void verifyIndividuals(IEnumerable<Individual> individuals) => individuals.Each(verifyIndividual);

      private void verifyIndividual(Individual individual)
      {
         individual.AllMolecules<IndividualProtein>().Each(m =>
         {
            m.Localization.ShouldNotBeEqualTo(Localization.None);

            //Should have created parameters in most compartments. About 30+
            var allExpressionParameters = individual.AllExpressionParametersFor(m);
            allExpressionParameters.Count.ShouldBeGreaterThan(30);
         });
      }
   }
}