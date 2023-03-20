using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using PKSim.Core.Commands;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IMoleculeExpressionTask<TSimulationSubject> where TSimulationSubject : ISimulationSubject
   {
      /// <summary>
      ///    Add an expression profile to the given individual named based on the
      ///    <paramref name="expressionProfile" />
      /// </summary>
      /// <param name="simulationSubject">Simulation subject where the molecule will be added</param>
      /// <param name="expressionProfile">Expression profile to add to the simulation subject</param>
      ICommand AddExpressionProfile(TSimulationSubject simulationSubject, ExpressionProfile expressionProfile);

      /// <summary>
      ///    Remove the given molecule from the simulationSubject
      /// </summary>
      /// <param name="moleculeToRemove">Molecule to be removed</param>
      /// <param name="simulationSubject">Simulation subject containing the molecule to be removed</param>
      ICommand RemoveExpressionProfileFor(IndividualMolecule moleculeToRemove, TSimulationSubject simulationSubject);

      /// <summary>
      ///    Updates the transport direction type for the transporter container given as parameter
      /// </summary>
      ICommand SetTransportDirection(TransporterExpressionContainer transporterContainer, TransportDirectionId transportDirection);

      /// <summary>
      ///    Update the localization of the protein
      /// </summary>
      ICommand SetExpressionLocalizationFor(IndividualProtein protein, Localization localization, TSimulationSubject simulationSubject);

      /// <summary>
      ///    Updates the transporter type for all organ defines for transporter with the given transporter type (only if the
      ///    transporter type is defined)
      /// </summary>
      ICommand SetTransporterTypeFor(IndividualTransporter transporter, TransportType transportType);
   }

   public class MoleculeExpressionTask<TSimulationSubject> : IMoleculeExpressionTask<TSimulationSubject> where TSimulationSubject : ISimulationSubject
   {
      private readonly IExecutionContext _executionContext;
      private readonly IIndividualMoleculeFactoryResolver _individualMoleculeFactoryResolver;
      private readonly ISimulationSubjectExpressionTask<TSimulationSubject> _simulationSubjectExpressionTask;
      private readonly IExpressionProfileUpdater _expressionProfileUpdater;

      public MoleculeExpressionTask(IExecutionContext executionContext,
         IIndividualMoleculeFactoryResolver individualMoleculeFactoryResolver,
         ISimulationSubjectExpressionTask<TSimulationSubject> simulationSubjectExpressionTask,
         IExpressionProfileUpdater expressionProfileUpdater
      )
      {
         _executionContext = executionContext;
         _individualMoleculeFactoryResolver = individualMoleculeFactoryResolver;
         _simulationSubjectExpressionTask = simulationSubjectExpressionTask;
         _expressionProfileUpdater = expressionProfileUpdater;
      }

      public ICommand RemoveExpressionProfileFor(IndividualMolecule molecule, TSimulationSubject simulationSubject)
      {
         var expressionProfile = simulationSubject.ExpressionProfileFor(molecule);
         simulationSubject.RemoveExpressionProfile(expressionProfile);
         return _simulationSubjectExpressionTask.RemoveMoleculeFrom(molecule, simulationSubject);
      }

      public ICommand AddExpressionProfile(TSimulationSubject simulationSubject, ExpressionProfile expressionProfile)
      {
         var moleculeFactory = _individualMoleculeFactoryResolver.FactoryFor(expressionProfile.Molecule);
         //this call not only creates a new molecule but modifies the subject in place to have the link to the expression profile
         var molecule = moleculeFactory.AddMoleculeTo(simulationSubject, expressionProfile.MoleculeName);
         simulationSubject.AddExpressionProfile(expressionProfile);
         _expressionProfileUpdater.SynchroniseSimulationSubjectWithExpressionProfile(simulationSubject, expressionProfile);
         return _simulationSubjectExpressionTask.AddMoleculeTo(molecule, simulationSubject);
      }

      public ICommand SetTransportDirection(TransporterExpressionContainer transporterContainer, TransportDirectionId transportDirection)
      {
         return new SetTransportDirectionCommand(transporterContainer, transportDirection, _executionContext).Run(_executionContext);
      }

      public ICommand SetExpressionLocalizationFor(IndividualProtein protein, Localization localization, TSimulationSubject simulationSubject)
      {
         return new SetExpressionLocalizationCommand(protein, localization, simulationSubject, _executionContext).Run(_executionContext);
      }

      public ICommand SetTransporterTypeFor(IndividualTransporter transporter, TransportType transportType)
      {
         return new SetTransportTypeCommand(transporter, transportType, _executionContext).Run(_executionContext);
      }
   }
}
