using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Assets;

namespace PKSim.Presentation.Presenters.Simulations
{
   public abstract class SimulationCompoundCollectorPresenterBase<TView, TSubCompoundPresenter> : AbstractSubPresenter<TView, ISimulationItemPresenter>, ISimulationItemPresenter
      where TView : ISimulationCompoundCollectorView
      where TSubCompoundPresenter : IEditSimulationCompoundPresenter

   {
      private readonly IApplicationController _applicationController;
      private readonly IConfigurableLayoutPresenter _configurableLayoutPresenter;
      private readonly IEventPublisher _eventPublisher;
      protected Simulation _simulation;

      //do not use the default sub presenter manager from base class since we need to reset this presenter all the time
      private readonly SubPresenterManager _subPresenterCollector;

      protected SimulationCompoundCollectorPresenterBase(TView view, IApplicationController applicationController,
         IConfigurableLayoutPresenter configurableLayoutPresenter, IEventPublisher eventPublisher) : base(view)
      {
         _applicationController = applicationController;
         _configurableLayoutPresenter = configurableLayoutPresenter;
         _eventPublisher = eventPublisher;
         _view.AddCollectorView(_configurableLayoutPresenter.BaseView);
         _subPresenterCollector = new SubPresenterManager();
      }

      public virtual void EditSimulation(Simulation simulation, CreationMode creationMode)
      {
         Clear();
         _simulation = simulation;
         AddSubPresentersFor(simulation);
         _configurableLayoutPresenter.AddViews(AllSubCompoundPresenters().Where(ShouldBeDisplayed).Select(x => x.BaseView));
      }

      /// <summary>
      ///    Specifies whether a view from the presenter should be displayed
      /// </summary>
      /// <param name="presenter">The presenter that should be checked whether or not display is required</param>
      /// <returns>True if the display should be shown, otherwise false.</returns>
      protected virtual bool ShouldBeDisplayed(TSubCompoundPresenter presenter)
      {
         return true;
      }

      protected virtual void AddSubPresentersFor(Simulation simulation)
      {
         simulation.Compounds.Each(c => _subPresenterCollector.Add((CreateCompoundPresenterFor(simulation, c))));
         //make sure we initialize all sub presenters
         _subPresenterCollector.InitializeWith(this);
      }

      protected virtual void Clear()
      {
         _configurableLayoutPresenter.RemoveViews();
         _subPresenterCollector.ReleaseFrom(_eventPublisher);
      }

      protected virtual TSubCompoundPresenter CreateCompoundPresenterFor(Simulation simulation, Compound compound)
      {
         var presenter = _applicationController.Start<TSubCompoundPresenter>();
         presenter.BaseView.Caption = compound.Name;
         presenter.BaseView.ApplicationIcon = ApplicationIcons.Compound;
         presenter.EditSimulation(simulation, compound);
         return presenter;
      }

      public virtual void SaveConfiguration()
      {
         AllSubCompoundPresenters().Each(x => x.SaveConfiguration());
      }

      protected IEnumerable<TSubCompoundPresenter> AllSubCompoundPresenters()
      {
         return _subPresenterCollector.OfType<TSubCompoundPresenter>();
      }

      public override bool CanClose => base.CanClose && _subPresenterCollector.CanClose;

      protected override void ClearSubPresenters(IEventPublisher eventPublisher)
      {
         base.ClearSubPresenters(eventPublisher);
         _subPresenterCollector.ReleaseFrom(_eventPublisher);
      }
   }
}