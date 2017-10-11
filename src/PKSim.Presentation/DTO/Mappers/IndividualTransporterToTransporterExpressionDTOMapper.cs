using OSPSuite.Utility;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Individuals;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface IIndividualTransporterToTransporterExpressionDTOMapper : IMapper<IndividualTransporter, TransporterExpressionDTO>
   {
   }

   public class IndividualTransporterToTransporterExpressionDTOMapper : IIndividualTransporterToTransporterExpressionDTOMapper
   {
      private readonly IExpressionContainerDTOUpdater _expressionContainerDTOUpdater;

      public IndividualTransporterToTransporterExpressionDTOMapper(IExpressionContainerDTOUpdater expressionContainerDTOUpdater)
      {
         _expressionContainerDTOUpdater = expressionContainerDTOUpdater;
      }

      public TransporterExpressionDTO MapFrom(IndividualTransporter transporter)
      {
         var transporterExpressionDTO = new TransporterExpressionDTO(transporter);

         foreach (var transporterExpressionContainer in transporter.AllExpressionsContainers())
         {
            addContainerExpression(transporterExpressionDTO, transporter, transporterExpressionContainer);
         }
         return transporterExpressionDTO;
      }

      private void addContainerExpression(TransporterExpressionDTO proteinExpressionDTO, IndividualTransporter transporter, TransporterExpressionContainer transporterExpressionContainer)
      {
         var expressionDTO = new TransporterExpressionContainerDTO(transporterExpressionContainer) {MoleculeName = transporter.Name, ContainerName = transporterExpressionContainer.Name};
         _expressionContainerDTOUpdater.UpdateProperties(expressionDTO, transporterExpressionContainer);
         proteinExpressionDTO.AddProteinExpression(expressionDTO);
      }
   }
}