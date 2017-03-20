using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Repositories
{
   public interface IFormulationKeyRepository : IRepository<string>
   {
   }

   public class FormulationKeyRepository : IFormulationKeyRepository
   {
      private readonly IProjectRetriever _projectRetriever;

      public FormulationKeyRepository(IProjectRetriever projectRetriever)
      {
         _projectRetriever = projectRetriever;
      }

      public IEnumerable<string> All()
      {
         return _projectRetriever.CurrentProject.DowncastTo<IPKSimProject>()
            .All<Protocol>()
            .Where(p => p.IsLoaded)
            .SelectMany(p => p.UsedFormulationKeys)
            .Where(key => !key.IsNullOrEmpty())
            .Distinct();
      }
   }
}