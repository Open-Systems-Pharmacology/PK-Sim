using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;

namespace PKSim.Presentation.DTO.Populations
{
   public class AdvancedParameterDTO
   {
      /// <summary>
      ///    Display path of the parameter according to its position in the hierarchy
      /// </summary>
      public string ParameterFullDisplayName { get; set; }

      /// <summary>
      ///    Type of distribution
      /// </summary>
      public DistributionType DistributionType { get; set; }

      /// <summary>
      ///    Parameters of distribution
      /// </summary>
      public IEnumerable<IParameter> Parameters { get; set; }
   }
}