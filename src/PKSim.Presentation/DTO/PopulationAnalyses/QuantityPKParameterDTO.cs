using OSPSuite.Core.Domain;

namespace PKSim.Presentation.DTO.PopulationAnalyses
{
   public class QuantityPKParameterDTO
   {
      public string DisplayName { get; set; }

      public string QuantityDisplayPath { get; set; }

      public string Description { get; set; }

      /// <summary>
      ///    The actual QuantityPKParameter
      /// </summary>
      public QuantityPKParameter PKParameter { get; set; }
   }
}