using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using ModelProtocol = PKSim.Core.Model.Protocol;
using SnapshotProtocol = PKSim.Core.Snapshots.Protocol;

namespace PKSim.Core.Snapshots.Mappers
{
   public class ProtocolMapper : ParameterContainerSnapshotMapperBase<ModelProtocol, SnapshotProtocol>
   {
      private readonly IProtocolFactory _protocolFactory;
      private readonly SchemaMapper _schemaMapper;

      public ProtocolMapper(ParameterMapper parameterMapper, IProtocolFactory protocolFactory, SchemaMapper schemaMapper) : base(parameterMapper)
      {
         _protocolFactory = protocolFactory;
         _schemaMapper = schemaMapper;
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

      private SnapshotProtocol createSnapshotFromAdvancedProtocol(AdvancedProtocol advancedProtocol)
      {
         return SnapshotFrom(advancedProtocol, snapshot =>
         {
            snapshot.Schemas = new List<Schema>(advancedProtocol.AllSchemas.Select(snapshotSchemaFrom));
            snapshot.TimeUnit = advancedProtocol.TimeUnit.Name;
         });
      }

      private Schema snapshotSchemaFrom(Model.Schema schema) => _schemaMapper.MapToSnapshot(schema);

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

      private AdvancedProtocol createAdvancedProtocolFrom(Protocol snapshotProtocol)
      {
         throw new NotImplementedException();
      }

      private SimpleProtocol createSimpleProtocolFrom(SnapshotProtocol snapshotProtocol)
      {
         var applicationType = ApplicationTypes.ByName(snapshotProtocol.ApplicationType);
         var dosingIntervalId = EnumHelper.ParseValue<DosingIntervalId>(snapshotProtocol.DosingInterval);
         var simpleProtocol = _protocolFactory.Create(ProtocolMode.Simple, applicationType).DowncastTo<SimpleProtocol>();
         simpleProtocol.DosingInterval = DosingIntervals.ById(dosingIntervalId);
         simpleProtocol.TargetOrgan = snapshotProtocol.TargetOrgan;
         simpleProtocol.TargetCompartment = snapshotProtocol.TargetCompartment;
         return simpleProtocol;
      }
   }
}