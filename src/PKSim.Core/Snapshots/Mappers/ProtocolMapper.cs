using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Utility;
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

      private Task<Schema> snapshotSchemaFrom(Model.Schema schema) => _schemaMapper.MapToSnapshot(schema);

      public override async Task<ModelProtocol> MapToModel(SnapshotProtocol snapshotProtocol)
      {
         var modelProtocol = await createModelProtocolFrom(snapshotProtocol);
         MapSnapshotPropertiesToModel(snapshotProtocol, modelProtocol);
         await UpdateParametersFromSnapshot(snapshotProtocol, modelProtocol, PKSimConstants.ObjectTypes.AdministrationProtocol);
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
         snapshot.Schemas = await Task.WhenAll(advancedProtocol.AllSchemas.Select(snapshotSchemaFrom));
         return snapshot;
      }

      private async Task<ModelProtocol> createAdvancedProtocolFrom(SnapshotProtocol snapshotProtocol)
      {
         var advancedProtocol = _protocolFactory.Create(ProtocolMode.Advanced).DowncastTo<AdvancedProtocol>();
         advancedProtocol.RemoveAllSchemas();
         advancedProtocol.TimeUnit = _dimensionRepository.Time.UnitOrDefault(snapshotProtocol.TimeUnit);

         if (snapshotProtocol.Schemas != null)
         {
            var tasks = snapshotProtocol.Schemas.Select(_schemaMapper.MapToModel);
            advancedProtocol.AddChildren(await Task.WhenAll(tasks));
         }

         return advancedProtocol;
      }

      private Task<SnapshotProtocol> createSnapshotFromSimpleProtocol(SimpleProtocol simpleProtocol)
      {
         return SnapshotFrom(simpleProtocol, snapshot =>
         {
            snapshot.ApplicationType = simpleProtocol.ApplicationType.Name;
            snapshot.DosingInterval = simpleProtocol.DosingInterval.Id.ToString();
            snapshot.TargetOrgan = SnapshotValueFor(simpleProtocol.TargetOrgan);
            snapshot.TargetCompartment = SnapshotValueFor(simpleProtocol.TargetCompartment);
         });
      }

      private Task<ModelProtocol> createSimpleProtocolFrom(SnapshotProtocol snapshotProtocol)
      {
         var applicationType = ApplicationTypes.ByName(snapshotProtocol.ApplicationType);
         var simpleProtocol = _protocolFactory.Create(ProtocolMode.Simple, applicationType).DowncastTo<SimpleProtocol>();
         var dosingIntervalId = EnumHelper.ParseValue<DosingIntervalId>(snapshotProtocol.DosingInterval);
         simpleProtocol.DosingInterval = DosingIntervals.ById(dosingIntervalId);
         simpleProtocol.TargetOrgan = snapshotProtocol.TargetOrgan;
         simpleProtocol.TargetCompartment = snapshotProtocol.TargetCompartment;
         return Task.FromResult<ModelProtocol>(simpleProtocol);
      }
   }
}