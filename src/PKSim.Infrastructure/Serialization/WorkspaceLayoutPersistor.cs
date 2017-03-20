using OSPSuite.Utility.Extensions;
using NHibernate;
using PKSim.Infrastructure.Serialization.ORM.Mappers;
using PKSim.Infrastructure.Serialization.ORM.MetaData;
using OSPSuite.Infrastructure.Serialization;
using OSPSuite.Presentation.Core;

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

         //first remove old layout if existing
         var layoutsFromDb = session.CreateCriteria<WorkspaceLayoutMetaData>().List<WorkspaceLayoutMetaData>();
         layoutsFromDb.Each(session.Delete);

         var workspaceLayoutMetaData = _workspaceLayoutMetaDataMapper.MapFrom(workspaceLayout);
         session.Save(workspaceLayoutMetaData);
      }

      public IWorkspaceLayout Load(ISession session)
      {
         var layoutsFromDb = session.CreateCriteria<WorkspaceLayoutMetaData>().List<WorkspaceLayoutMetaData>();
         if (layoutsFromDb.Count == 0) return null;
         var layoutFromDb = layoutsFromDb[0];
         return _workspaceLayoutMapper.MapFrom(layoutFromDb);
      }
   }
}