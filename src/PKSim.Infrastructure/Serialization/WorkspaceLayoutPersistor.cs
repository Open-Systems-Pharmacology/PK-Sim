using System.Linq;
using NHibernate;
using OSPSuite.Infrastructure.Serialization;
using OSPSuite.Presentation.Core;
using PKSim.Infrastructure.Serialization.ORM.Mappers;
using PKSim.Infrastructure.Serialization.ORM.MetaData;

namespace PKSim.Infrastructure.Serialization
{
   public interface IWorkspaceLayoutPersistor : ISessionPersistor<IWorkspaceLayout>
   {
   }

   public class WorkspaceLayoutPersistor : IWorkspaceLayoutPersistor
   {
      private readonly IWorkspaceLayoutToWorkspaceLayoutMetaDataMapper _workspaceLayoutMetaDataMapper;
      private readonly IWorkspaceLayoutMetaDataToWorkspaceLayoutMapper _workspaceLayoutMapper;

      public WorkspaceLayoutPersistor(IWorkspaceLayoutToWorkspaceLayoutMetaDataMapper workspaceLayoutMetaDataMapper,
         IWorkspaceLayoutMetaDataToWorkspaceLayoutMapper workspaceLayoutMapper)
      {
         _workspaceLayoutMetaDataMapper = workspaceLayoutMetaDataMapper;
         _workspaceLayoutMapper = workspaceLayoutMapper;
      }

      public void Save(IWorkspaceLayout workspaceLayout, ISession session)
      {
         if (workspaceLayout == null)
            return;

         var layoutFromDb = loadLayoutFromDb(session);

         var workspaceLayoutMetaData = _workspaceLayoutMetaDataMapper.MapFrom(workspaceLayout);
         if (layoutFromDb != null)
            layoutFromDb.UpdateFrom(workspaceLayoutMetaData, session);
         else
            layoutFromDb = workspaceLayoutMetaData;

         session.Save(layoutFromDb);
      }

      public IWorkspaceLayout Load(ISession session)
      {
         var layoutFromDb = loadLayoutFromDb(session);
         return layoutFromDb == null ? null : _workspaceLayoutMapper.MapFrom(layoutFromDb);
      }

      private static WorkspaceLayoutMetaData loadLayoutFromDb(ISession session)
      {
         var layoutsFromDb = session.CreateCriteria<WorkspaceLayoutMetaData>().List<WorkspaceLayoutMetaData>();
         return layoutsFromDb.FirstOrDefault();
      }
   }
}