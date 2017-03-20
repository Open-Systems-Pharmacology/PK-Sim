using System.Globalization;
using LumenWorks.Framework.IO.Csv;

namespace PKSim.Infrastructure.Extensions
{
   public static class CsvReaderExtensions
   {
      /// <summary>
      ///    Returns the double value define in the current record at the index <paramref name="fieldIndex" />
      /// </summary>
      public static double DoubleAt(this CsvReader csv, int fieldIndex)
      {
         var value = csv[fieldIndex];
         if (string.IsNullOrEmpty(value))
            return double.NaN;

         return double.Parse(value, NumberFormatInfo.InvariantInfo);
      }

      /// <summary>
      ///    Returns the float  value define in the current record at the index <paramref name="fieldIndex" />
      /// </summary>
      public static float FloatAt(this CsvReader csv, int fieldIndex)
      {
         var value = csv[fieldIndex];
         if (string.IsNullOrEmpty(value))
            return float.NaN;

         return float.Parse(value, NumberFormatInfo.InvariantInfo);
      }

      /// <summary>
      ///    Returns the int value define in the current record at the index <paramref name="fieldIndex" />
      /// </summary>
      public static int IntAt(this CsvReader csv, int fieldIndex)
      {
         return int.Parse(csv[fieldIndex], NumberFormatInfo.InvariantInfo);
      }
   }
}