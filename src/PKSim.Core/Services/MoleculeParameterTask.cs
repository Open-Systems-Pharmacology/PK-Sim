using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Core.Services
{
   public interface IMoleculeParameterTask
   {
      /// <summary>
      /// Updates the default global molecule parameters in the molecule based on the values defined in the database
      /// </summary>
      /// <param name="molecule">Molecule containihgg the parameters to update</param>
      /// <param name="moleculeName">Name of molecule to use to retrieve the default parameters. If not set, <paramref name="molecule"/> name will be used instead</param>
      void SetDefaultMoleculeParameters(IndividualMolecule molecule, string moleculeName = null);

   }

   public class MoleculeParameterTask : IMoleculeParameterTask
   {
      private readonly IMoleculeParameterRepository _moleculeParameterRepository;

      public MoleculeParameterTask(IMoleculeParameterRepository moleculeParameterRepository)
      {
         _moleculeParameterRepository = moleculeParameterRepository;
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

   }
}