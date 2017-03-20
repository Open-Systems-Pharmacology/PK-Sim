using System.Collections.Generic;

namespace PKSim.Core.Model
{
   public interface ITransporterContainer
   {
      /// <summary>
      ///    Internal process names that will be created in the simulation when this transporter is selected
      /// </summary>
      IEnumerable<string> ProcessNames { get; }

      /// <summary>
      ///    Where in the compartment does the transporter sit?
      /// </summary>
      MembraneLocation MembraneLocation { get; set; }

      void AddProcessName(string processName);

      void ClearProcessNames();
   }
}