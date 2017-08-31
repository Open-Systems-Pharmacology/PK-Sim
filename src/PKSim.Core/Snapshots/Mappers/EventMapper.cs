using SnapshotEvent = PKSim.Core.Snapshots.Event;
using ModelEvent = PKSim.Core.Model.PKSimEvent;


namespace PKSim.Core.Snapshots.Mappers
{
   public class EventMapper : ParameterContainerSnapshotMapperBase<ModelEvent, SnapshotEvent>
   {
      public EventMapper(ParameterMapper parameterMapper) : base(parameterMapper)
      {
      }

      public override SnapshotEvent MapToSnapshot(ModelEvent modelEvent)
      {
         var snapshotEvent = new SnapshotEvent();
         MapModelPropertiesIntoSnapshot(modelEvent, snapshotEvent);
         MapVisibleParameters(modelEvent, snapshotEvent);
         snapshotEvent.Template = modelEvent.TemplateName;
         return snapshotEvent;
      }

      public override ModelEvent MapToModel(SnapshotEvent snapshotEvent)
      {
         throw new System.NotImplementedException();
      }
   }
}