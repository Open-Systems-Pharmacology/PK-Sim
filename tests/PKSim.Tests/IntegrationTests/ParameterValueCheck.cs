using System.Collections.Generic;
using System.Linq;
using PKSim.Infrastructure.ProjectConverter;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Infrastructure.ORM.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_ParameterValueCheck : ContextForIntegration<IModelDatabase>
   {
      
   }

   public class When_checking_the_consistency_of_the_database : concern_for_ParameterValueCheck
   {
      private ISpeciesRepository _speciesRepository;
      private IDefaultIndividualRetriever _individualRetriever;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _speciesRepository = IoC.Resolve<ISpeciesRepository>();
         _individualRetriever = IoC.Resolve<IDefaultIndividualRetriever>();
      }

      [Observation]
      public void the_value_for_mean_body_weight_for_all_species_but_human_should_be_equal_to_the_weight_value()
      {
         var errorList = new List<string>();
         foreach (var species in _speciesRepository.All().Where(x => x.Name != CoreConstants.Species.HUMAN))
         {
            var individualForSpecies = _individualRetriever.DefaultIndividualFor(species);
            double meanWeight = individualForSpecies.MeanWeight;
            double bodyWeight = individualForSpecies.Organism.Parameter(CoreConstants.Parameters.WEIGHT).Value;
            if (!ValueComparer.AreValuesEqual(meanWeight, bodyWeight, 1e-2))
               errorList.Add(string.Format("Mean Body weight not equal to weight for '{0}': MeanWeight = {1}, BodyWeight={2}", species.Name, meanWeight, bodyWeight));
         }
         errorList.Count.ShouldBeEqualTo(0, errorList.ToString("\n"));

      }


      [Observation]
      public void the_value_for_small_and_large_intestine_volume_should_be_equal_to_the_default_volume()
      {
         var errorList = new List<string>();
         foreach (var species in _speciesRepository.All().Where(x => x.Name != CoreConstants.Species.HUMAN))
         {
            var individualForSpecies = _individualRetriever.DefaultIndividualFor(species);
            var smallIntestine = individualForSpecies.Organism.Organ(CoreConstants.Organ.SmallIntestine);
            var largeIntestine = individualForSpecies.Organism.Organ(CoreConstants.Organ.LargeIntestine);

            double siValue = smallIntestine.Parameter(Constants.Parameters.VOLUME).Value;
            double liValue = largeIntestine.Parameter(Constants.Parameters.VOLUME).Value;
            double siDefaultValue = smallIntestine.Parameter(ConverterConstants.Parameter.Default_Volume).Value;
            double liDefaultValue = largeIntestine.Parameter(ConverterConstants.Parameter.Default_Volume).Value;
            if (siValue != siDefaultValue)
               errorList.Add($"Small intestine value not equal to default value for '{species.Name}'");

            if (liValue != liDefaultValue)
               errorList.Add($"Large intestine value not equal to default value for '{species.Name}'");
         }
         errorList.Count.ShouldBeEqualTo(0, errorList.ToString("\n"));
      }

      [Observation]
      public void volume_mouse_should_be_equal_to_the_corresponding_mouse_volume()
      {
         var errorList = new List<string>();

         var mouse = _individualRetriever.DefaultIndividualFor(
            _speciesRepository.All().FindByName(CoreConstants.Species.MOUSE));
         var mouseTissueOrgans = mouse.Organism.OrgansByType(OrganType.Tissue).ToList();

         foreach (var species in _speciesRepository.All().Where(x => x.Name != CoreConstants.Species.HUMAN))
         {
            var individualForSpecies = species.Name.Equals(CoreConstants.Species.MOUSE)
               ? mouse
               : _individualRetriever.DefaultIndividualFor(species);

            var tissueOrgans = individualForSpecies.Organism.OrgansByType(OrganType.Tissue);

            foreach (var tissueOrgan in tissueOrgans)
            {
               var mouseTissueOrgan = mouseTissueOrgans.FindByName(tissueOrgan.Name);

               if (tissueOrgan.Parameter(CoreConstants.Parameters.VOLUME_MOUSE).Value !=
                   mouseTissueOrgan.Parameter(Constants.Parameters.VOLUME).Value)

               errorList.Add($"{species.Name}.{tissueOrgan.Name}.{Constants.Parameters.VOLUME} is not equal to the corresponding mouse volume value");
            }
         }

         errorList.Count.ShouldBeEqualTo(0, errorList.ToString("\n"));
      }
   }
}