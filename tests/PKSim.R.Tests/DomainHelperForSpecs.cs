using System;
using System.IO;

namespace PKSim.R;

internal static class DomainHelperForSpecs
{
   // Borrowing test data from PKSim.Tests
   private static readonly string _pathToData = "..\\..\\..\\Data\\";

   public static string DataFilePathFor(string fileNameWithExtension) => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _pathToData, fileNameWithExtension);
}