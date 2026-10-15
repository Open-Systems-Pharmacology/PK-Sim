using System.Data;

namespace PKSim.Core.Services
{
   public interface IProteinExpressionDataHelper
   {
      /// <summary>
      ///    Converts an arraylist of a class object to a data table object.
      /// </summary>
      DataTable ConvertToDataTable(object[] array);
   }
}