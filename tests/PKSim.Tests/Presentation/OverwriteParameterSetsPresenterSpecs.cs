using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Services;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Views.Compounds;

namespace PKSim.Presentation
{
   public abstract class concern_for_OverwriteParameterSetsPresenter : ContextSpecification<OverwriteParameterSetsPresenter>
   {
      protected IOverwriteParameterSetsView _view;
      protected IOverwriteParameterSetToOverwriteParameterSetDTOMapper _mapper;
      protected IOverwriteParameterSetTask _task;
      protected IDialogCreator _dialogCreator;

      protected override void Context()
      {
         _view = A.Fake<IOverwriteParameterSetsView>();
         _mapper = A.Fake<IOverwriteParameterSetToOverwriteParameterSetDTOMapper>();
         _task = A.Fake<IOverwriteParameterSetTask>();
         _dialogCreator = A.Fake<IDialogCreator>();
         sut = new OverwriteParameterSetsPresenter(_view, _mapper, _task, _dialogCreator);
         sut.InitializeWith(A.Fake<ICommandCollector>());
      }
   }

   public class When_editing_a_compound_with_overwrite_parameter_sets : concern_for_OverwriteParameterSetsPresenter
   {
      private Compound _compound;
      private OverwriteParameterSet _overwriteParameterSet;
      private OverwriteParameterSetDTO _dto;
      private IReadOnlyList<OverwriteParameterSetDTO> _boundDTOs;

      protected override void Context()
      {
         base.Context();
         _compound = new Compound();
         _overwriteParameterSet = new OverwriteParameterSet { Name = "TestSet" };
         _compound.AddOverwriteParameterSet(_overwriteParameterSet);

         _dto = A.Fake<OverwriteParameterSetDTO>();
         A.CallTo(() => _mapper.MapFrom(_overwriteParameterSet)).Returns(_dto);

         A.CallTo(() => _view.BindTo(A<IReadOnlyList<OverwriteParameterSetDTO>>._))
            .Invokes(call => _boundDTOs = call.GetArgument<IReadOnlyList<OverwriteParameterSetDTO>>(0));
      }

      protected override void Because()
      {
         sut.EditCompound(_compound);
      }

      [Observation]
      public void should_use_the_mapper_to_convert_each_overwrite_parameter_set()
      {
         A.CallTo(() => _mapper.MapFrom(_overwriteParameterSet)).MustHaveHappenedOnceExactly();
      }

      [Observation]
      public void should_bind_the_mapped_dtos_to_the_view()
      {
         _boundDTOs.ShouldNotBeNull();
         _boundDTOs.Count.ShouldBeEqualTo(1);
         _boundDTOs[0].ShouldBeEqualTo(_dto);
      }
   }

   public abstract class concern_for_OverwriteParameterSetsPresenter_editing : concern_for_OverwriteParameterSetsPresenter
   {
      protected Compound _compound;
      protected OverwriteParameterSet _overwriteParameterSet;
      protected ParameterValue _parameterValue;
      protected OverwriteParameterSetDTO _setDTO;
      protected OverwriteParameterValueDTO _valueDTO;
      protected const string _path = "Organism|Aspirin|Lipophilicity";

      protected override void Context()
      {
         base.Context();
         _compound = new Compound { Name = "Aspirin" };
         _overwriteParameterSet = new OverwriteParameterSet { Name = "MySet" };
         _parameterValue = new ParameterValue { Path = _path.ToObjectPath(), Value = 1.0 };
         _overwriteParameterSet.Add(_parameterValue);
         _compound.AddOverwriteParameterSet(_overwriteParameterSet);

         _setDTO = new OverwriteParameterSetDTO(_overwriteParameterSet);
         _valueDTO = new OverwriteParameterValueDTO(_parameterValue);

         sut.EditCompound(_compound);
      }
   }

   public class When_updating_a_parameter_value_through_the_presenter : concern_for_OverwriteParameterSetsPresenter_editing
   {
      private ICommand _command;

      protected override void Context()
      {
         base.Context();
         _command = A.Fake<ICommand>();
         A.CallTo(() => _task.UpdateParameterValue(_overwriteParameterSet, _compound, _path, 5.5)).Returns(_command);
      }

      protected override void Because()
      {
         sut.UpdateParameterValue(_setDTO, _valueDTO, 5.5);
      }

      [Observation]
      public void should_delegate_to_the_overwrite_parameter_set_task()
      {
         A.CallTo(() => _task.UpdateParameterValue(_overwriteParameterSet, _compound, _path, 5.5)).MustHaveHappenedOnceExactly();
      }
   }

   public class When_removing_a_parameter_value_through_the_presenter : concern_for_OverwriteParameterSetsPresenter_editing
   {
      private ICommand _command;

