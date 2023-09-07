using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility.Extensions;
using OSPSuite.Core.Commands;

namespace PKSim.Core.Commands
{
   public interface IPKSimCommand : IOSPSuiteCommand<IExecutionContext>
   {
   }

   public interface IPKSimReversibleCommand : IPKSimCommand, IReversibleCommand<IExecutionContext>
   {
   }

   public interface IPKSimMacroCommand : IPKSimReversibleCommand, IMacroCommand<IExecutionContext>
   {
      new IEnumerable<IOSPSuiteCommand> All();
   }

   public abstract class PKSimCommand : OSPSuiteCommand<IExecutionContext>, IPKSimCommand
   {
   }

   public abstract class PKSimReversibleCommand : OSPSuiteReversibleCommand<IExecutionContext>, IPKSimReversibleCommand
   {
   }

   public class PKSimEmptyCommand : OSPSuiteEmptyCommand<IExecutionContext>, IPKSimCommand
   {
   }

   public class PKSimMacroCommand : OSPSuiteMacroCommand<IExecutionContext>, IPKSimMacroCommand
   {
      public PKSimMacroCommand()
      {
      }

      public PKSimMacroCommand(IEnumerable<ICommand> subCommands) : this()
      {
         subCommands.Each(Add);
      }

      public new IEnumerable<IOSPSuiteCommand> All()
      {
         return base.All().Select(command => command.DowncastTo<IOSPSuiteCommand>());
      }
   }
}