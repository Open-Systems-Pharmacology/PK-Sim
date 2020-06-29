using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Descriptors;
using OSPSuite.Core.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Repositories;
using SnapshotObserver = PKSim.Core.Snapshots.Observer;
using ModelObserver = OSPSuite.Core.Domain.Builder.IObserverBuilder;

namespace PKSim.Core.Snapshots.Mappers
{
   public class ObserverMapper : ObjectBaseSnapshotMapperBase<ModelObserver, SnapshotObserver>
   {
      private const string AMOUNT_OBSERVER = "Amount";
      private const string CONTAINER_OBSERVER = "Container";

      private readonly DescriptorConditionMapper _descriptorConditionMapper;

      private readonly ExplicitFormulaMapper _explicitFormulaMapper;

      private readonly MoleculeListMapper _moleculeListMapper;
      private readonly IObjectBaseFactory _objectBaseFactory;
      private readonly IDimensionRepository _dimensionRepository;
      private readonly IOSPLogger _logger;

      public ObserverMapper(
         DescriptorConditionMapper descriptorConditionMapper, 
         ExplicitFormulaMapper explicitFormulaMapper, 
         MoleculeListMapper moleculeListMapper,
         IObjectBaseFactory objectBaseFactory,
         IDimensionRepository dimensionRepository,
         IOSPLogger logger)
      {
         _descriptorConditionMapper = descriptorConditionMapper;
         _explicitFormulaMapper = explicitFormulaMapper;
         _moleculeListMapper = moleculeListMapper;
         _objectBaseFactory = objectBaseFactory;
         _dimensionRepository = dimensionRepository;
         _logger = logger;
      }

      public override async Task<SnapshotObserver> MapToSnapshot(ModelObserver observer)
      {
         var snapshot = await SnapshotFrom(observer);
         snapshot.Dimension = SnapshotValueFor(observer.Dimension?.Name);
         snapshot.ContainerCriteria = await _descriptorConditionMapper.MapToSnapshots(observer.ContainerCriteria);

         //We do not support any other type of formula at the moment
         snapshot.Formula = await _explicitFormulaMapper.MapToSnapshot(observer.Formula as OSPSuite.Core.Domain.Formulas.ExplicitFormula);
         snapshot.MoleculeList = await _moleculeListMapper.MapToSnapshot(observer.MoleculeList);

         switch (observer)
         {
            case AmountObserverBuilder _:
               snapshot.Type = AMOUNT_OBSERVER;
               break;
            case ContainerObserverBuilder _:;
               snapshot.Type = CONTAINER_OBSERVER;
               break;
         }

         return snapshot;
      }

      public override async Task<ModelObserver> MapToModel(SnapshotObserver snapshot)
      {
         var observer = createObserverFrom(snapshot);
         if (observer == null)
            return null;

         MapSnapshotPropertiesToModel(snapshot, observer);
         observer.Dimension = _dimensionRepository.DimensionByName(snapshot.Dimension);
         var allDescriptorConditions = await _descriptorConditionMapper.MapToModels(snapshot.ContainerCriteria);

         observer.ContainerCriteria = new DescriptorCriteria();
         allDescriptorConditions?.Each(observer.ContainerCriteria.Add);

         observer.Formula = await _explicitFormulaMapper.MapToModel(snapshot.Formula);
         var moleculeList = await _moleculeListMapper.MapToModel(snapshot.MoleculeList);
         observer.MoleculeList.Update(moleculeList);
         return observer;

      }


      private IObserverBuilder createObserverFrom(SnapshotObserver snapshot)
      {
         if (string.IsNullOrEmpty(snapshot.Type))
            return null;

         switch (snapshot.Type)
         {
            case AMOUNT_OBSERVER:
               return _objectBaseFactory.Create<AmountObserverBuilder>();
            case CONTAINER_OBSERVER:
               return _objectBaseFactory.Create<ContainerObserverBuilder>();
         }

         _logger.AddError(PKSimConstants.Error.CannotCreateObserverFromSnapshot(snapshot.Type));
         return null;
      }
   }
}