using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Individuals;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface IIndividualProteinToIndividualProteinDTOMapper
   {
      IndividualProteinDTO MapFrom(IndividualProtein protein, ISimulationSubject simulationSubject);
   }

   public class IndividualProteinToIndividualProteinDTOMapper : IIndividualProteinToIndividualProteinDTOMapper
   {
      private readonly IExpressionParameterMapper<ExpressionParameterDTO> _expressionContainerMapper;

      public IndividualProteinToIndividualProteinDTOMapper(IExpressionParameterMapper<ExpressionParameterDTO> expressionContainerMapper)
      {
         _expressionContainerMapper = expressionContainerMapper;
      }

      public IndividualProteinDTO MapFrom(IndividualProtein protein, ISimulationSubject simulationSubject)
      {
         var dto = new IndividualProteinDTO(protein);
         simulationSubject.AllMoleculeContainersFor(protein)
            .SelectMany(x=>x.AllParameters())
            .Union(protein.AllGlobalExpressionParameters)
            .MapAllUsing(_expressionContainerMapper)
            .Each(dto.AddExpressionParameter);

         return dto;
      }

   }
}