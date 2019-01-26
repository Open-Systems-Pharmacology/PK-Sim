using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core;
using PKSim.Core.Events;
using PKSim.Presentation.Presenters.Main;
using PKSim.Presentation.Regions;
using OSPSuite.Core;
using OSPSuite.Core.Commands;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Presenters.Commands;
using OSPSuite.Presentation.Regions;
using IWorkspace = PKSim.Presentation.Core.IWorkspace;

namespace PKSim.Presentation
{
   public abstract class concern_for_HistoryPresenter : ContextSpecification<IHistoryPresenter>
   {
      protected IHistoryBrowserPresenter _historyBrowserPresenter;
      protected IRegion _region;
      protected IWorkspace _workspace;
      private IRegionResolver _regionResolver;
      protected IExecutionContext _executionContext;
      private IApplicationConfiguration _configuration;

      protected override void Context()
      {
         _region = A.Fake<IRegion>();
         _historyBrowserPresenter = A.Fake<IHistoryBrowserPresenter>();
         _workspace = A.Fake<IWorkspace>();
         _regionResolver = A.Fake<IRegionResolver>();
         _executionContext = A.Fake<IExecutionContext>();
         _configuration= A.Fake<IApplicationConfiguration>();
         A.CallTo(() => _regionResolver.RegionWithName(RegionNames.History)).Returns(_region);

         sut = new HistoryPresenter(_historyBrowserPresenter, _workspace, _regionResolver, _executionContext, _configuration);
      }
   }

   public class When_the_history_presenter_is_told_to_initialize : concern_for_HistoryPresenter
   {
      protected override void Because()
      {
         sut.Initialize();
      }

      [Observation]
      public void should_fill_the_history_region_with_the_history_browser_view()
      {
         A.CallTo(() => _region.Add(_historyBrowserPresenter.View)).MustHaveHappened();
      }
   }

   public class When_calling_the_initialize_method_twice_on_the_history_presenter : concern_for_HistoryPresenter
   {
      protected override void Context()
      {
         base.Context();
         sut.Initialize();
      }

      protected override void Because()
      {
         sut.Initialize();
      }

      [Observation]
      public void should_not_initialize_the_history_browser_presenter_twice()
      {
         A.CallTo(() => _historyBrowserPresenter.Initialize()).MustHaveHappenedOnceExactly();
      }
   }

   public class When_the_history_presenter_is_being_notified_than_an_object_was_converted : concern_for_HistoryPresenter
   {
      private IObjectBase _convertedObject;
      private OSPSuiteInfoCommand _command;

      protected override void Context()
      {
         base.Context();
         _convertedObject = A.Fake<IObjectBase>().WithName("BB");
         A.CallTo(() => _workspace.AddCommand(A<OSPSuiteInfoCommand>._))
            .Invokes(x => _command = x.GetArgument<OSPSuiteInfoCommand>(0));

         A.CallTo(() => _executionContext.TypeFor(_convertedObject)).Returns("TYPE");

      }

      protected override void Because()
      {
         sut.Handle(new ObjectBaseConvertedEvent(_convertedObject, ProjectVersions.V5_3_2));
      }

      [Observation]
      public void should_add_an_info_command_to_the_history_showing_the_conversion_info()
      {
         _command.ShouldNotBeNull();
         _command.ObjectType.ShouldBeEqualTo("TYPE");
         _command.BuildingBlockType.ShouldBeEqualTo("TYPE");
         _command.BuildingBlockName.ShouldBeEqualTo("BB");
      }
   }
}