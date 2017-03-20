using OSPSuite.Utility;
using PKSim.Core.Services;
using PKSim.Infrastructure.Serialization.ORM.MetaData;
using OSPSuite.Presentation.Core;

namespace PKSim.Infrastructure.Serialization.ORM.Mappers
{
   public interface IWorkspaceLayoutMetaDataToWorkspaceLayoutMapper : IMapper<WorkspaceLayoutMetaData, IWorkspaceLayout>
   {
   }

   public class WorkspaceLayoutMetaDataToWorkspaceLayoutMapper : IWorkspaceLayoutMetaDataToWorkspaceLayoutMapper
   {
      private readonly ICompressedSerializationManager _serializationManager;

      public WorkspaceLayoutMetaDataToWorkspaceLayoutMapper(ICompressedSerializationManager serializationManager)
      {
         _serializationManager = serializationManager;
      }

      public IWorkspaceLayout MapFrom(WorkspaceLayoutMetaData workspaceLayoutMetaData)
      {
         return _serializationManager.Deserialize<IWorkspaceLayout>(workspaceLayoutMetaData.Content.Data);
      }
   }
}