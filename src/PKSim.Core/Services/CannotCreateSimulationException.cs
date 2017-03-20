using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Extensions;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Services
{
   public class CannotCreateSimulationException : PKSimException
   {
      public CannotCreateSimulationException(ValidationResult validationResult)
         : base(validationResult.Messages.Select(x => new List<string>(x.Details) {x.Text}.ToString("\n")).ToString("\n"))
      {
      }
   }
}