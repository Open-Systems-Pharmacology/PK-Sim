using System.IO;
using System.Linq;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Batch;
using PKSim.Core.Batch.Mapper;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Services;
using Compound = PKSim.Core.Batch.Compound;
using Individual = PKSim.Core.Batch.Individual;
using Simulation = PKSim.Core.Batch.Simulation;

namespace PKSim.Presentation.UICommands
{
   public class GeneratePKMLTemplatesCommand : IUICommand
   {
      private readonly ISimulationMapper _simulationMapper;
      private readonly IDefaultIndividualRetriever _defaultIndividualRetriever;
      private readonly IBuildConfigurationTask _buildConfigurationTask;
      private readonly IPKMLPersistor _pkmlPersistor;
      private readonly IDialogCreator _dialogCreator;
      private readonly IHeavyWorkManager _heavyWorkManager;
      private readonly IMoBiExportTask _moBiExportTask;

      public GeneratePKMLTemplatesCommand(ISimulationMapper simulationMapper, IDefaultIndividualRetriever defaultIndividualRetriever,
                                          IBuildConfigurationTask buildConfigurationTask, IPKMLPersistor pkmlPersistor, IDialogCreator dialogCreator,
                                          IHeavyWorkManager heavyWorkManager, IMoBiExportTask moBiExportTask)
      {
         _simulationMapper = simulationMapper;
         _defaultIndividualRetriever = defaultIndividualRetriever;
         _buildConfigurationTask = buildConfigurationTask;
         _pkmlPersistor = pkmlPersistor;
         _dialogCreator = dialogCreator;
         _heavyWorkManager = heavyWorkManager;
         _moBiExportTask = moBiExportTask;
      }

      public void Execute()
      {
         var exportFolder = _dialogCreator.AskForFolder("Select output folder where results will be exported", Constants.DirectoryKey.MODEL_PART);
         if (string.IsNullOrEmpty(exportFolder)) return;

         _heavyWorkManager.Start(() => exportToFolder(exportFolder));
      }

      private void exportToFolder(string exportFolder)
      {
         var defaultIndividual = _defaultIndividualRetriever.DefaultHuman();
         var batchSimulation = new Simulation();
         batchSimulation.Individual = new Individual
            {
               Age = defaultIndividual.Age,
               Height = defaultIndividual.MeanHeight,
               Species = defaultIndividual.Species.Name,
               Population = defaultIndividual.Population.Name,
            };

         batchSimulation.Compounds.Add(new Compound {Name = "Standard Molecule", Lipophilicity = 3, FractionUnbound = 0.1, MolWeight = 4E-7, SolubilityAtRefpH = 9999, RefpH = 7});
         var intrevanousBolus = new ApplicationProtocol {CompoundName ="Standard Molecule", ApplicationType = CoreConstants.Application.Name.IntravenousBolus, Dose = 1, DoseUnit = "mg/kg", DosingInterval = DosingIntervals.Single.ToString()};


         batchSimulation.Configuration = new SimulationConfiguration {Model = CoreConstants.Model.FourComp};
         batchSimulation.ApplicationProtocols.Add(intrevanousBolus);
         var fourCompIvBolusMgPerKg = configurationFrom(batchSimulation);

         batchSimulation.Configuration = new SimulationConfiguration {Model = CoreConstants.Model.TwoPores};
         var twoPore = configurationFrom(batchSimulation);

         batchSimulation.ApplicationProtocols.Clear();
         intrevanousBolus.DoseUnit = "mg";
         batchSimulation.ApplicationProtocols.Add(intrevanousBolus);
         batchSimulation.Configuration = new SimulationConfiguration { Model = CoreConstants.Model.FourComp };
         var fourCompIvBolusMg = configurationFrom(batchSimulation);



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
         defaultCompound.Parameter(CoreConstants.Parameter.Lipophilicity).Value = double.NaN;
         defaultCompound.Parameter(CoreConstants.Parameter.MolecularWeight).Value = double.NaN;
         defaultCompound.Parameter(CoreConstants.Parameter.FractionUnbound).Value = double.NaN;
         defaultCompound.Parameter(CoreConstants.Parameter.SolubilityAtRefpH).Value = double.NaN;

         _moBiExportTask.UpdateObserverForAllFlag(fourCompIvBolusMgPerKg.Observers);

         var buildingBlocks = new IBuildingBlock[]
            {
               twoPore.SpatialStructure, twoPore.PassiveTransports,
               fourCompIvBolusMgPerKg.SpatialStructure, fourCompIvBolusMgPerKg.PassiveTransports, fourCompIvBolusMgPerKg.EventGroups, fourCompIvBolusMgPerKg.Molecules, fourCompIvBolusMgPerKg.Observers,
               fourCompIvBolusMg.EventGroups
            };

         buildingBlocks.Each(bb => saveToPKML(bb, exportFolder));
      }

      private IBuildConfiguration configurationFrom(Simulation batchSimulation)
      {
         return _buildConfigurationTask.CreateFor(_simulationMapper.MapFrom(batchSimulation).Simulation,shouldValidate:false,createAgingDataInSimulation:false);
      }

      private void saveToPKML(IBuildingBlock buildingBlock, string folder)
      {
         var fileName = Path.Combine(folder, buildingBlock.Name + ".pkml");
         _pkmlPersistor.SaveToPKML(buildingBlock, fileName);
      }
   }
}