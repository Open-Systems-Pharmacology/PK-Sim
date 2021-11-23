using System.Xml.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Visitor;
using PKSim.Core;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Services;

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
      private bool _converted;

      public Converter10to11(
         IExpressionProfileFactory expressionProfileFactory,
         IExpressionProfileUpdater expressionProfileUpdater,
         IPKSimProjectRetriever projectRetriever,
         IEventPublisher eventPublisher,
         IRegistrationTask registrationTask
      )
      {
         _expressionProfileFactory = expressionProfileFactory;
         _expressionProfileUpdater = expressionProfileUpdater;
         _projectRetriever = projectRetriever;
         _eventPublisher = eventPublisher;
         _registrationTask = registrationTask;
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
         return (ProjectVersions.V11, false);
      }

      public void Visit(Individual individual)
      {
         addExpressionProfilesUsedBySimulationSubjectToProject(individual);
      }

      public void Visit(Population population)
      {
         addExpressionProfilesUsedBySimulationSubjectToProject(population);
         makeInitialConcentrationParametersNotVariableInPopulation(population);
      }

      private void makeInitialConcentrationParametersNotVariableInPopulation(Population population)
      {
         population?.FirstIndividual?.GetAllChildren<IParameter>(x => x.IsNamed(CoreConstants.Parameters.INITIAL_CONCENTRATION))
            .Each(x => x.CanBeVariedInPopulation = false);
      }

      private void addExpressionProfilesUsedBySimulationSubjectToProject(ISimulationSubject simulationSubject)
      {
         foreach (var molecule in simulationSubject.AllMolecules())
         {
            var expressionProfile = _expressionProfileFactory.CreateFor(molecule.GetType(), simulationSubject.Species, molecule.Name);

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
   }
}