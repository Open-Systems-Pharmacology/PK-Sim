using System;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using PKSim.Assets;
using PKSim.Core.Model;
using SnapshotQualificationStep = PKSim.Core.Snapshots.QualificationStep;

namespace PKSim.Core.Snapshots.Mappers
{
   public class QualificationStepMapper : SnapshotMapperBase<IQualificationStep, SnapshotQualificationStep, PKSimProject>
   {
      private readonly IOSPLogger _logger;
      private const string QUALIFICATION_STEP_SUFFIX = "QualificationStep";

      public QualificationStepMapper(IOSPLogger logger)
      {
         _logger = logger;
      }

      public override async Task<SnapshotQualificationStep> MapToSnapshot(IQualificationStep qualificationStep)
      {
         var snapshot = await SnapshotFrom(qualificationStep, x => { x.Type = friendlyNameFor(qualificationStep.GetType().Name); });
         mapQualificationStepPropertiesToSnapsot(snapshot, qualificationStep);
         return snapshot;
      }

      private void mapQualificationStepPropertiesToSnapsot(SnapshotQualificationStep snapshot, IQualificationStep qualificationStep)
      {
         switch (qualificationStep)
         {
            case RunParameterIdentificationQualificationStep runParameterIdentification:
               snapshot.Subject = runParameterIdentification.ParameterIdentification.Name;
               break;
            case RunSimulationQualificationStep runSimulationQualificationStep:
               snapshot.Subject = runSimulationQualificationStep.Simulation.Name;
               break;
            default:
               throw new ArgumentException(PKSimConstants.Error.CouldNotFindQualificationStep(snapshot.Type));
         }
      }

      private string friendlyNameFor(string qualificationStepType) => qualificationStepType.Replace(QUALIFICATION_STEP_SUFFIX, "");

      public override Task<IQualificationStep> MapToModel(SnapshotQualificationStep snapshot, PKSimProject project)
      {
         var qualificationStep = createQualificationStepFrom(snapshot.Type);
         if (qualificationStep == null)
         {
            _logger.AddWarning(PKSimConstants.Error.CouldNotFindQualificationStep(snapshot.Type));
            return Task.FromResult<IQualificationStep>(null);
         }

         mapQualificationStepPropertiesToModel(qualificationStep, snapshot, project);

         return Task.FromResult(qualificationStep);
      }

      private void mapQualificationStepPropertiesToModel(IQualificationStep qualificationStep, SnapshotQualificationStep snapshot, PKSimProject project)
      {
         switch (qualificationStep)
         {
            case RunParameterIdentificationQualificationStep runParameterIdentification:
               var parameterIdentification = project.AllParameterIdentifications.FindByName(snapshot.Subject);
               if (parameterIdentification == null)
                  throw new SnapshotOutdatedException(PKSimConstants.Error.CouldNotFindParameterIdentification(snapshot.Subject));

               runParameterIdentification.ParameterIdentification = parameterIdentification;
               break;
            case RunSimulationQualificationStep runSimulationQualificationStep:
               var simulation = project.All<Model.Simulation>().FindByName(snapshot.Subject);
               if (simulation == null)
                  throw new SnapshotOutdatedException(PKSimConstants.Error.CouldNotFindSimulation(snapshot.Subject));

               runSimulationQualificationStep.Simulation = simulation;
               break;
            default:
               throw new ArgumentException(PKSimConstants.Error.NotMappingDefinedForQualificationStep(qualificationStep.GetType().Name));
         }
      }

      private IQualificationStep createQualificationStepFrom(string qualificationStepType)
      {
         return createIf<RunParameterIdentificationQualificationStep>(qualificationStepType)??
                createIf<RunSimulationQualificationStep>(qualificationStepType);
      }

      private IQualificationStep createIf<T>(string qualificationStepType) where T : class, IQualificationStep, new()
      {
         return typeof(T).Name.StartsWith(qualificationStepType) ? new T() : null;
      }
   }
}