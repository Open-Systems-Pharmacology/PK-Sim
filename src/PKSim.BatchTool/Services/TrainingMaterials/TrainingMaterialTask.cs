using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Batch;
using PKSim.Core.Batch.Mapper;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Core;
using PKSim.Presentation.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Services;
using Compound = PKSim.Core.Model.Compound;
using Individual = PKSim.Core.Model.Individual;
using Simulation = PKSim.Core.Model.Simulation;

namespace PKSim.BatchTool.Services.TrainingMaterials
{
   public class TrainingMaterialTask
   {
      private readonly IBuildingBlockTask _buildingBlockTask;
      private readonly IWorkspace _workspace;
      private readonly IBatchLogger _logger;
      private readonly IBatchToCoreMapper _mapper;
      private readonly ISimulationEngineFactory _simulationEngineFactory;
      private readonly ISimulationConstructor _simulationConstructor;
      private readonly ISimulationSettingsRetriever _simulationSettingsRetriever;
      private readonly IObservedDataTask _observedDataTask;
      private readonly IProjectTask _projectTask;
      private readonly IParameterAlternativeFactory _parameterAlternativeFactory;
      private readonly ICompoundAlternativeTask _compoundAlternativeTask;
      private readonly ITrainingObservedDataRepository _observedDataRepository;

      public TrainingMaterialTask(IBuildingBlockTask buildingBlockTask, IWorkspace workspace, IBatchLogger logger, IBatchToCoreMapper mapper,
         ISimulationEngineFactory simulationEngineFactory, ISimulationConstructor simulationConstructor, ISimulationSettingsRetriever simulationSettingsRetriever,
         IObservedDataTask observedDataTask, IProjectTask projectTask, IParameterAlternativeFactory parameterAlternativeFactory, 
         ICompoundAlternativeTask compoundAlternativeTask, ITrainingObservedDataRepository observedDataRepository)
      {
         _buildingBlockTask = buildingBlockTask;
         _workspace = workspace;
         _logger = logger;
         _mapper = mapper;
         _simulationEngineFactory = simulationEngineFactory;
         _simulationConstructor = simulationConstructor;
         _simulationSettingsRetriever = simulationSettingsRetriever;
         _observedDataTask = observedDataTask;
         _projectTask = projectTask;
         _parameterAlternativeFactory = parameterAlternativeFactory;
         _compoundAlternativeTask = compoundAlternativeTask;
         _observedDataRepository = observedDataRepository;
      }

      public Task ExecuteTask(Action action)
      {
         return Task.Run(() =>
         {
            _projectTask.NewProject();

            action();

            _workspace.CloseProject();
         });
      }
      public void UpdateDisplayUnit(IWithDisplayUnit withDisplayUnit, string unit)
      {
         withDisplayUnit.DisplayUnit = withDisplayUnit.Dimension.UnitOrDefault(unit);
      }

      public void AddBuildingBlockToProjectAndSave(IPKSimBuildingBlock buildingBlock, string outputFolder, string projectName)
      {
         AddBuildingBlockToProject(buildingBlock);
         SaveCurrentProjectUnder(outputFolder, projectName);
      }

      public void AddBuildingBlockToProject(IPKSimBuildingBlock buildingBlock)
      {
         _buildingBlockTask.AddToProject(buildingBlock);
      }

      public ClassifiableSimulation AddSimulationClassificationFor(Simulation simulation)
      {
         return AddClassisificationFor<ClassifiableSimulation, Simulation>(simulation);
      }

      public ClassifiableObservedData AddObservedDataClassificationFor(DataRepository observedData)
      {
         return AddClassisificationFor<ClassifiableObservedData, DataRepository>(observedData);
      }

      public TClassifiable AddClassisificationFor<TClassifiable, TSubject>(TSubject simulation)
         where TClassifiable : Classifiable<TSubject>, new()
         where TSubject : IWithId, IWithName
      {
         return _workspace.Project.GetOrCreateClassifiableFor<TClassifiable, TSubject>(simulation);
      }

      public TBuildingBlock FindByName<TBuildingBlock>(string name) where TBuildingBlock : class, IWithName
      {
         return _workspace.Project.All<TBuildingBlock>().FindByName(name);
      }

      public Compound Compound(string compoundName)
      {
         return FindByName<Compound>(compoundName);
      }

