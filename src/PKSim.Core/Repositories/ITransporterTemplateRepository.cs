using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Collections;
using PKSim.Core.Model;

namespace PKSim.Core.Repositories
{
   public interface ITransporterTemplateRepository : IStartableRepository<TransporterTemplate>
   {
      /// <summary>
      ///    Returns the name of all transporters defined in the PKSim Database (template transporters and their synonyms)
      /// </summary>
      IReadOnlyList<string> AllTransporterNames { get; }

      /// <summary>
      ///    Returns the <see cref="TransportType" /> defined for a transporter named <paramref name="transporterName" />
      ///    and for the species named <paramref name="speciesName" /> or default if nothing is defined in the database (TransportType.Efflux)
      /// </summary>
      /// <param name="speciesName">Species where the transporter will be defined</param>
      /// <param name="transporterName">Transporter name (e.g. user input)</param>
      TransportType TransportTypeOrDefaultFor(string speciesName, string transporterName);

      /// <summary>
      ///    Returns true if a template was defined for a transporter named <paramref name="transporterName" /> for the species
      ///    named <paramref name="speciesName" />
      ///    otherwise false;
      /// </summary>
      bool HasTransporterTemplateFor(string speciesName, string transporterName);

      TransporterTemplate TransporterTemplateFor(string speciesName, string transporterName);
   }
} 