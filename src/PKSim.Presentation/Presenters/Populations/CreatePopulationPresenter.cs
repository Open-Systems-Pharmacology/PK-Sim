using System.Collections.Generic;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Services;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Commands;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation.DTO.Core;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Views;
using PKSim.Presentation.Views.Populations;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Presenters.Populations
{
   public interface ICreatePopulationPresenter<TPopulation> : ICreateBuildingBlockPresenter<TPopulation>, IWizardPresenter,
      IListener<AddAdvancedParameterToContainerEvent>,
      IListener<RemoveAdvancedParameterFromContainerEvent>,
      IListener<AdvancedParameterDistributionChangedEvent>,
      IListener<AdvancedParameteSelectedEvent>,
      IListener<AddAdvancedParameterContainerToPopulationEvent>,
      IListener<RemoveAdvancedParameterContainerFromPopulationEvent>
      where TPopulation : Population
   {
      IPKSimCommand CreatePopulation(Individual basedIndividual);

      void CreatePopulation();
   }

   public abstract class CreatePopulationPresenter<TView, TPresenter, TPopulation> : PKSimWizardPresenter<TView, TPresenter, IPopulationItemPresenter>, ICreatePopulationPresenter<TPopulation>
      where TView : ICreatePopulationView, IModalView<TPresenter>
      where TPresenter : IContainerPresenter
      where TPopulation : Population
   {
      private readonly IBuildingBlockPropertiesMapper _propertiesMapper;
      private readonly IObjectBaseDTOFactory _buildingBlockDTOFactory;
      private readonly IBuildingBlockRepository _buildingBlockRepository;
      private IPopulationAdvancedParametersPresenter _advancedParametersPresenter;
      private IPopulationAdvancedParameterDistributionPresenter _distributionPresenter;
      private IPopulationMoleculesPresenter _moleculesPresenter;
      private IPopulationSettingsPresenter<TPopulation> _populationSettingsPresenter;
      private ObjectBaseDTO _populationPropertiesDTO;

      protected CreatePopulationPresenter(TView view, ISubPresenterItemManager<IPopulationItemPresenter> subPresenterItemManager,
         IReadOnlyList<ISubPresenterItem> subPresenterItems, IDialogCreator dialogCreator,
         IBuildingBlockPropertiesMapper propertiesMapper, IObjectBaseDTOFactory buildingBlockDTOFactory, IBuildingBlockRepository buildingBlockRepository)
         : base(view, subPresenterItemManager, subPresenterItems, dialogCreator)
      {
         _propertiesMapper = propertiesMapper;
         _buildingBlockDTOFactory = buildingBlockDTOFactory;
         _buildingBlockRepository = buildingBlockRepository;
      }

      public IPKSimCommand Create()
      {
         return CreatePopulation(_buildingBlockRepository.FirstOrDefault<Individual>());
      }

      public override void InitializeWith(ICommandCollector commandCollector)
      {
         base.InitializeWith(commandCollector);
         _advancedParametersPresenter = PresenterAt(AdvancedParametersPresenterItem);
         _moleculesPresenter = PresenterAt(MoleculesPresenterItem);
         _populationSettingsPresenter = RetrieveSettingsPresenter();
         _distributionPresenter = PresenterAt(DistributionPresenterItem);
         _populationSettingsPresenter.PopulationCreationFinished += editPopulation;
      }

      protected abstract IPopulationSettingsPresenter<TPopulation> RetrieveSettingsPresenter();

      protected override void UpdateControls(int indexThatWillHaveFocus)
      {
         UpdateViewStatus();
         _view.NextEnabled = _populationSettingsPresenter.CanClose;
         _view.OkEnabled = CanClose;
         _view.SetControlEnabled(AdvancedParametersPresenterItem, _populationSettingsPresenter.PopulationCreated);
         _view.SetControlEnabled(MoleculesPresenterItem, _populationSettingsPresenter.PopulationCreated);
         _view.SetControlEnabled(DistributionPresenterItem, _populationSettingsPresenter.PopulationCreated);
      }

      public IPKSimCommand CreatePopulation(Individual basedIndividual)
      {
         _populationPropertiesDTO = _buildingBlockDTOFactory.CreateFor<Population>();
         _view.BindToProperties(_populationPropertiesDTO);
         _populationSettingsPresenter.PrepareForCreating(basedIndividual);
         SetWizardButtonEnabled(SettingPresenterItem);
         _view.EnableControl(SettingPresenterItem);
         _view.Display();

         if (_view.Canceled)
            return new PKSimEmptyCommand();

         updatePopulationProperties();
         return _macroCommand;
      }

      private void updatePopulationProperties()
      {
         _propertiesMapper.MapProperties(_populationPropertiesDTO, BuildingBlock);
      }

      public void CreatePopulation()
      {
         //reset commands before generating a new population
         _macroCommand.Clear();

         _view.CancelEnabled = false;
         _view.NextEnabled = false;
         _populationSettingsPresenter.CreatePopulation();
      }

      public TPopulation BuildingBlock => _populationSettingsPresenter.Population;

      private void editPopulation(object sender, PopulationCreationEventArgs populationCreationEventArgs)
      {
         _view.CancelEnabled = true;
         _view.NextEnabled = true;
         if (!populationCreationEventArgs.Success) return;

         _moleculesPresenter.EditPopulation(BuildingBlock);
         refreshAdvancedParametersAndDistributionFromCurrentPopulation();

         if (!populationCreationEventArgs.HasWarningOrError)
            base.WizardNext(SettingPresenterItem.Index);
      }

      private void refreshAdvancedParametersAndDistributionFromCurrentPopulation()
      {
         _advancedParametersPresenter.EditPopulation(BuildingBlock);
         _distributionPresenter.EditPopulation(BuildingBlock);
      }

      public override void WizardNext(int previousIndex)
      {
         if (previousIndex == SettingPresenterItem.Index)
         {
            if (!_populationSettingsPresenter.PopulationCreated)
            {
               CreatePopulation();
               return;
            }
         }

         base.WizardNext(previousIndex);
      }

      protected override bool HasData()
      {
         return _populationSettingsPresenter.PopulationCreated;
      }

      public void Handle(AddAdvancedParameterToContainerEvent advancedParameterEvent)
      {
         if (!canHandle(advancedParameterEvent.DowncastTo<IAdvancedParameterEvent>())) return;
        _distributionPresenter.AddAdvancedParameter(advancedParameterEvent.AdvancedParameter);
      }

      public void Handle(RemoveAdvancedParameterFromContainerEvent advancedParameterEvent)
      {
         if (!canHandle(advancedParameterEvent.DowncastTo<IAdvancedParameterEvent>())) return;
         _distributionPresenter.RemoveAdvancedParameter(advancedParameterEvent.AdvancedParameter);
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

      private bool canHandle(IEntityEvent populationEvent)
      {
         return Equals(populationEvent.Subject, BuildingBlock);
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

      private bool canHandle(IAdvancedParameterEvent advancedParameterEvent)
      {
         return Equals(advancedParameterEvent.AdvancedParameterContainer, BuildingBlock);
      }

      protected abstract ISubPresenterItem SettingPresenterItem { get; }
      protected abstract ISubPresenterItem<IPopulationAdvancedParametersPresenter> AdvancedParametersPresenterItem { get; }
      protected abstract ISubPresenterItem<IPopulationMoleculesPresenter> MoleculesPresenterItem { get; }
      protected abstract ISubPresenterItem<IPopulationAdvancedParameterDistributionPresenter> DistributionPresenterItem { get; }
   }
}