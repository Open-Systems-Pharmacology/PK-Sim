using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Snapshots.Mappers;

namespace PKSim.Core
{
   public abstract class concern_for_DistributedTableFormulaMapper : ContextSpecificationAsync<DistributedTableFormulaMapper>
   {
      protected IFormulaFactory _formulaFactory;
      protected TableFormulaMapper _tableFormulaMapper;
      protected DistributedTableFormula _distributedTableFormula;
      protected Snapshots.DistributedTableFormula _snapshot;
      protected DistributionMetaData _distributionMetaData1;
      protected DistributionMetaData _distributionMetaData2;

      protected override Task Context()
      {
         _tableFormulaMapper = A.Fake<TableFormulaMapper>();
         _formulaFactory = A.Fake<IFormulaFactory>();
         sut = new DistributedTableFormulaMapper(_tableFormulaMapper, _formulaFactory);

         _distributedTableFormula = new DistributedTableFormula();

         _distributionMetaData1 = new DistributionMetaData
         {
            Mean = 1,
            Deviation = 2,
            Distribution = DistributionTypes.LogNormal

         };

         _distributionMetaData2 = new DistributionMetaData
         {
            Mean = 3,
            Deviation = 4,
            Distribution = DistributionTypes.Normal
         };


         _distributedTableFormula.AddDistributionMetaData(_distributionMetaData1);
         _distributedTableFormula.AddDistributionMetaData(_distributionMetaData2);

         _distributedTableFormula.Percentile = 0.8;

         return Task.FromResult(true);
      }
   }

   public class When_mapping_a_distributed_table_formula_to_snapshot : concern_for_DistributedTableFormulaMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_distributedTableFormula);
      }

      [Observation]
      public void should_save_the_distributed_formula_percentile()
      {
         _snapshot.Percentile.ShouldBeEqualTo(_distributedTableFormula.Percentile);
      }

      [Observation]
      public void should_use_the_table_formula_mapper_to_map_all_base_properties_of_table_formula()
      {
         A.CallTo(() => _tableFormulaMapper.UpdateSnapshotProperties(_snapshot, _distributedTableFormula)).MustHaveHappened();
      }

      [Observation]
      public void should_use_the_table_meta_data_to_create_the_snapshot()
      {
         _snapshot.DistributionMetaData.Count.ShouldBeEqualTo(_distributedTableFormula.AllDistributionMetaData().Count);
         _snapshot.DistributionMetaData[0].Mean.ShouldBeEqualTo(_distributionMetaData1.Mean);
         _snapshot.DistributionMetaData[0].Deviation.ShouldBeEqualTo(_distributionMetaData1.Deviation);
         _snapshot.DistributionMetaData[0].Distribution.ShouldBeEqualTo(_distributionMetaData1.Distribution.Id);

         _snapshot.DistributionMetaData[1].Mean.ShouldBeEqualTo(_distributionMetaData2.Mean);
         _snapshot.DistributionMetaData[1].Deviation.ShouldBeEqualTo(_distributionMetaData2.Deviation);
         _snapshot.DistributionMetaData[1].Distribution.ShouldBeEqualTo(_distributionMetaData2.Distribution.Id);
      }
   }

   public class When_mapping_a_distributed_formula_snapshot_to_distributed_table_formula : concern_for_DistributedTableFormulaMapper
   {
      private DistributedTableFormula _newFormula;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_distributedTableFormula);
         A.CallTo(() => _formulaFactory.CreateDistributedTableFormula()).Returns(new DistributedTableFormula());
      }

      protected override async Task Because()
      {
         _newFormula = await sut.MapToModel(_snapshot);
      }

      [Observation]
      public void should_use_the_table_formula_mapper_to_map_all_base_properties_of_table_formula()
      {
         A.CallTo(() => _tableFormulaMapper.UpdateModelProperties(_newFormula, _snapshot)).MustHaveHappened();
      }

      [Observation]
      public void should_have_updated_the_percentile_value()
      {
         _newFormula.Percentile.ShouldBeEqualTo(_snapshot.Percentile);
      }

      [Observation]
      public void should_update_the_table_meta_data()
      {
         _distributedTableFormula.AllDistributionMetaData().Count.ShouldBeEqualTo(_snapshot.DistributionMetaData.Count);
         _distributedTableFormula.AllDistributionMetaData()[0].Mean.ShouldBeEqualTo(_snapshot.DistributionMetaData[0].Mean);
         _distributedTableFormula.AllDistributionMetaData()[0].Deviation.ShouldBeEqualTo(_snapshot.DistributionMetaData[0].Deviation);
         _snapshot.DistributionMetaData[0].Deviation.ShouldBeEqualTo(_distributionMetaData1.Deviation);
         _snapshot.DistributionMetaData[0].Distribution.ShouldBeEqualTo(_distributionMetaData1.Distribution.Id);
      }
   }
}