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
      IndividualProteinDTO MapFrom(ISimulationSubject simulationSubject, IndividualProtein individualProtein);
   }

   public class IndividualProteinToIndividualProteinDTOMapper : IIndividualProteinToIndividualProteinDTOMapper
   {
      private readonly IExpressionContainerMapper _expressionContainerMapper;

      public IndividualProteinToIndividualProteinDTOMapper(IExpressionContainerMapper expressionContainerMapper)
      {
         _expressionContainerMapper = expressionContainerMapper;
      }

      public IndividualProteinDTO MapFrom(ISimulationSubject simulationSubject, IndividualProtein individualProtein)
      {
         var dto = new IndividualProteinDTO(individualProtein);
         allExpressionContainersFor(simulationSubject, individualProtein)
            .SelectMany(x => expressionContainerParameterFrom(individualProtein, x))
            .Union(globalExpressionParametersFrom(individualProtein))
            .Each(dto.AddExpressionParameter);
         return dto;
      }

      private IEnumerable<ExpressionParameterDTO> globalExpressionParametersFrom(IndividualProtein individualProtein)
      {
         return individualProtein.AllParameters()
            .Except(new[] {individualProtein.ReferenceConcentration, individualProtein.HalfLifeLiver, individualProtein.HalfLifeIntestine, individualProtein.OntogenyFactorParameter, individualProtein.OntogenyFactorGIParameter})
            .Select(x => _expressionContainerMapper.MapFrom(individualProtein, x));
      }

      private IEnumerable<ExpressionParameterDTO> expressionContainerParameterFrom(
         IndividualProtein protein, MoleculeExpressionContainer moleculeExpressionContainer)
      {
         return moleculeExpressionContainer.AllParameters().Select(x => _expressionContainerMapper.MapFrom(moleculeExpressionContainer, protein, x));
      }

      private IReadOnlyList<MoleculeExpressionContainer> allExpressionContainersFor(ISimulationSubject simulationSubject,
         IndividualProtein individualProtein)
      {
         //TODO MOVE TO INDIVIDUAL
         //Also check with parent usage
         return simulationSubject.GetAllChildren<MoleculeExpressionContainer>(x => x.IsNamed(individualProtein.Name));
      }
   }
}