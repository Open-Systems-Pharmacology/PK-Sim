using System;
using System.Linq;
using PKSim.Assets;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Services;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface IConfigureSimulationPresenter : ISimulationWizardPresenter
   {
      /// <summary>
      ///    Starts the configuation workflow for the given simulationToClone
      /// </summary>
      /// <param name="simulation"> Simulation to configure </param>
      IPKSimCommand ConfigureSimulation(Simulation simulation);

      /// <summary>
      ///    Starts the configuation workflow for the given simulationToClone and set the template building block als selected
      ///    building block
      /// </summary>
      /// <param name="simulation"> Simulation to configure </param>
      /// <param name="templateBuildingBlock">
      ///    Template building block that should be selected (even of the simulationToClone is
      ///    not using it)
      /// </param>
      IPKSimCommand ConfigureSimulationWithBuildingBlock(Simulation simulation, IPKSimBuildingBlock templateBuildingBlock);
   }

   public class ConfigureSimulationPresenter : ConfigureSimulationPresenterBase<IConfigureSimulationView>
   {
      public ConfigureSimulationPresenter(IConfigureSimulationView view, ISubPresenterItemManager<ISimulationItemPresenter> subPresenterItemManager, ISimulationModelCreator simulationModelCreator,
         IHeavyWorkManager heavyWorkManager, ICloner cloner, IDialogCreator dialogCreator, ISimulationParametersUpdater simulationParametersUpdater,
         IFullPathDisplayResolver fullPathDisplayResolver, IBuildingBlockInSimulationSynchronizer buildingBlockInSimulationSynchronizer)
         : base(view, subPresenterItemManager, simulationModelCreator, heavyWorkManager, cloner, dialogCreator, simulationParametersUpdater, fullPathDisplayResolver, buildingBlockInSimulationSynchronizer, CreationMode.Configure)
      {
      }

      protected override string HeavyWorkCaption => PKSimConstants.UI.UpdatingSimulation;

      protected override string ViewCaption(Simulation simulation)
      {
         return PKSimConstants.UI.ConfigureSimulation(simulation.Name);
      }
   }

   public abstract class ConfigureSimulationPresenterBase<TSimulationView> : SimulationWizardPresenter<TSimulationView>, IConfigureSimulationPresenter where TSimulationView : ISimulationWizardView
   {
      private readonly ICloner _cloner;
      private readonly ISimulationParametersUpdater _simulationParametersUpdater;
      private readonly IFullPathDisplayResolver _fullPathDisplayResolver;
      private readonly IBuildingBlockInSimulationSynchronizer _buildingBlockInSimulationSynchronizer;
      private readonly CreationMode _creationMode;
      private Simulation _originalSimulation;
      private Simulation _simulationToConfigure;

      protected ConfigureSimulationPresenterBase(TSimulationView view, ISubPresenterItemManager<ISimulationItemPresenter> subPresenterItemManager,
         ISimulationModelCreator simulationModelCreator, IHeavyWorkManager heavyWorkManager,
         ICloner cloner, IDialogCreator dialogCreator, ISimulationParametersUpdater simulationParametersUpdater, IFullPathDisplayResolver fullPathDisplayResolver, IBuildingBlockInSimulationSynchronizer buildingBlockInSimulationSynchronizer, CreationMode creationMode)
         : base(view, subPresenterItemManager, simulationModelCreator, heavyWorkManager, dialogCreator)
      {
         _cloner = cloner;
         _simulationParametersUpdater = simulationParametersUpdater;
         _fullPathDisplayResolver = fullPathDisplayResolver;
         _buildingBlockInSimulationSynchronizer = buildingBlockInSimulationSynchronizer;
         _creationMode = creationMode;
         AllowQuickFinish = true;
      }

      public IPKSimCommand ConfigureSimulation(Simulation simulation)
      {
         return configureSimulation(simulation, SimulationItems.Model, () => { });
      }

      protected abstract string ViewCaption(Simulation simulation);

      public IPKSimCommand ConfigureSimulationWithBuildingBlock(Simulation simulation, IPKSimBuildingBlock templateBuildingBlock)
      {
         Action updateAction = () => { };
         ISubPresenterItem itemToActivate = SimulationItems.Model;
         switch (templateBuildingBlock.BuildingBlockType)
         {
            case PKSimBuildingBlockType.Individual:
            case PKSimBuildingBlockType.Population:
               itemToActivate = SimulationItems.Model;
               updateAction = () => PresenterAt(SimulationItems.Model).UpdateSelectedSubject(templateBuildingBlock.DowncastTo<ISimulationSubject>());
               break;
            case PKSimBuildingBlockType.Compound:
               itemToActivate = SimulationItems.Model;
               updateAction = () => PresenterAt(SimulationItems.Model).UpdateSelectedCompound(templateBuildingBlock.DowncastTo<Compound>());
               break;
            case PKSimBuildingBlockType.Formulation:
               itemToActivate = SimulationItems.CompoundProtocols;
               updateAction = () => PresenterAt(SimulationItems.CompoundProtocols).UpdateSelectedFormulation(templateBuildingBlock.DowncastTo<Formulation>());
               break;
            case PKSimBuildingBlockType.Protocol:
               itemToActivate = SimulationItems.CompoundProtocols;
               updateAction = () => PresenterAt(SimulationItems.CompoundProtocols).UpdateSelectedProtocol(templateBuildingBlock.DowncastTo<Protocol>());
               break;
            case PKSimBuildingBlockType.Event:
               break;
            case PKSimBuildingBlockType.ObserverSet:
               itemToActivate = SimulationItems.Observers;
               break;
            default:
               throw new ArgumentOutOfRangeException(templateBuildingBlock.BuildingBlockType.ToString());
         }
         return configureSimulation(simulation, itemToActivate, updateAction);
      }

      private IPKSimCommand configureSimulation(Simulation simulation, ISubPresenterItem subPresenterItem, Action actionToPerformBeforeDisplayingView)
      {
         _originalSimulation = simulation;
         _simulationToConfigure = createSimulationToConfigureBasedOn();
         PresenterAt(SimulationItems.Model).EditSimulation(_simulationToConfigure, _creationMode);
         editSimulation(_simulationToConfigure);
         _view.ActivateControl(subPresenterItem);
         SetWizardButtonEnabled(subPresenterItem);
         actionToPerformBeforeDisplayingView();
         _view.Caption = ViewCaption(simulation);
         _view.Display();

         if (_view.Canceled)
            return new PKSimEmptyCommand();

         //simulationToClone only returns an empty macro command as create simulationToClone is an atomic process
         return new PKSimMacroCommand();
      }
      
      private Simulation createSimulationToConfigureBasedOn()
      {
         var simulationToConfigure = _cloner.CloneForModel(_originalSimulation);
         _buildingBlockInSimulationSynchronizer.UpdateUsedBuildingBlockBasedOnTemplateIn(simulationToConfigure);
         return simulationToConfigure;
      }

      private void editSimulation(Simulation simulation)
      {
         AllSimulationItemsAfterModel.Each(x => PresenterAt(x).EditSimulation(simulation, _creationMode));
      }

      protected override void UpdateSimulationProperties()
      {
         SaveBuildingBlocksConfiguration();

         _simulationModelCreator.CreateModelFor(Simulation);

         //now update all parameters from the orginal simulationToClone
         var validationResult = _simulationParametersUpdater.ReconciliateSimulationParametersBetween(_originalSimulation, Simulation);
         displayMissingParametersMessage(validationResult);

         //last update the version of the new simulationToClone
         Simulation.Version++;
         Simulation.StructureVersion++;
      }

      private void displayMissingParametersMessage(ValidationResult validationResult)
      {
         if (validationResult.ValidationState == ValidationState.Valid)
            return;

         var missingParameters = validationResult.Messages.Select(x => _fullPathDisplayResolver.FullPathFor(x.Object));
         _dialogCreator.MessageBoxInfo(PKSimConstants.Warning.MissingSimulationParametersWereOverwritten(missingParameters));
      }

      public override void ModelConfigurationDone()
      {
         //individual and model settings did not change. Nothing to do
         if (PresenterAt(SimulationItems.Model).SimulationCreated) return;

         //Create a new simulationToClone and update the building blocks used from the original simulationToClone
         PresenterAt(SimulationItems.Model).CreateSimulationBasedOn(_simulationToConfigure);

         editSimulation(Simulation);
      }
   }
}