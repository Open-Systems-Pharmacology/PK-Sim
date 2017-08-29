using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using PKSim.Assets;
using PKSim.Core.Events;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class SwitchAdvancedParameterDistributionTypeCommand : BuildingBlockChangeCommand
   {
      private readonly DistributionType _oldDistributionType;
      private AdvancedParameter _advancedParameter;
      private IAdvancedParameterContainer _advancedParameterContainer;
      private IDistributedParameter _newDistributedParameter;
      private DistributionType _newDistributionType;
      private IDistributedParameter _oldDistributedParameter;
      private IParameter _parameter;
      private byte[] _serializationStream;

      public SwitchAdvancedParameterDistributionTypeCommand(IParameter parameter, IAdvancedParameterContainer advancedParameterContainer, DistributionType newDistributionType, IExecutionContext context)
      {
         _advancedParameterContainer = advancedParameterContainer;
         _newDistributionType = newDistributionType;
         _parameter = parameter;
         ParameterId = parameter.Id;
         BuildingBlockId = _advancedParameterContainer.Id;
         var entityPathResolver = context.Resolve<IEntityPathResolver>();
         var advancedParameterFactory = context.Resolve<IAdvancedParameterFactory>();
         _advancedParameter = _advancedParameterContainer.AdvancedParameterFor(entityPathResolver, _parameter);
         _oldDistributedParameter = _advancedParameter.DistributedParameter;
         _oldDistributionType = _advancedParameter.DistributionType;
         var newParameter = advancedParameterFactory.Create(_parameter, _newDistributionType);
         _newDistributedParameter = newParameter.DistributedParameter;
         ObjectType = context.TypeFor(_advancedParameter);
         Description = PKSimConstants.Command.SwitchAdvancedParameterDistributionTypeDescription(_advancedParameter.ParameterPath, _oldDistributionType.ToString(), _newDistributionType.ToString());
         context.UpdateBuildinBlockProperties(this, _advancedParameterContainer);
      }

      public string ParameterId { get; private set; }

      protected override void ClearReferences()
      {
         _advancedParameter = null;
         _advancedParameterContainer = null;
         _parameter = null;
         _oldDistributedParameter = null;
         _newDistributedParameter = null;
      }

      public override void RestoreExecutionData(IExecutionContext context)
      {
         base.RestoreExecutionData(context);
         _advancedParameterContainer = context.Get<RandomPopulation>(BuildingBlockId);
         _parameter = context.Get<IParameter>(ParameterId);
         var entityPathResolver = context.Resolve<IEntityPathResolver>();
         _advancedParameter = _advancedParameterContainer.AdvancedParameterFor(entityPathResolver, _parameter);
         _oldDistributedParameter = context.Deserialize<IDistributedParameter>(_serializationStream);
         _newDistributedParameter = _advancedParameter.DistributedParameter;
         _newDistributionType = _advancedParameter.DistributionType;
      }

      protected override IReversibleCommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new SwitchAdvancedParameterDistributionTypeCommand(_parameter, _advancedParameterContainer, _oldDistributionType, context).AsInverseFor(this);
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
         //save old
         _serializationStream = context.Serialize(_oldDistributedParameter);
         context.Unregister(_oldDistributedParameter);
         //set new
         _advancedParameter.DistributedParameter = _newDistributedParameter;
         context.Register(_newDistributedParameter);

         //update distribution according to the new definition of the advancd parameter
         _advancedParameterContainer.GenerateRandomValuesFor(_advancedParameter);
         context.PublishEvent(new AdvancedParameterDistributionChangedEvent(_advancedParameterContainer, _advancedParameter));
      }

      public override void UpdateInternalFrom(IBuildingBlockChangeCommand buildingBlockChangeCommand)
      {
         base.UpdateInternalFrom(buildingBlockChangeCommand);
         var switchDistributionTypeCommand = buildingBlockChangeCommand as SwitchAdvancedParameterDistributionTypeCommand;
         if (switchDistributionTypeCommand == null) return;
         _newDistributedParameter = switchDistributionTypeCommand._oldDistributedParameter;
         _oldDistributedParameter = switchDistributionTypeCommand._newDistributedParameter;
      }
   }
}