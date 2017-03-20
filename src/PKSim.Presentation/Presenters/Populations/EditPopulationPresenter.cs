using System.Collections.Generic;
using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Presentation.Views;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Presenters.Populations
{
   public interface IEditPopulationPresenter<TPopulation> : IEditBuildingBockPresenter<TPopulation>,
      IListener<AddAdvancedParameterToContainerEvent>,
      IListener<RemoveAdvancedParameterFromContainerEvent>,
      IListener<AdvancedParameterDistributionChangedEvent>,
      IListener<AdvancedParameteSelectedEvent>,
      IListener<AddAdvancedParameterContainerToPopulationEvent>,
      IListener<RemoveAdvancedParameterContainerFromPopulationEvent>
      where TPopulation : Population
   {
   }

   public abstract class EditPopulationPresenter<TView, TPresenter, TPopulation> : SingleStartContainerPresenter<TView, TPresenter, TPopulation, IPopulationItemPresenter>
      where TPresenter : IEditPopulationPresenter<TPopulation>
      where TPopulation : Population
      where TView : IView<TPresenter>, IMdiChildView
   {
      private IPopulationAdvancedParameterDistributionPresenter _distributionPresenter;
      private IPopulationSettingsPresenter<TPopulation> _settingsPresenter;
      private IPopulationAdvancedParametersPresenter _advancedParametersPresenter;
      private IPopulationMoleculesPresenter _populationMoleculesPresenter;

      protected EditPopulationPresenter(TView view, ISubPresenterItemManager<IPopulationItemPresenter> subPresenterItemManager, IReadOnlyList<ISubPresenterItem> subPresenterItems)
         : base(view, subPresenterItemManager, subPresenterItems)
      {
      }

      public override void Edit(TPopulation populationToEdit)
      {
         _settingsPresenter.LoadPopulation(populationToEdit);
         _populationMoleculesPresenter.EditPopulation(populationToEdit);
         refreshAdvancedParametersAndDistributionFromCurrentPopulation();
         _view.EnableControl(SettingPresenterItem);
         _view.EnableControl(AdvancedParameterPresenterItem);
         _view.EnableControl(MoleculesPresenterItem);
         _view.ActivateControl(DistributionPresenterItem);
         UpdateCaption();
         _view.Display();
      }

      public override void InitializeWith(ICommandCollector commandRegister)
      {
         base.InitializeWith(commandRegister);
         _distributionPresenter = PresenterAt(DistributionPresenterItem);
         _advancedParametersPresenter = PresenterAt(AdvancedParameterPresenterItem);
         _populationMoleculesPresenter = PresenterAt(MoleculesPresenterItem);
         _settingsPresenter = PresenterAt(SettingPresenterItem);
      }

      protected override void UpdateCaption()
      {
         _view.Caption = PKSimConstants.UI.EditPopulation(population.Name);
      }

      private TPopulation population
      {
         get { return _settingsPresenter.Population; }
      }

      public void Handle(AddAdvancedParameterContainerToPopulationEvent eventToHandle)
      {
         if (!canHandle(eventToHandle)) return;
         refreshAdvancedParametersAndDistributionFromCurrentPopulation();
      }

      public void Handle(RemoveAdvancedParameterContainerFromPopulationEvent eventToHandle)
      {
         if (!canHandle(eventToHandle)) return;
         refreshAdvancedParametersAndDistributionFromCurrentPopulation();
      }

      public void Handle(AddAdvancedParameterToContainerEvent advancedParameterEvent)
      {
         if (!canHandle(advancedParameterEvent.DowncastTo<IAdvancedParameterEvent>())) return;
         _distributionPresenter.AddAdvancedParameter(advancedParameterEvent.AdvancedParameter);
      }

      private void refreshAdvancedParametersAndDistributionFromCurrentPopulation()
      {
         _advancedParametersPresenter.EditPopulation(population);
         _distributionPresenter.EditPopulation(population);
      }

      public void Handle(RemoveAdvancedParameterFromContainerEvent advancedParameterEvent)
      {
         if (!canHandle(advancedParameterEvent.DowncastTo<IAdvancedParameterEvent>())) return;
         _distributionPresenter.RemoveAdvancedParameter(advancedParameterEvent.AdvancedParameter);
      }

      public void Handle(AdvancedParameterDistributionChangedEvent advancedParameterEvent)
      {
         refreshDistribution(advancedParameterEvent);
      }

      public void Handle(AdvancedParameteSelectedEvent advancedParameterEvent)
      {
         refreshDistribution(advancedParameterEvent);
      }

      private void refreshDistribution(IAdvancedParameterEvent advancedParameterEvent)
      {
         if (!canHandle(advancedParameterEvent)) return;
         _distributionPresenter.Select(advancedParameterEvent.AdvancedParameter);
      }

      private bool canHandle(IAdvancedParameterEvent populationEvent)
      {
         return Equals(populationEvent.AdvancedParameterContainer, population);
      }

      private bool canHandle(IEntityEvent populationEvent)
      {
         return Equals(populationEvent.Subject, population);
      }

      public override object Subject
      {
         get { return population; }
      }

      protected abstract ISubPresenterItem<IPopulationSettingsPresenter<TPopulation>> SettingPresenterItem { get; }
      protected abstract ISubPresenterItem<IPopulationAdvancedParameterDistributionPresenter> DistributionPresenterItem { get; }
      protected abstract ISubPresenterItem<IPopulationAdvancedParametersPresenter> AdvancedParameterPresenterItem { get; }
      protected abstract ISubPresenterItem<IPopulationMoleculesPresenter> MoleculesPresenterItem { get; }
   }
}