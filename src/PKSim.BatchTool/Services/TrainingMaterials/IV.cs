using System.Linq;
using System.Threading.Tasks;
using PKSim.Core;
using PKSim.Core.Batch;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using Compound = PKSim.Core.Batch.Compound;
using Individual = PKSim.Core.Batch.Individual;
using SystemicProcess = PKSim.Core.Batch.SystemicProcess;

namespace PKSim.BatchTool.Services.TrainingMaterials
{
   public class IV : ITrainingMaterialExercise
   {
      private readonly TrainingMaterialTask _task;
      private string _outputFolder;

      private const string INDIVIDUAL_NAME = "Healthy Female";
      private const string COMPOUND_NAME = "Diclofenac";
      private const string PROTOCOL_NAME = "IV";
      private const string OBSERVED_DATA_NAME = "Workshop_SBSuite.Ex_IV Willis 1979 MEAN";

      public IV(TrainingMaterialTask task)
      {
         _task = task;
      }

      public Task Generate(string outputFolder)
      {
         _outputFolder = outputFolder;

         return _task.ExecuteTask(() =>
         {
            createIV_1();
            createIV_2();
            createIV_3();
            createIV_4();
            createIV_5();
            createIV_End();
         });
      }

      private void createIV_1()
      {
         var batchIndividual = new Individual
         {
            Age = 21,
            Height = 16.9,
            Weight = 62.3,
            Gender = CoreConstants.Gender.Female,
            Species = CoreConstants.Species.Human,
            Population = CoreConstants.Population.ICRP,
         };

         var indiviudal = _task.MapFrom<Core.Model.Individual>(batchIndividual).WithName(INDIVIDUAL_NAME);
         _task.UpdateDisplayUnit(indiviudal.Organism.Parameter(CoreConstants.Parameter.HEIGHT), "cm");

         _task.AddBuildingBlockToProjectAndSave(indiviudal, _outputFolder, "IV_1");
      }

      private void createIV_2()
      {
         var batchCompound = new Compound()
         {
            Lipophilicity = 4.4,
            FractionUnbound = 0.002,
            MolWeight = 296 * 1e-9,
            Cl = 2,
            PkaTypes = {new PkaType {Type = "Acid", Value = 4.20}},
            SolubilityAtRefpH = 2.37 * 1e-6,
            RefpH = 7,
            SystemicProcesses =
            {
               new SystemicProcess
               {
                  SystemicProcessType = "Hepatic",
                  InternalName = "LiverClearance",
                  DataSource = "Paper",
                  Species = CoreConstants.Species.Human,
                  ParameterValues = {{CoreConstants.Parameter.SPECIFIC_CLEARANCE, 167.6}}
               },
               new SystemicProcess
               {
                  SystemicProcessType = "Renal",
                  InternalName = "KidneyClearance",
                  DataSource = "Paper",
                  Species = CoreConstants.Species.Human,
                  ParameterValues = {{CoreConstants.Parameter.SPECIFIC_CLEARANCE, 3.52}}
               }
            },
            CalculationMethods = {CoreConstants.CalculationMethod.RodgerAndRowland}
         };

         var compound = _task.MapFrom<Core.Model.Compound>(batchCompound).WithName(COMPOUND_NAME);

         _task.AddBuildingBlockToProjectAndSave(compound, _outputFolder, "IV_2");
      }

      private void createIV_3()
      {
         var batchProtocol = new ApplicationProtocol
         {
            ApplicationType = CoreConstants.Application.Name.Intravenous,
            Dose = 46.5,
            DoseUnit = "mg",
            DosingInterval = DosingIntervals.Single.ToString(),
         };

         var protocol = _task.MapFrom<Protocol>(batchProtocol).WithName(PROTOCOL_NAME);
         var infusionTime = protocol.Parameter(Constants.Parameters.INFUSION_TIME);
         infusionTime.Value = 2;
         _task.AddBuildingBlockToProjectAndSave(protocol, _outputFolder, "IV_3");
      }

      private void createIV_4()
      {
         var simulationConstruction = createConstruction("Diclofenac IV");
         var simulation = _task.CreateSimulation(simulationConstruction);
         simulation.OutputSchema.Intervals.Last().EndTime.Value = 360;
         _task.SelectDefaultOutputFor(simulation);
         _task.RunSimulation(simulation);
         _task.AddBuildingBlockToProjectAndSave(simulation, _outputFolder, "IV_4");
      }

      private void createIV_5()
      {
         var observedData = _task.AddObservedDataToProject(OBSERVED_DATA_NAME);
         _task.AddObservedDataToAnalysable(observedData, _task.Simulation("Diclofenac IV"));
         _task.SaveCurrentProjectUnder(_outputFolder, "IV_5");
      }

      private void createIV_End()
      {
         var compound = _task.Compound(COMPOUND_NAME);

         _task.CreateLipophilicityAlternative(compound, "log MA", 2.94);

         var simulationConstruction = createConstruction("Diclofenac IV log MA");
         var simulation = _task.CreateSimulation(simulationConstruction);
         simulation.OutputSchema.Intervals.Last().EndTime.Value = 360;
         _task.SelectDefaultOutputFor(simulation);
         _task.RunSimulation(simulation);
         _task.AddObservedDataToAnalysable(_task.ObservedData(OBSERVED_DATA_NAME), simulation);
         _task.AddBuildingBlockToProjectAndSave(simulation, _outputFolder, "IV_End");
      }

      private SimulationConstruction createConstruction(string simulationName)
      {
         return _task.CreateSimulationConstruction(simulationName, INDIVIDUAL_NAME, new[] {COMPOUND_NAME}, new[] {PROTOCOL_NAME});
      }
   }
}