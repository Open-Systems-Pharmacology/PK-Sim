using SnapshotSimpleProtocol = PKSim.Core.Snapshots.SimpleProtocol;
using ModelSimpleProtocol = PKSim.Core.Model.SimpleProtocol;


namespace PKSim.Core.Snapshots.Mappers
{
   public class SimpleProtocolMapper : ParameterContainerSnapshotMapperBase<ModelSimpleProtocol, SnapshotSimpleProtocol>
   {
      public SimpleProtocolMapper(ParameterMapper parameterMapper) : base(parameterMapper)
      {
      }

      public override SnapshotSimpleProtocol MapToSnapshot(ModelSimpleProtocol modelSimpleProtocol)
      {
         var snapshot = new SnapshotSimpleProtocol
         {
            ApplicationType = modelSimpleProtocol.ApplicationType.Name,
            DosingInterval = modelSimpleProtocol.DosingInterval.Id.ToString(),
            TargetOrgan = modelSimpleProtocol.TargetOrgan,
            TargetCompartment = modelSimpleProtocol.TargetCompartment,
         };

         MapModelPropertiesIntoSnapshot(modelSimpleProtocol, snapshot);
         MapVisibleParameters(modelSimpleProtocol, snapshot);

         return snapshot;
      }

      public override ModelSimpleProtocol MapToModel(SnapshotSimpleProtocol snapshotSimpleProtocol)
      {
         throw new System.NotImplementedException();
      }
   }
}