using System.Collections.Generic;
using System.Linq;
using OSPSuite.Presentation.DTO;
using OSPSuite.Utility.Validation;
using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Presentation.DTO.Simulations
{
   public class CompoundCommitDTO : DxValidatableDTO
   {
      public string CompoundName { get; init; }
      public Compound Compound { get; init; }

      /// <summary>
      ///    The sets of the project compound, used to reject the name of a new set that already exists.
      /// </summary>
      public IReadOnlyList<OverwriteParameterSet> AvailableExistingSets { get; init; }
      public List<ParameterCommitDTO> Parameters { get; init; } = new();
      public bool CreateNew { get; set; } = true;
      public string NewSetName { get; set; }

      /// <summary>
      ///    The set of the project compound selected for the compound in the simulation, or <c>null</c> if none is
      ///    selected. It is the only set that a commit can update.
      /// </summary>
      public OverwriteParameterSet SetSelectedInSimulation { get; init; }

      public bool CanUpdateSetSelectedInSimulation => SetSelectedInSimulation != null;

      /// <summary>
      ///    A new set is built from the changed parameters and the untouched entries of the selected set. Reset parameters
      ///    alone cannot make up a new set.
      /// </summary>
      public bool CanCreateNewSet => Parameters.Any(x => !x.IsRemoval);

      /// <summary>
      ///    The parameters the dialog shows for the commit mode currently selected. A new set is built from everything it
      ///    will contain, so the entries carried over from the set applied to the simulation are listed and the paths the
      ///    user reset are not. The selected set is patched with the changes only.
      /// </summary>
      public IReadOnlyList<ParameterCommitDTO> VisibleParameters =>
         Parameters.Where(x => CreateNew ? !x.IsRemoval : !x.IsUnchanged).ToList();

      public CompoundCommitDTO()
      {
         Rules.AddRange(AllRules.All());
      }

      private static class AllRules
      {
         private static IBusinessRule updateTargetsSetSelectedInSimulation { get; } = CreateRule.For<CompoundCommitDTO>()
            .Property(x => x.CreateNew)
            .WithRule((dto, createNew) => createNew || dto.CanUpdateSetSelectedInSimulation)
            .WithError((dto, createNew) => PKSimConstants.Error.NoOverwriteParameterSetSelectedForCompoundInSimulation(dto.CompoundName));

         private static IBusinessRule newSetNameNotEmpty { get; } = CreateRule.For<CompoundCommitDTO>()
            .Property(x => x.NewSetName)
            .WithRule((dto, name) => !dto.CreateNew || !string.IsNullOrWhiteSpace(name))
            .WithError(PKSimConstants.Error.NameIsRequired);

         private static IBusinessRule newSetNameNotExisting { get; } = CreateRule.For<CompoundCommitDTO>()
            .Property(x => x.NewSetName)
            .WithRule((dto, name) => !dto.CreateNew || dto.AvailableExistingSets == null || dto.AvailableExistingSets.All(s => s.Name != name))
            .WithError((dto, name) => PKSimConstants.Error.NameAlreadyExistsInContainerType(name, PKSimConstants.ObjectTypes.Compound));

         public static IEnumerable<IBusinessRule> All()
         {
            yield return updateTargetsSetSelectedInSimulation;
            yield return newSetNameNotEmpty;
            yield return newSetNameNotExisting;
         }
      }
   }
}
