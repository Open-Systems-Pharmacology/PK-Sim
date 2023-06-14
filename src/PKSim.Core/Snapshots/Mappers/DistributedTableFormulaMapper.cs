using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using SnapshotTableFormula = PKSim.Core.Snapshots.DistributedTableFormula;
using ModelTableFormula = OSPSuite.Core.Domain.Formulas.DistributedTableFormula;

namespace PKSim.Core.Snapshots.Mappers
{
   public class DistributedTableFormulaMapper : ObjectBaseSnapshotMapperBase<ModelTableFormula, SnapshotTableFormula>
   {
      private readonly IFormulaFactory _formulaFactory;
      private readonly TableFormulaMapper _tableFormulaMapper;

      public DistributedTableFormulaMapper(TableFormulaMapper tableFormulaMapper, IFormulaFactory formulaFactory)
      {
         _tableFormulaMapper = tableFormulaMapper;
         _formulaFactory = formulaFactory;
      }

      public override Task<SnapshotTableFormula> MapToSnapshot(ModelTableFormula tableFormula)
      {
         return SnapshotFrom(tableFormula, snapshot =>
         {
            _tableFormulaMapper.UpdateSnapshotProperties(snapshot, tableFormula);
            snapshot.Percentile = tableFormula.Percentile;
            snapshot.DistributionMetaData = distributionMetaDataFrom(tableFormula.AllDistributionMetaData());
         });
      }

      private List<DistributionMetaData> distributionMetaDataFrom(IReadOnlyList<OSPSuite.Core.Domain.Formulas.DistributionMetaData> allDistributionMetaData)
      {
         return allDistributionMetaData.Select(x => new DistributionMetaData
         {
            Mean = x.Mean,
            Deviation = x.Deviation,
            Distribution = x.Distribution
         }).ToList();
      }

      public override Task<ModelTableFormula> MapToModel(SnapshotTableFormula snapshot, SnapshotContext snapshotContext)
      {
         var tableFormula = _formulaFactory.CreateDistributedTableFormula();
         _tableFormulaMapper.UpdateModelProperties(tableFormula, snapshot);
         tableFormula.Percentile = snapshot.Percentile;
         snapshot.DistributionMetaData.Select(modelDistributionDataFrom).Each(tableFormula.AddDistributionMetaData);
         return Task.FromResult(tableFormula);
      }

      private OSPSuite.Core.Domain.Formulas.DistributionMetaData modelDistributionDataFrom(DistributionMetaData distributionMetaData)
      {
         return new OSPSuite.Core.Domain.Formulas.DistributionMetaData
         {
            Mean = distributionMetaData.Mean,
            Deviation = distributionMetaData.Deviation,
            Distribution = distributionMetaData.Distribution
         };
      }
   }
}