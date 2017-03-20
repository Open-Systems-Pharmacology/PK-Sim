using System.Collections.Generic;

namespace PKSim.Core.Model
{
   /// <summary>
   ///    Defines an item that can be simulated in a simulaton (e.g. a population or an individual)
   /// </summary>
   public interface ISimulationSubject : IPKSimBuildingBlock
   {
      /// <summary>
      ///    Origin data defining the property used to create the individual or the population
      /// </summary>
      OriginData OriginData { get; }

      /// <summary>
      ///    Species used to create the individual
      /// </summary>
      Species Species { get; }

      /// <summary>
      ///    Returns true if the underlying subject is preterm
      /// </summary>
      bool IsPreterm { get; }

      /// <summary>
      ///    Returns true if the underlying species is Human
      /// </summary>
      bool IsHuman { get; }

      Organism Organism { get; }

      IEnumerable<IndividualMolecule> AllMolecules();


      IEnumerable<TMolecules> AllMolecules<TMolecules>() where TMolecules : IndividualMolecule;

      /// <summary>
      ///    Add a <paramref name="molecule"/> to the subject
      /// </summary>
      /// <param name="molecule"> Molecule to add </param>
      void AddMolecule(IndividualMolecule molecule);

      /// <summary>
      ///    Remove a <paramref name="molecule"/> from the subject
      /// </summary>
      /// <param name="molecule"> Molecule to remove </param>
      void RemoveMolecule(IndividualMolecule molecule);
   }
}