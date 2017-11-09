using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Simulations;

namespace PKSim.Presentation.Services
{
   public class SimulationSettingsRetriever : ISimulationSettingsRetriever
   {
      private readonly IApplicationController _applicationController;
      private readonly IPKSimProjectRetriever _projectRetriever;
      private readonly IEntityPathResolver _entityPathResolver;
      private readonly IKeyPathMapper _keyPathMapper;
      private readonly ICoreUserSettings _userSettings;

      public SimulationSettingsRetriever(IApplicationController applicationController, IPKSimProjectRetriever projectRetriever,
         IEntityPathResolver entityPathResolver, IKeyPathMapper keyPathMapper, ICoreUserSettings userSettings)
      {
         _applicationController = applicationController;
         _projectRetriever = projectRetriever;
         _entityPathResolver = entityPathResolver;
         _keyPathMapper = keyPathMapper;
         _userSettings = userSettings;
      }

      public OutputSelections SettingsFor(Simulation simulation)
      {
         updateDefaultSettings(simulation);
         switch (simulation)
         {
            case IndividualSimulation individualSimulation:
               return outputSelectionFor(individualSimulation);
            case PopulationSimulation populationSimulation:
               return outputSelectionFor(populationSimulation);
            default:
               return null;
         }
      }

      private OutputSelections outputSelectionFor<T>(T simulation) where T : Simulation
      {
         using (var presenter = _applicationController.Start<ISimulationOutputSelectionPresenter<T>>())
         {
            return presenter.CreateSettings(simulation);
         }
      }

      public void SynchronizeSettingsIn(Simulation simulation)
      {
         if (simulation.OutputSelections == null) return;
         var clone = simulation.OutputSelections.Clone();
         updateSelectionFromTemplate(simulation, clone);
      }

      private void updateDefaultSettings(Simulation simulation)
      {
         var settings = simulation.OutputSelections;
         if (settings.HasSelection) return;

         //retrieve default from project
         var templateSettings = retrieveTemplateSettings();
         if (templateSettings == null || !templateSettings.HasSelection)
            CreatePKSimDefaults(simulation);
         else
            updateSelectionFromTemplate(simulation, templateSettings);

         //this could happen if nothing can be match in template
         if (!settings.HasSelection)
            CreatePKSimDefaults(simulation);
      }

      private OutputSelections retrieveTemplateSettings()
      {
         return _userSettings.OutputSelections ??
                _projectRetriever.Current.OutputSelections;
      }

      private void updateSelectionFromTemplate(Simulation simulation, OutputSelections templateOutputSelections)
      {
         var settings = simulation.OutputSelections;
         settings.Clear();

         //Cache all quantities by path key
         var quantityCache = new Cache<string, List<IQuantity>>();
         foreach (var quantity in simulation.All<IQuantity>())
         {
            var key = _keyPathMapper.MapFrom(quantity);
            if (!quantityCache.Contains(key))
               quantityCache[key] = new List<IQuantity>();

            quantityCache[key].Add(quantity);
         }

         //find them from the template and select them
         foreach (var selectedQuantity in templateOutputSelections)
         {
            var key = _keyPathMapper.MapFrom(selectedQuantity);
            if (!quantityCache.Contains(key)) continue;
            var selectedQuantityType = selectedQuantity.QuantityType;

            //we only select quantity that have the exact same type
            quantityCache[key].Where(q => q.QuantityType == selectedQuantityType)
               .Each(q => settings.AddOutput(selectionFrom(q)));
         }
      }

      private QuantitySelection selectionFrom(IQuantity quantity)
      {
         return new QuantitySelection(_entityPathResolver.PathFor(quantity), quantity.QuantityType);
      }

      public void CreatePKSimDefaults(Simulation simulation)
      {
         var outputSelections = simulation.OutputSelections;
         simulation.Compounds.Each(compound => addDefaultOutputsFor(simulation, compound, outputSelections));
      }

      private void addDefaultOutputsFor(Simulation simulation, Compound compound, OutputSelections outputSelections)
      {
         var individual = simulation.Individual;
         if (individual == null)
            return;

         //Default is peripheral venous blood plasma
         var observer = peripheralVenousBloodObserverFor(simulation, compound);
         if (speciesUsesVenousBlood(individual.Species))
            observer = venousBloodObservedFor(simulation, compound);

         if (observer == null)
            return;

         var observedPath = _entityPathResolver.PathFor(observer);
         outputSelections.AddOutput(new QuantitySelection(observedPath, observer.QuantityType));
      }

      private IObserver venousBloodObservedFor(Simulation simulation, Compound compound)
      {
         return simulation.Model.Root
            .EntityAt<IObserver>(Constants.ORGANISM, CoreConstants.Organ.VenousBlood,
               CoreConstants.Compartment.Plasma, compound.Name, CoreConstants.Observer.CONCENTRATION);
      }

      private bool speciesUsesVenousBlood(Species species)
      {
         return species.NameIsOneOf(CoreConstants.Species.SpeciesUsingVenousBlood);
      }

      private static IObserver peripheralVenousBloodObserverFor(Simulation simulation, Compound compound)
      {
         return simulation.Model.Root
            .EntityAt<IObserver>(Constants.ORGANISM, CoreConstants.Organ.PeripheralVenousBlood,
               compound.Name, CoreConstants.Observer.PLASMA_PERIPHERAL_VENOUS_BLOOD);
      }
   }
}