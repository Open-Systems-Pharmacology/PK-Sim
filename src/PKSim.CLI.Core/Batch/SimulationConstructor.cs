using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;

namespace PKSim.Core.Batch
{
   internal class SimulationConstruction
   {
      public ISimulationSubject SimulationSubject { get; set; }
      public IReadOnlyList<Model.Compound> TemplateCompounds { get; set; }
      public IReadOnlyList<Protocol> TemplateProtocols { get; set; }
      public Model.Formulation TemplateFormulation { get; set; }
      public ModelProperties ModelProperties { get; set; }
      public string SimulationName { get; set; }
      public bool AllowAging { get; set; }
      public IReadOnlyList<InteractionSelection> Interactions { get; set; }

      public SimulationConstruction()
      {
         Interactions = new List<InteractionSelection>();
         TemplateCompounds = new List<Model.Compound>();
         TemplateProtocols = new List<Protocol>();
      }
   }

   internal interface ISimulationConstructor
   {
      Model.Simulation CreateModelLessSimulationWith(SimulationConstruction simulationConstruction);
      Model.Simulation CreateSimulation(SimulationConstruction simulationConstruction, Action<Model.Simulation> preModelCreationAction = null);
      void AddModelToSimulation(Model.Simulation simulation);
   }

   internal class SimulationConstructor : ISimulationConstructor
   {
      private readonly ISimulationFactory _simulationFactory;
      private readonly ISimulationBuildingBlockUpdater _buildingBlockUpdater;
      private readonly ISimulationModelCreator _simulationModelCreator;
      private readonly IRegistrationTask _registrationTask;
      private readonly ILogger _batchLogger;

      public SimulationConstructor(ISimulationFactory simulationFactory, ISimulationBuildingBlockUpdater buildingBlockUpdater,
         ISimulationModelCreator simulationModelCreator, IRegistrationTask registrationTask, ILogger batchLogger)
      {
         _simulationFactory = simulationFactory;
         _buildingBlockUpdater = buildingBlockUpdater;
         _simulationModelCreator = simulationModelCreator;
         _registrationTask = registrationTask;
         _batchLogger = batchLogger;
      }

      public Model.Simulation CreateModelLessSimulationWith(SimulationConstruction simulationConstruction)
      {
         var sim = _simulationFactory.CreateFrom(simulationConstruction.SimulationSubject, simulationConstruction.TemplateCompounds, simulationConstruction.ModelProperties);
         sim.Name = simulationConstruction.SimulationName;

         for (int index = 0; index < simulationConstruction.TemplateCompounds.Count; index++)
         {
            var compoundProperties = sim.CompoundPropertiesList[index];
            var compound = compoundProperties.Compound;

            //for now: only simple protocol
            var simpleProtocol = simulationConstruction.TemplateProtocols[index] as SimpleProtocol;
            if (simpleProtocol != null && simpleProtocol.ApplicationType.NeedsFormulation && simulationConstruction.TemplateFormulation != null)
            {
               simpleProtocol.FormulationKey = CoreConstants.DEFAULT_FORMULATION_KEY;
               simpleProtocol.FormulationKey = CoreConstants.DEFAULT_FORMULATION_KEY;
               compoundProperties.ProtocolProperties.AddFormulationMapping(new FormulationMapping {FormulationKey = CoreConstants.DEFAULT_FORMULATION_KEY, TemplateFormulationId = simulationConstruction.TemplateFormulation.Id, Formulation = simulationConstruction.TemplateFormulation });
            }

            var processes = compoundProperties.Processes;

            //add all systemic processes
            addSystemicProcesses(compound, processes.MetabolizationSelection, SystemicProcessTypes.Hepatic);
            addSystemicProcesses(compound, processes.TransportAndExcretionSelection, SystemicProcessTypes.GFR);
            addSystemicProcesses(compound, processes.TransportAndExcretionSelection, SystemicProcessTypes.Renal);
            addSystemicProcesses(compound, processes.TransportAndExcretionSelection, SystemicProcessTypes.Biliary);

            //add all partial processes
            var individual = simulationConstruction.SimulationSubject as Model.Individual ?? simulationConstruction.SimulationSubject.DowncastTo<Population>().FirstIndividual;

            addPartialProcesses<EnzymaticProcess, IndividualEnzyme, EnzymaticProcessSelection>(compound, individual, processes.MetabolizationSelection);
            addPartialProcesses<TransportPartialProcess, IndividualTransporter, ProcessSelection>(compound, individual, processes.TransportAndExcretionSelection);
            addPartialProcesses<SpecificBindingPartialProcess, IndividualMolecule, ProcessSelection>(compound, individual, processes.SpecificBindingSelection);

            _buildingBlockUpdater.UpdateMultipleUsedBuildingBlockInSimulationFromTemplate(sim, simulationConstruction.TemplateProtocols.Where(p => p != null), PKSimBuildingBlockType.Protocol);

            var templateProtocol = simulationConstruction.TemplateProtocols[index];
            if (templateProtocol != null)
               compoundProperties.ProtocolProperties.Protocol = sim.AllBuildingBlocks<Protocol>().FindByName(templateProtocol.Name);

            if (simulationConstruction.TemplateFormulation != null)
               _buildingBlockUpdater.UpdateMultipleUsedBuildingBlockInSimulationFromTemplate(sim, new[] {simulationConstruction.TemplateFormulation}, PKSimBuildingBlockType.Formulation);
         }

         simulationConstruction.Interactions.Each(sim.InteractionProperties.AddInteraction);
         sim.AllowAging = simulationConstruction.AllowAging;
         return sim;
      }

