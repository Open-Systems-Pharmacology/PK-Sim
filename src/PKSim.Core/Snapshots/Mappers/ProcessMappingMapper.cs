using System.Threading.Tasks;
using PKSim.Core.Model;

namespace PKSim.Core.Snapshots.Mappers
{
   public class CompoundProcessSnapshotContext : SnapshotContext
   {
      public Model.CompoundProcess Process { get; }

      public CompoundProcessSnapshotContext(Model.CompoundProcess process, SnapshotContext baseContext) : base(baseContext)
      {
         Process = process;
      }
   }

   public class ProcessMappingMapper : SnapshotMapperBase<IProcessMapping, CompoundProcessSelection, CompoundProcessSnapshotContext>
   {
      public override async Task<CompoundProcessSelection> MapToSnapshot(IProcessMapping partialProcessMapping)
      {
         var snapshot = await SnapshotFrom(partialProcessMapping, x => { x.Name = SnapshotValueFor(partialProcessMapping.ProcessName); });

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
            case SystemicProcessSelection systemicProcessSelection:
               snapshot.SystemicProcessType = systemicProcessSelection.ProcessType.SystemicProcessTypeId.ToString();
               break;
         }

         return snapshot;
      }

      public override Task<IProcessMapping> MapToModel(CompoundProcessSelection snapshot, CompoundProcessSnapshotContext snapshotContext)
      {
         IProcessMapping processMapping;
         var process = snapshotContext.Process;
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

         processMapping.ProcessName = ModelValueFor(snapshot.Name);
         //Parent compound may be null for process that are representing a non existent selection
         processMapping.CompoundName = ModelValueFor(process.ParentCompound?.Name);
         processMapping.MoleculeName = ModelValueFor(snapshot.MoleculeName);

         return Task.FromResult(processMapping);
      }
   }
}