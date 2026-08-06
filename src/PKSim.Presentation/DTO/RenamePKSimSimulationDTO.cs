using System.Collections.Generic;
using OSPSuite.Presentation.DTO;
using OSPSuite.Utility.Validation;
using PKSim.Assets;

namespace PKSim.Presentation.DTO
{
   public class RenamePKSimSimulationDTO : RenameObjectDTO
   {
      private readonly List<string> _compoundNames = new();

      public RenamePKSimSimulationDTO(string name) : base(name)
      {
         Rules.Add(RenamePKSimSimulationDTORules.NameShouldNotBeTheSameAsCompound);
      }


      public void AddCompoundNames(IReadOnlyList<string> compoundNames) => _compoundNames.AddRange(compoundNames);

      private static class RenamePKSimSimulationDTORules
      {
         public static IBusinessRule NameShouldNotBeTheSameAsCompound { get; } = CreateRule.For<RenamePKSimSimulationDTO>()
            .Property(x => x.Name)
            .WithRule((x, y) => !x._compoundNames.Contains(y))
            .WithError(PKSimConstants.Error.SimulationCannotShareNamesWithCompounds);
      }
   }
}