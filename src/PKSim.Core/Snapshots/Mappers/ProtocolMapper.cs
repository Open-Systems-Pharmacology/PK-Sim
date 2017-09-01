using System;
using PKSim.Core.Model;
using ModelProtocol = PKSim.Core.Model.Protocol;

namespace PKSim.Core.Snapshots.Mappers
{
   public class ProtocolMapper : ParameterContainerSnapshotMapperBase<ModelProtocol, Protocol>
   {
      public ProtocolMapper(ParameterMapper parameterMapper) : base(parameterMapper)
      {
      }

      public override Protocol MapToSnapshot(ModelProtocol modelProtocol)
      {
         var snapshot = createSnapshotFrom(modelProtocol);
         MapModelPropertiesIntoSnapshot(modelProtocol, snapshot);
         MapVisibleParameters(modelProtocol, snapshot);
         return snapshot;
      }

      private Protocol createSnapshotFrom(ModelProtocol modelProtocol)
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

      private Protocol createSnapshotFromAdvancedProtocol(AdvancedProtocol advancedProtocol)
      {
         throw new NotImplementedException();
      }

      private Protocol createSnapshotFromSimpleProtocol(SimpleProtocol simpleProtocol)
      {
         return new Protocol
         {
            ApplicationType = simpleProtocol.ApplicationType.Name,
            DosingInterval = simpleProtocol.DosingInterval.Id.ToString(),
            TargetOrgan = simpleProtocol.TargetOrgan,
            TargetCompartment = simpleProtocol.TargetCompartment,
         };
      }

      public override ModelProtocol MapToModel(Protocol snapshotProtocol)
      {
         throw new NotImplementedException();
      }
   }
}