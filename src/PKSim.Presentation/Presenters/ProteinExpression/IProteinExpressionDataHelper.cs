using System.Collections;
using System.Data;

namespace PKSim.Presentation.Presenters.ProteinExpression
{
   public enum JoinType
   {
      Inner,
      LeftOuter,
      RightOuter,
      FullOuter
   }

   public interface IProteinExpressionDataHelper
   {
      DataTable CreateDataJoin(DataRelation dr, JoinType jt, string tableName);

      /// <summary>
      ///    Converts an arraylist of a class object to a data table object.
      /// </summary>
      DataTable ConvertToDataTable(object[] array);

      /// <summary>
      ///    This helping method retrieves a string collection with all distinct values of the given column.
      /// </summary>
      ICollection GetDistinctLoV(DataColumn column);
   }
}