using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using PKSim.Assets;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Parameters;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Parameters;
using System.Collections.Generic;

namespace PKSim.Presentation
{
   public abstract class concern_for_HalogensPresenter : ContextSpecification<HalogensPresenter>
   {
      protected IMultiParameterEditView _view;
      protected IScaleParametersPresenter _scaleParametersPresenter;
      protected IEditParameterPresenterTask _editParameterPresenterTask;
      protected IParameterTask _parameterTask;
      protected IParameterToParameterDTOMapper _parameterDTOMapper;
      protected IParameterContextMenuFactory _contextMenuFactory;
      protected IDialogCreator _dialogCreator;

      protected IParameter _parameter;
      protected ParameterDTO _parameterDTO;
      protected IParameter _effectiveMolWeight;
      protected IReadOnlyList<IParameter> _halogens;

      protected override void Context()
      {
         _view = A.Fake<IMultiParameterEditView>();
         _scaleParametersPresenter = A.Fake<IScaleParametersPresenter>();
         _editParameterPresenterTask = A.Fake<IEditParameterPresenterTask>();
         _parameterTask = A.Fake<IParameterTask>();
         _parameterDTOMapper = A.Fake<IParameterToParameterDTOMapper>();
         _contextMenuFactory = A.Fake<IParameterContextMenuFactory>();
         _dialogCreator = A.Fake<IDialogCreator>();

         A.CallTo(() => _scaleParametersPresenter.View).Returns(A.Fake<IScaleParametersView>());

         _parameter = new Parameter();
         _parameterDTO = new ParameterDTO(_parameter);

         _effectiveMolWeight = A.Fake<IParameter>();

         A.CallTo(() => _parameterDTOMapper.MapFrom(A<IParameter>._))
            .ReturnsLazily(x => new ParameterDTO(x.GetArgument<IParameter>(0)));

         sut = new HalogensPresenter(
            _view,
            _scaleParametersPresenter,
            _editParameterPresenterTask,
            _parameterTask,
            _parameterDTOMapper,
            _contextMenuFactory,
            _dialogCreator);

         _halogens = new List<IParameter> { _parameter };
      }
   }

   public class When_editing_halogens : concern_for_HalogensPresenter
   {
      protected override void Because()
      {
         sut.Edit(_halogens, _effectiveMolWeight);
      }

      [Observation]
      public void should_display_the_parameters_in_the_underlying_view()
      {
         A.CallTo(() => _view.BindTo(A<IEnumerable<ParameterDTO>>._)).MustHaveHappened();
      }
   }

   public class When_setting_a_value_that_makes_effective_mol_weight_negative : concern_for_HalogensPresenter
   {
      private double _valueInDisplayUnits;

      protected override void Context()
      {
         base.Context();

         _valueInDisplayUnits = 5;
         A.CallTo(() => _effectiveMolWeight.Value).Returns(-0.1);
         sut.Edit(_halogens, _effectiveMolWeight);
      }

      protected override void Because()
      {
         sut.SetParameterValue(_parameterDTO, _valueInDisplayUnits);
      }

      [Observation]
      public void should_show_an_error_message_indicating_effective_mol_weight_cannot_be_negative()
      {
         A.CallTo(() => _dialogCreator.MessageBoxError(PKSimConstants.Error.EffectiveMolWeightCannotBeNegative)).MustHaveHappened();
      }

      [Observation]
      public void should_not_delegate_the_set_operation_to_the_edit_parameter_task()
      {
         A.CallTo(() => _editParameterPresenterTask.SetParameterValue(sut, _parameterDTO, _valueInDisplayUnits)).MustNotHaveHappened();
      }
   }

   public class When_setting_a_value_that_keeps_effective_mol_weight_positive : concern_for_HalogensPresenter
   {
      private double _valueInDisplayUnit;

      protected override void Context()
      {
         base.Context();

         _valueInDisplayUnit = 5;
         A.CallTo(() => _effectiveMolWeight.Value).Returns(0.1);
         
         sut.Edit(_halogens, _effectiveMolWeight);
      }

      protected override void Because()
      {
         sut.SetParameterValue(_parameterDTO, _valueInDisplayUnit);
      }

      [Observation]
      public void should_delegate_the_set_operation_to_the_edit_parameter_task()
      {
         A.CallTo(() => _editParameterPresenterTask.SetParameterValue(sut, _parameterDTO, _valueInDisplayUnit)).MustHaveHappened();
      }

      [Observation]
      public void should_not_show_an_error_message()
      {
         A.CallTo(() => _dialogCreator.MessageBoxError(A<string>._)).MustNotHaveHappened();
      }
   }
}