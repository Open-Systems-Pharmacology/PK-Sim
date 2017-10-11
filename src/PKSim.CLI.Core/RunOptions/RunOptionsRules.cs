using OSPSuite.Core.Domain;
using OSPSuite.Utility.Validation;

namespace PKSim.CLI.Core.RunOptions
{
   public static class RunOptionsRules
   {
      public static IBusinessRule OutputFolderDefined { get; } = GenericRules.NonEmptyRule<IWithOutputFolder>(x => x.OutputFolder);
      public static IBusinessRule InputFolderDefined { get; } = GenericRules.NonEmptyRule<IWithInputFolder>(x => x.InputFolder);
   }
}