using System;
using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Events;
using PKSim.Assets;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Simulations;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface ISimulationParametersPresenter : ISubPresenter, IListener<SimulationUpdatedEvent>
   {
   }

   public abstract class SimulationParametersPresenter<TSimulation> : AbstractSubPresenter<ISimulationParametersView, ISimulationParametersPresenter>, ISimulationParametersPresenter, IEditSimulationItemPresenter<TSimulation> where TSimulation : Simulation
   {
      protected readonly IParameterGroupsPresenter _parameterGroupsPresenter;
      private TSimulation _simulation;

      protected SimulationParametersPresenter(ISimulationParametersView view, IParameterGroupsPresenter parameterGroupsPresenter)
         : base(view)
      {
         _parameterGroupsPresenter = parameterGroupsPresenter;
         _parameterGroupsPresenter.NoSelectionCaption = PKSimConstants.Information.NoParametersInSimulationSelection;
         _view.AddParametersView(_parameterGroupsPresenter.View);
         AddSubPresenters(parameterGroupsPresenter);
      }

      public override bool CanClose => _parameterGroupsPresenter.CanClose;


      public void EditSimulation(TSimulation simulation)
      {
         _simulation = simulation;
         _parameterGroupsPresenter.InitializeWith(simulation.Model.Root, AllSimulationParametersToShow(simulation));
      }

      protected abstract IEnumerable<IParameter> AllSimulationParametersToShow(TSimulation simulation);

      public void Handle(SimulationUpdatedEvent eventToHandle)
      {
         if (!canHandle(eventToHandle))
            return;

         //When an update is performed to the simulation via update, we want to force a refresh of the active node
         _parameterGroupsPresenter.RefreshActivePresenter();
      }

      private bool canHandle(SimulationUpdatedEvent eventToHandle)
      {
         return Equals(_simulation, eventToHandle.Simulation);
      }
   }
}