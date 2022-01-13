using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_DiseaseStateRepository : ContextForIntegration<IDiseaseStateRepository>
   {
   }

   public class When_loading_all_available_disease_states_from_the_database : concern_for_DiseaseStateRepository
   {
      private Species _human;

      protected override void Context()
      {
         base.Context();
         var speciesRepository = IoC.Resolve<ISpeciesRepository>();
         _human = speciesRepository.FindByName(CoreConstants.Species.HUMAN);
      }

      [Observation]
      public void should_have_loaded_all_disease_states()
      {
         sut.All().Any().ShouldBeTrue();
      }

      [Observation]
      public void should_have_loaded_diseases_states_for_CKD()
      {
         var ckd = sut.FindByName("CKD");
         ckd.ShouldNotBeNull();

         var parameter = ckd.Parameter(CKDDiseaseStateImplementation.TARGET_GFR);
         parameter.ShouldNotBeNull();
         parameter.ValueInDisplayUnit.ShouldBeEqualTo(60, 1e-2);
      }

      [Observation]
      public void should_return_disease_state_available_for_the_human_species_except()
      {
         sut.AllFor(_human.PopulationByName(CoreConstants.Population.ICRP)).Any().ShouldBeTrue();
      }
   }
}