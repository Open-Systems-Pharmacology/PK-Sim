using System.Collections.Generic;
using System.Linq;
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

      public override SnapshotProtocol MapToSnapshot(ModelProtocol modelProtocol)
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

      private Schema snapshotSchemaFrom(Model.Schema schema) => _schemaMapper.MapToSnapshot(schema);

      public override ModelProtocol MapToModel(SnapshotProtocol snapshotProtocol)
      {
         var modelProtocol = createModelProtocolFrom(snapshotProtocol);
         MapSnapshotPropertiesIntoModel(snapshotProtocol, modelProtocol);
         UpdateParametersFromSnapshot(modelProtocol, snapshotProtocol, PKSimConstants.ObjectTypes.AdministrationProtocol);
         return modelProtocol;
      }

      private ModelProtocol createModelProtocolFrom(SnapshotProtocol snapshotProtocol)
      {
         if (snapshotProtocol.IsSimple)
            return createSimpleProtocolFrom(snapshotProtocol);

         return createAdvancedProtocolFrom(snapshotProtocol);
      }

      private SnapshotProtocol createSnapshotFromAdvancedProtocol(AdvancedProtocol advancedProtocol)
      {
         return SnapshotFrom(advancedProtocol, snapshot =>
         {
            snapshot.Schemas = new List<Schema>(advancedProtocol.AllSchemas.Select(snapshotSchemaFrom));
            snapshot.TimeUnit = advancedProtocol.TimeUnit.Name;
         });
      }

      private AdvancedProtocol createAdvancedProtocolFrom(SnapshotProtocol snapshotProtocol)
      {
         var advancedProtocol = _protocolFactory.Create(ProtocolMode.Advanced).DowncastTo<AdvancedProtocol>();
         advancedProtocol.RemoveAllSchemas();
         if (snapshotProtocol.Schemas != null)
            advancedProtocol.AddChildren(snapshotProtocol.Schemas.Select(_schemaMapper.MapToModel));

         advancedProtocol.TimeUnit = _dimensionRepository.Time.UnitOrDefault(snapshotProtocol.TimeUnit);
         return advancedProtocol;
      }

      private SnapshotProtocol createSnapshotFromSimpleProtocol(SimpleProtocol simpleProtocol)
      {
         return SnapshotFrom(simpleProtocol, snapshot =>
         {
            snapshot.ApplicationType = simpleProtocol.ApplicationType.Name;
            snapshot.DosingInterval = simpleProtocol.DosingInterval.Id.ToString();
            snapshot.TargetOrgan = SnapshotValueFor(simpleProtocol.TargetOrgan);
            snapshot.TargetCompartment = SnapshotValueFor(simpleProtocol.TargetCompartment);
         });
      }

      private SimpleProtocol createSimpleProtocolFrom(SnapshotProtocol snapshotProtocol)
      {
         var applicationType = ApplicationTypes.ByName(snapshotProtocol.ApplicationType);
         var simpleProtocol = _protocolFactory.Create(ProtocolMode.Simple, applicationType).DowncastTo<SimpleProtocol>();
         var dosingIntervalId = EnumHelper.ParseValue<DosingIntervalId>(snapshotProtocol.DosingInterval);
         simpleProtocol.DosingInterval = DosingIntervals.ById(dosingIntervalId);
         simpleProtocol.TargetOrgan = snapshotProtocol.TargetOrgan;
         simpleProtocol.TargetCompartment = snapshotProtocol.TargetCompartment;
         return simpleProtocol;
      }
   }
}