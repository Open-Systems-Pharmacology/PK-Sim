using OSPSuite.Infrastructure.Serialization.ORM.MetaData;
using OSPSuite.Infrastructure.Services;

namespace PKSim.Infrastructure.Serialization.ORM.Queries
{
   public abstract class MetaDataContentQuery : IQuery<MetaDataContent, string>
   {
      private readonly ISessionManager _sessionManager;
      private readonly string _mappingObjectName;

      protected MetaDataContentQuery(ISessionManager sessionManager, string mappingObjectName)
      {
         _sessionManager = sessionManager;
         _mappingObjectName = mappingObjectName;
      }

      public MetaDataContent ResultFor(string objectId)
      {
         MetaDataContent content;
         using (var session = _sessionManager.OpenSession())
         {
            content = session.CreateQuery($"select metaData.Content from {_mappingObjectName} metaData where metaData.Id =:id")
               .SetString("id", objectId)
               .UniqueResult<MetaDataContent>();
         }
         return content;
      }
   }
}