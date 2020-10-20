using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IMoleculeExpressionTask<TSimulationSubject> where TSimulationSubject : ISimulationSubject
   {
      /// <summary>
      ///    Add a protein of type <typeparamref name="TMolecule" /> to the given individual
      /// </summary>
      /// <typeparam name="TMolecule">Type of molecule to add. The molecule will be created depending on this type </typeparam>
      /// <param name="simulationSubject">Simulation subject where the molecule will be added</param>
      ICommand AddMoleculeTo<TMolecule>(TSimulationSubject simulationSubject) where TMolecule : IndividualMolecule;

      /// <summary>
      ///    Add a default molecule of type <typeparamref name="TMolecule" /> to the given <paramref name="simulationSubject" />
      ///    bypassing the expression
      ///    database
      /// </summary>
      /// <typeparam name="TMolecule">Type of molecule to add. The molecule will be created depending on this type </typeparam>
      /// <param name="simulationSubject">Simulation subject where the molecule will be added</param>
      ICommand AddDefaultMolecule<TMolecule>(TSimulationSubject simulationSubject) where TMolecule : IndividualMolecule;

      /// <summary>
      ///    Set the value of the relative expression for  <paramref name="relativeExpressionParameter"/>
      /// </summary>
      /// <param name="relativeExpressionParameter">
      ///    Relative expression parameter
      /// </param>
      /// <param name="value">relative expression</param>
      ICommand SetRelativeExpressionFor(IParameter relativeExpressionParameter, double value);

      /// <summary>
      ///    Remove the given molecule from the simulationSubject
      /// </summary>
      /// <param name="moleculeToRemove">Molecule to be removed</param>
      /// <param name="simulationSubject">Simulation subject containing the molecule to be removed</param>
      ICommand RemoveMoleculeFrom(IndividualMolecule moleculeToRemove, TSimulationSubject simulationSubject);

      /// <summary>
      ///    Edit the given molecule defined in the simulationSubject
      /// </summary>
      /// <param name="molecule">Edited molecule</param>
      /// <param name="simulationSubject">Simulation subject  containing the edited molecule</param>
      ICommand EditMolecule(IndividualMolecule molecule, TSimulationSubject simulationSubject);

      /// <summary>
      ///    return true if a protein expression database was defined for the <paramref name="simulationSubject" />, otherwise
      ///    false
      /// </summary>
      bool CanQueryProteinExpressionsFor(TSimulationSubject simulationSubject);

      /// <summary>
      ///    Updates the membrane type for the transporter container given as parameter
      /// </summary>
      ICommand SetMembraneLocationFor(TransporterExpressionContainer transporterContainer, TransportType transportType, MembraneLocation membraneLocation);

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
}