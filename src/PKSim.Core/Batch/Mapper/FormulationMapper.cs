﻿using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;

namespace PKSim.Core.Batch.Mapper
{
   public interface IFormulationMapper : IMapper<Formulation, Model.Formulation>
   {
   }

   public class FormulationMapper : IFormulationMapper
   {
      private readonly IFormulationRepository _formulationRepository;
      private readonly ICloner _cloner;
      private readonly IBatchLogger _batchLogger;

      public FormulationMapper(IFormulationRepository formulationRepository, ICloner cloner, IBatchLogger batchLogger)
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
            _batchLogger.AddParameterValueToDebug(parameter);
         }

         return formulation;
      }
   }
}