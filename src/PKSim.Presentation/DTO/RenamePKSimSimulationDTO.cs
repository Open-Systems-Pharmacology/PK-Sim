using System.Collections.Generic;
using OSPSuite.Presentation.DTO;
using OSPSuite.Utility.Validation;
using PKSim.Assets;

namespace PKSim.Presentation.DTO
{
   public class RenamePKSimSimulationDTO : RenameObjectDTO
   {
      private readonly List<string> _compoundNames = new List<string>();

      public RenamePKSimSimulationDTO(string name) : base(name)
      {
         Rules.AddRange(AllRules.All());
      }

      private static class AllRules
      {
         public static IEnumerable<IBusinessRule> All()
         {
            yield return CreateRule.For<RenamePKSimSimulationDTO>()
               .Property(x => x.Name)
               .WithRule((x, y) => !x._compoundNames.Contains(y))
               .WithError(PKSimConstants.Error.SimulationCannotShareNamesWithCompounds);
         }
      }

      public void AddCompoundNames(IReadOnlyList<string> compoundNames) => _compoundNames.AddRange(compoundNames);
   }
}