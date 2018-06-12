using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using NUnit.Framework;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Infrastructure.ProjectConverter.v5_2;
using PKSim.IntegrationTests;
using OSPSuite.Core.Domain;

namespace PKSim.ProjectConverter.v5_2
{
   public abstract class concern_for_PopulationConverter : ContextWithLoadedProject<Converter514To521>
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("Population_515");
      }
   }

   public class When_converting_a_population_from_5_1_to_5_2 : concern_for_PopulationConverter
   {
      private PopulationSimulation _populationSimulation;
      private Individual _individual;
      private RandomPopulation _population;

      protected override void Context()
      {
         _populationSimulation = First<PopulationSimulation>();
         _individual = _populationSimulation.BuildingBlock<Individual>();
         _population = _populationSimulation.BuildingBlock<RandomPopulation>();
      }

      [Observation]
      public void should_have_removed_the_ontogeny_factor_from_all_container_in_the_individual()
      {
         foreach (var molecule in _individual.AllMolecules())
         {
            molecule.Parameter(CoreConstants.Parameters.ONTOGENY_FACTOR).ShouldNotBeNull();
            if(molecule.Name!=CoreConstants.Molecule.UndefinedLiver)
               molecule.Parameter(CoreConstants.Parameters.ONTOGENY_FACTOR_GI).ShouldNotBeNull();

            foreach (var container in molecule.AllExpressionsContainers())
            {
               container.Parameter(CoreConstants.Parameters.ONTOGENY_FACTOR).ShouldBeNull();
            }
         }
      }

      [Observation]
      public void should_have_adjusted_the_path_of_the_individual_parameters_in_the_population()
      {
         foreach (var molecule in _individual.AllMolecules())
         {
            if (molecule.Name == CoreConstants.Molecule.UndefinedLiver)
               continue;
            
            _population.IndividualPropertiesCache.Has(string.Format("{0}|{1}", molecule.Name, CoreConstants.Parameters.ONTOGENY_FACTOR)).ShouldBeTrue();
            _population.IndividualPropertiesCache.Has(string.Format("{0}|{1}", molecule.Name, CoreConstants.Parameters.ONTOGENY_FACTOR_GI)).ShouldBeTrue();
         }
      }

      [Observation]
      public void should_have_removed_the_old_ontogeny_factor_path()
      {
         foreach (var molecule in _individual.AllMolecules())
         {
  
            _population.IndividualPropertiesCache.Has(string.Format("{0}|Liver|{1}", molecule.Name, CoreConstants.Parameters.ONTOGENY_FACTOR)).ShouldBeFalse();
            _population.IndividualPropertiesCache.Has(string.Format("{0}|Duodenum|{1}", molecule.Name, CoreConstants.Parameters.ONTOGENY_FACTOR)).ShouldBeFalse();
            _population.IndividualPropertiesCache.Has(string.Format("{0}|Lumen-Duodenum|{1}", molecule.Name, CoreConstants.Parameters.ONTOGENY_FACTOR)).ShouldBeFalse();
         }
      }
   }
}