﻿using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Core.Snapshots.Services;
using Project = PKSim.Core.Snapshots.Project;

namespace PKSim.Presentation.UICommands
{
   public class GeneratePKMLTemplatesCommand : IUICommand
   {
      private readonly ISimulationConfigurationTask _simulationConfigurationTask;
      private readonly IPKMLPersistor _pkmlPersistor;
      private readonly IDialogCreator _dialogCreator;
      private readonly IHeavyWorkManager _heavyWorkManager;
      private readonly ISnapshotObjectCreator _snapshotObjectCreator;

      public GeneratePKMLTemplatesCommand(ISimulationConfigurationTask simulationConfigurationTask, IPKMLPersistor pkmlPersistor, IDialogCreator dialogCreator,
         IHeavyWorkManager heavyWorkManager, ISnapshotObjectCreator snapshotObjectCreator)
      {
         _simulationConfigurationTask = simulationConfigurationTask;
         _pkmlPersistor = pkmlPersistor;
         _dialogCreator = dialogCreator;
         _heavyWorkManager = heavyWorkManager;
         _snapshotObjectCreator = snapshotObjectCreator;
      }

      public void Execute()
      {
         var exportFolder = _dialogCreator.AskForFolder("Select output folder where results will be exported", Constants.DirectoryKey.MODEL_PART);
         if (string.IsNullOrEmpty(exportFolder)) return;

         _heavyWorkManager.Start(() => exportToFolder(exportFolder).Wait());
      }

      private async Task exportToFolder(string exportFolder)
      {
         var project = new Project();
         var individual = await _snapshotObjectCreator.DefaultIndividual();
         project.Individuals = new[] {individual};

         var compound = await _snapshotObjectCreator.StandardCompound(lipophilicity: 3, fractionUnbound: 0.1, molWeight: 400, name: "Standard Molecule");
         compound.Name = "Standard Molecule";
         project.Compounds = new[] {compound};

         var intravenousBolusMgPerKg = await _snapshotObjectCreator.SimpleProtocol(dose: 1, doseUnit: "mg/kg", applicationType: ApplicationTypes.IntravenousBolus);
         var intravenousBolusMg = await _snapshotObjectCreator.SimpleProtocol(dose: 1, doseUnit: "mg", applicationType: ApplicationTypes.IntravenousBolus);
         project.Protocols = new[] {intravenousBolusMgPerKg, intravenousBolusMg};

         var snapshotConfiguration = new SimulationConstruction
         {
            Individual = individual,
            Compounds = new[] {compound},
            Protocols = new[] {intravenousBolusMgPerKg},
            ModelName = CoreConstants.Model.FOUR_COMP,
         };

         var fourCompIvBolusMgPerKg = await configurationFrom(snapshotConfiguration);

         snapshotConfiguration.ModelName = CoreConstants.Model.TWO_PORES;
         var twoPore = await configurationFrom(snapshotConfiguration);

         snapshotConfiguration.Protocols = new[] {intravenousBolusMg};
         snapshotConfiguration.ModelName = CoreConstants.Model.FOUR_COMP;
         var fourCompIvBolusMg = await configurationFrom(snapshotConfiguration);
         

         twoPore.SpatialStructure.Name = "Human 2 Pores";
         twoPore.PassiveTransports.Name = "2 Pores Passive Transports";

         fourCompIvBolusMgPerKg.SpatialStructure.Name = "Human Standard";
         fourCompIvBolusMgPerKg.PassiveTransports.Name = "Standard Passive Transports";
         fourCompIvBolusMgPerKg.EventGroups.Name = "IV Bolus";
         fourCompIvBolusMgPerKg.Molecules.Name = "Standard Molecule";
         fourCompIvBolusMgPerKg.Observers.Name = "Standard Observer";


         fourCompIvBolusMg.EventGroups.Name = "IV Bolus (mg)";

         var defaultCompound = fourCompIvBolusMgPerKg.Molecules.First();
         defaultCompound.Name = string.Empty;
         defaultCompound.Parameter(CoreConstants.Parameters.LIPOPHILICITY).Value = double.NaN;
         defaultCompound.Parameter(CoreConstants.Parameters.MOLECULAR_WEIGHT).Value = double.NaN;
         defaultCompound.Parameter(CoreConstants.Parameters.FRACTION_UNBOUND_PLASMA_REFERENCE_VALUE).Value = double.NaN;
         defaultCompound.Parameter(CoreConstants.Parameters.SOLUBILITY_AT_REFERENCE_PH).Value = double.NaN;

         var buildingBlocks = new IBuildingBlock[]
         {
            twoPore.SpatialStructure,
            twoPore.PassiveTransports,
            fourCompIvBolusMgPerKg.SpatialStructure,
            fourCompIvBolusMgPerKg.PassiveTransports,
            fourCompIvBolusMgPerKg.EventGroups,
            fourCompIvBolusMgPerKg.Molecules,
            fourCompIvBolusMgPerKg.Observers,
            fourCompIvBolusMg.EventGroups
         };

         buildingBlocks.Each(bb => saveToPKML(bb, exportFolder));
      }

      private async Task<Module> configurationFrom(SimulationConstruction simulationConstruction)
      {
         var simulation = await _snapshotObjectCreator.SimulationFor(simulationConstruction);

         var simulationConfiguration= _simulationConfigurationTask.CreateFor(simulation, shouldValidate: false, createAgingDataInSimulation: false);
         return simulationConfiguration.ModuleConfigurations.Select(x => x.Module).First();
      }

      private void saveToPKML(IBuildingBlock buildingBlock, string folder)
      {
         var fileName = Path.Combine(folder, buildingBlock.Name + ".pkml");
         _pkmlPersistor.SaveToPKML(buildingBlock, fileName);
      }
   }
}