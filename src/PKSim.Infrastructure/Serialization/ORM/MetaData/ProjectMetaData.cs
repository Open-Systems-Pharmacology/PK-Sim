using System.Collections.Generic;
using NHibernate;
using PKSim.Core;
using OSPSuite.Infrastructure.Serialization.ORM.MetaData;

namespace PKSim.Infrastructure.Serialization.ORM.MetaData
{
   public class ProjectMetaData : MetaDataWithContent<int>, IUpdatableFrom<ProjectMetaData>
   {
      public virtual string Name { get; set; }
      public virtual int Version { get; set; }
      public virtual string Description { get; set; }
      public virtual ICollection<BuildingBlockMetaData> BuildingBlocks { get; }
      public virtual ICollection<SimulationComparisonMetaData> SimulationComparisons { get; }
      public virtual ICollection<ParameterIdentificationMetaData> ParameterIdentifications { get; set; }
      public virtual ICollection<SensitivityAnalysisMetaData> SensitivityAnalyses { get; set; }
      public virtual ICollection<ObservedDataMetaData> AllObservedData { get; set; }

      public ProjectMetaData()
      {
         Version = ProjectVersions.Current;
         BuildingBlocks = new HashSet<BuildingBlockMetaData>();
         SimulationComparisons = new HashSet<SimulationComparisonMetaData>();
         ParameterIdentifications = new HashSet<ParameterIdentificationMetaData>();
         SensitivityAnalyses = new HashSet<SensitivityAnalysisMetaData>();
         AllObservedData = new HashSet<ObservedDataMetaData>();
      }

      public virtual void UpdateFrom(ProjectMetaData sourceProject, ISession session)
      {
         Name = sourceProject.Name;
         Description = sourceProject.Description;
         Version = sourceProject.Version;
         UpdateContentFrom(sourceProject);
         BuildingBlocks.UpdateFrom<string, BuildingBlockMetaData>(sourceProject.BuildingBlocks, session);
         SimulationComparisons.UpdateFrom<string, SimulationComparisonMetaData>(sourceProject.SimulationComparisons, session);
         ParameterIdentifications.UpdateFrom<string, ParameterIdentificationMetaData>(sourceProject.ParameterIdentifications, session);
         SensitivityAnalyses.UpdateFrom<string, SensitivityAnalysisMetaData>(sourceProject.SensitivityAnalyses, session);
         AllObservedData.UpdateFrom<string, ObservedDataMetaData>(sourceProject.AllObservedData, session);
      }

      public virtual void AddBuildingBlock(BuildingBlockMetaData buildingBlock)
      {
         BuildingBlocks.Add(buildingBlock);
      }

      public virtual void AddSimulationComparison(SimulationComparisonMetaData simulationComparison)
      {
         SimulationComparisons.Add(simulationComparison);
      }

      public virtual void AddParameterIdentification(ParameterIdentificationMetaData parameterIdentification)
      {
         ParameterIdentifications.Add(parameterIdentification);
      }

      public virtual void AddSensitivityAnalysis(SensitivityAnalysisMetaData sensitivityAnalysisMetaData)
      {
         SensitivityAnalyses.Add(sensitivityAnalysisMetaData);
      }

      public virtual void AddObservedData(ObservedDataMetaData observedDataMetaData)
      {
         AllObservedData.Add(observedDataMetaData);
      }
   }
}