using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Views;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Presentation.UICommands;

namespace PKSim.Presentation
{
   public abstract class concern_for_RenameSubjectUICommand : ContextSpecification<RenameSubjectUICommand>
   {
      protected RenameBuildingBlockUICommand _renameBuildingBlockCommand;
      protected RenameObjectUICommand _renameObjectCommand;
      protected RenameObservedDataUICommand _renameObservedDataCommand;
      protected IMdiChildView _mdiChildView;
      protected ISingleStartPresenter _mdiChildPresenter;

      protected override void Context()
      {
         _renameBuildingBlockCommand= A.Fake<RenameBuildingBlockUICommand>();
         _renameObjectCommand= A.Fake<RenameObjectUICommand>();
         _renameObservedDataCommand= A.Fake<RenameObservedDataUICommand>();
         _mdiChildView= A.Fake<IMdiChildView>();
         _mdiChildPresenter= A.Fake<ISingleStartPresenter>();
         A.CallTo(() => _mdiChildView.Presenter).Returns(_mdiChildPresenter);
         sut = new RenameSubjectUICommand(_renameBuildingBlockCommand, _renameObservedDataCommand, _renameObjectCommand) {Subject = _mdiChildView};

      }
      protected override void Because()
      {
         sut.Execute();
      }

   }
   //TODO Uncomment those tests when Execute is made Virtual in OSPSuite

//   public class When_executing_the_rename_subject_command_for_a_building_block : concern_for_RenameSubjectUICommand
//   {
//      private IPKSimBuildingBlock _buildingBlock;
//
//      protected override void Context()
//      {
//         base.Context();
//         _buildingBlock= A.Fake<IPKSimBuildingBlock>();
//         A.CallTo(() => _mdiChildPresenter.Subject).Returns(_buildingBlock);
//      }
//
//      [Observation]
//      public void should_execute_the_rename_building_block_command()
//      {
//         A.CallTo(() => _renameBuildingBlockCommand.Execute()).MustHaveHappened();         
//      }
//   }
//
//   public class When_executing_the_rename_subject_command_for_some_observed_data : concern_for_RenameSubjectUICommand
//   {
//      private DataRepository _dataRepository;
//
//      protected override void Context()
//      {
//         base.Context();
//         _dataRepository = DomainHelperForSpecs.ObservedData();
//         A.CallTo(() => _mdiChildPresenter.Subject).Returns(_dataRepository);
//      }
//
//      [Observation]
//      public void should_execute_the_rename_observed_data_command()
//      {
//         A.CallTo(() => _renameObservedDataCommand.Execute()).MustHaveHappened();
//      }
//   }
//
//   public class When_executing_the_rename_subject_command_for_a_standard_component_with_nanem : concern_for_RenameSubjectUICommand
//   {
//      private IWithName _withName;
//
//      protected override void Context()
//      {
//         base.Context();
//         _withName = A.Fake<IWithName>();
//         A.CallTo(() => _mdiChildPresenter.Subject).Returns(_withName);
//      }
//
//      [Observation]
//      public void should_execute_the_rename_object_command()
//      {
//         A.CallTo(() => _renameObjectCommand.Execute()).MustHaveHappened();
//      }
//   }
}	