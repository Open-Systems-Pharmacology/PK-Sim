using System.Threading.Tasks;
using OSPSuite.Utility.Extensions;
using SnapshotMoleculeList = PKSim.Core.Snapshots.MoleculeList;
using ModelMoleculeList = OSPSuite.Core.Domain.Builder.MoleculeList;

namespace PKSim.Core.Snapshots.Mappers
{
   public class MoleculeListMapper : SnapshotMapperBase<ModelMoleculeList, SnapshotMoleculeList>
   {
      public override Task<SnapshotMoleculeList> MapToSnapshot(ModelMoleculeList moleculeList)
      {
         return SnapshotFrom(moleculeList, x =>
         {
            x.ForAll = moleculeList.ForAll;
            x.MoleculeNamesToInclude = SnapshotValueFor(moleculeList.MoleculeNames);
            x.MoleculeNamesToExclude = SnapshotValueFor(moleculeList.MoleculeNamesToExclude);
         });
      }

      public override Task<ModelMoleculeList> MapToModel(SnapshotMoleculeList snapshot, SnapshotContext snapshotContext)
      {
         var moleculeList = new ModelMoleculeList {ForAll = snapshot.ForAll};

         snapshot.MoleculeNamesToInclude?.Each(moleculeList.AddMoleculeName);
         snapshot.MoleculeNamesToExclude?.Each(moleculeList.AddMoleculeNameToExclude);

         return Task.FromResult(moleculeList);
      }
   }
}