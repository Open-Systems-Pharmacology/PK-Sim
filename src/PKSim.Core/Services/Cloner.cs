using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Services
{
   public interface ICloner : ICloneManager
   {
      /// <summary>
      ///    Clones an object in the context of a model building=>Will not share formula in any case but origin id for formula
      ///    will
      ///    be saved to minimize outputs
      /// </summary>
      T CloneForModel<T>(T objectToClone) where T : class, IUpdatable;

      /// <summary>
      ///    Clones the given <paramref name="objectToClone" /> using either the standard IUpdatable strategy if
      ///    <paramref name="objectToClone" /> implements
      ///    <see cref="IUpdatable" />. Otherwise, falls back to serializing and deserializing the same stream. The id of the
      ///    returned parameter if defined will be reset
      /// </summary>
      T CloneObject<T>(T objectToClone) where T : class;
   }

   public class Cloner : ICloner
   {
      private readonly ICloneManagerForModel _cloneManagerForModel;
      private readonly ICloneManagerForBuildingBlock _cloneManagerForBuildingBlock;
      private readonly IBuildingBlockFinalizer _buildingBlockFinalizer;
      private readonly ISerializationManager _serializationManager;
      private readonly IObjectIdResetter _objectIdResetter;

      public Cloner(ICloneManagerForModel cloneManagerForModel, ICloneManagerForBuildingBlock cloneManagerForBuildingBlock,
         IBuildingBlockFinalizer buildingBlockFinalizer, ISerializationManager serializationManager, IObjectIdResetter objectIdResetter)
      {
         _cloneManagerForModel = cloneManagerForModel;
         _cloneManagerForBuildingBlock = cloneManagerForBuildingBlock;
         _buildingBlockFinalizer = buildingBlockFinalizer;
         _serializationManager = serializationManager;
         _objectIdResetter = objectIdResetter;
      }

      public T Clone<T>(T objectToClone) where T : class, IUpdatable
      {
         //formula cache are never used in pksim explicitely. And if need, we access CloneManagerForBuildingBlock
         _cloneManagerForBuildingBlock.FormulaCache = new FormulaCache();
         return createClone(objectToClone, _cloneManagerForBuildingBlock);
      }

      public T CloneForModel<T>(T objectToClone) where T : class, IUpdatable
      {
         return createClone(objectToClone, _cloneManagerForModel);
      }

      public T CloneObject<T>(T objectToClone) where T : class
      {
         var updatable = objectToClone as IUpdatable;
         if (updatable != null)
            return Clone(updatable) as T;

         var stream = _serializationManager.Serialize(objectToClone);
         var clone = _serializationManager.Deserialize<T>(stream);
         _objectIdResetter.ResetIdFor(clone);
         return clone;
      }

      private T createClone<T>(T objectToClone, ICloneManager cloneManager) where T : class, IUpdatable
      {
         var clone = cloneManager.Clone(objectToClone);
         var buildingBlock = clone as IPKSimBuildingBlock;
         if (buildingBlock != null)
            _buildingBlockFinalizer.Finalize(buildingBlock);

         return clone;
      }
   }
}