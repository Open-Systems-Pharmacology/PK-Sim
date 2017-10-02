using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Visitor;
using PKSim.Core.Chart;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Diagram;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public abstract class Simulation : PKSimBuildingBlock, ISimulation, IWithChartTemplates
   {
      private readonly ICache<string, UsedBuildingBlock> _usedBuildingBlocks = new Cache<string, UsedBuildingBlock>(bb => bb.TemplateId);
      private readonly ICache<string, UsedObservedData> _usedObservedData = new Cache<string, UsedObservedData>(bb => bb.Id);
      private readonly List<ISimulationAnalysis> _allSimulationAnalyses = new List<ISimulationAnalysis>();
      private SimulationProperties _properties;
      private SimulationResults _results;

      /// <summary>
      ///    Returns the version that the simulation had when it was run and the simulation was saved
      /// </summary>
      public virtual int ResultsVersion { get; set; }

      /// <summary>
      ///    underlying Core Model used to run the simulation
      /// </summary>
      public virtual IModel Model { get; set; }

      public virtual IDiagramModel ReactionDiagramModel { get; set; }

      public virtual ISimulationSettings SimulationSettings { get; set; }

      /// <summary>
      ///    The reaction building block used to create the simulation. This is only use as meta information
      ///    on model creation for now. Adding <see cref="Reaction" /> to the building block will not change the model structure
      /// </summary>
      public virtual IReactionBuildingBlock Reactions { get; set; }

      protected Simulation() : base(PKSimBuildingBlockType.Simulation)
      {
         ContainerType = ContainerType.Simulation;
         _results = new NullSimulationResults();
         ResultsHaveChanged = false;
      }

      /// <summary>
      ///    Returns all building blocks used by the simulation.
      /// </summary>
      public virtual IEnumerable<UsedBuildingBlock> UsedBuildingBlocks => _usedBuildingBlocks;

      /// <summary>
      ///    Returns the building block with the given building block id
      /// </summary>
      public virtual UsedBuildingBlock UsedBuildingBlockById(string usedBuildingBlockId)
      {
         return _usedBuildingBlocks.FirstOrDefault(x => x.Id == usedBuildingBlockId);
      }

      /// <summary>
      ///    Returns <see cref="UsedBuildingBlock" /> using the <paramref name="buildingBlockInSimulation" /> as BuildingBlock
      /// </summary>
      public virtual UsedBuildingBlock UsedBuildingBlockBy(IPKSimBuildingBlock buildingBlockInSimulation)
      {
         if (buildingBlockInSimulation == null)
            return null;

         return UsedBuildingBlockById(buildingBlockInSimulation.Id);
      }

      /// <summary>
      ///    Returns the template id of the <see cref="IPKSimBuildingBlock" /> used by the simulation
      ///    <see cref="IPKSimBuildingBlock" /> <paramref name="buildingBlockInSimulation" />
      /// </summary>
      public virtual string TemplateBuildingBlockIdUsedBy(IPKSimBuildingBlock buildingBlockInSimulation)
      {
         var usedBuildingBlock = UsedBuildingBlockBy(buildingBlockInSimulation);
         if (usedBuildingBlock == null)
            return string.Empty;

         return usedBuildingBlock.TemplateId;
      }

      /// <summary>
      ///    Returns <see cref="UsedBuildingBlock" /> with the given building block template id
      /// </summary>
      public virtual UsedBuildingBlock UsedBuildingBlockByTemplateId(string templateId)
      {
         return _usedBuildingBlocks.Contains(templateId) ? _usedBuildingBlocks[templateId] : null;
      }

      /// <summary>
      ///    Returns <see cref="UsedBuildingBlock" /> with the given building block template id
      /// </summary>
      public virtual TBuildingBlock BuildingBlockByTemplateId<TBuildingBlock>(string templateId) where TBuildingBlock : class, IPKSimBuildingBlock
      {
         var usedBuildingBlock = UsedBuildingBlockByTemplateId(templateId);
         return usedBuildingBlock?.BuildingBlock as TBuildingBlock;
      }

      /// <summary>
      ///    return true if the building block with the given id was altered in the simulation otherwise false
      /// </summary>
      /// <param name="usedBuildingBlockId">id of the building block for which the altered status is retrieved</param>
      public virtual bool GetAltered(string usedBuildingBlockId)
      {
         return UsedBuildingBlockById(usedBuildingBlockId).Altered;
      }

      /// <summary>
      ///    Set the altered status of the building block with the id builingBlockId in the simulation to the altered value.
      ///    <param name="altered">the value of the altered status</param>
      ///    <param name="usedBuildingBlockId">id of the building block for which the status should be changed</param>
      /// </summary>
      public virtual void SetAltered(string usedBuildingBlockId, bool altered)
      {
         UsedBuildingBlockById(usedBuildingBlockId).Altered = altered;
      }

      /// <summary>
      ///    return all parameters defined with the given building block type
      /// </summary>
      public virtual IEnumerable<IParameter> ParametersOfType(PKSimBuildingBlockType buildingBlockType)
      {
         //Do not need neighborhood parameters, since neighborhood is defined a a child of root
         return All<IParameter>().Where(p => p.IsOfType(buildingBlockType));
      }

      /// <summary>
      ///    Returns all entities of the given type defined in the simulation (from config entities to model entities)
      /// </summary>
      public virtual IEnumerable<TEntity> All<TEntity>() where TEntity : class, IEntity
      {
         return GetAllChildren<TEntity>()
            .Union(Model.Root.GetAllChildren<TEntity>())
            .Union(allFromSettings<TEntity>());
      }

      private IEnumerable<TEntity> allFromSettings<TEntity>() where TEntity : class, IEntity
      {
         if (SimulationSettings == null)
            return Enumerable.Empty<TEntity>();

         return OutputSchema.GetAllChildren<TEntity>()
            .Union(Solver.GetAllChildren<TEntity>());
      }

      /// <summary>
      ///    Add a building block as being used by the simulation
      /// </summary>
      public virtual void AddUsedBuildingBlock(UsedBuildingBlock usedBuildingBlock)
      {
         if (usedBuildingBlock == null) return;

         var existingUsedBuildignBlock = UsedBuildingBlockByTemplateId(usedBuildingBlock.TemplateId);
         if (existingUsedBuildignBlock != null)
         {
            usedBuildingBlock.UpdateVersionFrom(existingUsedBuildignBlock);
            _usedBuildingBlocks.Remove(usedBuildingBlock.TemplateId);
         }

         _usedBuildingBlocks.Add(usedBuildingBlock);
      }

      /// <summary>
      ///    Remove the building block as used in the simulation
      /// </summary>
      public virtual void RemoveUsedBuildingBlock(IPKSimBuildingBlock buildingBlock)
      {
         var usedBuildingBlock = UsedBuildingBlockBy(buildingBlock);
         if (usedBuildingBlock == null) return;
         RemoveUsedBuildingBlock(usedBuildingBlock);
      }

      /// <summary>
      ///    Remove the building block as used in the simulation
      /// </summary>
      public virtual void RemoveUsedBuildingBlock(UsedBuildingBlock usedBuildingBlock)
      {
         if (usedBuildingBlock == null) return;
         RemoveUsedBuildingBlock(usedBuildingBlock.TemplateId);
      }

      /// <summary>
      ///    Remove the building block as used in the simulation using the templateId
      /// </summary>
      public virtual void RemoveUsedBuildingBlock(string buildingBlockTemplateId)
      {
         _usedBuildingBlocks.Remove(buildingBlockTemplateId);
      }

      /// <summary>
      ///    Remove the building blocks as used in the simulation by type
      /// </summary>
      public virtual void RemoveAllBuildingBlockOfType(PKSimBuildingBlockType buildingBlockType)
      {
         var allBuidlingBlocksOfType = _usedBuildingBlocks.Where(x => x.BuildingBlockType.Is(buildingBlockType)).ToList();
         allBuidlingBlocksOfType.Each(RemoveUsedBuildingBlock);
      }

      /// <summary>
      ///    Return true if the simulation uses the given building block otherwise false.
      /// </summary>
      public virtual bool UsesBuildingBlock(UsedBuildingBlock usedBuildingBlock)
      {
         return UsesBuildingBlock(usedBuildingBlock.TemplateId);
      }

      /// <summary>
      ///    Return true if the simulation uses the given building block id otherwise false.
      /// </summary>
      public virtual bool UsesBuildingBlock(string templateId)
      {
         return _usedBuildingBlocks.Contains(templateId);
      }

      /// <summary>
      ///    add a analysis to the simulation
      /// </summary>
      public virtual void AddAnalysis(ISimulationAnalysis simulationAnalysis)
      {
         _allSimulationAnalyses.Add(simulationAnalysis);
         simulationAnalysis.Analysable = this;
         HasChanged = true;
      }

      public virtual void AddAnalyses(IEnumerable<ISimulationAnalysis> simulationAnalyses)
      {
         simulationAnalyses.Each(AddAnalysis);
      }

      /// <summary>
      ///    Returns all used observed data in the simulation
      /// </summary>
      public virtual IEnumerable<UsedObservedData> UsedObservedData => _usedObservedData;

      /// <summary>
      ///    All analyses defined for the simulation
      /// </summary>
      public virtual IEnumerable<ISimulationAnalysis> Analyses => _allSimulationAnalyses;


      /// <summary>
      ///    All analyses defined for the simulation
      /// </summary>
      public virtual IEnumerable<T> AnalysesOfType<T>() where T : ISimulationAnalysis => _allSimulationAnalyses.OfType<T>();

      /// <summary>
      ///    All charts defined for the simulation
      /// </summary>
      public virtual IEnumerable<CurveChart> Charts => _allSimulationAnalyses.OfType<CurveChart>();

      /// <summary>
      ///    remove the chart from the simulation
      /// </summary>
      public virtual void RemoveAnalysis(ISimulationAnalysis simulationAnalysis)
      {
         _allSimulationAnalyses.Remove(simulationAnalysis);
         HasChanged = true;
      }

      public virtual void AddUsedObservedData(DataRepository dataRepository)
      {
         AddUsedObservedData(OSPSuite.Core.Domain.UsedObservedData.From(dataRepository));
         ChartWithObservedData.Each(c => c.AddObservedData(dataRepository));
      }

      private IEnumerable<IWithObservedData> analysesWithObservedData => _allSimulationAnalyses.OfType<IWithObservedData>();
      public virtual  IEnumerable<ChartWithObservedData> ChartWithObservedData => analysesWithObservedData.OfType<ChartWithObservedData>();

      public virtual void AddUsedObservedData(UsedObservedData usedObservedData)
      {
         if (usesObservedData(usedObservedData))
            return;

         _usedObservedData.Add(usedObservedData);
         usedObservedData.Simulation = this;
         HasChanged = true;
      }

      /// <summary>
      ///    Mark the observed data as unused in the simulation
      /// </summary>
      public virtual void RemoveUsedObservedData(DataRepository dataRepository)
      {
         if (!UsesObservedData(dataRepository))
            return;

         _usedObservedData.Remove(dataRepository.Id);
         analysesWithObservedData.Each(c => c.RemoveObservedData(dataRepository));
         HasChanged = true;
      }

      /// <summary>
      ///    returns true if the observed data is used in the current simulaton otherwise false
      /// </summary>
      public virtual bool UsesObservedData(DataRepository dataRepository)
      {
         return usesObservedData(OSPSuite.Core.Domain.UsedObservedData.From(dataRepository));
      }

      private bool usesObservedData(UsedObservedData usedObservedData)
      {
         return _usedObservedData.Contains(usedObservedData.Id);
      }

      /// <summary>
      ///    Results of the simulation, if the simulation was run
      /// </summary>
      public virtual SimulationResults Results
      {
         get => _results;
         set
         {
            _results = value ?? new NullSimulationResults();
            HasChanged = true;
            ResultsVersion = Version;
            ResultsHaveChanged = true;
         }
      }

      /// <summary>
      ///    Returns true if the simulation originated from PK-Sim. Otherwise false
      /// </summary>
      public virtual bool ComesFromPKSim => Origin == Origins.PKSim;

      /// <summary>
      ///    Returns true if the simulation was imported (e.g. throught pkml load). Otherwise false
      /// </summary>
      public virtual bool IsImported => ModelProperties == null || ModelConfiguration == null;

      public virtual IContainer ApplicationsContainer => Model?.Root?.Container(Constants.APPLICATIONS);

      /// <summary>
      ///    Returns true if the simulation results are uptodate.
      ///    (true: simulation was performed with current parameters, false: simulation parameters have changed ...)
      /// </summary>
      public virtual bool HasUpToDateResults => Version == ResultsVersion;

      public abstract bool HasResults { get; }

      /// <summary>
      ///    Returns true if a simulation run was performed and the results where updated since the simulation was loaded
      /// </summary>
      public virtual bool ResultsHaveChanged { get; set; }

      /// <summary>
      ///    Remove the available results and the corresponding charts
      /// </summary>
      public virtual void ClearResults()
      {
         _allSimulationAnalyses.Clear();
         Results.Clear();
      }

      public override string Name
      {
         get => base.Name;
         set
         {
            base.Name = value;
            setName(Model, value);
            setName(Model?.Root, value);
            setName(Reactions, value);
            setName(SimulationSettings, value);
         }
      }

      private void setName(IWithName withName, string value)
      {
         if (withName != null)
            withName.Name = value;
      }

      public override void UpdatePropertiesFrom(IUpdatable sourceObject, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(sourceObject, cloneManager);
         var sourceSimulation = sourceObject as Simulation;
         if (sourceSimulation == null) return;
         Properties = sourceSimulation.Properties.Clone(cloneManager);
         sourceSimulation.UsedBuildingBlocks.Each(bb => AddUsedBuildingBlock(bb.Clone(cloneManager)));
         Model = cloneManager.Clone(sourceSimulation.Model);
         sourceSimulation.UsedObservedData.Each(data => AddUsedObservedData(data.Clone()));
         Reactions = cloneManager.Clone(sourceSimulation.Reactions);
         SimulationSettings = cloneManager.Clone(sourceSimulation.SimulationSettings);
         ReactionDiagramModel = sourceSimulation.ReactionDiagramModel.CreateCopy();
         updateBuildingBlockReferences(sourceSimulation);
      }

      private void updateBuildingBlockReferences(Simulation sourceSimulation)
      {
         sourceSimulation.CompoundPropertiesList.Each(sourceCompoundProperties =>
         {
            var thisCompoundProperties = CompoundPropertiesFor(sourceCompoundProperties.Compound);
            if (thisCompoundProperties == null)
               return;

            thisCompoundProperties.Compound = correspondingBuildingBlock(sourceSimulation, sourceCompoundProperties.Compound);
            thisCompoundProperties.ProtocolProperties.Protocol = correspondingBuildingBlock(sourceSimulation, sourceCompoundProperties.ProtocolProperties.Protocol);
         });
      }

      private TBuildingBlock correspondingBuildingBlock<TBuildingBlock>(Simulation sourceSimulation, TBuildingBlock sourceBuildingBlock) where TBuildingBlock : class, IPKSimBuildingBlock
      {
         if (sourceBuildingBlock == null)
            return null;

         return BuildingBlockByTemplateId<TBuildingBlock>(sourceSimulation.TemplateBuildingBlockIdUsedBy(sourceBuildingBlock));
      }

      /// <summary>
      ///    Update the common settings from the original simulation. This is only used when configuring
      ///    a simulation based on another one (no clone involved)
      /// </summary>
      public virtual void UpdateFromOriginalSimulation(Simulation originalSimulation)
      {
         //no clone here. Just copy references
         Name = originalSimulation.Name;
         Description = originalSimulation.Description;
         SimulationSettings = originalSimulation.SimulationSettings;
         Version = originalSimulation.Version;
         StructureVersion = originalSimulation.StructureVersion;
         originalSimulation.UsedObservedData.Each(AddUsedObservedData);
      }

      /// <summary>
      ///    returns the total drug mass per body weight in [umol/kg BW]
      /// </summary>
      public virtual double? TotalDrugMassPerBodyWeightFor(string compoundName)
      {
         var totalDrugMass = TotalDrugMassFor(compoundName);
         if (totalDrugMass == null)
            return null;

         var bodyWeightParameter = BodyWeight;

         if (bodyWeightParameter?.Value > 0)
            return totalDrugMass.Value / bodyWeightParameter.Value;

         return null;
      }

      /// <summary>
      ///    Returns the endtime of the simulation in kernel unit
      /// </summary>
      public virtual double? EndTime
      {
         get { return OutputSchema.Intervals.Select(x => x.EndTime.Value).Max(); }
      }

      /// <summary>
      ///    gets or sets the simulation properties used to configure the simulation
      /// </summary>
      public virtual SimulationProperties Properties
      {
         get => _properties;
         set
         {
            _properties = value;
            _properties.Simulation = this;
         }
      }

      /// <summary>
      ///    Returns the Body weight <see cref="IParameter" /> if available in the simulation otherwise null.
      /// </summary>
      public virtual IParameter BodyWeight => Model.Root.EntityAt<IParameter>(Constants.ORGANISM, CoreConstants.Parameter.WEIGHT);

      /// <summary>
      ///    Returns the total drug mass defined in the simulation.
      /// </summary>
      public virtual double? TotalDrugMassFor(string compoundName)
      {
         //total drug mass is a parameter defined under the compound molecule global property
         var totalDrugMassParameter = Model.Root.EntityAt<IParameter>(compoundName, CoreConstants.Parameter.TotalDrugMass);
         return totalDrugMassParameter?.Value;
      }

      public override void AcceptVisitor(IVisitor visitor)
      {
         base.AcceptVisitor(visitor);
         if (!IsLoaded) return;

         _usedBuildingBlocks.Each(x => x.AcceptVisitor(visitor));
         Model?.AcceptVisitor(visitor);
         SimulationSettings?.AcceptVisitor(visitor);
         Charts.Each(x => x.AcceptVisitor(visitor));
      }

      /// <summary>
      ///    returns the building block used in the simulation with the given type
      /// </summary>
      /// <typeparam name="TBuildingBlock">type of the building block we are lookiung for in the simulation</typeparam>
      public abstract TBuildingBlock BuildingBlock<TBuildingBlock>() where TBuildingBlock : class, IPKSimBuildingBlock;

      /// <summary>
      ///    returns all building blocks used in the simulation of a given type
      /// </summary>
      /// <typeparam name="TBuildingBlock">type of the building blocks we are looking for in the simulation</typeparam>
      public virtual IEnumerable<TBuildingBlock> AllBuildingBlocks<TBuildingBlock>() where TBuildingBlock : class, IPKSimBuildingBlock
      {
         return from usedBb in UsedBuildingBlocks
            let bb = usedBb.BuildingBlock as TBuildingBlock
            where bb != null
            select bb;
      }

      /// <summary>
      ///    Returns the building block used in the simulation with the given building block type
      /// </summary>
      /// <typeparam name="TBuildingBlock">type of the building blocks we are looking for in the simulation</typeparam>
      public virtual UsedBuildingBlock UsedBuildingBlockInSimulation<TBuildingBlock>() where TBuildingBlock : class, IPKSimBuildingBlock
      {
         return UsedBuildingBlocksInSimulation<TBuildingBlock>().SingleOrDefault();
      }

      /// <summary>
      ///    Returns the building blocks used in the simulation with the given building block type
      /// </summary>
      /// <typeparam name="TBuildingBlock">type of the building blocks we are looking for in the simulation</typeparam>
      public virtual IEnumerable<UsedBuildingBlock> UsedBuildingBlocksInSimulation<TBuildingBlock>() where TBuildingBlock : class, IPKSimBuildingBlock
      {
         return from usedBb in UsedBuildingBlocks
            let bb = usedBb.BuildingBlock as TBuildingBlock
            where bb != null
            select usedBb;
      }

      /// <summary>
      ///    Returns the building block used in the simulation with the given building block type
      /// </summary>
      public virtual UsedBuildingBlock UsedBuildingBlockInSimulation(PKSimBuildingBlockType buildingBlockType)
      {
         return UsedBuildingBlocksInSimulation(buildingBlockType).SingleOrDefault();
      }

      public virtual string BuildingBlockName(PKSimBuildingBlockType buildingBlockType)
      {
         var usedBuildingBlock = UsedBuildingBlockInSimulation(buildingBlockType);
         return usedBuildingBlock == null ? string.Empty : usedBuildingBlock.Name;
      }

      /// <summary>
      ///    Returns all the buildng block with the given type used in the simulation
      /// </summary>
      public virtual IEnumerable<UsedBuildingBlock> UsedBuildingBlocksInSimulation(PKSimBuildingBlockType buildingBlockType)
      {
         return from usedBuildingBlock in UsedBuildingBlocks
            where usedBuildingBlock.BuildingBlockType.Is(buildingBlockType)
            select usedBuildingBlock;
      }

      public override bool IsLoaded
      {
         set
         {
            base.IsLoaded = value;
            UsedBuildingBlocks.Each(bb => bb.IsLoaded = value);
         }
      }

      #region Accessor Properties

      /// <summary>
      ///    Specifies if the simulation subject should age during simulation run
      /// </summary>
      public virtual bool AllowAging
      {
         get => Properties.AllowAging;
         set => Properties.AllowAging = value;
      }

      public virtual Origin Origin
      {
         get => Properties.Origin;
         set => Properties.Origin = value;
      }

      /// <summary>
      ///    Returns the settings for the simulation
      /// </summary>
      public virtual OutputSelections OutputSelections
      {
         get => SimulationSettings.OutputSelections;
         set => SimulationSettings.OutputSelections = value;
      }

      /// <summary>
      ///    return the model properties used in the simulation configuration
      /// </summary>
      public virtual ModelProperties ModelProperties
      {
         get => Properties.ModelProperties;
         set => Properties.ModelProperties = value;
      }

      /// <summary>
      ///    Returns the <see cref="OSPSuite.Core.Domain.OutputSchema" /> defined in the settings
      /// </summary>
      public virtual OutputSchema OutputSchema
      {
         get => SimulationSettings.OutputSchema;
         set => SimulationSettings.OutputSchema = value;
      }

      /// <summary>
      ///    Returns the <see cref="Solver" />  defined in the settings
      /// </summary>
      public virtual SolverSettings Solver
      {
         get => SimulationSettings.Solver;
         set => SimulationSettings.Solver = value;
      }

      public virtual IReadOnlyList<Compound> Compounds => AllBuildingBlocks<Compound>().ToList();

      public virtual IReadOnlyList<Protocol> Protocols => AllBuildingBlocks<Protocol>().ToList();

      public virtual IReadOnlyList<string> CompoundNames => Compounds.AllNames().ToList();

      /// <summary>
      ///    Returns the compound properties used in the simulation configuration
      /// </summary>
      public virtual IReadOnlyList<CompoundProperties> CompoundPropertiesList => Properties.CompoundPropertiesList;

      public virtual CompoundProperties CompoundPropertiesFor(Compound compound)
      {
         return CompoundPropertiesList.FirstOrDefault(x => Equals(compound, x.Compound));
      }

      public virtual CompoundProperties CompoundPropertiesFor(string compoundName)
      {
         return CompoundPropertiesFor(Compounds.FindByName(compoundName));
      }

      /// <summary>
      ///    Returns the model used in the simulation
      /// </summary>
      public virtual ModelConfiguration ModelConfiguration
      {
         get => ModelProperties.ModelConfiguration;
         set => ModelProperties.ModelConfiguration = value;
      }

      /// <summary>
      ///    Returns the events used in the simulation configuration
      /// </summary>
      public virtual EventProperties EventProperties
      {
         get => Properties.EventProperties;
         set => Properties.EventProperties = value;
      }

      /// <summary>
      ///    Returns the interactions used in the simulation configuration
      /// </summary>
      public virtual InteractionProperties InteractionProperties
      {
         get => Properties.InteractionProperties;
         set => Properties.InteractionProperties = value;
      }

      public virtual Individual Individual => BuildingBlock<Individual>();

      public IEnumerable<CurveChartTemplate> ChartTemplates => SimulationSettings.ChartTemplates;

      public CurveChartTemplate DefaultChartTemplate => SimulationSettings.DefaultChartTemplate;

      #endregion

      public void AddChartTemplate(CurveChartTemplate chartTemplate)
      {
         SimulationSettings.AddChartTemplate(chartTemplate);
      }

      public void RemoveChartTemplate(string chartTemplateName)
      {
         SimulationSettings.RemoveChartTemplate(chartTemplateName);
      }

      public CurveChartTemplate ChartTemplateByName(string templateName)
      {
         return SimulationSettings.ChartTemplateByName(templateName);
      }

      public void RemoveAllChartTemplates()
      {
         SimulationSettings.RemoveAllChartTemplates();
      }
   }
}