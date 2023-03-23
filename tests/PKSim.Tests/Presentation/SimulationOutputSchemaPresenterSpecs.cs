using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.Core.Commands.Core;
using FakeItEasy;
using PKSim.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Presentation.DTO;
using ISimulationSettingsTask = PKSim.Core.Services.ISimulationSettingsTask;

namespace PKSim.Presentation
{
   public abstract class concern_for_SimulationOutputSchemaPresenter : ContextSpecification<IEditOutputSchemaPresenter>
   {
      protected IEditOutputSchemaView _view;
      protected IOutputIntervalToOutputIntervalDTOMapper _outputIntervalToOutputIntervalDTOMapper;
      protected ISimulationSettingsTask _simulationSettingsTask;
      protected IndividualSimulation _simulationToEdit;
      protected OutputSchema _outputSchema;
      protected IList<OutputInterval> _allIntervals;
      protected ICommandCollector _commandRegister;
      private IEditParameterPresenterTask _editParameterPresenterTask;
      private SimulationSettings _simulationSetting;
      private IExecutionContext _context;

      protected override void Context()
      {
         _view = A.Fake<IEditOutputSchemaView>();
         _simulationSettingsTask = A.Fake<ISimulationSettingsTask>();
         _outputIntervalToOutputIntervalDTOMapper = A.Fake<IOutputIntervalToOutputIntervalDTOMapper>();
         _simulationToEdit = A.Fake<IndividualSimulation>();
         _commandRegister = A.Fake<ICommandCollector>();
         _simulationToEdit.Properties = A.Fake<SimulationProperties>();
         _outputSchema = A.Fake<OutputSchema>();
         _simulationSetting  = A.Fake<SimulationSettings>();
         _simulationToEdit.Settings = _simulationSetting;
         A.CallTo(() => _simulationSetting.OutputSchema).Returns(_outputSchema);
         _allIntervals = new List<OutputInterval>();
         _editParameterPresenterTask = A.Fake<IEditParameterPresenterTask>();
         _context= A.Fake<IExecutionContext>();
         A.CallTo(() => _outputSchema.Intervals).Returns(_allIntervals);
         sut = new EditOutputSchemaPresenter(_view, _outputIntervalToOutputIntervalDTOMapper, _simulationSettingsTask, _editParameterPresenterTask,_context);
         sut.InitializeWith(_commandRegister);
      }
   }

   public class the_simulation_output_schema_presenter_is_editing_the_simulation_output_schema : concern_for_SimulationOutputSchemaPresenter
   {
      protected override void Because()
      {
         sut.EditSettingsFor(_simulationToEdit);
      }

      [Observation]
      public void should_retrieve_the_settings_from_the_simulation_and_display_them_into_the_view()
      {
         A.CallTo(() => _view.BindTo(A<IEnumerable<OutputIntervalDTO>>.Ignored)).MustHaveHappened();
      }
   }

   public class When_asked_to_add_a_simulation_interval_to_the_edited_simulation_output : concern_for_SimulationOutputSchemaPresenter
   {
      private IPKSimCommand _addCommand;

      protected override void Context()
      {
         base.Context();
         _addCommand = A.Fake<IPKSimCommand>();
         A.CallTo(() => _simulationSettingsTask.AddSimulationIntervalTo(_outputSchema)).Returns(_addCommand);
         sut.EditSettingsFor(_simulationToEdit);
      }

      protected override void Because()
      {
         sut.AddOutputInterval();
      }

      [Observation]
      public void should_add_new_interval_to_the_simulation_output_and_register_the_command()
      {
         A.CallTo(() => _commandRegister.AddCommand(_addCommand)).MustHaveHappened();
      }
   }

   public class When_asked_to_remove_a_simulation_interval_from_the_simulation_output : concern_for_SimulationOutputSchemaPresenter
   {
      private IPKSimCommand _removeCommand;
      private OutputInterval _oneInterval;
      private OutputIntervalDTO _oneIntervalDTO;

      protected override void Context()
      {
         base.Context();
         _removeCommand = A.Fake<IPKSimCommand>();
         _oneIntervalDTO = new OutputIntervalDTO();
         _oneInterval = A.Fake<OutputInterval>();
         _oneIntervalDTO.OutputInterval = _oneInterval;
         A.CallTo(() => _simulationSettingsTask.RemoveSimulationIntervalFrom(_oneInterval, _outputSchema)).Returns(_removeCommand);
         A.CallTo(() => _outputIntervalToOutputIntervalDTOMapper.MapFrom(_oneInterval)).Returns(_oneIntervalDTO);
         _allIntervals.Add(_oneInterval);
         sut.EditSettingsFor(_simulationToEdit);
      }

      protected override void Because()
      {
         sut.RemoveOutputInterval(_oneIntervalDTO);
      }

      [Observation]
      public void should_delegate_to_the_simulation_settings_to_create_a_command_that_will_remove_the_simulaton_interval()
      {
         A.CallTo(() => _commandRegister.AddCommand(_removeCommand)).MustHaveHappened();
      }
   }
}