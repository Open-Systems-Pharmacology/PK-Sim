using System.Linq;
using System.Xml.Linq;
using OSPSuite.Core.Converters.v11;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
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
using static PKSim.Infrastructure.ProjectConverter.ConverterConstants.Parameters;
using static OSPSuite.Core.Domain.Constants.ContainerName;
using Population = PKSim.Core.Model.Population;

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
      private readonly IContainerTask _containerTask;
      private readonly Converter100To110 _coreConverter;
      private bool _converted;

      public Converter10to11(
         IExpressionProfileFactory expressionProfileFactory,
         IExpressionProfileUpdater expressionProfileUpdater,
         IPKSimProjectRetriever projectRetriever,
         IEventPublisher eventPublisher,
         IRegistrationTask registrationTask,
         IDefaultIndividualRetriever defaultIndividualRetriever,
         ICloner cloner,
         IContainerTask containerTask,
         Converter100To110 coreConverter
      )
      {
         _expressionProfileFactory = expressionProfileFactory;
         _expressionProfileUpdater = expressionProfileUpdater;
         _projectRetriever = projectRetriever;
         _eventPublisher = eventPublisher;
         _registrationTask = registrationTask;
         _defaultIndividualRetriever = defaultIndividualRetriever;
         _cloner = cloner;
         _containerTask = containerTask;
         _coreConverter = coreConverter;
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
         element.DescendantsAndSelf("OriginData").Each(convertOriginDataElement);
         var (_, coreConverted) = _coreConverter.ConvertXml(element);
         return (ProjectVersions.V11, _converted || coreConverted);
      }

      private void convertOriginDataElement(XElement originDataElement)
      {
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
         convertIndividual(individual);
      }

      public void Visit(Population population)
      {
         addExpressionProfilesUsedBySimulationSubjectToProject(population);
         makeInitialConcentrationParametersNotVariableInPopulation(population);
         convertIndividual(population.FirstIndividual);
      }

      private void convertIndividual(Individual individual)
      {
         if (individual == null)
            return;

         addEstimatedGFRParameterTo(individual);
         updateIsChangedByCreatedIndividualFlag(individual);
         updateFractionOfBloodForSampling(individual);
      }

      private void updateFractionOfBloodForSampling(Individual individual)
      {
         var defaultHuman = _defaultIndividualRetriever.DefaultHuman();
         var oneStandardFractionOfBloodParameter = defaultHuman.GetAllChildren<IParameter>(x => x.IsNamed(FRACTION_OF_BLOOD_FOR_SAMPLING)).First();
         var allFractionOfBloodParameters = individual.GetAllChildren<IParameter>(x => x.IsNamed(FRACTION_OF_BLOOD_FOR_SAMPLING));
         allFractionOfBloodParameters.Each(x => { x.Info.UpdatePropertiesFrom(oneStandardFractionOfBloodParameter.Info); });
      }

      private void updateIsChangedByCreatedIndividualFlag(Individual individual)
      {
         var defaultHuman = _defaultIndividualRetriever.DefaultHuman();
         var allIsChangedByIndividualParameter = _containerTask.CacheAllChildrenSatisfying<IParameter>(defaultHuman, x => x.IsChangedByCreateIndividual);
         var allParameters = _containerTask.CacheAllChildren<IParameter>(individual);
         allIsChangedByIndividualParameter.KeyValues.Each(kv =>
         {
            var parameter = allParameters[kv.Key];
            //can be null for a new parameter added at some point in the future
            if (parameter != null)
               parameter.IsChangedByCreateIndividual = true;
         });
      }

      private void makeInitialConcentrationParametersNotVariableInPopulation(Population population)
      {
         population?.FirstIndividual?.GetAllChildren<IParameter>(x => x.IsNamed(INITIAL_CONCENTRATION))
            .Each(x => x.CanBeVariedInPopulation = false);
      }

      private void addEstimatedGFRParameterTo(Individual individual)
      {
         var kidney = individual.Organism.Organ(KIDNEY);
         var gfr = kidney.Parameter(GFR);
         var bsa = individual.Organism.Parameter(BSA);
         //This is an old individual without GFR (v6.x) or BSA Return
         if (gfr == null || bsa == null)
            return;

         var defaultHuman = _defaultIndividualRetriever.DefaultHuman();
         var parameter = defaultHuman.Organism.EntityAt<IParameter>(KIDNEY, E_GFR);
         kidney.Add(_cloner.Clone(parameter));
      }

      private void addExpressionProfilesUsedBySimulationSubjectToProject(ISimulationSubject simulationSubject)
      {
         var project = _projectRetriever.Current;
         foreach (var molecule in simulationSubject.AllMolecules())
         {
            var defaultExpressionProfileName = ExpressionProfileName(molecule.Name, simulationSubject.Species?.DisplayName, simulationSubject.Name);

            var expressionProfileName = _containerTask.CreateUniqueName(project.All<ExpressionProfile>(), defaultExpressionProfileName, canUseBaseName: true);
            var expressionProfile = _expressionProfileFactory.Create(molecule.GetType(), simulationSubject.Species, molecule.Name);

            //Use a unique name in project
            expressionProfile.Name = expressionProfileName;
            _expressionProfileUpdater.SynchronizeExpressionProfileWithSimulationSubject(expressionProfile, simulationSubject);

            //Some parameters are probably marked as FixedValue event thought they have not changed (Formula=>constant) due to change in 
            //definition of Fraction expressed basolateral going from a constant to a formula. We reset is fixed value and default state
            expressionProfile.Individual.AllMoleculeParametersFor(expressionProfile.Molecule)
               .Where(x => !x.Visible)
               .Where(x => x.IsFixedValue)
               .Each(x => x.IsFixedValue = false);

            expressionProfile.Individual.AllMoleculeParametersFor(expressionProfile.Molecule)
                 .Where(x => !x.Editable)
                 .Where(x => !x.IsDefault)
                 .Each(x => x.IsDefault = true);

            //only add at the end once the expression profile has been updated
            simulationSubject.AddExpressionProfile(expressionProfile);
            project.AddBuildingBlock(expressionProfile);
            _registrationTask.Register(expressionProfile);
            _eventPublisher.PublishEvent(new BuildingBlockAddedEvent(expressionProfile, _projectRetriever.Current));
         }

         _converted = true;
      }
   }
}