using System.Collections.Generic;
using OSPSuite.Utility.Collections;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Repositories
{
   public interface IModelContainerMoleculeRepository : IStartableRepository<string>
   {
      /// <summary>
      /// All molecules names - without DRUG, present in given model<para/>
      /// (only static molecules, dynamically created moelcules (metabolites, enzymes, ...) are not present here
      /// </summary>
      IReadOnlyList<string> MoleculeNamesWithoutDrug(string modelName);

      /// <summary>
      /// Same as <see cref="MoleculeNamesWithoutDrug"/>, but DRUG is also included into the list
      /// </summary>
      IReadOnlyList<string> MoleculeNamesIncludingDrug(string modelName);

      /// <summary>
      /// Checks if molecule is available in container (for given model).<para/>
      /// Default behaviour for the case no entry for {model, container, molecule} is found:
      ///   - for moleculeName = DRUG return true
      ///   - for moleculeName != DRUG return false
      /// </summary>
      bool IsPresent(string modelName, int containerId, string moleculeName);

      /// <summary>
      /// Checks if molecule is available in container (for given model).<para/>
      /// Default behaviour for the case no entry for {model, container, molecule} is found:
      ///   - for moleculeName = DRUG return true
      ///   - for moleculeName != DRUG return false
      /// </summary>
      bool IsPresent(string modelName, IObjectPath containerPath, string moleculeName);

      /// <summary>
      /// Checks if molecule is available in container (for given model).<para/>
      /// Default behaviour for the case no entry for {model, container, molecule} is found: return false
      /// </summary>
      bool NegativeValuesAllowed(string modelName, IObjectPath containerPath, string moleculeName);
   }

}
