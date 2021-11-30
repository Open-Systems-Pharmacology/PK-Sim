using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Core.Services
{
   public interface IMoleculeParameterTask
   {
      /// <summary>
      ///    Updates the default global molecule parameters in the molecule based on the values defined in the database
      /// </summary>
      /// <param name="molecule">Molecule containing the parameters to update</param>
      /// <param name="moleculeName">
      ///    Name of molecule to use to retrieve the default parameters. If not set,
      ///    <paramref name="molecule" /> name will be used instead
      /// </param>
      void SetDefaultMoleculeParameters(IndividualMolecule molecule, string moleculeName = null);

      /// <summary>
      ///    Updates the default molecule parameters in the molecule based on the values defined in the database
      /// </summary>
      /// <param name="expressionProfile">Expression profile containing the parameters to update</param>
      /// <param name="moleculeName">
      ///    Name of molecule to use to retrieve the default parameters. If not set,
      ///    <paramref name="expressionProfile" /> molecule name will be used instead
      /// </param>
      void SetDefaultFor(ExpressionProfile expressionProfile, string moleculeName = null);
   }

   public class MoleculeParameterTask : IMoleculeParameterTask
   {
      private readonly IMoleculeParameterRepository _moleculeParameterRepository;
      private readonly ITransportContainerUpdater _transportContainerUpdater;
      private readonly IOntogenyRepository _ontogenyRepository;
      private readonly IOntogenyTask _ontogenyTask;

      public MoleculeParameterTask(
         IMoleculeParameterRepository moleculeParameterRepository,
         ITransportContainerUpdater transportContainerUpdater,
         IOntogenyRepository ontogenyRepository,
         IOntogenyTask ontogenyTask)
      {
         _moleculeParameterRepository = moleculeParameterRepository;
         _transportContainerUpdater = transportContainerUpdater;
         _ontogenyRepository = ontogenyRepository;
         _ontogenyTask = ontogenyTask;
      }

      public void SetDefaultMoleculeParameters(IndividualMolecule molecule, string moleculeName = null)
      {
         var name = moleculeName ?? molecule.Name;
         setDefaultParameter(name, molecule.ReferenceConcentration);
         setDefaultParameter(name, molecule.HalfLifeLiver);
         setDefaultParameter(name, molecule.HalfLifeIntestine);
      }

      private void setDefaultParameter(string moleculeName, IParameter parameter)
      {
         var value = _moleculeParameterRepository.ParameterValueFor(moleculeName, parameter.Name, parameter.Value);
         parameter.DefaultValue = value;
         parameter.Value = value;
      }

      public void SetDefaultFor(ExpressionProfile expressionProfile, string moleculeName = null)
      {
         var (molecule, individual) = expressionProfile;
         var moleculeNameToUse = moleculeName ?? molecule.Name;
         setDefaultSettingsForTransporter(molecule, individual, moleculeNameToUse);
         setDefaultOntogeny(molecule, individual, moleculeNameToUse);
         SetDefaultMoleculeParameters(molecule, moleculeNameToUse);
      }

      private void setDefaultSettingsForTransporter(IndividualMolecule molecule, Individual individual, string moleculeName)
      {
         if (!(molecule is IndividualTransporter transporter)) return;

         _transportContainerUpdater.SetDefaultSettingsForTransporter(individual, transporter, moleculeName);
      }

      private void setDefaultOntogeny(IndividualMolecule molecule, Individual individual, string moleculeName)
      {
         var ontogeny = _ontogenyRepository.AllFor(individual.Species.Name).FindByName(moleculeName);
         if (ontogeny == null)
            return;

         _ontogenyTask.SetOntogenyForMolecule(molecule, ontogeny, individual);
      }
   }
}