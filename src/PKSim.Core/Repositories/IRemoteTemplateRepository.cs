using System.Collections.Generic;
using System.Threading.Tasks;
using OSPSuite.Utility.Collections;
using PKSim.Core.Model;

namespace PKSim.Core.Repositories
{
   public interface IRemoteTemplateRepository : IStartableRepository<RemoteTemplate>
   {
      /// <summary>
      ///    Version of the OSP template library
      /// </summary>
      string Version { get; }

      IReadOnlyList<RemoteTemplate> AllTemplatesFor(TemplateType templateType);
      RemoteTemplate TemplateBy(TemplateType templateType, string name);
      Task<T> LoadTemplateAsync<T>(RemoteTemplate remoteTemplate);
      IReadOnlyList<RemoteTemplate> AllReferenceTemplatesFor<T>(RemoteTemplate remoteTemplate, T loadedTemplate);

      /// <summary>
      ///    Downloads the summary file of the OSP template library and reloads the repository
      /// </summary>
      Task UpdateLocalTemplateSummaryFile();

      /// <summary>
      ///    Downloads the summary file of every additional template source defined in the user settings without interacting
      ///    with the user and reloads the repository. Sources that cannot be reached keep their previously downloaded summary.
      /// </summary>
      Task UpdateTemplateSourcesAsync();

      /// <summary>
      ///    Downloads the summary file of the additional template sources that are not available yet (for instance because
      ///    credentials were missing) and reloads the repository. The user may be prompted for credentials.
      /// </summary>
      Task LoadUnavailableTemplateSourcesAsync();
   }
}
