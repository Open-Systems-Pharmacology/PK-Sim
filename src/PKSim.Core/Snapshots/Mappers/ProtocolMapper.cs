using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using ModelProtocol = PKSim.Core.Model.Protocol;
using SnapshotProtocol = PKSim.Core.Snapshots.Protocol;

namespace PKSim.Core.Snapshots.Mappers
{
   public class ProtocolMapper : ParameterContainerSnapshotMapperBase<ModelProtocol, SnapshotProtocol>
   {
      private readonly IProtocolFactory _protocolFactory;
      private readonly SchemaMapper _schemaMapper;
      private readonly IDimensionRepository _dimensionRepository;

      public ProtocolMapper(ParameterMapper parameterMapper, IProtocolFactory protocolFactory, SchemaMapper schemaMapper, IDimensionRepository dimensionRepository) : base(parameterMapper)
      {
         _protocolFactory = protocolFactory;
         _schemaMapper = schemaMapper;
         _dimensionRepository = dimensionRepository;
      }

      public override Task<SnapshotProtocol> MapToSnapshot(ModelProtocol modelProtocol)
      {
         switch (modelProtocol)
         {
            case SimpleProtocol simpleProtocol:
               return createSnapshotFromSimpleProtocol(simpleProtocol);

            case AdvancedProtocol advancedProtocol:
               return createSnapshotFromAdvancedProtocol(advancedProtocol);
         }
         return null;
      }
     public override async Task<ModelProtocol> MapToModel(SnapshotProtocol snapshotProtocol)
      {
         var modelProtocol = await createModelProtocolFrom(snapshotProtocol);
         MapSnapshotPropertiesToModel(snapshotProtocol, modelProtocol);
         await UpdateParametersFromSnapshot(snapshotProtocol, modelProtocol);
         return modelProtocol;
      }

      private Task<ModelProtocol> createModelProtocolFrom(SnapshotProtocol snapshotProtocol)
      {
         if (snapshotProtocol.IsSimple)
            return createSimpleProtocolFrom(snapshotProtocol);

         return createAdvancedProtocolFrom(snapshotProtocol);
      }

      private async Task<SnapshotProtocol> createSnapshotFromAdvancedProtocol(AdvancedProtocol advancedProtocol)
      {
         var snapshot = await SnapshotFrom(advancedProtocol, x =>
         {
            x.TimeUnit = advancedProtocol.TimeUnit.Name;
         });
         snapshot.Schemas = await _schemaMapper.MapToSnapshots(advancedProtocol.AllSchemas);
         return snapshot;
      }

      private async Task<ModelProtocol> createAdvancedProtocolFrom(SnapshotProtocol snapshotProtocol)
      {
         var advancedProtocol = _protocolFactory.Create(ProtocolMode.Advanced).DowncastTo<AdvancedProtocol>();
         advancedProtocol.RemoveAllSchemas();
         advancedProtocol.TimeUnit = _dimensionRepository.Time.UnitOrDefault(snapshotProtocol.TimeUnit);

         if (snapshotProtocol.Schemas == null)
            return advancedProtocol;

         advancedProtocol.AddChildren(await _schemaMapper.MapToModels(snapshotProtocol.Schemas));
         return advancedProtocol;
      }

      private Task<SnapshotProtocol> createSnapshotFromSimpleProtocol(SimpleProtocol simpleProtocol)
      {
         return SnapshotFrom(simpleProtocol, snapshot =>
         {
            snapshot.ApplicationType = simpleProtocol.ApplicationType.Name;
            snapshot.DosingInterval = simpleProtocol.DosingInterval.Id;
            snapshot.TargetOrgan = SnapshotValueFor(simpleProtocol.TargetOrgan);
            snapshot.TargetCompartment = SnapshotValueFor(simpleProtocol.TargetCompartment);
         });
      }

      private Task<ModelProtocol> createSimpleProtocolFrom(SnapshotProtocol snapshotProtocol)
      {
         var applicationType = ApplicationTypes.ByName(snapshotProtocol.ApplicationType);
         var simpleProtocol = _protocolFactory.Create(ProtocolMode.Simple, applicationType).DowncastTo<SimpleProtocol>();
         simpleProtocol.DosingInterval = DosingIntervals.ById(snapshotProtocol.DosingInterval);
         simpleProtocol.TargetOrgan = snapshotProtocol.TargetOrgan;
         simpleProtocol.TargetCompartment = snapshotProtocol.TargetCompartment;
         return Task.FromResult<ModelProtocol>(simpleProtocol);
      }
   }
}