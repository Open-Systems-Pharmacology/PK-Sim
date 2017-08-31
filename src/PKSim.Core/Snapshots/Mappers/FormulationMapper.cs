using OSPSuite.Core.Domain;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using SnapshotFormulation = PKSim.Core.Snapshots.Formulation;
using ModelFormulation = PKSim.Core.Model.Formulation;

namespace PKSim.Core.Snapshots.Mappers
{
   public class FormulationMapper : ParameterContainerSnapshotMapperBase<ModelFormulation, SnapshotFormulation>
   {
      private readonly IFormulationRepository _formulationRepository;
      private readonly ICloner _cloner;

      public FormulationMapper(ParameterMapper parameterMapper, IFormulationRepository formulationRepository, ICloner cloner) : base(parameterMapper)
      {
         _formulationRepository = formulationRepository;
         _cloner = cloner;
      }

      public override SnapshotFormulation MapToSnapshot(ModelFormulation modelFormulation)
      {
         var snapshotFormulation = new SnapshotFormulation();
         MapModelPropertiesIntoSnapshot(modelFormulation, snapshotFormulation);
         MapVisibleParameters(modelFormulation, snapshotFormulation);
         snapshotFormulation.FormulationType = modelFormulation.FormulationType;
         return snapshotFormulation;
      }

      public override ModelFormulation MapToModel(SnapshotFormulation formulationSnapshot)
      {
         var template = _formulationRepository.FormulationBy(formulationSnapshot.FormulationType);
         var formulation = _cloner.Clone(template);

         MapSnapshotPropertiesIntoModel(formulationSnapshot, formulation);
         foreach (var snapshotParameter in formulationSnapshot.Parameters)
         {
            var modelParameter = formulation.Parameter(snapshotParameter.Name);

            if (modelParameter == null)
               throw new SnapshotParameterNotFoundException(snapshotParameter.Name, formulationSnapshot.FormulationType);

            _parameterMapper.UpdateParameterFromSnapshot(modelParameter, snapshotParameter);
         }

         return formulation;
      }
   }
}