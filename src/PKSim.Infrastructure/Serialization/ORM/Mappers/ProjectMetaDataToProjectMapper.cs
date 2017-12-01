using System;
using OSPSuite.Core;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Visitor;
using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure.Serialization.ORM.MetaData;
using PKSim.Infrastructure.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.ParameterIdentifications;
using OSPSuite.Core.Domain.SensitivityAnalyses;
using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Infrastructure.Serialization.ORM.MetaData;
using PKSim.Core;

namespace PKSim.Infrastructure.Serialization.ORM.Mappers
{
   public interface IProjectMetaDataToProjectMapper : IMapper<ProjectMetaData, PKSimProject>
   {
   }

   public class ProjectMetaDataToProjectMapper : IProjectMetaDataToProjectMapper,
      IStrictVisitor,
      IVisitor<CompoundMetaData>,
      IVisitor<IndividualMetaData>,
      IVisitor<SimulationMetaData>,
      IVisitor<ProtocolMetaData>,
      IVisitor<FormulationMetaData>,
      IVisitor<EventMetaData>,
      IVisitor<RandomPopulationMetaData>,
      IVisitor<ImportPopulationMetaData>

   {
      private readonly ISimulationMetaDataToSimulationMapper _simulationMapper;
      private readonly ICompressedSerializationManager _serializationManager;
      private readonly ISerializationContextFactory _serializationContextFactory;
      private IPKSimBuildingBlock _buildingBlock;

      public ProjectMetaDataToProjectMapper(ISimulationMetaDataToSimulationMapper simulationMapper, ICompressedSerializationManager serializationManager, ISerializationContextFactory serializationContextFactory)
      {
         _simulationMapper = simulationMapper;
         _serializationManager = serializationManager;
         _serializationContextFactory = serializationContextFactory;
      }

      public PKSimProject MapFrom(ProjectMetaData projectMetaData)
      {
         var project = new PKSimProject
         {
            Name = projectMetaData.Name,
            Description = projectMetaData.Description
         };

         //Observed data needs to be loaded first into project
         projectMetaData.AllObservedData.Each(x => project.AddObservedData(mapFrom(x)));

         projectMetaData.BuildingBlocks.Each(x => project.AddBuildingBlock(mapFrom(x)));

         //we need a shared context for all object referencing observed data and simulations
         using (var context = _serializationContextFactory.Create(project.AllObservedData, project.All<ISimulation>()))
         {
            var localContext = context;
            projectMetaData.ParameterIdentifications.Each(x => project.AddParameterIdentification(mapFrom(x, localContext)));
            projectMetaData.SensitivityAnalyses.Each(x => project.AddSensitivityAnalysis(mapFrom(x, localContext)));
         }

         projectMetaData.SimulationComparisons.Each(x => project.AddSimulationComparison(mapFrom(x)));

         //Once reference to dynamic meta data was added, deserialize the project itself
         _serializationManager.Deserialize(project, projectMetaData.Content.Data);

         //if the project DB Version is the same as the current project, the project did not change
         if (projectMetaData.Version == ProjectVersions.Current)
            project.HasChanged = false;

         return project;
      }

      private DataRepository mapFrom(ObservedDataMetaData observedDataMetaData)
      {
         return _serializationManager.Deserialize<DataRepository>(observedDataMetaData.DataRepository.Content.Data);
      }

      private ParameterIdentification mapFrom(ParameterIdentificationMetaData parameterIdentificationMetaData, SerializationContext context)
      {
         return new ParameterIdentification
         {
            Id = parameterIdentificationMetaData.Id,
            Name = parameterIdentificationMetaData.Name,
            Description = parameterIdentificationMetaData.Description,
            OutputMappings = deserializeProperty<OutputMappings>(parameterIdentificationMetaData, context)
         };
      }

      private SensitivityAnalysis mapFrom(SensitivityAnalysisMetaData sensitivityAnalysisMetaData, SerializationContext context)
      {
         return new SensitivityAnalysis
         {
            Id = sensitivityAnalysisMetaData.Id,
            Name = sensitivityAnalysisMetaData.Name,
            Description = sensitivityAnalysisMetaData.Description,
         };
      }

      private ISimulationComparison mapFrom(SimulationComparisonMetaData simulationComparisonMetaData)
      {
         ISimulationComparison simulationComparison;
         if (simulationComparisonMetaData.IsAnImplementationOf<IndividualSimulationComparisonMetaData>())
            simulationComparison = new IndividualSimulationComparison();

         else if (simulationComparisonMetaData.IsAnImplementationOf<PopulationSimulationComparisonMetaData>())
            simulationComparison = new PopulationSimulationComparison();
         else
            throw new ArgumentException($"Unable to serialize simulation comparison of type {simulationComparisonMetaData.GetType()}");

         return simulationComparison
            .WithId(simulationComparisonMetaData.Id)
            .WithName(simulationComparisonMetaData.Name)
            .WithDescription(simulationComparisonMetaData.Description);
      }

      private IPKSimBuildingBlock mapFrom(BuildingBlockMetaData buildingBlockMetaData)
      {
         try
         {
            _buildingBlock = null;
            this.Visit(buildingBlockMetaData);
            if (_buildingBlock == null)
               return null;

            _buildingBlock.Id = buildingBlockMetaData.Id;
            _buildingBlock.Name = buildingBlockMetaData.Name;
            _buildingBlock.Version = buildingBlockMetaData.Version;
            _buildingBlock.StructureVersion = buildingBlockMetaData.StructureVersion;
            _buildingBlock.Description = buildingBlockMetaData.Description;
            _buildingBlock.Icon = buildingBlockMetaData.Icon;

            if (!buildingBlockIsLazyLoaded(_buildingBlock))
               deserialize(buildingBlockMetaData);

            return _buildingBlock;
         }
         finally
         {
            _buildingBlock = null;
         }
      }

      private void deserialize(BuildingBlockMetaData buildingBlockMetaData)
      {
         _serializationManager.Deserialize(_buildingBlock, buildingBlockMetaData.Content.Data);
      }

      private bool buildingBlockIsLazyLoaded(IPKSimBuildingBlock buildingBlock)
      {
         return buildingBlock.BuildingBlockType == PKSimBuildingBlockType.Simulation ||
                buildingBlock.BuildingBlockType == PKSimBuildingBlockType.Population ||
                buildingBlock.BuildingBlockType == PKSimBuildingBlockType.Individual;
      }

      private T deserializeProperty<T>(IMetaDataWithProperties metaData, SerializationContext serializationContext = null)
      {
         var propertyData = metaData.Properties?.Data;

         if (propertyData == null)
            return default(T);

         return _serializationManager.Deserialize<T>(propertyData, serializationContext);
      }

      public void Visit(CompoundMetaData objToVisit)
      {
         _buildingBlock = new Compound();
      }

      public void Visit(IndividualMetaData individualMetaData)
      {
         _buildingBlock = new Individual
         {
            OriginData = deserializeProperty<OriginData>(individualMetaData)
         };
      }

      public void Visit(SimulationMetaData simulationMetaData)
      {
         _buildingBlock = _simulationMapper.MapFrom(simulationMetaData);
      }

      public void Visit(ProtocolMetaData protocol)
      {
         switch (protocol.ProtocolMode)
         {
            case ProtocolMode.Simple:
               _buildingBlock = new SimpleProtocol();
               break;
            case ProtocolMode.Advanced:
               _buildingBlock = new AdvancedProtocol();
               break;
            default:
               throw new ArgumentOutOfRangeException();
         }
      }

      public void Visit(FormulationMetaData formulationMetaData)
      {
         _buildingBlock = new Formulation {FormulationType = formulationMetaData.FormulationType};
      }

      public void Visit(RandomPopulationMetaData randomPopulationMetaData)
      {
         _buildingBlock = new RandomPopulation
         {
            Settings = _serializationManager.Deserialize<RandomPopulationSettings>(randomPopulationMetaData.Properties.Data)
         };
      }

      public void Visit(EventMetaData objToVisit)
      {
         _buildingBlock = new PKSimEvent();
      }

      public void Visit(ImportPopulationMetaData importPopulation)
      {
         var population = new ImportPopulation();
         _serializationManager.Deserialize(population.Settings, importPopulation.Properties.Data);
         _buildingBlock = population;
      }
   }
}