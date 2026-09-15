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
      public IReadOnlyList<OverwriteParameterSet> AvailableExistingSets { get; init; }
      public List<ParameterCommitDTO> Parameters { get; init; } = new();
      public bool CreateNew { get; set; } = true;
      public string NewSetName { get; set; }
      public OverwriteParameterSet SelectedExistingSet { get; set; }

      /// <summary>
      ///    The set of the project compound selected for the compound in the simulation, or <c>null</c> if none is
      ///    selected. Removals of reset parameters can only be committed to this set.
      /// </summary>
      public OverwriteParameterSet SetSelectedInSimulation { get; init; }

      public bool HasSelectedRemovals => Parameters.Any(p => p.Selected && p.IsRemoval);

      public CompoundCommitDTO()
      {
         Rules.AddRange(AllRules.All());
      }

      private static class AllRules
      {
         private static IBusinessRule removalsTargetSetSelectedInSimulation { get; } = CreateRule.For<CompoundCommitDTO>()
            .Property(x => x.SelectedExistingSet)
            .WithRule((dto, selectedSet) => !dto.HasSelectedRemovals || (!dto.CreateNew && selectedSet == dto.SetSelectedInSimulation))
            .WithError(PKSimConstants.Error.ResetParametersCanOnlyBeRemovedFromSelectedParameterSet);

         private static IBusinessRule newSetNameNotEmpty { get; } = CreateRule.For<CompoundCommitDTO>()
            .Property(x => x.NewSetName)
            .WithRule((dto, name) => !dto.CreateNew || !string.IsNullOrWhiteSpace(name))
            .WithError(PKSimConstants.Error.NameIsRequired);

         private static IBusinessRule newSetNameNotExisting { get; } = CreateRule.For<CompoundCommitDTO>()
            .Property(x => x.NewSetName)
            .WithRule((dto, name) => !dto.CreateNew || dto.AvailableExistingSets == null || dto.AvailableExistingSets.All(s => s.Name != name))
            .WithError((dto, name) => PKSimConstants.Error.NameAlreadyExistsInContainerType(name, PKSimConstants.ObjectTypes.Compound));

         private static IBusinessRule existingSetMustBeSelected { get; } = CreateRule.For<CompoundCommitDTO>()
            .Property(x => x.SelectedExistingSet)
            .WithRule((dto, selectedSet) => dto.CreateNew || selectedSet != null)
            .WithError(PKSimConstants.Error.ValueIsRequired);

         public static IEnumerable<IBusinessRule> All()
         {
            yield return removalsTargetSetSelectedInSimulation;
            yield return newSetNameNotEmpty;
            yield return newSetNameNotExisting;
            yield return existingSetMustBeSelected;
         }
      }
   }
}