      private void addPartialProcesses<TPartialProcess, TIndividualMolecule, TProcessSelection>(Model.Compound compound, Model.Individual individual, ProcessSelectionGroup processSelectionGroup)
         where TPartialProcess : Model.PartialProcess
         where TIndividualMolecule : IndividualMolecule
         where TProcessSelection : ProcessSelection, new()
      {
         //default mapping with processes: Mapping done only by name
         foreach (var process in compound.AllProcesses<TPartialProcess>())
         {
            var molecule = individual.MoleculeByName<TIndividualMolecule>(process.MoleculeName);
            //enzyme not found in individual
            if (molecule == null)
            {
               _batchLogger.AddDebug($"Molecule '{process.MoleculeName}' not found in individual  but is defined in compound for process '{process.Name}'");
               continue;
            }

            _batchLogger.AddDebug($"Adding process {process.Name} for molecule {molecule.Name}");
            processSelectionGroup.AddPartialProcessSelection(new TProcessSelection {ProcessName = process.Name, MoleculeName = molecule.Name, CompoundName = compound.Name});
         }
      }

      private void addSystemicProcesses(Model.Compound compound, ProcessSelectionGroup processSelectionGroup, SystemicProcessType systemicProcessType)
      {
         //add all systemic processes
         compound.AllSystemicProcessesOfType(systemicProcessType).Each(systemicProcess =>
         {
            var processSelection = new SystemicProcessSelection {ProcessName = systemicProcess.Name, ProcessType = systemicProcess.SystemicProcessType};
            processSelectionGroup.AddSystemicProcessSelection(processSelection);
         });
      }

      public Model.Simulation CreateSimulation(SimulationConstruction simulationConstruction, Action<Model.Simulation> preModelCreationAction = null)
      {
         var sim = CreateModelLessSimulationWith(simulationConstruction);
         if (preModelCreationAction != null)
            preModelCreationAction(sim);

         AddModelToSimulation(sim);
         return sim;
      }

      public void AddModelToSimulation(Model.Simulation simulation)
      {
         _simulationModelCreator.CreateModelFor(simulation);

         simulation.Solver.Parameter(Constants.Parameters.ABS_TOL).Value = CoreConstants.DEFAULT_ABS_TOL;
         simulation.Solver.Parameter(Constants.Parameters.REL_TOL).Value = CoreConstants.DEFAULT_REL_TOL;

         _registrationTask.Register(simulation);
      }
   }
}