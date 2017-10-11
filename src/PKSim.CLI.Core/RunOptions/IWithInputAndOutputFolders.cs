using System.Text;

namespace PKSim.CLI.Core.RunOptions
{
   public interface IWithInputFolder
   {
      string InputFolder { get; set; }
   }

   public interface IWithOutputFolder
   {
      string OutputFolder { get; set; }
   }

   public interface IWithInputAndOutputFolders : IWithInputFolder, IWithOutputFolder
   {
   }

   public static class WithInputAndOutputFoldersExtensions
   {
      public static void LogOption(this IWithInputAndOutputFolders option, StringBuilder sb)
      {
         sb.AppendLine($"Input Folder: {option.InputFolder}");
         sb.AppendLine($"Output Folder: {option.OutputFolder}");
      }
   }
}