using System.Collections.Generic;
using FakeItEasy;
using NHibernate;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Presentation.Core;
using PKSim.Infrastructure.Serialization;
using PKSim.Infrastructure.Serialization.ORM.Mappers;
using PKSim.Infrastructure.Serialization.ORM.MetaData;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_WorkspaceLayoutPersistor : ContextSpecification<IWorkspaceLayoutPersistor>
   {
      protected IWorkspaceLayoutToWorkspaceLayoutMetaDataMapper _workspaceLayoutMetaDataMapper;
      protected IWorkspaceLayoutMetaDataToWorkspaceLayoutMapper _workspaceLayoutMapper;
      protected IWorkspaceLayout _workspaceLayout;
      protected ISession _session;
      protected WorkspaceLayoutMetaData _workspaceLayoutMetaData;
      protected WorkspaceLayoutMetaData _dbWorkspaceLayoutMetaData;
      protected ICriteria _workspaceLayoutMetaDataCriteria;

      protected override void Context()
      {
         _workspaceLayoutMetaDataMapper = A.Fake<IWorkspaceLayoutToWorkspaceLayoutMetaDataMapper>();
         _workspaceLayoutMapper = A.Fake<IWorkspaceLayoutMetaDataToWorkspaceLayoutMapper>();
         sut = new WorkspaceLayoutPersistor(_workspaceLayoutMetaDataMapper, _workspaceLayoutMapper);

         _workspaceLayout = A.Fake<IWorkspaceLayout>();
         _session = A.Fake<ISession>();
         _workspaceLayoutMetaDataCriteria = A.Fake<ICriteria>();
         _workspaceLayoutMetaData = new WorkspaceLayoutMetaData();
         _workspaceLayoutMetaData.Content.Data = new byte[] {125, 14};
         _dbWorkspaceLayoutMetaData = new WorkspaceLayoutMetaData();

         A.CallTo(() => _session.CreateCriteria<WorkspaceLayoutMetaData>()).Returns(_workspaceLayoutMetaDataCriteria);
         A.CallTo(() => _workspaceLayoutMetaDataMapper.MapFrom(_workspaceLayout)).Returns(_workspaceLayoutMetaData);
      }
   }

   public class When_saving_the_workspace_layout_to_the_database_when_no_previous_layout_were_defined : concern_for_WorkspaceLayoutPersistor
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(_workspaceLayoutMetaDataCriteria).WithReturnType<IList<WorkspaceLayoutMetaData>>().Returns(new List<WorkspaceLayoutMetaData>());
      }

      protected override void Because()
      {
         sut.Save(_workspaceLayout, _session);
      }

      [Observation]
      public void should_simply_save_the_new_layout()
      {
         A.CallTo(() => _session.Save(_workspaceLayoutMetaData)).MustHaveHappened();
      }
   }

   public class When_saving_the_workspace_layout_to_the_database_when_previous_layout_were_defined : concern_for_WorkspaceLayoutPersistor
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(_workspaceLayoutMetaDataCriteria).WithReturnType<IList<WorkspaceLayoutMetaData>>().Returns(new[] {_dbWorkspaceLayoutMetaData,});
      }

      protected override void Because()
      {
         sut.Save(_workspaceLayout, _session);
      }

      [Observation]
      public void should_uddate_the_existing_layout()
      {
         _dbWorkspaceLayoutMetaData.Content.Data.ShouldBeEqualTo(_workspaceLayoutMetaData.Content.Data);
         A.CallTo(() => _session.Save(_dbWorkspaceLayoutMetaData)).MustHaveHappened();
      }
   }
}