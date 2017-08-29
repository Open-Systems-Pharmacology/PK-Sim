using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Utility;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

namespace PKSim.Core.Batch.Mapper
{
   internal interface IFormulationMapper : IMapper<Formulation, Model.Formulation>
   {
   }

   internal class FormulationMapper : IFormulationMapper
   {
      private readonly IFormulationRepository _formulationRepository;
      private readonly ICloner _cloner;
      private readonly ILogger _batchLogger;

      public FormulationMapper(IFormulationRepository formulationRepository, ICloner cloner, ILogger batchLogger)
      {
         _formulationRepository = formulationRepository;
         _cloner = cloner;
         _batchLogger = batchLogger;
      }

      public Model.Formulation MapFrom(Formulation batchFormulation)
      {
         if (batchFormulation == null)
            return null;

         var template = _formulationRepository.FormulationBy(batchFormulation.FormulationType);
         var formulation = _cloner.Clone(template);

         formulation.Name = batchFormulation.Name;
         foreach (var parameterValue in batchFormulation.Parameters)
         {
            var parameter = formulation.Parameter(parameterValue.Key);
            if (parameter == null)
            {
               _batchLogger.AddWarning($"Parameter '{parameterValue.Key}' not found in formulation '{formulation.Name}'");
               continue;
            }

            parameter.Value = parameterValue.Value;
         }

         return formulation;
      }
   }
}