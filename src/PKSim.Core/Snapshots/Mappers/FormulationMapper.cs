using PKSim.Core.Model;
using SnapshotFormulation = PKSim.Core.Snapshots.Formulation;
using ModelFormulation = PKSim.Core.Model.Formulation;

namespace PKSim.Core.Snapshots.Mappers
{
   public class FormulationMapper : ParameterContainerSnapshotMapperBase<ModelFormulation, SnapshotFormulation>
   {
      public FormulationMapper(ParameterMapper parameterMapper) : base(parameterMapper)
      {
      }

      public override SnapshotFormulation MapToSnapshot(ModelFormulation modelFormulation)
      {
         var snapshotFormulation = new SnapshotFormulation();
         MapBaseProperties(modelFormulation, snapshotFormulation);
         MapParameters(modelFormulation.AllVisibleParameters(), snapshotFormulation);
         snapshotFormulation.FormulationType = modelFormulation.FormulationType;
         return snapshotFormulation;
      }
   }
}