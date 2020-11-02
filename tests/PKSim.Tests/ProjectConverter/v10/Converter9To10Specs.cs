using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Infrastructure.ProjectConverter.v10;
using PKSim.Infrastructure.ProjectConverter.v9;
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
         verifyMoleculesInIndividuals(_allSimulations.Select(x=>x.BuildingBlock<Individual>()));
         verifyMoleculesInIndividuals(_allIndividuals);
         verifyMoleculesInIndividuals(_allPopulations.Select(x=>x.Individual));
      }

      private void verifyMoleculesInIndividuals(IEnumerable<Individual> individuals)
      {
         individuals.SelectMany(x => x.AllMolecules<IndividualProtein>()).Each(m =>
         {
            m.Localization.ShouldNotBeEqualTo(Localization.None);
         });

      }
   }

}