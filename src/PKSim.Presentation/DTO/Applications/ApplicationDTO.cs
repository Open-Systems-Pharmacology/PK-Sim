using System.Collections.Generic;
using System.Linq;
using PKSim.Presentation.DTO.Parameters;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.DTO.Applications
{
   public class ApplicationDTO
   {
      public string Name { get; set; }
      private readonly List<IParameterDTO> _parameters = new List<IParameterDTO>();
      public string Icon { get; set; }

      public double StartTime
      {
         get
         {
            var startTime = _parameters.FirstOrDefault(x => string.Equals(x.Name, Constants.Parameters.START_TIME));
            if (startTime == null) return 0;
            return startTime.Value;
         }
      }

      public List<IParameterDTO> Parameters
      {
         get { return _parameters; }
      }

      public void AddParameter(IParameterDTO parameterToAdd)
      {
         _parameters.Add(parameterToAdd);
      }
   }
}