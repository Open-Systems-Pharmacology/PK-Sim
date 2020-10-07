using OSPSuite.Utility;
using PKSim.Core.Model;

using PKSim.Presentation.DTO.Individuals;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface IIndividualProteinToProteinExpressionDTOMapper : IMapper<IndividualProtein, ProteinExpressionDTO>
   {
   }

   public class IndividualProteinToProteinExpressionDTOMapper : IIndividualProteinToProteinExpressionDTOMapper
   {
      private readonly IExpressionContainerMapper _expressionContainerMapper;

      public IndividualProteinToProteinExpressionDTOMapper(IExpressionContainerMapper expressionContainerMapper)
      {
         _expressionContainerMapper = expressionContainerMapper;
      }

      public ProteinExpressionDTO MapFrom(IndividualProtein protein)
      {
         var proteinExpressionDTO = new ProteinExpressionDTO(protein);

         foreach (var enzymeExpressionContainer in protein.AllExpressionsContainers())
         {
            addContainerExpression(proteinExpressionDTO, protein, enzymeExpressionContainer);
         }
         return proteinExpressionDTO;
      }

      private void addContainerExpression(ProteinExpressionDTO proteinExpressionDTO, IndividualProtein protein, MoleculeExpressionContainer moleculeExpressionContainer)
      {
         var expressionDTO = new ExpressionContainerDTO {MoleculeName = protein.Name, ContainerName = moleculeExpressionContainer.Name};
         _expressionContainerMapper.UpdateProperties(expressionDTO, moleculeExpressionContainer);
         proteinExpressionDTO.AddProteinExpression(expressionDTO);
      }
   }
}