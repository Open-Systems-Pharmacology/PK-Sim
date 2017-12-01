using System.Threading.Tasks;
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

      public override Task<SnapshotFormulation> MapToSnapshot(ModelFormulation modelFormulation)
      {
         return SnapshotFrom(modelFormulation, snapshot =>
         {
            snapshot.FormulationType = modelFormulation.FormulationType;
         });
      }

      public override async Task<ModelFormulation> MapToModel(SnapshotFormulation snapshotFormulation)
      {
         var template = _formulationRepository.FormulationBy(snapshotFormulation.FormulationType);
         var formulation = _cloner.Clone(template);
         MapSnapshotPropertiesToModel(snapshotFormulation, formulation);
         await UpdateParametersFromSnapshot(snapshotFormulation, formulation);
         return formulation;
      }
   }
}