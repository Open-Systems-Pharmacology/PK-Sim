using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IIndividualExpressionsUpdater
   {
      void Update(Individual sourceIndividual, Individual targetIndividual);
   }

   public class IndividualExpressionsUpdater : IIndividualExpressionsUpdater
   {
      private readonly IOntogenyTask<Individual> _ontogenyTask;
      private readonly ICloner _cloner;

      public IndividualExpressionsUpdater(IOntogenyTask<Individual> ontogenyTask, ICloner cloner)
      {
         _ontogenyTask = ontogenyTask;
         _cloner = cloner;
      }

      public void Update(Individual sourceIndividual, Individual targetIndividual)
      {
         foreach (var molecule in sourceIndividual.AllMolecules())
         {
            var newMolecule = _cloner.Clone(molecule);
            targetIndividual.AddMolecule(newMolecule);

            //Make sure parameters that user defined parameters are reset to default to ensure proper scaling
            resetMoleculeParametersToDefault(newMolecule);

            //we have to reset the ontogeny for the molecule based on the target individual properties
            _ontogenyTask.SetOntogenyForMolecule(newMolecule, newMolecule.Ontogeny, targetIndividual);
         }
      }

      private void resetMoleculeParametersToDefault(IndividualMolecule molecule)
      {
         molecule.AllParameters()
            .Where(x => x.ValueDiffersFromDefault())
            .Each(x => x.ResetToDefault());
      }
   }
}