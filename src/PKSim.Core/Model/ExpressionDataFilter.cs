using System.Collections.Generic;
using System.Linq;

namespace PKSim.Core.Model
{
   public enum ExpressionDataFilterType
   {
      Included,
      Excluded
   }

   /// <summary>
   ///    Filters the expression data records on the values of one field
   /// </summary>
   public class ExpressionDataFieldFilter
   {
      public ExpressionDataFieldFilter(string fieldName, ExpressionDataFilterType filterType, IEnumerable<object> values, bool showBlanks)
      {
         FieldName = fieldName;
         FilterType = filterType;
         Values = values.ToList();
         ShowBlanks = showBlanks;
      }

      public string FieldName { get; }
      public ExpressionDataFilterType FilterType { get; }

      /// <summary>
      ///    Values accepted or rejected by the filter, depending on <see cref="FilterType" />
      /// </summary>
      public IReadOnlyList<object> Values { get; }

      /// <summary>
      ///    Whether records without value for the field are accepted, independently of <see cref="FilterType" />
      /// </summary>
      public bool ShowBlanks { get; }

      public bool Accepts(object value)
      {
         if (value == null)
            return ShowBlanks;

         return Values.Contains(value) == (FilterType == ExpressionDataFilterType.Included);
      }

      public override string ToString()
      {
         var field = $"[{FieldName}]";
         var values = $"{field} In ({string.Join(", ", Values.Select(x => $"'{x}'"))})";
         if (FilterType == ExpressionDataFilterType.Included)
            return ShowBlanks ? $"{values} Or {field} Is Null" : values;

         return ShowBlanks ? $"Not {values} Or {field} Is Null" : $"Not {values} And {field} Is Not Null";
      }
   }

   /// <summary>
   ///    Accepts the expression data records accepted by all its field filters
   /// </summary>
   public class ExpressionDataFilter
   {
      public ExpressionDataFilter() : this(Enumerable.Empty<ExpressionDataFieldFilter>())
      {
      }

      public ExpressionDataFilter(IEnumerable<ExpressionDataFieldFilter> fieldFilters)
      {
         FieldFilters = fieldFilters.ToList();
      }

      public IReadOnlyList<ExpressionDataFieldFilter> FieldFilters { get; }

      public bool Accepts(ContainerExpressionDataRecord containerRecord) => FieldFilters.All(x => x.Accepts(containerRecord.ValueFor(x.FieldName)));

      public override string ToString() => FieldFilters.Count == 1 ? FieldFilters[0].ToString() : string.Join(" And ", FieldFilters.Select(x => $"({x})"));
   }
}
