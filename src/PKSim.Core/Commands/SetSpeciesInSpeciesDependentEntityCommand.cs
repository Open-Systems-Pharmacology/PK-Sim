using OSPSuite.Core.Commands.Core;
using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class SetSpeciesInSpeciesDependentEntityCommand : BuildingBlockChangeCommand
   {
      private readonly string _entityId;
      private readonly Species _oldSpecies;
      private ISpeciesDependentEntity _entity;
      private Species _newSpecies;

      public SetSpeciesInSpeciesDependentEntityCommand(ISpeciesDependentEntity entity, Species species, IExecutionContext context)
      {
         _entity = entity;
         _entityId = entity.Id;
         BuildingBlockId = context.BuildingBlockIdContaining(entity);
         ObjectType = context.TypeFor(entity);
         CommandType = PKSimConstants.Command.CommandTypeEdit;
         _oldSpecies = entity.Species;
         _newSpecies = species;
         Description = PKSimConstants.Command.SetSpeciesInSpeciesDependentEntityDescription(context.TypeFor(_entity), entity.Name, _oldSpecies.Name, _newSpecies.Name);
         context.UpdateBuildinBlockProperties(this, context.BuildingBlockContaining(entity));
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
         _entity.Species = _newSpecies;
      }

      public override void RestoreExecutionData(IExecutionContext context)
      {
         base.RestoreExecutionData(context);
         _entity = context.Get<ISpeciesDependentEntity>(_entityId);
      }

      protected override void ClearReferences()
      {
         _entity = null;
         _newSpecies = null;
         //do not clear ref for old species, as old species will be used in the rollback command
      }

      protected override IReversibleCommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new SetSpeciesInSpeciesDependentEntityCommand(_entity, _oldSpecies, context).AsInverseFor(this);
      }
   }
}