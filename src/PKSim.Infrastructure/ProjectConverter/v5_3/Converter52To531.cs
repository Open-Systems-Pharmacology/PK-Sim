using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Visitor;
using PKSim.Core;
using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure.Serialization.Xml.Serializers;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Serialization;
using ICompoundConverter52 = PKSim.Infrastructure.ProjectConverter.v5_2.ICompoundConverter;
using IoC = OSPSuite.Utility.Container.IContainer;
using ISolverSettingsFactory = PKSim.Core.Model.ISolverSettingsFactory;

namespace PKSim.Infrastructure.ProjectConverter.v5_3
{
   public class Converter52To531 : IObjectConverter,
      IVisitor<Compound>,
      IVisitor<IndividualSimulation>,
      IVisitor<PopulationSimulation>

   {
      private readonly ISolverSettingsFactory _solverSettingsFactory;
      private readonly ISimulationResultsLoader _simulationResultsLoader;
      private readonly ICompoundConverter52 _compoundConverter;
      private readonly IObjectPathFactory _objectPathFactory;
      private readonly IoC _container;
      private readonly IDimensionFactory _dimensionFactory;
      private readonly List<XElement> _simulationChartElementCache;
      private bool _converted;

      public Converter52To531(ISolverSettingsFactory solverSettingsFactory, ISimulationResultsLoader simulationResultsLoader,
         ICompoundConverter52 compoundConverter, IObjectPathFactory objectPathFactory, IoC container, IDimensionFactory dimensionFactory)
      {
         _solverSettingsFactory = solverSettingsFactory;
         _simulationResultsLoader = simulationResultsLoader;
         _compoundConverter = compoundConverter;
         _objectPathFactory = objectPathFactory;
         _container = container;
         _dimensionFactory = dimensionFactory;
         _simulationChartElementCache = new List<XElement>();
      }

      public (int convertedToVersion, bool conversionHappened) Convert(object objectToConvert, int originalVersion)
      {
         _converted = false;
         this.Visit(objectToConvert);
         return (ProjectVersions.V5_3_1, _converted);
      }

      public (int convertedToVersion, bool conversionHappened) ConvertXml(XElement element, int originalVersion)
      {
         _converted = false;
         updateParameterInfo(element);

         if (element.Name.IsOneOf("PopulationSimulation", "IndividualSimulation"))
         {
            convertModel(element);
            convertSimulationOutput(element);
            addChartElementsFromIndividualSimulation(element);
            _converted = true;
         }

         else if (element.Name == "SummaryChart")
         {
            renameChartElementToCurveChart(new[] {element});
            _converted = true;
         }

         return (ProjectVersions.V5_3_1, _converted);
      }

      private void addChartElementsFromIndividualSimulation(XElement element)
      {
         if (element.Name != "IndividualSimulation")
            return;

         _simulationChartElementCache.Clear();
         var concentrationChartElements = element.Descendants("ConcentrationChart").ToList();

         renameChartElementToCurveChart(concentrationChartElements);

         foreach (var concentrationChartElement in concentrationChartElements)
         {
            concentrationChartElement.Name = "SimulationConcentrationChart";
         }

         _simulationChartElementCache.AddRange(concentrationChartElements);
      }

      private void renameChartElementToCurveChart(IEnumerable<XElement> chartContainerElement)
      {
         foreach (var chartElement in chartContainerElement.Descendants("Chart").ToList())
         {
            rename(chartElement, "CurveChart");
         }
      }

      private void convertModel(XElement element)
      {
         var modelElement = element.Descendants("PKSimModel").FirstOrDefault();
         if (modelElement == null) return;
         rename(modelElement, "Model");
      }

      private void convertSimulationOutput(XElement element)
      {
         var simulationOutputElement = element.Descendants("SimulationOutput").FirstOrDefault();
         if (simulationOutputElement == null) return;

         rename(simulationOutputElement, "OutputSchema");

         foreach (var simulationIntervalElement in simulationOutputElement.Descendants("SimulationInterval").ToList())
         {
            rename(simulationIntervalElement, "OutputInterval");
         }
      }

      private void rename(XElement element, string newName)
      {
         element.Name = newName;
      }

      private void updateParameterInfo(XElement element)
      {
         var allParameterInfos = element.XPathSelectElements(".//Info");
         foreach (var parameterInfoNode in allParameterInfos)
         {
            moveAttribute(parameterInfoNode, Constants.Serialization.Attribute.Dimension);
            moveAttribute(parameterInfoNode, CoreConstants.Serialization.Attribute.BuildingBlockType);
            _converted = true;
         }
      }

      private void moveAttribute(XElement node, string attributeName)
      {
         var attribute = node.Attribute(attributeName);
         if (attribute == null)
            return;

         node.Parent?.Add(attribute);
      }

      public bool IsSatisfiedBy(int version) => version == ProjectVersions.V5_2_2;

      private void convertSimulation(Simulation simulation)
      {
         Visit(simulation.BuildingBlock<Compound>());
         addMissingSolverParameters(simulation);

         var halogenParameters = simulation.Model.Root.GetAllChildren<IParameter>(x => x.IsNamed(CoreConstants.Parameters.HAS_HALOGENS)).FirstOrDefault();
         updateHalogensParameter(halogenParameters);

         convertTotalDrugMassParameter(simulation);
         convertEHCStructure(simulation);
         _converted = true;
      }

      private void convertEHCStructure(Simulation simulation)
      {
         var root = simulation.Model.Root;
         var compoundName = simulation.CompoundNames.First();

         var organism = root.Container(Constants.ORGANISM);
         var gallBladder = organism.Container(CoreConstants.Organ.Gallbladder);
         if (gallBladder == null) return;

         var emptyingParameter = gallBladder.Parameter(ConverterConstants.Parameter.Gallbladder_emptying_rate);
         emptyingParameter.Name = ConverterConstants.Parameter.Gallbladder_emptying_active;

         var neighborhood = simulation.Model.Neighborhoods.GetSingleChildByName<IContainer>(ConverterConstants.Neighborhoods.GallbladderLumenDuo);
         var compoundUnderNeighborhood = neighborhood.Container(compoundName);
         if (compoundUnderNeighborhood == null) return;

         var transport = compoundUnderNeighborhood.GetSingleChildByName<ITransport>(ConverterConstants.Events.GallbladderEmptying);
         if (transport == null) return;

         var formula = transport.Formula.DowncastTo<ExplicitFormula>();
         formula.FormulaString = "EHC_Active ? ln(2) / EHC_Halftime * M * EHC_EjectionFraction : 0";
         foreach (var objectPath in formula.ObjectPaths.ToList())
         {
            formula.RemoveObjectPath(objectPath);
         }

         formula.AddObjectPath(createFormulaUsablePath(gallBladder, compoundName, "M"));
         formula.AddObjectPath(createFormulaUsablePath(gallBladder, "Gallbladder ejection fraction", "EHC_EjectionFraction"));
         formula.AddObjectPath(createFormulaUsablePath(gallBladder, "Gallbladder ejection half-time", "EHC_Halftime"));
         formula.AddObjectPath(createFormulaUsablePath(gallBladder, ConverterConstants.Parameter.Gallbladder_emptying_active, "EHC_Active"));

         var assignments = root.GetAllChildren<IEvent>(x => x.IsNamed(ConverterConstants.Events.EHCStartEvent))
            .SelectMany(e => e.Assignments.Where(a => a.ChangedEntity.IsNamed(ConverterConstants.Parameter.Gallbladder_emptying_active)));

         assignments.Each(a =>
         {
            var explicitFormula = a.Formula.DowncastTo<ExplicitFormula>();
            explicitFormula.FormulaString = "1";
            explicitFormula.ClearObjectPaths();
            a.ObjectPath.Replace(ConverterConstants.Parameter.Gallbladder_emptying_rate, ConverterConstants.Parameter.Gallbladder_emptying_active);
         });
      }

      private IFormulaUsablePath createFormulaUsablePath(IContainer container, string usableName, string alias)
      {
         return _objectPathFactory.CreateAbsoluteFormulaUsablePath(container.GetSingleChildByName<IFormulaUsable>(usableName)).WithAlias(alias);
      }

      private void convertTotalDrugMassParameter(Simulation simulation)
      {
         var root = simulation.Model.Root;
         var compoundName = simulation.CompoundNames.First();
         var applications = root.Container(Constants.APPLICATIONS);
         var totalDrugMassParameter = applications.EntityAt<IParameter>(ConverterConstants.Parameter.TotalDrugMass);
         var globalCompoundParameter = root.Container(compoundName);

         totalDrugMassParameter.Name = CoreConstants.Parameters.TOTAL_DRUG_MASS;
         globalCompoundParameter.Add(totalDrugMassParameter);
         applications.RemoveChild(totalDrugMassParameter);
         foreach (var path in totalDrugMassParameter.Formula.ObjectPaths)
         {
            //relative path to drug mass parameter has changed (used to be relative to Applications.. now needs to be relative to compound container)
            path.Replace(ObjectPath.PARENT_CONTAINER, new[] {ObjectPath.PARENT_CONTAINER, ObjectPath.PARENT_CONTAINER, Constants.APPLICATIONS});
         }

         foreach (var allDrugMassParameters in applications.GetAllChildren<IParameter>(x => x.IsNamed(Constants.DRUG_MASS)))
         {
            allDrugMassParameters.AddTag(CoreConstants.Tags.MOLECULE);
         }

         foreach (var path in simulation.All<IObserver>().SelectMany(pathReferencingTotalDrugMass))
         {
            path.Replace(Constants.APPLICATIONS, compoundName);
            path.Replace(ConverterConstants.Parameter.TotalDrugMass, CoreConstants.Parameters.TOTAL_DRUG_MASS);
         }
      }

      private IEnumerable<IFormulaUsablePath> pathReferencingTotalDrugMass(IObserver observer)
      {
         return observer.Formula.ObjectPaths.Where(x => x.Alias == ConverterConstants.Parameter.TotalDrugMass);
      }

      private void addMissingSolverParameters(Simulation simulation)
      {
         var solverSettings = simulation.Solver;
         var defaultSettings = _solverSettingsFactory.CreateDefault();
         solverSettings.Add(defaultSettings.Parameter(Constants.Parameters.H_MIN));
         solverSettings.Add(defaultSettings.Parameter(Constants.Parameters.H_MAX));
         solverSettings.Add(defaultSettings.Parameter(Constants.Parameters.H0));
         solverSettings.Add(defaultSettings.Parameter(Constants.Parameters.MX_STEP));
      }

      public void Visit(Compound compound)
      {
         updateHalogensParameter(compound.Parameter(CoreConstants.Parameters.HAS_HALOGENS));

         //wrong conversion between 5.1.4 and 5.2. So we need to call the conversion again
         _compoundConverter.UpdateGainPerChargeInAlternatives(compound, updateValues: false);
         _converted = true;
      }

      private void updateHalogensParameter(IParameter parameter)
      {
         if (parameter == null) return;
         parameter.Visible = true;
      }

      public void Visit(IndividualSimulation individualSimulation)
      {
         convertSimulation(individualSimulation);
         if (!_simulationChartElementCache.Any()) return;

         //load results is required so that charts can be loaded on the fly
         _simulationResultsLoader.LoadResultsFor(individualSimulation);

         var serializerRepository = _container.Resolve<IPKSimXmlSerializerRepository>();
         var serializer = serializerRepository.SerializerFor<SimulationTimeProfileChart>();
         using (var serializationContext = SerializationTransaction.Create(dimensionFactory: _dimensionFactory))
         {
            var context = serializationContext;
            serializationContext.AddRepository(individualSimulation.DataRepository);
            _simulationChartElementCache.Each(e => individualSimulation.AddAnalysis(serializer.Deserialize<SimulationTimeProfileChart>(e, context)));
         }
         _simulationChartElementCache.Clear();
         _converted = true;
      }

      public void Visit(PopulationSimulation populationSimulation)
      {
         convertSimulation(populationSimulation);
         _converted = true;
      }
   }
}