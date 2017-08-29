using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Presentation.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Core.Snapshots.Mappers;
using PKSim.Core.Snapshots.Services;
using Project = PKSim.Core.Snapshots.Project;

namespace PKSim.Presentation.UICommands
{
   public class GeneratePKMLTemplatesCommand : IUICommand
   {
      private readonly IBuildConfigurationTask _buildConfigurationTask;
      private readonly IPKMLPersistor _pkmlPersistor;
      private readonly IDialogCreator _dialogCreator;
      private readonly IHeavyWorkManager _heavyWorkManager;
      private readonly IMoBiExportTask _moBiExportTask;
      private readonly ISnapshotObjectCreator _snapshotObjectCreator;

      public GeneratePKMLTemplatesCommand(IBuildConfigurationTask buildConfigurationTask, IPKMLPersistor pkmlPersistor, IDialogCreator dialogCreator,
         IHeavyWorkManager heavyWorkManager, IMoBiExportTask moBiExportTask, ISnapshotObjectCreator snapshotObjectCreator)
      {
         _buildConfigurationTask = buildConfigurationTask;
         _pkmlPersistor = pkmlPersistor;
         _dialogCreator = dialogCreator;
         _heavyWorkManager = heavyWorkManager;
         _moBiExportTask = moBiExportTask;
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

         var compound = await _snapshotObjectCreator.StandardCompound(lipophilicity: 3, fractionUnbound: 0.1, molWeight: 400, name:"Standard Molecule");
         compound.Name = "Standard Molecule";
         project.Compounds = new[] {compound};

         var intrevanousBolusMgPerKg = await _snapshotObjectCreator.SimpleProtocol(dose: 1, doseUnit: "mg/kg", applicationType: ApplicationTypes.IntravenousBolus);
         var intrevanousBolusMg = await _snapshotObjectCreator.SimpleProtocol(dose: 1, doseUnit: "mg", applicationType: ApplicationTypes.IntravenousBolus);
         project.Protocols = new[] {intrevanousBolusMgPerKg, intrevanousBolusMg};

         var snapshotConfiguration = new SimulationConstruction
         {
            Individual = individual,
            Compounds = new[] {compound },
            Protocols = new [] {intrevanousBolusMgPerKg },
            ModelName = CoreConstants.Model.FourComp,
         };

         var fourCompIvBolusMgPerKg = await configurationFrom(snapshotConfiguration);

         snapshotConfiguration.ModelName = CoreConstants.Model.TwoPores;
         var twoPore = await configurationFrom(snapshotConfiguration);

         snapshotConfiguration.Protocols = new[] { intrevanousBolusMg};
         snapshotConfiguration.ModelName = CoreConstants.Model.FourComp;
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
         defaultCompound.Parameter(CoreConstants.Parameter.LIPOPHILICITY).Value = double.NaN;
         defaultCompound.Parameter(CoreConstants.Parameter.MOLECULAR_WEIGHT).Value = double.NaN;
         defaultCompound.Parameter(CoreConstants.Parameter.FRACTION_UNBOUND_PLASMA_REFERENCE_VALUE).Value = double.NaN;
         defaultCompound.Parameter(CoreConstants.Parameter.SOLUBILITY_AT_REFERENCE_PH).Value = double.NaN;

         _moBiExportTask.UpdateObserverForAllFlag(fourCompIvBolusMgPerKg.Observers);

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

      private async Task<IBuildConfiguration> configurationFrom(SimulationConstruction simulationConstruction)
      {
         var simulation = await _snapshotObjectCreator.SimulationFor(simulationConstruction);

         return _buildConfigurationTask.CreateFor(simulation, shouldValidate: false, createAgingDataInSimulation: false);
      }

      private void saveToPKML(IBuildingBlock buildingBlock, string folder)
      {
         var fileName = Path.Combine(folder, buildingBlock.Name + ".pkml");
         _pkmlPersistor.SaveToPKML(buildingBlock, fileName);
      }
   }
}