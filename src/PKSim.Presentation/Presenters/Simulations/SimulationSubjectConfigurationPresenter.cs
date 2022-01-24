using System;
using System.Linq;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface ISimulationSubjectConfigurationPresenter : ISimulationModelConfigurationItemPresenter
   {
      /// <summary>
      ///    This method is called whenever the selected subject has changed
      /// </summary>
      void UpdateSelectedSubject(ISimulationSubject simulationSubject);

      /// <summary>
      ///    did the subject of the simulation (individual or population changed?)
      /// </summary>
      bool SubjectChanged { get; }

      /// <summary>
      ///    Returns the underlying simulation subject
      /// </summary>
      ISimulationSubject SelectedSubject { get; }

      bool AllowAging { get; }

      event Action SubjectSelectionChanged;
   }

   public class SimulationSubjectConfigurationPresenter : AbstractSubPresenter<ISimulationSubjectConfigurationView, ISimulationSubjectConfigurationPresenter>,
      ISimulationSubjectConfigurationPresenter
   {
      private readonly ILazyLoadTask _lazyLoadTask;
      private readonly IBuildingBlockInProjectManager _buildingBlockInProjectManager;
      private SimulationSubjectDTO _simulationSubjectDTO;
      public bool SubjectChanged { get; private set; }
      public event Action SubjectSelectionChanged = delegate { };

      public SimulationSubjectConfigurationPresenter(ISimulationSubjectConfigurationView view, ILazyLoadTask lazyLoadTask, IBuildingBlockInProjectManager buildingBlockInProjectManager)
         : base(view)
      {
         _lazyLoadTask = lazyLoadTask;
         _buildingBlockInProjectManager = buildingBlockInProjectManager;
         SubjectChanged = false;
      }

      public override void Initialize()
      {
         base.Initialize();
         _simulationSubjectDTO = new SimulationSubjectDTO();
         _view.BindTo(_simulationSubjectDTO);
      }

      public void EditSimulation(Simulation simulation, CreationMode creationMode)
      {
         _simulationSubjectDTO = new SimulationSubjectDTO
         {
            BuildingBlock = _buildingBlockInProjectManager.TemplateBuildingBlocksUsedBy<ISimulationSubject>(simulation).SingleOrDefault(),
            AllowAging = simulation.AllowAging
         };

         _view.BindTo(_simulationSubjectDTO);

         _lazyLoadTask.Load(SelectedSubject);
         OnStatusChanged();
      }

      public void UpdateSelectedSubject(ISimulationSubject simulationSubject)
      {
         if (Equals(SelectedSubject, simulationSubject))
            return;

         _simulationSubjectDTO.BuildingBlock = simulationSubject;
         SubjectChanged = true;
         //Load subject before accessing its property
         _lazyLoadTask.Load(SelectedSubject);

         SubjectSelectionChanged();

         _view.AllowAgingVisible = simulationSubject.IsAgeDependent;
         _simulationSubjectDTO.AllowAging = simulationSubject.IsPreterm;
      }

      public ISimulationSubject SelectedSubject => _simulationSubjectDTO.BuildingBlock;

      public bool AllowAging => _simulationSubjectDTO.AllowAging;
   }
}