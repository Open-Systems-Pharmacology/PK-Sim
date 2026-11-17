using System.Collections.Generic;

namespace PKSim.Core.Model
{
   /// <summary>
   ///    Query of the expression of a protein in a gene expression database: the records retrieved for the protein, how their
   ///    tissues map to containers and the filter selecting the records used to compute the relative expressions
   /// </summary>
   public class ExpressionQuery
   {
      public ExpressionQuery(string proteinName, IReadOnlyList<ExpressionDataRecord> records, IReadOnlyList<TissueContainerMapping> mapping)
      {
         ProteinName = proteinName;
         Records = records;
         Mapping = mapping;
      }

      public string ProteinName { get; }
      public IReadOnlyList<ExpressionDataRecord> Records { get; }
      public IReadOnlyList<TissueContainerMapping> Mapping { get; set; }

      /// <summary>
      ///    Filter selecting the records used to compute the relative expressions.
      ///    Null when the query was saved by a version of PK-Sim that only kept the filter in the <see cref="LayoutSettings" />:
      ///    the filter can then only be restored by the view displaying the query
      /// </summary>
      public ExpressionDataFilter Filter { get; set; } = new ExpressionDataFilter();

      /// <summary>
      ///    Unit of the relative expressions. When empty, the first unit with data is used
      /// </summary>
      public string SelectedUnit { get; set; }

      /// <summary>
      ///    Layout of the view displaying the query, saved with the query. It has no influence on the relative expressions
      /// </summary>
      public string LayoutSettings { get; set; }
   }
}