      protected override void Context()
      {
         base.Context();
         _command = A.Fake<ICommand>();
         A.CallTo(() => _task.RemoveParameterValue(_overwriteParameterSet, _compound, _path)).Returns(_command);
      }

      protected override void Because()
      {
         sut.RemoveParameterValue(_setDTO, _valueDTO);
      }

      [Observation]
      public void should_delegate_to_the_overwrite_parameter_set_task()
      {
         A.CallTo(() => _task.RemoveParameterValue(_overwriteParameterSet, _compound, _path)).MustHaveHappenedOnceExactly();
      }
   }

   public class When_setting_is_default_through_the_presenter : concern_for_OverwriteParameterSetsPresenter_editing
   {
      private ICommand _command;

      protected override void Context()
      {
         base.Context();
         _command = A.Fake<ICommand>();
         A.CallTo(() => _task.SetIsDefault(_overwriteParameterSet, _compound, true)).Returns(_command);
      }

      protected override void Because()
      {
         sut.SetIsDefault(_setDTO, true);
      }

      [Observation]
      public void should_delegate_to_the_overwrite_parameter_set_task()
      {
         A.CallTo(() => _task.SetIsDefault(_overwriteParameterSet, _compound, true)).MustHaveHappenedOnceExactly();
      }
   }

   public class When_clearing_is_default_through_the_presenter : concern_for_OverwriteParameterSetsPresenter_editing
   {
      private ICommand _command;

      protected override void Context()
      {
         base.Context();
         _command = A.Fake<ICommand>();
         A.CallTo(() => _task.SetIsDefault(_overwriteParameterSet, _compound, false)).Returns(_command);
      }

      protected override void Because()
      {
         sut.SetIsDefault(_setDTO, false);
      }

      [Observation]
      public void should_delegate_to_the_overwrite_parameter_set_task()
      {
         A.CallTo(() => _task.SetIsDefault(_overwriteParameterSet, _compound, false)).MustHaveHappenedOnceExactly();
      }
   }

   public class When_handling_an_overwrite_parameter_set_changed_event_for_the_edited_compound : concern_for_OverwriteParameterSetsPresenter_editing
   {
      protected override void Because()
      {
         Fake.ClearRecordedCalls(_view);
         sut.Handle(new OverwriteParameterSetChangedEvent(_compound, _overwriteParameterSet));
      }

      [Observation]
      public void should_rebind_the_view()
      {
         A.CallTo(() => _view.BindTo(A<IReadOnlyList<OverwriteParameterSetDTO>>._)).MustHaveHappenedOnceExactly();
      }
   }

   public class When_handling_an_overwrite_parameter_set_changed_event_for_another_compound : concern_for_OverwriteParameterSetsPresenter_editing
   {
      protected override void Because()
      {
         Fake.ClearRecordedCalls(_view);
         sut.Handle(new OverwriteParameterSetChangedEvent(new Compound { Name = "OtherCompound" }, _overwriteParameterSet));
      }

      [Observation]
      public void should_not_rebind_the_view()
      {
         A.CallTo(() => _view.BindTo(A<IReadOnlyList<OverwriteParameterSetDTO>>._)).MustNotHaveHappened();
      }
   }

   public class When_removing_a_set_through_the_presenter_and_the_user_confirms : concern_for_OverwriteParameterSetsPresenter_editing
   {
      private ICommand _command;

      protected override void Context()
      {
         base.Context();
         _command = A.Fake<ICommand>();
         A.CallTo(() => _dialogCreator.MessageBoxYesNo(A<string>._, ViewResult.Yes)).Returns(ViewResult.Yes);
         A.CallTo(() => _task.RemoveSet(_overwriteParameterSet, _compound)).Returns(_command);
      }

      protected override void Because()
      {
         sut.RemoveSet(_setDTO);
      }

      [Observation]
      public void should_ask_the_user_to_confirm()
      {
         A.CallTo(() => _dialogCreator.MessageBoxYesNo(A<string>._, ViewResult.Yes)).MustHaveHappened();
      }

      [Observation]
      public void should_delegate_to_the_overwrite_parameter_set_task()
      {
         A.CallTo(() => _task.RemoveSet(_overwriteParameterSet, _compound)).MustHaveHappenedOnceExactly();
      }
   }

   public class When_removing_a_set_through_the_presenter_and_the_user_cancels : concern_for_OverwriteParameterSetsPresenter_editing
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _dialogCreator.MessageBoxYesNo(A<string>._, ViewResult.Yes)).Returns(ViewResult.No);
      }

      protected override void Because()
      {
         sut.RemoveSet(_setDTO);
      }

      [Observation]
      public void should_not_delegate_to_the_task()
      {
         A.CallTo(() => _task.RemoveSet(A<OverwriteParameterSet>._, A<Compound>._)).MustNotHaveHappened();
      }
   }
}
