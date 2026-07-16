using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Snapshots.Mappers;
using PKSim.Core.Model;
using ModelEvent = PKSim.Core.Model.PKSimEvent;
using SnapshotEvent = PKSim.Core.Snapshots.Event;

namespace PKSim.Core.Snapshots.Mappers
{
   public class EventMapper : ParameterContainerSnapshotMapperBase<ModelEvent, SnapshotEvent, SnapshotContext>
   {
      private readonly IEventFactory _eventFactory;

      public EventMapper(ParameterMapper parameterMapper,
         IEventFactory eventFactory) : base(parameterMapper)
      {
         _eventFactory = eventFactory;
      }

      public override Task<SnapshotEvent> MapToSnapshot(ModelEvent modelEvent)
      {
         return SnapshotFrom(modelEvent, snapshot => { snapshot.Template = modelEvent.TemplateName; });
      }

      public override async Task<ModelEvent> MapToModel(SnapshotEvent snapshotEvent, SnapshotContext snapshotContext)
      {
         var modelEvent = _eventFactory.Create(snapshotEvent.Template);
         MapSnapshotPropertiesToModel(snapshotEvent, modelEvent);
         await UpdateParametersFromSnapshot(snapshotEvent, modelEvent, snapshotContext, snapshotEvent.Template);
         return modelEvent;
      }

      protected override bool ShouldExportToSnapshot(IParameter parameter) => parameter.ShouldExportToSnapshot();
   }
}