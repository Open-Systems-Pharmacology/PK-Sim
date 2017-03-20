using System.Collections.Generic;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.Presentation.DTO.Populations
{
   public class AdvancedParameterDTO
   {
      /// <summary>
      /// Display path of the parameter according to its position in the hiearchy
      /// </summary>
      public string ParameterFullDisplayName { get; set; }

      /// <summary>
      /// Type of distribution
      /// </summary>
      public DistributionType DistributionType { get; set; }

      /// <summary>
      /// Parameters of distribution
      /// </summary>
      public IEnumerable<IParameter> Parameters { get; set; }
   }
}