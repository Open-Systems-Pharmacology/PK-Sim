using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain.Formulas;
using PKSim.Core.Snapshots.Mappers;

namespace PKSim.Core
{
   public abstract class concern_for_TableFormulaMapper : ContextSpecification<TableFormulaMapper>
   {
      protected override void Context()
      {
         sut = new TableFormulaMapper();
      }
   }

   public class When_mapping_a_table_formula_to_snapshot : concern_for_TableFormulaMapper
   {
      private TableFormula _tableFormula;
      private Snapshots.TableFormula _snapshot;

      protected override void Context()
      {
         base.Context();
         _tableFormula = new TableFormula
         {
            XName = "pH",
            YName = "Value",
            XDimension = DomainHelperForSpecs.TimeDimensionForSpecs(),
            Dimension = DomainHelperForSpecs.LengthDimensionForSpecs()
         };

         _tableFormula.XDisplayUnit = _tableFormula.XDimension.Unit("h");
         _tableFormula.YDisplayUnit = _tableFormula.Dimension.Unit("cm");


         _tableFormula.AddPoint(60, 1); //60 min, 1m
         _tableFormula.AddPoint(120, 2); //120 min, 2m
      }

      protected override void Because()
      {
         _snapshot = sut.MapToSnapshot(_tableFormula);
      }

      [Observation]
      public void should_save_the_basic_properties_in_the_snapshot()
      {
         _snapshot.XName.ShouldBeEqualTo(_tableFormula.XName);
         _snapshot.YName.ShouldBeEqualTo(_tableFormula.YName);
      }

      [Observation]
      public void should_save_the_points_value_using_the_dislay_values()
      {
         _snapshot.Points[0].X.ShouldBeEqualTo(1);
         _snapshot.Points[0].Y.ShouldBeEqualTo(100);

         _snapshot.Points[1].X.ShouldBeEqualTo(2);
         _snapshot.Points[1].Y.ShouldBeEqualTo(200);
      }
   }
}