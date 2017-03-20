using System.Collections.Generic;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface ISimulationModelConfigurationItemPresenter : IEditSimulationPresenter
   {
   }

   public interface ISimulationModelConfigurationPresenter : ISimulationItemPresenter, IContainerPresenter
   {
      /// <summary>
      ///    Return true if the simulation was created otherwise false
      /// </summary>
      bool SimulationCreated { get; }

      /// <summary>
      ///    return the edited or created simulation
      /// </summary>
      Simulation Simulation { get; }

      /// <summary>
      ///    Create a template simulation with the selected individual
      /// </summary>
      void CreateSimulation();

      /// <summary>
      ///    Create a template simulation based on the given simulation (same used building blocks)
      /// </summary>
      void CreateSimulationBasedOn(Simulation baseSimulation);

      /// <summary>
      /// Ensures that the <paramref name="templateSimulationSubject"/> is the one being used instead of the original <see cref="ISimulationSubject"/> that was used
      /// </summary>
      void UpdateSelectedSubject(ISimulationSubject templateSimulationSubject);

      /// <summary>
      /// Ensures that the <paramref name="templateCompound"/> is the one being used instead of the original <see cref="Compound"/> that was used
      /// </summary>
      /// <param name="templateCompound"></param>
      void UpdateSelectedCompound(Compound templateCompound);
   }

   public class SimulationModelConfigurationPresenter : AbstractSubPresenterContainer<ISimulationModelConfigurationView, ISimulationModelConfigurationPresenter, ISimulationModelConfigurationItemPresenter>,
      ISimulationModelConfigurationPresenter
   {
      private readonly ISimulationFactory _simulationFactory;
      public bool SimulationCreated { get; private set; }
      public Simulation Simulation { get; private set; }

      public SimulationModelConfigurationPresenter(ISimulationModelConfigurationView view, ISubPresenterItemManager<ISimulationModelConfigurationItemPresenter> subPresenterItemManager,
         ISimulationFactory simulationFactory)
         : base(view, subPresenterItemManager, SimulationModelConfigurationItems.All)
      {
         _simulationFactory = simulationFactory;
      }

      public void EditSimulation(Simulation simulation, CreationMode creationMode)
      {
         Simulation = simulation;
         _subPresenterItemManager.AllSubPresenters.Each(x => x.EditSimulation(simulation, creationMode));
         SimulationCreated = true;
         OnStatusChanged();
      }

      public override void InitializeWith(ICommandCollector commandRegister)
      {
         base.InitializeWith(commandRegister);
         subjectConfigurationPresenter.SubjectSelectionChanged += updateModelSelectionBasedOnCurrentSubject;

         //triggers once after initalization
         updateModelSelectionBasedOnCurrentSubject();
      }

      private void updateModelSelectionBasedOnCurrentSubject()
      {
         modelSelectionPresenter.EditModelConfiguration(selectedSubject);
      }

      public void CreateSimulation()
      {
         createSimulation(Simulation);
      }

      public void CreateSimulationBasedOn(Simulation baseSimulation)
      {
         createSimulation(baseSimulation);
      }

      private void createSimulation(Simulation originalSimulation)
      {
         //nothing changed
         if (SimulationCreated) return;
         Simulation = _simulationFactory.CreateFrom(selectedSubject, selectedCompounds, modelSelectionPresenter.ModelProperties, originalSimulation);
         Simulation.AllowAging = subjectConfigurationPresenter.AllowAging;
         SimulationCreated = true;
      }

      public void SaveConfiguration()
      {
         /*nothing to do for now*/
      }

      public void UpdateSelectedSubject(ISimulationSubject templateSimulationSubject)
      {
         subjectConfigurationPresenter.UpdateSelectedSubject(templateSimulationSubject);
      }

      public void UpdateSelectedCompound(Compound templateCompound)
      {
         compoundsSelectionPresenter.UpdateSelectedCompound(templateCompound);
      }

      private ISimulationSubject selectedSubject => subjectConfigurationPresenter.SelectedSubject;

      private IReadOnlyList<Compound> selectedCompounds => compoundsSelectionPresenter.SelectedCompounds;

      private ISimulationModelSelectionPresenter modelSelectionPresenter => _subPresenterItemManager.PresenterAt(SimulationModelConfigurationItems.ModelSelection);

      private ISimulationSubjectConfigurationPresenter subjectConfigurationPresenter => _subPresenterItemManager.PresenterAt(SimulationModelConfigurationItems.Subject);

      private ISimulationCompoundsSelectionPresenter compoundsSelectionPresenter => _subPresenterItemManager.PresenterAt(SimulationModelConfigurationItems.CompoundsSelection);

      public override void ViewChanged()
      {
         SimulationCreated = false;
         OnStatusChanged();
      }

      public override void AddSubItemView(ISubPresenterItem subPresenterItem, IView view)
      {
         View.AddSubView(subPresenterItem, view);
      }
   }
}