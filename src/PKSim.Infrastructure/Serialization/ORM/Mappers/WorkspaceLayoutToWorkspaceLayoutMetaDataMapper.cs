using PKSim.Core.Services;
using PKSim.Infrastructure.Serialization.ORM.MetaData;
using OSPSuite.Presentation.Core;
using OSPSuite.Utility;

namespace PKSim.Infrastructure.Serialization.ORM.Mappers
{
   public interface IWorkspaceLayoutToWorkspaceLayoutMetaDataMapper : IMapper<IWorkspaceLayout, WorkspaceLayoutMetaData>
   {
   }

   public class WorkspaceLayoutToWorkspaceLayoutMetaDataMapper : IWorkspaceLayoutToWorkspaceLayoutMetaDataMapper
   {
      private readonly ICompressedSerializationManager _serializationManager;

      public WorkspaceLayoutToWorkspaceLayoutMetaDataMapper(ICompressedSerializationManager serializationManager)
      {
         _serializationManager = serializationManager;
      }

      public WorkspaceLayoutMetaData MapFrom(IWorkspaceLayout workspaceLayout)
      {
         var workspaceLayoutMetaData = new WorkspaceLayoutMetaData();
         workspaceLayoutMetaData.Content.Data = _serializationManager.Serialize(workspaceLayout);
         return workspaceLayoutMetaData;
      }
   }
}