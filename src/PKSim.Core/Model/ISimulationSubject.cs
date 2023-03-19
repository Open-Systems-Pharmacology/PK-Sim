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

      /// <summary>
      ///    Returns the molecule named <paramref name="moleculeName" /> or NULL if not found
      /// </summary>
      TMolecules MoleculeByName<TMolecules>(string moleculeName) where TMolecules : IndividualMolecule;

      IEnumerable<TMolecules> AllMolecules<TMolecules>() where TMolecules : IndividualMolecule;

      /// <summary>
      /// Returns the expression profile associated with the <paramref name="molecule"/>
      /// </summary>
      ExpressionProfile ExpressionProfileFor(IndividualMolecule molecule);

      /// <summary>
      ///    Adds a <paramref name="expressionProfile" /> reference to the underlying subject
      /// </summary>
      /// <param name="expressionProfile"> Expression profile to add </param>
      void AddExpressionProfile(ExpressionProfile expressionProfile);
      
      /// <summary>
      ///    Removes a <paramref name="expressionProfile" /> from the subject
      /// </summary>
      /// <param name="expressionProfile">Expression profile to remove </param>s
      void RemoveExpressionProfile(ExpressionProfile expressionProfile);

      /// <summary>
      ///   Returns all expression profiles used by the subject
      /// </summary>
      IReadOnlyList<ExpressionProfile> AllExpressionProfiles();

      /// <summary>
      ///  Returns the molecule associated with the expression profile or null if the expression profile isn't used
      /// </summary>
      /// <param name="expressionProfile"></param>
      /// <returns></returns>
      IndividualMolecule MoleculeFor(ExpressionProfile expressionProfile);

      /// <summary>
      /// Returns <c>true</c> if the expression profile is used by the subject otherwise <c>false</c>
      /// </summary>
      bool Uses(ExpressionProfile expressionProfile);

      /// <summary>
      ///    Add a <paramref name="molecule" /> to the subject
      /// </summary>
      /// <param name="molecule"> Molecule to add </param>
      void AddMolecule(IndividualMolecule molecule);

      /// <summary>
      ///    Remove a <paramref name="molecule" /> from the subject
      /// </summary>
      /// <param name="molecule"> Molecule to remove </param>
      /// <returns>all containers for the simulation subject that were removed from the individual structure</returns>
      IReadOnlyList<IContainer> RemoveMolecule(IndividualMolecule molecule);

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
      ///    Returns all possible molecule containers of the individual in which <paramref name="molecule" /> will be defined.
      ///    This also returns global container under the global molecule named after <paramref name="molecule" />
      /// </summary>
      IReadOnlyList<T> AllMoleculeContainersFor<T>(IndividualMolecule molecule) where T : MoleculeExpressionContainer;
   }
}