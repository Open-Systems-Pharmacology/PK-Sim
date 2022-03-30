using System.Collections.Generic;
using OSPSuite.Utility.Collections;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;

namespace PKSim.Core.Repositories
{
   public interface ITransporterContainerTemplateRepository : IStartableRepository<TransporterContainerTemplate>
   {
      /// <summary>
      /// Returns the default <see cref="TransportType"/> defined for a transporter named <paramref name="transporterName"/> 
      /// and for the species named <paramref name="speciesName"/>
      /// </summary>
      /// <param name="speciesName">Species where the transporter will be defined</param>
      /// <param name="transporterName">Transporter name (e.g. user input)</param>
      TransportType TransportTypeFor(string speciesName, string transporterName);

      /// <summary>
      /// Returns true if a template was defined for a transporter named <paramref name="transporterName"/> for the species named <paramref name="speciesName"/> 
      /// otherwise false;
      /// </summary>
      bool HasTransporterTemplateFor(string speciesName, string transporterName);

      /// <summary>
      /// Returns the default transporter templates defined for the concrete location <paramref name="containerName"/> and for a specific gene. 
      /// </summary>
      /// <param name="speciesName">Species where the transporter will be defined</param>
      /// <param name="containerName">Concrete transporter location (e.g Kidney, Liver etc...)</param>
      /// <param name="geneName">Specific gene for which templates should be returned</param>
      IEnumerable<TransporterContainerTemplate> TransportersFor(string speciesName, string containerName, string geneName);


      /// <summary>
      /// Returns the default transporter templates defined for the concrete location <paramref name="containerName"/> 
      /// </summary>
      /// <param name="speciesName">Species where the transporter will be defined</param>
      /// <param name="containerName">Concrete transporter location (e.g Kidney, Liver etc...)</param>
      IEnumerable<TransporterContainerTemplate> TransportersFor(string speciesName, string containerName);

      /// <summary>
      /// Returns the name of all transporters defined in the PKSim Database (template transporters and their synonyms)
      /// </summary>
      IReadOnlyList<string> AllTransporterNames { get; }
   }
}