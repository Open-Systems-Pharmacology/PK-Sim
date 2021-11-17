using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IIndividualExpressionsUpdater
   {
      void Update(Individual sourceIndividual, Individual targetIndividual);
   }

   public class IndividualExpressionsUpdater : IIndividualExpressionsUpdater
   {
      private readonly IOntogenyTask _ontogenyTask;
      private readonly IMoleculeExpressionTask<Individual> _moleculeExpressionTask;
      private readonly IEntityPathResolver _entityPathResolver;
      private readonly ICloner _cloner;
      private readonly IParameterUpdater _parameterUpdater;

      public IndividualExpressionsUpdater(
         IOntogenyTask ontogenyTask, 
         IMoleculeExpressionTask<Individual> moleculeExpressionTask, 
         IEntityPathResolver entityPathResolver,
         ICloner cloner, 
         IParameterUpdater parameterUpdater)
      {
         _ontogenyTask = ontogenyTask;
         _moleculeExpressionTask = moleculeExpressionTask;
         _entityPathResolver = entityPathResolver;
         _cloner = cloner;
         _parameterUpdater = parameterUpdater;
      }

      public void Update(Individual sourceIndividual, Individual targetIndividual)
      {
         foreach (var sourceMolecule in sourceIndividual.AllMolecules())
         {
            //Add molecule to the target individual based on the new individual
            _moleculeExpressionTask.AddMoleculeTo(targetIndividual, sourceMolecule);

            var targetMolecule = targetIndividual.MoleculeByName(sourceMolecule.Name);

            //ensure that all global properties are updated
            targetMolecule.UpdatePropertiesFrom(sourceMolecule, _cloner);

            //Update all parameters from source to target
            updatePathCache(sourceIndividual.AllMoleculeParametersFor(sourceMolecule), targetIndividual.AllMoleculeParametersFor(targetMolecule));

            //Update all containers from source to target
            updatePathCache(sourceIndividual.AllMoleculeContainersFor(sourceMolecule), targetIndividual.AllMoleculeContainersFor(targetMolecule));

            //Make sure global parameters are reset to default to ensure proper scaling (they will be part of the scaling algorithm)
            resetGlobalMoleculeParametersToDefault(targetMolecule);

            //we have to reset the ontogeny for the molecule based on the target individual properties
            _ontogenyTask.SetOntogenyForMolecule(targetMolecule, targetMolecule.Ontogeny, targetIndividual);
         }
      }

      private void updatePathCache<T>(IEnumerable<T> sourceEntities, IEnumerable<T> targetEntities) where T: class, IEntity
      {
         var sourcePathCache = new PathCache<T>(_entityPathResolver).For(sourceEntities);
         var targetPathCache = new PathCache<T>(_entityPathResolver).For(targetEntities);

         sourcePathCache.KeyValues.Each(kv => updateEntities(kv.Value, targetPathCache[kv.Key]));
      }

      private void updateEntities<T>(T sourceEntity, T targetEntity) where T : class, IEntity
      {
         //this should never happen
         if (targetEntity == null)
            return;

         targetEntity.UpdatePropertiesFrom(sourceEntity, _cloner);   

         //for Parameters, we need to also update the values for constant parameters updated by the user. This is not done by default 
         if (!(targetEntity is IParameter targetParam) || !(sourceEntity is IParameter sourceParam))
            return;

         _parameterUpdater.UpdateValue(sourceParam, targetParam);
      }

      private void resetGlobalMoleculeParametersToDefault(IndividualMolecule molecule)
      {
         molecule.AllParameters()
            .Where(x => x.ValueDiffersFromDefault())
            .Each(x => x.ResetToDefault());
      }
   }
}