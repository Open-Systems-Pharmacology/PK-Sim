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
            //we have to reset the new ontogeny for the molecule
            _ontogenyTask.SetOntogenyForMolecule(newMolecule, newMolecule.Ontogeny, targetIndividual);
         }
      }
   }
}