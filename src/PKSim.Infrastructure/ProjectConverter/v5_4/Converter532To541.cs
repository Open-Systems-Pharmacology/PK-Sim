using System.Linq;
using System.Xml.Linq;
using OSPSuite.Core.Converter.v5_4;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Visitor;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Repositories;

namespace PKSim.Infrastructure.ProjectConverter.v5_4
{
   public class Converter532To541 : IObjectConverter,
      IVisitor<PKSimProject>,
      IVisitor<Simulation>,
      IVisitor<PopulationSimulation>
   {
      private readonly IDataRepositoryConverter _dataRepositoryConverter;
      private readonly IDimensionRepository _dimensionRepository;
      private bool _converted;

      public Converter532To541(IDataRepositoryConverter dataRepositoryConverter, IDimensionRepository dimensionRepository)
      {
         _dataRepositoryConverter = dataRepositoryConverter;
         _dimensionRepository = dimensionRepository;
      }

      public bool IsSatisfiedBy(int version) => version == ProjectVersions.V5_3_2;

      public (int convertedToVersion, bool conversionHappened) Convert(object objectToConvert, int originalVersion)
      {
         _converted = false;
         this.Visit(objectToConvert);
         return (ProjectVersions.V5_4_1, _converted);
      }

      public (int convertedToVersion, bool conversionHappened) ConvertXml(XElement element, int originalVersion)
      {
         //nothing to do here
         return (ProjectVersions.V5_4_1, false);
      }

      public void Visit(PKSimProject project)
      {
         foreach (var observedData in project.AllObservedData)
         {
            _dataRepositoryConverter.Convert(observedData);
            project.AddClassifiable(new ClassifiableObservedData {Subject = observedData});
         }

         foreach (var simulation in project.All<Simulation>())
         {
            project.AddClassifiable(new ClassifiableSimulation {Subject = simulation});
         }

         foreach (var comparison in project.AllSimulationComparisons)
         {
            project.AddClassifiable(new ClassifiableComparison {Subject = comparison});
         }
         _converted = true;
      }

      public void Visit(Simulation simulation)
      {
         convertSimulation(simulation);
         _converted = true;
      }

      private static void convertSimulation(Simulation simulation)
      {
         var root = simulation.Model.Root;
         //direct Event group children should get types event groups
         foreach (var eventGroup in root.GetChildren<EventGroup>())
         {
            eventGroup.ContainerType = ContainerType.EventGroup;

            //do not set type for event group defined under events
            if (eventGroup.IsNamed(CoreConstants.Tags.EVENTS))
               continue;

            foreach (var childEventGroup in eventGroup.GetAllChildren<EventGroup>())
            {
               if (childEventGroup.Name.StartsWith(CoreConstants.APPLICATION_NAME_TEMPLATE))
                  childEventGroup.ContainerType = ContainerType.Application;
               else
                  childEventGroup.ContainerType = ContainerType.Formulation;
            }
         }
      }

      public void Visit(PopulationSimulation populationSimulation)
      {
         convertSimulation(populationSimulation);
         foreach (var timeProfileAnalysis in populationSimulation.Analyses.OfType<TimeProfileAnalysisChart>())
         {
            timeProfileAnalysis.PopulationAnalysis.TimeUnit = _dimensionRepository.Time.DefaultUnit;
         }
         _converted = true;
      }
   }
}