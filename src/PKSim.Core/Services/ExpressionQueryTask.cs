using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PKSim.Assets;
using PKSim.Core.Mappers;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   /// <summary>
   ///    Creates, restores and evaluates expression queries without user interface
   /// </summary>
   public interface IExpressionQueryTask
   {
      /// <summary>
      ///    Creates a query on the expression data of the gene <paramref name="geneId" /> in the connected gene expression
      ///    database, using the default tissue to container mapping of the database
      /// </summary>
      ExpressionQuery CreateQueryFor(long geneId, string proteinName);

      /// <summary>
      ///    Restores the query saved in <paramref name="queryConfiguration" />
      /// </summary>
      ExpressionQuery QueryFrom(string queryConfiguration);

      /// <summary>
      ///    Returns the relative expressions of the <paramref name="containers" /> in the selected unit of
      ///    <paramref name="query" />, or in the first unit with data when no unit is selected. The selected unit of
      ///    <paramref name="query" /> is set to the unit used.
      /// </summary>
      QueryExpressionResults ResultsFor(ExpressionQuery query, IReadOnlyList<ExpressionContainerInfo> containers);
   }

   public class ExpressionQueryTask : IExpressionQueryTask
   {
      private readonly IGeneExpressionQueries _geneExpressionQueries;
      private readonly IExpressionDataTableMapper _expressionDataTableMapper;
      private readonly IExpressionQueryCalculator _expressionQueryCalculator;
      private readonly IExpressionQuerySerializer _expressionQuerySerializer;

      public ExpressionQueryTask(
         IGeneExpressionQueries geneExpressionQueries,
         IExpressionDataTableMapper expressionDataTableMapper,
         IExpressionQueryCalculator expressionQueryCalculator,
         IExpressionQuerySerializer expressionQuerySerializer)
      {
         _geneExpressionQueries = geneExpressionQueries;
         _expressionDataTableMapper = expressionDataTableMapper;
         _expressionQueryCalculator = expressionQueryCalculator;
         _expressionQuerySerializer = expressionQuerySerializer;
      }

      public ExpressionQuery CreateQueryFor(long geneId, string proteinName)
      {
         var records = _expressionDataTableMapper.RecordsFrom(_geneExpressionQueries.GetExpressionDataByGeneId(geneId));
         var mapping = _expressionDataTableMapper.MappingFrom(_geneExpressionQueries.GetContainerTissueMapping());

         var mappedTissues = new HashSet<string>(mapping.Select(x => x.Tissue));
         var unmappedTissues = records.Select(x => x.Tissue).Distinct()
            .Where(x => !mappedTissues.Contains(x))
            .OrderBy(x => x, StringComparer.CurrentCultureIgnoreCase)
            .Select(x => new TissueContainerMapping(x, container: null));

         return new ExpressionQuery(proteinName, records, mapping.Concat(unmappedTissues).ToList());
      }

      public ExpressionQuery QueryFrom(string queryConfiguration) => _expressionQuerySerializer.Deserialize(queryConfiguration);

      public QueryExpressionResults ResultsFor(ExpressionQuery query, IReadOnlyList<ExpressionContainerInfo> containers)
      {
         var unitExpressions = _expressionQueryCalculator.UnitExpressionsFor(query, containers);
         var unitExpression = string.IsNullOrEmpty(query.SelectedUnit)
            ? unitExpressions.FirstOrDefault()
            : unitExpressions.FirstOrDefault(x => string.Equals(x.Unit, query.SelectedUnit));

         if (unitExpression == null)
            throw new PKSimException(PKSimConstants.Error.NoExpressionDataForUnit(query.ProteinName, query.SelectedUnit));

         query.SelectedUnit = unitExpression.Unit;
         var expressionResults = unitExpression.ContainerExpressions
            .Select(x => new ExpressionResult {ContainerName = x.Container.ContainerName, RelativeExpression = x.RelativeExpression ?? 0})
            .ToList();

         return new QueryExpressionResults(expressionResults)
         {
            ProteinName = query.ProteinName,
            SelectedUnit = query.SelectedUnit,
            QueryConfiguration = _expressionQuerySerializer.Serialize(query),
            Description = descriptionFor(query)
         };
      }

      private static string descriptionFor(ExpressionQuery query)
      {
         var description = new StringBuilder();
         description.AppendLine($"Selected protein: {query.ProteinName}");
         description.AppendLine($"Selected unit: {query.SelectedUnit}");
         if (query.Filter.FieldFilters.Any())
         {
            description.AppendLine("Filter used: ");
            description.AppendLine(query.Filter.ToString());
         }

         description.AppendLine("Mapping used: ");
         foreach (var mapping in query.Mapping.Where(x => !string.IsNullOrEmpty(x.Tissue) && !string.IsNullOrEmpty(x.Container)))
         {
            description.AppendLine($"Tissue [{mapping.Tissue}] -> Container [{mapping.Container}]");
         }

         return description.ToString();
      }
   }
}
