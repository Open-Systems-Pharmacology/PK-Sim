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
      public static void LogInputFolder(this IWithInputFolder option, StringBuilder sb)
      {
         sb.AppendLine($"Input Folder: {option.InputFolder}");
      }

      public static void LogOutputFolder(this IWithOutputFolder option, StringBuilder sb)
      {
         sb.AppendLine($"Output Folder: {option.OutputFolder}");
      }

      public static void LogFolders(this IWithInputAndOutputFolders option, StringBuilder sb)
      {
         LogInputFolder((IWithInputFolder) option, sb);
         LogOutputFolder((IWithOutputFolder) option, sb);
      }
   }
}