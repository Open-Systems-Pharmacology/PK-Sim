using System.Linq;
using System.Xml.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Serializer.Xml.Extensions;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Visitor;
using PKSim.Core;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Services;
using static PKSim.Core.CoreConstants.Organ;
using static PKSim.Core.CoreConstants.Parameters;

namespace PKSim.Infrastructure.ProjectConverter.v11
{
   public class Converter10to11 : IObjectConverter,
      IVisitor<Individual>,
      IVisitor<Population>
   {
      private readonly IExpressionProfileFactory _expressionProfileFactory;
      private readonly IExpressionProfileUpdater _expressionProfileUpdater;
      private readonly IPKSimProjectRetriever _projectRetriever;
      private readonly IEventPublisher _eventPublisher;
      private readonly IRegistrationTask _registrationTask;
      private readonly IDefaultIndividualRetriever _defaultIndividualRetriever;
      private readonly ICloner _cloner;
      private bool _converted;

      public Converter10to11(
         IExpressionProfileFactory expressionProfileFactory,
         IExpressionProfileUpdater expressionProfileUpdater,
         IPKSimProjectRetriever projectRetriever,
         IEventPublisher eventPublisher,
         IRegistrationTask registrationTask,
         IDefaultIndividualRetriever defaultIndividualRetriever,
         ICloner cloner
      )
      {
         _expressionProfileFactory = expressionProfileFactory;
         _expressionProfileUpdater = expressionProfileUpdater;
         _projectRetriever = projectRetriever;
         _eventPublisher = eventPublisher;
         _registrationTask = registrationTask;
         _defaultIndividualRetriever = defaultIndividualRetriever;
         _cloner = cloner;
      }

      public bool IsSatisfiedBy(int version) => version == ProjectVersions.V10;

      public (int convertedToVersion, bool conversionHappened) Convert(object objectToConvert, int originalVersion)
      {
         _converted = false;
         this.Visit(objectToConvert);
         return (ProjectVersions.V11, _converted);
      }

      public (int convertedToVersion, bool conversionHappened) ConvertXml(XElement element, int originalVersion)
      {
         _converted = false;
         element.DescendantsAndSelf("Individual").Each(convertOriginDataInIndividualNode);
         element.DescendantsAndSelf("BaseIndividual").Each(convertOriginDataInIndividualNode);
         return (ProjectVersions.V11, _converted);
      }

      private void convertOriginDataInIndividualNode(XElement individualElement)
      {
         var originDataElement = individualElement.Element("OriginData");
         if (originDataElement == null)
            return;

         var age = originDataElement.GetAttribute("age");
         var ageUnit = originDataElement.GetAttribute("ageUnit");
         var gestationalAge = originDataElement.GetAttribute("gestationalAge");
         var gestationalAgeUnit = originDataElement.GetAttribute("gestationalAgeUnit");
         var BMI = originDataElement.GetAttribute("bMI");
         var BMIUnit = originDataElement.GetAttribute("bMIUnit");
         var height = originDataElement.GetAttribute("height");
         var heightUnit = originDataElement.GetAttribute("heightUnit");
         var weight = originDataElement.GetAttribute("weight");
         var weightUnit = originDataElement.GetAttribute("weightUnit");
         var population = originDataElement.GetAttribute("speciesPopulation");
         originDataElement.SetAttributeValue("population", population);
         addOriginDataNodeToOriginData(age, ageUnit, "Age", originDataElement);
         addOriginDataNodeToOriginData(gestationalAge, gestationalAgeUnit, "GestationalAge", originDataElement);
         addOriginDataNodeToOriginData(BMI, BMIUnit, "BMI", originDataElement);
         addOriginDataNodeToOriginData(height, heightUnit, "Height", originDataElement);
         addOriginDataNodeToOriginData(weight, weightUnit, "Weight", originDataElement);

         _converted = true;
      }

      private void addOriginDataNodeToOriginData(string value, string unit, string nodeName, XElement originDataElement)
      {
         if (value.IsNullOrEmpty())
            return;

         var originParameterElement = new XElement(nodeName);
         originParameterElement.SetAttributeValue("value", value);
         originParameterElement.SetAttributeValue("unit", unit);
         originDataElement.Add(originParameterElement);

         return;
      }

      public void Visit(Individual individual)
      {
         addExpressionProfilesUsedBySimulationSubjectToProject(individual);
         addEstimatedGFRParameterTo(individual);
      }

      public void Visit(Population population)
      {
         addExpressionProfilesUsedBySimulationSubjectToProject(population);
         makeInitialConcentrationParametersNotVariableInPopulation(population);
         addEstimatedGFRParameterTo(population.FirstIndividual);
      }

      private void makeInitialConcentrationParametersNotVariableInPopulation(Population population)
      {
         population?.FirstIndividual?.GetAllChildren<IParameter>(x => x.IsNamed(INITIAL_CONCENTRATION))
            .Each(x => x.CanBeVariedInPopulation = false);
      }

      private void addEstimatedGFRParameterTo(Individual individual)
      {
         if (individual == null)
            return;


         var defaultHuman = _defaultIndividualRetriever.DefaultHuman();
         var parameter = defaultHuman.Organism.EntityAt<IParameter>(KIDNEY, E_GFR);
         var kidney = individual.Organism.Organ(KIDNEY);
         kidney.Add(_cloner.Clone(parameter));
      }

      private void addExpressionProfilesUsedBySimulationSubjectToProject(ISimulationSubject simulationSubject)
      {
         foreach (var molecule in simulationSubject.AllMolecules())
         {
            var expressionProfile = _expressionProfileFactory.Create(molecule.GetType(), simulationSubject.Species, molecule.Name);

            //Make sure the name does not have our separator
            expressionProfile.Category = simulationSubject.Name;
            _expressionProfileUpdater.SynchronizeExpressionProfileWithSimulationSubject(expressionProfile, simulationSubject);

            //only add at the end once the expression profile has been updated
            simulationSubject.AddExpressionProfile(expressionProfile);
            _projectRetriever.Current.AddBuildingBlock(expressionProfile);
            _registrationTask.Register(expressionProfile);
            _eventPublisher.PublishEvent(new BuildingBlockAddedEvent(expressionProfile, _projectRetriever.Current));
         }

         _converted = true;
      }

      private bool isChangedByCreatedIndividual(IParameter parameter)
      {
         var Name = parameter.Name;
         if (string.IsNullOrEmpty(Name))
            return false;

         if (!parameter.IsOfType(PKSimBuildingBlockType.Individual))
            return false;

         if (StandardCreateIndividualParameters.Contains(Name))
            return true;

         if (DerivedCreatedIndividualParameters.Contains(Name))
            return true;

         if (OntogenyFactors.Contains(Name))
            return true;

         if (AllPlasmaProteinOntogenyFactors.Contains(Name))
            return true;

         //only parameter in these 4 organs needs to be treated specially
         if (!isInFatOrMuscleOrLungOrPortalVein(parameter))
            return false;

         if (VolumeFractionWaterParameters.Contains(Name) && isInFatOrMuscle(parameter))
            return true;

         //formula parameter indirectly changed by create individual 
         if (string.Equals(FRACTION_INTRACELLULAR, Name) && isInFatOrMuscle(parameter))
            return true;

         if (VolumeFractionLipidsParameters.Contains(Name) && isInFat(parameter))
            return true;

         if (VolumeFractionProteinsParameters.Contains(Name) && isInMuscle(parameter))
            return true;

         return false;
      }

      private bool isInFatOrMuscleOrLungOrPortalVein(IParameter parameter) => isInFatOrMuscle(parameter) || isInLungOrPortalVein(parameter);

      private bool isInFatOrMuscle(IParameter parameter) => isInFat(parameter) || isInMuscle(parameter);

      private bool isInLungOrPortalVein(IParameter parameter) => isInLung(parameter) || isInPortalVein(parameter);

      private bool isInPortalVein(IParameter parameter) => isIn(PORTAL_VEIN, parameter);

      private bool isInLung(IParameter parameter) => isIn(LUNG, parameter);

      private bool isInFat(IParameter parameter) => isIn(FAT, parameter);

      private bool isInMuscle(IParameter parameter) => isIn(MUSCLE, parameter);

      private bool isIn(string organName, IParameter parameter)
      {
         if (parameter.ParentContainer == null)
            return false;

         return string.Equals(parameter.ParentContainer.Name, organName);
      }
   }
}