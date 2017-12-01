using OSPSuite.Core.Commands.Core;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;

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
      ///    Set the value of the relative expression for the given molecule in the container named
      ///    <paramref name="moleculeContainerName" />
      /// </summary>
      /// <param name="molecule">Protein for which the relative expression should be set</param>
      /// <param name="moleculeContainerName">
      ///    Container where the protein is being expressed and for which the expression value
      ///    should be changed
      /// </param>
      /// <param name="value">relative expression</param>
      ICommand SetRelativeExpressionFor(IndividualMolecule molecule, string moleculeContainerName, double value);

      /// <summary>
      ///    Set the value of the relative expression for the parameter in the simulation
      /// </summary>
      ICommand SetRelativeExpressionInSimulationFor(IParameter parameter, double value);

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
      ///    Update the tissue location of the protein
      /// </summary>
      ICommand SetTissueLocationFor(IndividualProtein protein, TissueLocation tissueLocation);

      /// <summary>
      ///    Updates the transporter type for all organ defines for transporter with the given transporter type (only if the
      ///    transporter type is defined)
      /// </summary>
      ICommand SetTransporterTypeFor(IndividualTransporter transporter, TransportType transportType);

      ICommand SetMembraneLocationFor(IndividualProtein individualProteinOrEnzyme, MembraneLocation membraneLocation);

      ICommand SetIntracellularVascularEndoLocation(IndividualProtein protein, IntracellularVascularEndoLocation vascularEndoLocation);
   }
}