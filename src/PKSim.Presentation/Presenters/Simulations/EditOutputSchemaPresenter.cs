using System.Collections.Generic;
using System.Linq;
using OSPSuite.Assets;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface IEditOutputSchemaPresenter : IEditParameterPresenter,
      ICustomParametersPresenter,
      IListener<AddOutputIntervalToOutputSchemaEvent>,
      IListener<RemoveOutputIntervalFromOutputIntervalEvent>
   {
      void AddOutputInterval();
      void RemoveOutputInterval(OutputIntervalDTO outputIntervalDTO);
      bool CanRemoveInterval();
      void EditSettingsFor(Simulation simulation);
   }

   public class EditOutputSchemaPresenter : EditParameterPresenter<IEditOutputSchemaView, IEditOutputSchemaPresenter>, IEditOutputSchemaPresenter
   {
      private readonly IOutputIntervalToOutputIntervalDTOMapper _outputIntervalToOutputIntervalDTOMapper;
      private readonly ISimulationSettingsTask _simulationSettingsTask;
      private readonly IExecutionContext _context;
      private INotifyList<OutputIntervalDTO> _allIntervals;
      private OutputSchema _outputSchema;
      public string Description { get; set; }

      public EditOutputSchemaPresenter(IEditOutputSchemaView view, IOutputIntervalToOutputIntervalDTOMapper outputIntervalToOutputIntervalDTOMapper,
         ISimulationSettingsTask simulationSettingsTask, IEditParameterPresenterTask parameterTask, IExecutionContext context) : base(view, parameterTask)
      {
         _outputIntervalToOutputIntervalDTOMapper = outputIntervalToOutputIntervalDTOMapper;
         _simulationSettingsTask = simulationSettingsTask;
         _context = context;
      }

      public void EditSettingsFor(Simulation simulation)
      {
         Edit(simulation.SimulationSettings);
      }

      public void Edit(ISimulationSettings simulationSettings)
      {
         _outputSchema = simulationSettings.OutputSchema;
         _allIntervals = new NotifyList<OutputIntervalDTO>(_outputSchema.Intervals.MapAllUsing(_outputIntervalToOutputIntervalDTOMapper));
         _view.BindTo(_allIntervals);
      }

      public void AddOutputInterval()
      {
         AddCommand(_simulationSettingsTask.AddSimulationIntervalTo(_outputSchema));
      }

      public void RemoveOutputInterval(OutputIntervalDTO outputIntervalDTO)
      {
         AddCommand(_simulationSettingsTask.RemoveSimulationIntervalFrom(simulationIntervalFrom(outputIntervalDTO), _outputSchema));
      }

      public bool CanRemoveInterval()
      {
         return _outputSchema.Intervals.Count() > 1;
      }

      private OutputInterval simulationIntervalFrom(OutputIntervalDTO outputIntervalDTO)
      {
         return outputIntervalDTO.OutputInterval;
      }

      public void Handle(AddOutputIntervalToOutputSchemaEvent eventToHandle)
      {
         if (!Equals(_outputSchema, eventToHandle.Container)) return;
         _allIntervals.Add(_outputIntervalToOutputIntervalDTOMapper.MapFrom(eventToHandle.Entity));
      }

      public void Handle(RemoveOutputIntervalFromOutputIntervalEvent eventToHandle)
      {
         if (!Equals(_outputSchema, eventToHandle.Container)) return;
         var simulationIntervalDTO = simulationIntervalDTOFrom(eventToHandle.Entity);
         _allIntervals.Remove(simulationIntervalDTO);
      }

      private OutputIntervalDTO simulationIntervalDTOFrom(OutputInterval simulationInterval)
      {
         return _allIntervals.First(dto => Equals(dto.OutputInterval, simulationInterval));
      }

      public void Edit(IEnumerable<IParameter> parameters)
      {
         var allParameters = parameters.ToList();
         if (!allParameters.Any()) return;
         var firstParameter = allParameters[0];

         EditSettingsFor(_context.Get<Simulation>(firstParameter.Origin.SimulationId));
      }

      public bool ForcesDisplay { get; } = false;
      public bool AlwaysRefresh { get; } = false;


      public IEnumerable<IParameter> EditedParameters => Enumerable.Empty<IParameter>();

      public ApplicationIcon Icon => View.ApplicationIcon;
   }
}