using System.Collections.Generic;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.DTO.Individuals
{
   public class IndividualTransporterDTO : ValidatableDTO<IndividualTransporter>
   {
      private readonly IndividualTransporter _individualTransporter;

      private readonly List <TransporterExpressionParameterDTO> _allExpressionParameters = new List<TransporterExpressionParameterDTO>();

      public IndividualTransporterDTO(IndividualTransporter individualTransporter) : base(individualTransporter)
      {
         _individualTransporter = individualTransporter;
      }

      public IReadOnlyList<TransporterExpressionParameterDTO> AllExpressionParameters => _allExpressionParameters;

      public void AddExpressionParameter(TransporterExpressionParameterDTO expressionParameterDTO)
      {
         _allExpressionParameters.Add(expressionParameterDTO);
      }

      public TransportType TransportType
      {
         get => _individualTransporter.TransportType;
         set => _individualTransporter.TransportType = value;
      }
   }
}