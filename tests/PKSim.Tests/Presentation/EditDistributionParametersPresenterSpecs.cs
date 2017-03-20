using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility.Events;
using PKSim.Presentation.DTO.Parameters;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Presenters.Populations;
using PKSim.Presentation.Views.Parameters;
using OSPSuite.Core.Domain.UnitSystem;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;



using PKSim.Presentation.Services;

using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation
{
   public abstract class concern_for_EditDistributionParametersPresenter : ContextSpecification<IEditDistributionParametersPresenter>
   {
      private IParameterToParameterDTOMapper _parameterMapper;
      protected IMultiParameterEditView _view;
      private IScaleParametersPresenter _scaleParametersPresenter;
      protected IParameterTask _parameterTask;
      protected ICommandCollector _commandRegister;
      protected IParameterContextMenuFactory _contextMenuFactory;
      protected IEditParameterPresenterTask _editParameterPresenterTask;

      protected override void Context()
      {
         _parameterMapper =A.Fake<IParameterToParameterDTOMapper>();
         _view = A.Fake<IMultiParameterEditView>();
         _scaleParametersPresenter =A.Fake<IScaleParametersPresenter>();
         _parameterTask =A.Fake<IParameterTask>();
         _commandRegister = A.Fake<ICommandCollector>();
         _contextMenuFactory = A.Fake<IParameterContextMenuFactory>();
         _editParameterPresenterTask =A.Fake<IEditParameterPresenterTask>();
         sut = new EditDistributionParametersPresenter(_view, _scaleParametersPresenter, _editParameterPresenterTask, _parameterTask, _parameterMapper, _contextMenuFactory);
         sut.InitializeWith(_commandRegister);
      }
   }

   
 
   
   public class When_a_value_is_being_edited_for_a_distribution_parameter : concern_for_EditDistributionParametersPresenter
   {
      private ParameterDTO _parameterDTO;
      private double _valueInGuiUnit;
      private IPKSimCommand _command;

      protected override void Context()
      {
         base.Context();
         _valueInGuiUnit = 20;
         _command =A.Fake<IPKSimCommand>();
         _parameterDTO =A.Fake<ParameterDTO>();
         A.CallTo(() => _parameterDTO.Parameter).Returns(A.Fake<IParameter>());
         A.CallTo(() => _parameterTask.SetAdvancedParameterDisplayValue(_parameterDTO.Parameter,_valueInGuiUnit)).Returns(_command);
      }
      protected override void Because()
      {
         sut.SetParameterValue(_parameterDTO,_valueInGuiUnit);
      }

      [Observation]
      public void should_set_the_value_for_the_advanced_parameter()
      {
         A.CallTo(() => _parameterTask.SetAdvancedParameterDisplayValue(_parameterDTO.Parameter, _valueInGuiUnit)).MustHaveHappened();
      }

      [Observation]
      public void should_register_the_resulting_command()
      {
         A.CallTo(() => _commandRegister.AddCommand(_command)).MustHaveHappened();
      }

   }

   
   public class When_a_unit_is_being_edited_for_a_distribution_parameter : concern_for_EditDistributionParametersPresenter
   {
      private ParameterDTO _parameterDTO;
      private Unit _unit;
      private IPKSimCommand _command;

      protected override void Context()
      {
         base.Context();
         _unit = A.Fake<Unit>();
         _parameterDTO = A.Fake<ParameterDTO>();
         _command =A.Fake<IPKSimCommand>();
         A.CallTo(() => _parameterDTO.Parameter).Returns(A.Fake<IParameter>());
         A.CallTo(() => _parameterTask.SetAdvancedParameterUnit(_parameterDTO.Parameter, _unit)).Returns(_command);
      }
      protected override void Because()
      {
         sut.SetParameterUnit(_parameterDTO, _unit);
      }
      [Observation]
      public void should_set_the_value_for_the_advanced_parameter()
      {
         A.CallTo(() => _parameterTask.SetAdvancedParameterUnit(_parameterDTO.Parameter, _unit)).MustHaveHappened();
      }

      [Observation]
      public void should_register_the_resulting_command()
      {
         A.CallTo(() => _commandRegister.AddCommand(_command)).MustHaveHappened();
      }

   }
}	