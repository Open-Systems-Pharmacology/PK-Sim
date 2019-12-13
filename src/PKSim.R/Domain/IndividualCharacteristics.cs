using System.Collections.Generic;
using PKSim.Core.Snapshots;

namespace PKSim.R.Domain
{
   /// <summary>
   /// This object is required because one cannot pass array of object from R to .NET
   /// </summary>
   public class IndividualCharacteristics
   {
      private readonly List<MoleculeOntogeny> _moleculeOntogenies = new List<MoleculeOntogeny>();


      public OriginData OriginData { get; set; }

      public IReadOnlyList<MoleculeOntogeny> MoleculeOntogenies => _moleculeOntogenies;

      public void AddMoleculeOntogeny(MoleculeOntogeny moleculeOntogeny)
      {
         _moleculeOntogenies.Add(moleculeOntogeny);
      }
   }
}