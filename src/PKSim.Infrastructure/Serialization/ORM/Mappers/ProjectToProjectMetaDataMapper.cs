using System;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Visitor;
using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure.Serialization.ORM.MetaData;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.ParameterIdentifications;
using OSPSuite.Core.Domain.SensitivityAnalyses;
using OSPSuite.Infrastructure.Serialization.ORM.MetaData;

namespace PKSim.Infrastructure.Serialization.ORM.Mappers
{
   /// <summary>
   ///    Map a project to its meta data representation
   /// </summary>
   public interface IProjectToProjectMetaDataMapper : IMapper<PKSimProject, ProjectMetaData>
   {
   }

   /// <inherit />
   public class ProjectToProjectMetaDataMapper : IProjectToProjectMetaDataMapper,
      IStrictVisitor,
      IVisitor<Compound>,
      IVisitor<Individual>,
      IVisitor<IndividualSimulation>,
      IVisitor<PopulationSimulation>,
      IVisitor<AdvancedProtocol>,
      IVisitor<SimpleProtocol>,
      IVisitor<Formulation>,
      IVisitor<RandomPopulation>,
      IVisitor<PKSimEvent>,
      IVisitor<ImportPopulation>
   {
      private readonly ICompressedSerializationManager _serializationManager;
      private readonly ISimulationToSimulationMetaDataMapper _simulationMetaDataMapper;
      private BuildingBlockMetaData _metaData;

      public ProjectToProjectMetaDataMapper(ICompressedSerializationManager serializationManager, ISimulationToSimulationMetaDataMapper simulationMetaDataMapper)
      {
         _serializationManager = serializationManager;
         _simulationMetaDataMapper = simulationMetaDataMapper;
      }

      public ProjectMetaData MapFrom(PKSimProject project)
      {
         var projectMetaData = new ProjectMetaData();
         project.All<IPKSimBuildingBlock>().Each(x => projectMetaData.AddBuildingBlock(mapFrom(x)));
         project.AllSimulationComparisons.Each(x => projectMetaData.AddSimulationComparison(mapFrom(x)));
         project.AllParameterIdentifications.Each(x => projectMetaData.AddParameterIdentification(mapFrom(x)));
         project.AllSensitivityAnalyses.Each(x => projectMetaData.AddSensitivityAnalysis(mapFrom(x)));
         project.AllObservedData.Each(x => projectMetaData.AddObservedData(mapFrom(x)));

         projectMetaData.Name = project.Name;
         projectMetaData.Description = project.Description;
         projectMetaData.Content.Data = _serializationManager.Serialize(project);
         _metaData = null;
         return projectMetaData;
      }

      private SimulationComparisonMetaData mapFrom(ISimulationComparison simulationComparison)
      {
         SimulationComparisonMetaData metaData;
         if (simulationComparison.IsAnImplementationOf<IndividualSimulationComparison>())
            metaData = new IndividualSimulationComparisonMetaData();
         else if (simulationComparison.IsAnImplementationOf<PopulationSimulationComparison>())
            metaData = new PopulationSimulationComparisonMetaData();
         else
            throw new ArgumentException($"Unable to serialize simulation comparison of type {simulationComparison.GetType()}");

         mapObjectBase(metaData, simulationComparison);

         if (!simulationComparison.IsLoaded)
            return metaData;

         serializeContent(metaData, simulationComparison, compress: true);
         return metaData;
      }

      private ParameterIdentificationMetaData mapFrom(ParameterIdentification parameterIdentification)
      {
         var metaData = new ParameterIdentificationMetaData
         {
            Properties = {Data = _serializationManager.Serialize(parameterIdentification.OutputMappings)},
         };
         mapObjectBase(metaData, parameterIdentification);

         if (!parameterIdentification.IsLoaded)
            return metaData;

         if (!parameterIdentification.HasChanged)
            return metaData;

         serializeContent(metaData, parameterIdentification, compress: true);
         return metaData;
      }

      private SensitivityAnalysisMetaData mapFrom(SensitivityAnalysis sensitivityAnalysis)
      {
         var metaData = new SensitivityAnalysisMetaData();
         mapObjectBase(metaData, sensitivityAnalysis);

         if (!sensitivityAnalysis.IsLoaded)
            return metaData;

         if (!sensitivityAnalysis.HasChanged)
            return metaData;

         serializeContent(metaData, sensitivityAnalysis, compress: true);
         return metaData;
      }

      private ObservedDataMetaData mapFrom(DataRepository observedData)
      {
         var dataRepositoryMetaData = new DataRepositoryMetaData
         {
            Content = {Data = _serializationManager.Serialize(observedData, compress: true)}
         };
         mapObjectBase(dataRepositoryMetaData,observedData);

         return new ObservedDataMetaData
         {
            Id = observedData.Id,
            DataRepository = dataRepositoryMetaData
         };
      }

      private BuildingBlockMetaData mapFrom(IPKSimBuildingBlock buildingBlock)
      {
         _metaData = null;
         try
         {
            this.Visit(buildingBlock);
            if (_metaData == null) return null;
            mapObjectBase(_metaData, buildingBlock);
            _metaData.Version = buildingBlock.Version;
            _metaData.StructureVersion = buildingBlock.StructureVersion;
            _metaData.Icon = buildingBlock.Icon;
            return _metaData;
         }
         finally
         {
            _metaData = null;
         }
      }

      private void mapObjectBase<T>(ObjectBaseMetaData<T> metaData, IObjectBase objectBase) where T : ObjectBaseMetaData<T>
      {
         metaData.Id = objectBase.Id;
         metaData.Name = objectBase.Name;
         metaData.Description = objectBase.Description;
      }

      public void Visit(Compound compound)
      {
         _metaData = new CompoundMetaData();
         serializeContentFor(compound);
      }

      public void Visit(Individual individual)
      {
         _metaData = new IndividualMetaData();
         serializeContentFor(individual);
         _metaData.Properties.Data = _serializationManager.Serialize(individual.OriginData);
      }

      private void serializeContentFor(IPKSimBuildingBlock buildingBlock, bool compress = true)
      {
         if (!buildingBlock.IsLoaded) return;
         if (!buildingBlock.HasChanged) return;
         serializeContent(_metaData, buildingBlock, compress);
      }

      private void serializeContent<TObject>(MetaDataWithContent<string> metaDataWithContent, TObject objectToSerialize, bool compress = true)
      {
         metaDataWithContent.Content.Data = _serializationManager.Serialize(objectToSerialize, compress);
      }

      public void Visit(IndividualSimulation simulation)
      {
         _metaData = _simulationMetaDataMapper.MapFrom(simulation);
         serializeContentFor(simulation);
      }

      public void Visit(PopulationSimulation populationSimulation)
      {
         _metaData = _simulationMetaDataMapper.MapFrom(populationSimulation);
         serializeContentFor(populationSimulation);
      }

      public void Visit(AdvancedProtocol protocol)
      {
         _metaData = new ProtocolMetaData {ProtocolMode = ProtocolMode.Advanced};
         serializeContentFor(protocol);
      }

      public void Visit(SimpleProtocol protocol)
      {
         _metaData = new ProtocolMetaData {ProtocolMode = ProtocolMode.Simple};
         serializeContentFor(protocol);
      }

      public void Visit(Formulation formulation)
      {
         _metaData = new FormulationMetaData {FormulationType = formulation.FormulationType};
         serializeContentFor(formulation);
      }

      public void Visit(RandomPopulation randomPopulation)
      {
         _metaData = new RandomPopulationMetaData();
         //no need to compress population data. This is mostly only double arrays
         serializeContentFor(randomPopulation, compress: false);
         _metaData.Properties.Data = _serializationManager.Serialize(randomPopulation.Settings);
      }

      public void Visit(PKSimEvent pkSimEvent)
      {
         _metaData = new EventMetaData();
         serializeContentFor(pkSimEvent);
      }

      public void Visit(ImportPopulation importPopulation)
      {
         _metaData = new ImportPopulationMetaData();
         //no need to compress population data. This is mostly only double arrays
         serializeContentFor(importPopulation, compress: false);
         _metaData.Properties.Data = _serializationManager.Serialize(importPopulation.Settings);
      }
   }
}