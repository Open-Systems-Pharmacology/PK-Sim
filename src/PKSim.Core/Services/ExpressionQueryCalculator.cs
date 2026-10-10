using System;
using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IExpressionQueryCalculator
   {
      /// <summary>
      ///    Returns the records of <paramref name="query" /> assigned to each of the <paramref name="containers" /> their tissue
      ///    is mapped to. Records of tissues not mapped to any of the <paramref name="containers" /> are not returned
      /// </summary>
      IReadOnlyList<ContainerExpressionDataRecord> ContainerRecordsFor(ExpressionQuery query, IReadOnlyList<ExpressionContainerInfo> containers);

      /// <summary>
      ///    Returns, for each unit ordered by name, the expression of all <paramref name="containers" />: the mean normalized
      ///    value of the container records accepted by the filter of <paramref name="query" />, and that mean relative to the
      ///    highest mean of the unit
      /// </summary>
      IReadOnlyList<UnitExpression> UnitExpressionsFor(ExpressionQuery query, IReadOnlyList<ExpressionContainerInfo> containers);
   }

   public class ExpressionQueryCalculator : IExpressionQueryCalculator
   {
      public IReadOnlyList<ContainerExpressionDataRecord> ContainerRecordsFor(ExpressionQuery query, IReadOnlyList<ExpressionContainerInfo> containers)
      {
         var containersByName = containers.ToLookup(x => x.ContainerName, StringComparer.OrdinalIgnoreCase);
         var containerNamesByTissue = query.Mapping.Where(x => x.Container != null).ToLookup(x => x.Tissue, x => x.Container, StringComparer.OrdinalIgnoreCase);

         return query.Records.SelectMany(record => containerNamesByTissue[record.Tissue]
               .SelectMany(containerName => containersByName[containerName])
               .Select(container => new ContainerExpressionDataRecord(record, container)))
            .ToList();
      }

      public IReadOnlyList<UnitExpression> UnitExpressionsFor(ExpressionQuery query, IReadOnlyList<ExpressionContainerInfo> containers)
      {
         if (query.Filter == null)
            throw new PKSimException(PKSimConstants.Error.ExpressionQueryFilterOnlyInLayout(query.ProteinName));

         return ContainerRecordsFor(query, containers)
            .Where(x => x.Record.NormValue.HasValue && !string.IsNullOrEmpty(x.Record.Unit))
            .Where(query.Filter.Accepts)
            .GroupBy(x => x.Record.Unit)
            .OrderBy(x => x.Key, StringComparer.CurrentCultureIgnoreCase)
            .Select(x => unitExpressionFor(x.Key, x, containers))
            .ToList();
      }

      private static UnitExpression unitExpressionFor(string unit, IEnumerable<ContainerExpressionDataRecord> unitRecords, IReadOnlyList<ExpressionContainerInfo> containers)
      {
         var meanByContainer = unitRecords.GroupBy(x => x.Container).ToDictionary(x => x.Key, x => x.Average(record => record.Record.NormValue.Value));
         var maximum = meanByContainer.Values.Max();

         return new UnitExpression(unit, containers.Select(container =>
            meanByContainer.TryGetValue(container, out var mean)
               ? new ContainerExpression(container, mean, maximum == 0 ? 0 : mean / maximum)
               : new ContainerExpression(container, null, null)).ToList());
      }
   }
}
