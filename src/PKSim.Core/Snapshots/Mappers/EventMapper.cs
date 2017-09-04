using PKSim.Core.Model;
using SnapshotEvent = PKSim.Core.Snapshots.Event;
using ModelEvent = PKSim.Core.Model.PKSimEvent;

namespace PKSim.Core.Snapshots.Mappers
{
   public class EventMapper : ParameterContainerSnapshotMapperBase<ModelEvent, SnapshotEvent>
   {
      private readonly IEventFactory _eventFactory;

      public EventMapper(ParameterMapper parameterMapper, IEventFactory eventFactory) : base(parameterMapper)
      {
         _eventFactory = eventFactory;
      }

      public override SnapshotEvent MapToSnapshot(ModelEvent modelEvent)
      {
         return SnapshotFrom(modelEvent, snapshot =>
         {
            snapshot.Template = modelEvent.TemplateName;
         });
      }

      public override ModelEvent MapToModel(SnapshotEvent snapshotEvent)
      {
         var modelEvent = _eventFactory.Create(snapshotEvent.Template);
         MapSnapshotPropertiesToModel(snapshotEvent, modelEvent);
         UpdateParametersFromSnapshot(modelEvent, snapshotEvent, snapshotEvent.Template);
         return modelEvent;
      }
   }
}