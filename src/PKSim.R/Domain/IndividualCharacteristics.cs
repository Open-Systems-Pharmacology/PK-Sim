using System.Collections.Generic;
using PKSim.Core.Snapshots;

namespace PKSim.R.Domain
{
   /// <summary>
   /// Wrapper object for .net that encapsulates origin data and molecule ontogeny
   /// </summary>
   public class IndividualCharacteristics : OriginData
   {
      private readonly List<MoleculeOntogeny> _moleculeOntogenies = new List<MoleculeOntogeny>();

      public IReadOnlyList<MoleculeOntogeny> MoleculeOntogenies => _moleculeOntogenies;
      public MoleculeOntogeny[] MoleculeOntogeniesAsArray => _moleculeOntogenies.ToArray();

      public void AddMoleculeOntogeny(MoleculeOntogeny moleculeOntogeny)
      {
         _moleculeOntogenies.Add(moleculeOntogeny);
      }
   }
}