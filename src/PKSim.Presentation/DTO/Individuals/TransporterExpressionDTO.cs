using System.Collections.Generic;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.DTO.Individuals
{
   public class TransporterExpressionDTO : ValidatableDTO<IndividualTransporter>
   {
      private readonly IndividualTransporter _individualTransporter;

      private readonly IList<TransporterExpressionContainerDTO> _allContainerExpressions;
      public double ProteinContent { get; set; }

      public TransporterExpressionDTO(IndividualTransporter individualTransporter) : base(individualTransporter)
      {
         _individualTransporter = individualTransporter;
         _allContainerExpressions = new List<TransporterExpressionContainerDTO>();
      }

      public IEnumerable<TransporterExpressionContainerDTO> AllContainerExpressions
      {
         get { return _allContainerExpressions; }
      }

      public void AddProteinExpression(TransporterExpressionContainerDTO proteinExpressionContainerDTO)
      {
         _allContainerExpressions.Add(proteinExpressionContainerDTO);
      }

      public TransportType TransportType
      {
         get { return _individualTransporter.TransportType; }
         set { _individualTransporter.TransportType = value; }
      }
   }
}