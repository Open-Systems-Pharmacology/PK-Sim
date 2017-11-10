using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.Repositories;
using PKSim.Infrastructure;
using System;

namespace PKSim.IntegrationTests
{
    public abstract class concern_for_PopulationRepository : ContextForIntegration<IPopulationRepository>
    {
   }

    
    public class When_retrieving_all_populations_from_the_repository : concern_for_PopulationRepository
    {
        private IEnumerable<SpeciesPopulation> _result;

        protected override void Because()
        {
            _result = sut.All();
        }

        [Observation]
        public void should_return_at_least_one_element()
        {
            _result.Count().ShouldBeGreaterThan(0);
        }

        [Observation]
        public void all_populations_contain_at_least_one_gender()
        {
            foreach (var pop in _result)
                pop.Genders.Count().ShouldBeGreaterThan(0);
        }

       [Observation]
       public void should_contain_japanese_population()
       {
          _result.Select(x=>x.Name).ShouldContain(CoreConstants.Population.Japanese);
       }

      [Observation]
      public void all_populations_should_be_creatable()
      {
         foreach (var pop in _result)
         {
            createPopulation(pop.Name).ShouldBeTrue($"Population {pop.Name} could not be created");
         }
      }

      private bool createPopulation(string populationName)
      {
         try
         {
            var individual = DomainFactoryForSpecs.CreateStandardIndividual(populationName);
            var population = DomainFactoryForSpecs.CreateDefaultPopulation(individual);

            return true;
         }
         catch (Exception)
         {
            return false;
         }
      }
   }
}