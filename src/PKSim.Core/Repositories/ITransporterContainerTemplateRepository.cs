using System.Collections.Generic;
using OSPSuite.Utility.Collections;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;

namespace PKSim.Core.Repositories
{
   public interface ITransporterContainerTemplateRepository : IStartableRepository<TransporterContainerTemplate>
   {
      /// <summary>
      /// Returns the default transporter template container defined for the concrete location <paramref name="containerName"/> and for a specific gene. 
      /// </summary>
      /// <param name="speciesName">Species where the transporter will be defined</param>
      /// <param name="containerName">Concrete transporter location (e.g Kidney, Liver etc...)</param>
      /// <param name="geneName">Specific gene for which templates should be returned</param>
      TransporterContainerTemplate TransporterContainerTemplateFor(string speciesName, string containerName, string geneName);

   }
}