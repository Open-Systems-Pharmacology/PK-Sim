using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatProteinSynonymRepository : IMetaDataRepository<FlatProteinSynonym>
   {
      IReadOnlyList<string> AllSynonymsFor(string proteinName);
      void AddSynonymsTo(WithSynonyms withSynonyms);
   }

   public class FlatProteinSynonymRepository : MetaDataRepository<FlatProteinSynonym>, IFlatProteinSynonymRepository
   {
      public FlatProteinSynonymRepository(IDbGateway dbGateway, IDataTableToMetaDataMapper<FlatProteinSynonym> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.VIEW_PROTEIN_SYNONYMS)
      {
      }

      public IReadOnlyList<string> AllSynonymsFor(string proteinName)
      {
         Start();
         return AllElements().Where(x => string.Equals(x.ProteinName, proteinName))
            .Select(x => x.Synonym).ToList();
      }

      public void AddSynonymsTo(WithSynonyms withSynonyms)
      {
         AllSynonymsFor(withSynonyms.Name).Each(withSynonyms.AddSynonym);
      }
   }
}