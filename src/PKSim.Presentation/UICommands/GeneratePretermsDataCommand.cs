using System.Data;
using System.IO;
using System.Linq;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Batch;
using PKSim.Core.Batch.Mapper;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Services;
using Compound = PKSim.Core.Batch.Compound;
using Individual = PKSim.Core.Batch.Individual;
using Simulation = PKSim.Core.Model.Simulation;

namespace PKSim.Presentation.UICommands
{
   public class GeneratePretermsDataCommand : IUICommand
   {
      private readonly IDialogCreator _dialogCreator;
      private readonly IHeavyWorkManager _heavyWorkManager;
      private readonly ISimulationMapper _simulationMapper;
      private readonly IEntityPathResolver _entityPathResolver;

      public GeneratePretermsDataCommand(IDialogCreator dialogCreator, IHeavyWorkManager heavyWorkManager, ISimulationMapper simulationMapper, IEntityPathResolver entityPathResolver)
      {
         _dialogCreator = dialogCreator;
         _heavyWorkManager = heavyWorkManager;
         _simulationMapper = simulationMapper;
         _entityPathResolver = entityPathResolver;
      }

      public void Execute()
      {
         var exportFolder = _dialogCreator.AskForFolder("Select output folder where results will be exported", Constants.DirectoryKey.MODEL_PART);
         if (string.IsNullOrEmpty(exportFolder)) return;

         _heavyWorkManager.Start(() => exportToFolder(exportFolder));
      }

      private void exportToFolder(string exportFolder)
      {
         var batchSimulation = new PKSim.Core.Batch.Simulation();
         batchSimulation.Compounds.Add(new Compound {Name = "Standard Molecule", Lipophilicity = 3, FractionUnbound = 0.1, MolWeight = 4E-7, SolubilityAtRefpH = 9999, RefpH = 7});
         batchSimulation.ApplicationProtocols.Add(new ApplicationProtocol {CompoundName ="Standard Molecule", ApplicationType = CoreConstants.Application.Name.IntravenousBolus, Dose = 1, DoseUnit = "mg/kg", DosingInterval = DosingIntervals.Single.ToString()});
         batchSimulation.Individual = new Individual {Age = 0, Species = CoreConstants.Species.Human, Population = CoreConstants.Population.Preterm, Optimize = true};
         batchSimulation.Configuration = new SimulationConfiguration {Model = CoreConstants.Model.FourComp, AllowAging = true};

         foreach (var gestationalAge in CoreConstants.PretermRange)
         {
            batchSimulation.Individual.GestationalAge = gestationalAge;
            var simulation = _simulationMapper.MapFrom(batchSimulation);
            exportSimulation(simulation.Simulation, exportFolder, gestationalAge);
         }
      }

      private void exportSimulation(Simulation simulation, string exportFolder, int gestationalAge)
      {
         var exportFile = Path.Combine(exportFolder, string.Format("{0}.xlsx", gestationalAge));
         var dataTable = new DataTable();
         dataTable.AddColumn("ParameterPath");
         dataTable.AddColumn<float>("Age");
         dataTable.AddColumn<float>("Value");

         foreach (var parameter in simulation.Model.Root.GetAllChildren<IParameter>().Where(x => x.Formula.IsTable()))
         {
            var formula = parameter.Formula.DowncastTo<TableFormula>();
            var path = _entityPathResolver.PathFor(parameter);
            foreach (var point in formula.AllPoints())
            {
               var row = dataTable.NewRow();
               row["ParameterPath"] = path;
               row["Age"] = (point.X / (60 * 24 * 365.25));
               row["Value"] = point.Y;
               dataTable.Rows.Add(row);
            }
         }
      }
   }
}