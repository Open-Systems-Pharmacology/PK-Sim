using System.Collections.Generic;
using PKSim.Core.Snapshots;

namespace PKSim.R.Domain
{
   /// <summary>
   ///    Wrapper object for .net that encapsulates population settings and molecule ontogenies
   /// </summary>
   public class PopulationCharacteristics : PopulationSettings
   {
      private readonly List<MoleculeOntogeny> _moleculeOntogenies = new List<MoleculeOntogeny>();

      public IReadOnlyList<MoleculeOntogeny> MoleculeOntogenies => _moleculeOntogenies;
      public MoleculeOntogeny[] MoleculeOntogeniesAsArray => _moleculeOntogenies.ToArray();

      public void AddMoleculeOntogeny(MoleculeOntogeny moleculeOntogeny) => _moleculeOntogenies.Add(moleculeOntogeny);

      public PopulationCharacteristics()
      {
         Individual = new Individual {OriginData = new OriginData()};
      }

      public string Species
      {
         get => Individual.OriginData.Species;
         set => Individual.OriginData.Species = value;
      }

      public string Population
      {
         get => Individual.OriginData.Population;
         set => Individual.OriginData.Population = value;
      }

      public int? Seed
      {
         get => Individual.Seed;
         set => Individual.Seed = value;
      }
   }
}