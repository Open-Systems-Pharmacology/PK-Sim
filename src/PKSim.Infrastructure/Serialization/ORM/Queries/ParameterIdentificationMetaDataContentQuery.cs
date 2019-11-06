using OSPSuite.Infrastructure.Serialization.ORM.MetaData;
using OSPSuite.Infrastructure.Serialization.Services;

namespace PKSim.Infrastructure.Serialization.ORM.Queries
{
   public interface IParameterIdentificationMetaDataContentQuery : IQuery<MetaDataContent, string>
   {
   }

   public class ParameterIdentificationMetaDataContentQuery : MetaDataContentQuery, IParameterIdentificationMetaDataContentQuery
   {
      public ParameterIdentificationMetaDataContentQuery(ISessionManager sessionManager)
         : base(sessionManager, "ParameterIdentificationMetaData")
      {
      }
   }
}