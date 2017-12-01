using System.Threading.Tasks;
using PKSim.Core.Model;

namespace PKSim.Core.Snapshots.Mappers
{
   public class ProcessMappingMapper : SnapshotMapperBase<IProcessMapping, CompoundProcessSelection, Model.CompoundProcess>
   {
      public override async Task<CompoundProcessSelection> MapToSnapshot(IProcessMapping partialProcessMapping)
      {
         var snapshot = await SnapshotFrom(partialProcessMapping, x => { x.Name = partialProcessMapping.ProcessName; });

         switch (partialProcessMapping)
         {
            case EnzymaticProcessSelection enzymaticProcessSelection:
               snapshot.MoleculeName = SnapshotValueFor(enzymaticProcessSelection.MoleculeName);
               snapshot.MetaboliteName = SnapshotValueFor(enzymaticProcessSelection.MetaboliteName);
               break;
            case InteractionSelection interactionSelection:
               snapshot.MoleculeName = SnapshotValueFor(interactionSelection.MoleculeName);
               snapshot.CompoundName = SnapshotValueFor(interactionSelection.CompoundName);
               break;
            case ProcessSelection processSelection:
               snapshot.MoleculeName = SnapshotValueFor(processSelection.MoleculeName);
               break;
         }

         return snapshot;
      }

      public override Task<IProcessMapping> MapToModel(CompoundProcessSelection snapshot, Model.CompoundProcess process)
      {
         IProcessMapping processMapping;
         switch (process)
         {
            case SystemicProcess systemicProcess:
               processMapping = new SystemicProcessSelection
               {
                  ProcessType = systemicProcess.SystemicProcessType,
               };
               break;
            case EnzymaticProcess _:
               processMapping = new EnzymaticProcessSelection
               {
                  MetaboliteName = snapshot.MetaboliteName,
               };
               break;
            case InteractionProcess _:
               processMapping = new InteractionSelection();
               break;
            default:
               processMapping = new ProcessSelection();
               break;
         }

         processMapping.ProcessName = snapshot.Name;
         processMapping.CompoundName = process.ParentCompound.Name;
         processMapping.MoleculeName = snapshot.MoleculeName;

         return Task.FromResult(processMapping);
      }
   }
}