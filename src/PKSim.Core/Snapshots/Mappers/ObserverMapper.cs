using System.Threading.Tasks;
using SnapshotObserver = PKSim.Core.Snapshots.Observer;
using ModelObserver = OSPSuite.Core.Domain.Builder.IObserverBuilder;

namespace PKSim.Core.Snapshots.Mappers
{
   public class ObserverMapper : ObjectBaseSnapshotMapperBase<ModelObserver, SnapshotObserver>
   {
      private readonly DescriptorConditionMapper _descriptorConditionMapper;

      private readonly ExplicitFormulaMapper _explicitFormulaMapper;

      private readonly MoleculeListMapper _moleculeListMapper;
      public ObserverMapper(
         DescriptorConditionMapper descriptorConditionMapper, 
         ExplicitFormulaMapper explicitFormulaMapper, 
         MoleculeListMapper moleculeListMapper)
      {
         _descriptorConditionMapper = descriptorConditionMapper;
         _explicitFormulaMapper = explicitFormulaMapper;
         _moleculeListMapper = moleculeListMapper;
      }

      public override async Task<SnapshotObserver> MapToSnapshot(ModelObserver observer)
      {
         var snapshot = await SnapshotFrom(observer);
         snapshot.Dimension = SnapshotValueFor(observer.Dimension?.Name);
         snapshot.ContainerCriteria = await _descriptorConditionMapper.MapToSnapshots(observer.ContainerCriteria);

         //We do not support any other type of formula at the moment
         snapshot.Formula = await _explicitFormulaMapper.MapToSnapshot(observer.Formula as OSPSuite.Core.Domain.Formulas.ExplicitFormula);
         snapshot.MoleculeList = await _moleculeListMapper.MapToSnapshot(observer.MoleculeList);
         return snapshot;
      }

      public override Task<ModelObserver> MapToModel(SnapshotObserver snapshot)
      {
         throw new System.NotImplementedException();
      }
   }
}