      public Individual Individual(string individualName)
      {
         return FindByName<Individual>(individualName);
      }

      public Protocol Protocol(string protocolName)
      {
         return FindByName<Protocol>(protocolName);
      }

      public Simulation Simulation(string simulationName)
      {
         return FindByName<Simulation>(simulationName);
      }

      public DataRepository ObservedData(string observedDataName)
      {
         return _workspace.Project.AllObservedData.FindByName(observedDataName);
      }

      public TBuildingBlock First<TBuildingBlock>() where TBuildingBlock : class, IWithName
      {
         return _workspace.Project.All<TBuildingBlock>().First();
      }

      public void SaveCurrentProjectUnder(string outputFolder, string projectName)
      {
         var fileName = Path.Combine(outputFolder, projectName + CoreConstants.Filter.PROJECT_EXTENSION);
         _workspace.SaveProject(fileName);
         _logger.AddInfo($"Saving project '{projectName}'");
      }

      public TCoreObject MapFrom<TCoreObject>(object batchBuilingBlock, params object[] parameters) where TCoreObject : class
      {
         return _mapper.MapFrom<TCoreObject>(batchBuilingBlock, parameters);
      }

      public void RunSimulation(Simulation simulation)
      {
         var simnulationEngine = _simulationEngineFactory.Create<IndividualSimulation>();
         simnulationEngine.Run(simulation.DowncastTo<IndividualSimulation>());
      }

      public Simulation CreateSimulation(SimulationConstruction simulationConstruction, Action<Simulation> preModelCreationAction = null)
      {
         var simulation =  _simulationConstructor.CreateSimulation(simulationConstruction, preModelCreationAction);
         AddSimulationClassificationFor(simulation);
         return simulation;
      }

      public void SelectDefaultOutputFor(Simulation simulation)
      {
         _simulationSettingsRetriever.CreatePKSimDefaults(simulation);
      }

      public void AddCommand(ICommand command)
      {
         _workspace.AddCommand(command);
      }

      public DataRepository AddObservedDataToProject(string observedDataName)
      {
         var observedData = _observedDataRepository.FindByName(observedDataName);
         _observedDataTask.AddObservedDataToProject(observedData);
         AddObservedDataClassificationFor(observedData);
         return observedData;
      }

      public void AddObservedDataToAnalysable(IReadOnlyList<DataRepository> observedData, IAnalysable analysable)
      {
         _observedDataTask.AddObservedDataToAnalysable(observedData, analysable);
      }

      public ParameterAlternative CreateCompoundAlternative(Compound compound, string group, string alternativeName, double value, string parameterName, bool setDefault = true)
      {
         var alternativeGroup = compound.ParameterAlternativeGroup(group);
         var alternative = _parameterAlternativeFactory.CreateAlternativeFor(alternativeGroup).WithName(alternativeName);
         AddCommand(_compoundAlternativeTask.AddParameterGroupAlternativeTo(alternativeGroup, alternative));
         AddCommand(_compoundAlternativeTask.SetAlternativeParameterValue(alternative.Parameter(parameterName), value));
         if (setDefault)
            AddCommand(_compoundAlternativeTask.SetDefaultAlternativeFor(alternativeGroup, alternative));

         return alternative;
      }

      public ParameterAlternative CreateLipophilicityAlternative(Compound compound, string alternativeName, double value, bool setDefault = true)
      {
         return CreateCompoundAlternative(compound, CoreConstants.Groups.COMPOUND_LIPOPHILICITY, alternativeName, value, CoreConstants.Parameter.LIPOPHILICITY, setDefault);
      }

      public SimulationConstruction CreateSimulationConstruction(string simulationName, string simulationSubjectName, IReadOnlyList<string> compoundNames, IReadOnlyList<string> protocolNames, string modelName = CoreConstants.Model.FourComp  )
      {
         var simulationSubject = FindByName<ISimulationSubject>(simulationSubjectName);
         var configuration = new SimulationConfiguration {Model = modelName};
         var modelProperties = MapFrom<ModelProperties>(configuration, simulationSubject);

         return new SimulationConstruction
         {
            SimulationSubject = simulationSubject,
            TemplateCompounds = compoundNames.Select(Compound).ToArray(),
            TemplateProtocols = protocolNames.Select(Protocol).ToArray(),
            ModelProperties = modelProperties,
            SimulationName = simulationName
         };
      }


   }
}