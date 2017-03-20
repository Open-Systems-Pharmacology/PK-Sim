using System.Collections.Generic;

namespace PKSim.Core.Model
{
   public class QueryExpressionSettings
   {
      public QueryExpressionSettings(IEnumerable<ExpressionContainerInfo> expressionContainers, string queryConfiguration)
      {
         ExpressionContainers = expressionContainers;
         QueryConfiguration = queryConfiguration;
      }

      /// <summary>
      ///    All containers for which a protein expression might be retrieved
      /// </summary>
      public IEnumerable<ExpressionContainerInfo> ExpressionContainers { get; private set; }

      /// <summary>
      ///    Former query configuration when the component is called for the second time or after project load
      /// </summary>
      public string QueryConfiguration { get; private set; }
   }
}