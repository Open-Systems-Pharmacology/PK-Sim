using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Presentation.DTO.Core;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.DTO.Compounds
{
   public class RenameMoleculeInPartialProcessDTO : ObjectBaseDTO
   {
      private readonly List<string> _partialProcessesDataSourceToRename;

      public RenameMoleculeInPartialProcessDTO()
      {
         _partialProcessesDataSourceToRename = new List<string>();
      }

      public void AddUsedDataSource(IEnumerable<string> usedDataSource)
      {
         _partialProcessesDataSourceToRename.AddRange(usedDataSource.Select(x => x.ToLower()));
      }

      protected override bool IsNameUnique(string moleculeName)
      {
         if (string.IsNullOrEmpty(moleculeName))
            return true;

         var newMoleculeName = moleculeName.ToLower().Trim();

         return _partialProcessesDataSourceToRename.Select(usedDataSource => CoreConstants.ContainerName.PartialProcessName(newMoleculeName, usedDataSource))
            .All(partialProcessName => !_usedNames.Contains(partialProcessName));
      }

      protected override string NameAlreadyExistsError(string moleculeName)
      {
         return PKSimConstants.Error.MoleculeNameCannotBeUsedAsItWouldCreateDuplicateProcesses(moleculeName);
      }
   }
}