using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Serialization;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Visitor;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Infrastructure.Serialization.Xml.Serializers;
using IoC = OSPSuite.Utility.Container.IContainer;

namespace PKSim.Infrastructure.ProjectConverter.v6_2
{
   public class Converter612To621 : IObjectConverter,
      IVisitor<IProject>,
      IVisitor<Compound>,
      IVisitor<Simulation>

   {
      private readonly IoC _container;
      private readonly IDimensionFactory _dimensionFactory;
      private readonly IObservedDataConvertor _observedDataConvertor;
      private readonly List<XElement> _projectObservedDataElementCache = new List<XElement>();
      private int _originalVersion;
      private bool _converted;

      public Converter612To621(IoC container, IDimensionFactory dimensionFactory, IObservedDataConvertor observedDataConvertor)
      {
         _container = container;
         _dimensionFactory = dimensionFactory;
         _observedDataConvertor = observedDataConvertor;
      }

      public bool IsSatisfiedBy(int version)
      {
         return version == ProjectVersions.V6_1_2;
      }

      public (int convertedToVersion, bool conversionHappened) Convert(object objectToConvert, int originalVersion)
      {
         _originalVersion = originalVersion;
         _converted = false;
         this.Visit(objectToConvert);
         return (ProjectVersions.V6_2_1, _converted);
      }

      public (int convertedToVersion, bool conversionHappened) ConvertXml(XElement element, int originalVersion)
      {
         _converted = false;
         if (element.Name == "SimulationConcentrationChart")
         {
            element.Name = "SimulationTimeProfileChart";
            _converted = true;
         }

         if (element.Name.IsOneOf("PKSimProject", "Project"))
         {
            addObservedDataElementsToCache(element);
            _converted = true;
         }

         return (ProjectVersions.V6_2_1, _converted);
      }

      private void addObservedDataElementsToCache(XElement element)
      {
         _projectObservedDataElementCache.Clear();
         var allObservedDataElement = element.Element("AllObservedData");
         if (allObservedDataElement == null)
            return;

         var observedDataElements = allObservedDataElement.Descendants(Constants.Serialization.DATA_REPOSITORY).ToList();

         _projectObservedDataElementCache.AddRange(observedDataElements);
      }

      public void Visit(IProject project)
      {
         convertProject(project);
      }

      private void convertProject(IProject project)
      {
         if (!_projectObservedDataElementCache.Any()) return;

         var serializerRepository = _container.Resolve<IPKSimXmlSerializerRepository>();
         var serializer = serializerRepository.SerializerFor<DataRepository>();
         using (var serializationContext = SerializationTransaction.Create(_container, dimensionFactory: _dimensionFactory, withIdRepository: new WithIdRepository()))
         {
            var context = serializationContext;
            _projectObservedDataElementCache.Each(e =>
            {
               _observedDataConvertor.ConvertDimensionIn(e);
               project.AddObservedData(serializer.Deserialize<DataRepository>(e, context));
            });
         }

         _projectObservedDataElementCache.Clear();

         project.AllObservedData.Each(x => _observedDataConvertor.Convert(project, x, _originalVersion));
         _converted = true;
      }

      private void convertCompound(Compound compound)
      {
         compound.Parameter(Constants.Parameters.MOL_WEIGHT).CanBeVaried = false;
         _converted = true;
      }

      public void Visit(Compound compound)
      {
         convertCompound(compound);
      }

      public void Visit(Simulation simulation)
      {
         simulation.AllBuildingBlocks<Compound>().Each(convertCompound);
      }
   }
}