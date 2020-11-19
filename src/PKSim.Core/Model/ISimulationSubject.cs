using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Collections;

namespace PKSim.Core.Model
{
   /// <summary>
   ///    Defines an item that can be simulated in a simulation (e.g. a population or an individual)
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
      ///    Returns <c>true</c> if the underlying species is Human otherwise <c>false</c>
      /// </summary>
      bool IsHuman { get; }

      /// <summary>
      ///    Returns <c>true</c> if the underlying subject is preterm otherwise <c>false</c>
      /// </summary>
      bool IsPreterm { get; }

      /// <summary>
      ///    Returns <c>true</c> if the underlying individual is age dependent otherwise <c>false</c>
      /// </summary>
      bool IsAgeDependent { get; }

      Organism Organism { get; }

      IEnumerable<IndividualMolecule> AllMolecules();

      /// <summary>
      ///    Returns the molecule named <paramref name="moleculeName" /> or NULL if not found
      /// </summary>
      IndividualMolecule MoleculeByName(string moleculeName);

      IEnumerable<TMolecules> AllMolecules<TMolecules>() where TMolecules : IndividualMolecule;

      /// <summary>
      ///    Add a <paramref name="molecule" /> to the subject
      /// </summary>
      /// <param name="molecule"> Molecule to add </param>
      void AddMolecule(IndividualMolecule molecule);

      /// <summary>
      ///    Remove a <paramref name="molecule" /> from the subject
      /// </summary>
      /// <param name="molecule"> Molecule to remove </param>
      void RemoveMolecule(IndividualMolecule molecule);

      /// <summary>
      ///    The underlying individual associated with the simulation subject
      /// </summary>
      Individual Individual { get; }

      /// <summary>
      ///    Returns a cache with all expression parameters defined for <paramref name="molecule" /> in the simulation subject.
      ///    Global expression parameters are also returned
      /// </summary>
      /// <example>If we have the following structure Kidney|Intracellular|CYP3A4|RelExp, it will return (Kidney, RelExp)</example>
      ICache<string, IParameter> AllExpressionParametersFor(IndividualMolecule molecule);


      /// <summary>
      ///  Returns all possible molecule containers of the individual in which <paramref name="molecule"/> will be defined.
      /// </summary>
      IReadOnlyList<MoleculeExpressionContainer> AllMoleculeContainersFor(IndividualMolecule molecule);

      /// <summary>
      ///  Returns all possible molecule containers of the individual in which <paramref name="molecule"/> will be defined.
      /// </summary>
      IReadOnlyList<T> AllMoleculeContainersFor<T>(IndividualMolecule molecule) where T : MoleculeExpressionContainer;
   }
}