using System.Collections.Generic;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Model
{
   public class QueryExpressionResults
   {
      /// <summary>
      ///    Query configuration that will be saved in pksim and should be used to restore the query when loaded a second time
      /// </summary>
      public string QueryConfiguration { get; set; }

      /// <summary>
      ///    Name of selected Protein
      /// </summary>
      public string ProteinName { get; set; }

      /// <summary>
      ///    Name of selected Unit
      /// </summary>
      public string SelectedUnit { get; set; }

      /// <summary>
      ///    Description of whole action to be used for command description.
      /// </summary>
      public string Description { get; set; }

      public QueryExpressionResults(IReadOnlyList<ExpressionResult> expressionResults)
      {
         ExpressionResults = expressionResults;
      }

      /// <summary>
      ///    Results of query
      /// </summary>
      public IReadOnlyList<ExpressionResult> ExpressionResults { get; }

      public ExpressionResult ExpressionResultFor(string containerName) => ExpressionResults.Find(x => string.Equals(x.ContainerName, containerName));
   }
}