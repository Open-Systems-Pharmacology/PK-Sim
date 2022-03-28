using System.Collections.Generic;
using OSPSuite.Presentation.DTO;
using PKSim.Core.Model;

namespace PKSim.Presentation.DTO.Individuals
{
   public class IndividualTransporterDTO : ValidatableDTO<IndividualTransporter>
   {
      private readonly List<TransporterExpressionParameterDTO> _allExpressionParameters = new List<TransporterExpressionParameterDTO>();
      public IndividualTransporter Transporter { get; }

      public IndividualTransporterDTO(IndividualTransporter individualTransporter) : base(individualTransporter)
      {
         Transporter = individualTransporter;
      }

      public IReadOnlyList<TransporterExpressionParameterDTO> AllExpressionParameters => _allExpressionParameters;

      public void AddExpressionParameter(TransporterExpressionParameterDTO expressionParameterDTO)
      {
         _allExpressionParameters.Add(expressionParameterDTO);
      }

      public TransportTypeDTO TransportType
      {
         get => TransportTypes.By(Transporter.TransportType);
         set => Transporter.TransportType = value.TransportType;
      }

      /// <summary>
      /// Returns true if some information could be ready from the database otherwise false
      /// </summary>
      public bool DefaultAvailableInDatabase { get; set; }

      public void ClearExpressionParameters() => _allExpressionParameters.Clear();
   }
}