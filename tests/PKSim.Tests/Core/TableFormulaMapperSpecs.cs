using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain.Formulas;
using PKSim.Core.Repositories;
using PKSim.Core.Snapshots.Mappers;
using IFormulaFactory = PKSim.Core.Model.IFormulaFactory;

namespace PKSim.Core
{
   public abstract class concern_for_TableFormulaMapper : ContextSpecificationAsync<TableFormulaMapper>
   {
      protected IFormulaFactory _formulaFactory;
      private IDimensionRepository _dimensionRepository;
      protected TableFormula _tableFormula;

      protected override Task Context()
      {
         _formulaFactory= A.Fake<IFormulaFactory>();  
         _dimensionRepository= A.Fake<IDimensionRepository>();

         sut = new TableFormulaMapper(_formulaFactory,_dimensionRepository);

         _tableFormula = new TableFormula
         {
            XName = "pH",
            YName = "Value",
            XDimension = DomainHelperForSpecs.TimeDimensionForSpecs(),
            Dimension = DomainHelperForSpecs.LengthDimensionForSpecs()
         };

         _tableFormula.XDisplayUnit = _tableFormula.XDimension.Unit("h");
         _tableFormula.YDisplayUnit = _tableFormula.Dimension.Unit("cm");

         _tableFormula.UseDerivedValues = true;

         _tableFormula.AddPoint(60, 1); //60 min, 1m
         _tableFormula.AddPoint(120, 2); //120 min, 2m
         _tableFormula.AllPoints().ElementAt(1).RestartSolver = true;

         A.CallTo(() => _dimensionRepository.DimensionByName(_tableFormula.XDimension.Name)).Returns(_tableFormula.XDimension);
         A.CallTo(() => _dimensionRepository.DimensionByName(_tableFormula.Dimension.Name)).Returns(_tableFormula.Dimension);

         return Task.FromResult(true);
      }
   }

   public class When_mapping_a_table_formula_to_snapshot : concern_for_TableFormulaMapper
   {
      private Snapshots.TableFormula _snapshot;

      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_tableFormula);
      }

      [Observation]
      public void should_save_the_basic_properties_in_the_snapshot()
      {
         _snapshot.XName.ShouldBeEqualTo(_tableFormula.XName);
         _snapshot.YName.ShouldBeEqualTo(_tableFormula.YName);

         _snapshot.UseDerivedValues.ShouldBeEqualTo(_tableFormula.UseDerivedValues);
      }

      [Observation]
      public void should_save_the_points_value_using_the_dislay_values()
      {
         _snapshot.Points[0].X.ShouldBeEqualTo(1);
         _snapshot.Points[0].Y.ShouldBeEqualTo(100);

         _snapshot.Points[1].X.ShouldBeEqualTo(2);
         _snapshot.Points[1].Y.ShouldBeEqualTo(200);
         _snapshot.Points[1].RestartSolver.ShouldBeTrue();
      }
   }

   public class When_mapping_a_table_formula_snapshot_to_a_table_formula : concern_for_TableFormulaMapper
   {
      private Snapshots.TableFormula _snapshot;
      private TableFormula _newTableFormula;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_tableFormula);
         A.CallTo(() => _formulaFactory.CreateTableFormula()).Returns(new TableFormula());
      }

      protected override async Task Because()
      {
         _newTableFormula = await sut.MapToModel(_snapshot);
      }

      [Observation]
      public void should_create_a_new_table_formula_and_set_its_properties_based_on_the_snapshot_values()
      {
         _newTableFormula.XName.ShouldBeEqualTo(_snapshot.XName);
         _newTableFormula.XDisplayUnit.Name.ShouldBeEqualTo(_snapshot.XUnit);
         _newTableFormula.XDimension.Name.ShouldBeEqualTo(_snapshot.XDimension);

         _newTableFormula.YName.ShouldBeEqualTo(_snapshot.YName);
         _newTableFormula.YDisplayUnit.Name.ShouldBeEqualTo(_snapshot.YUnit);
         _newTableFormula.Dimension.Name.ShouldBeEqualTo(_snapshot.YDimension);

         _newTableFormula.UseDerivedValues.ShouldBeEqualTo(_snapshot.UseDerivedValues);
      }

      [Observation]
      public void should_create_one_value_point_for_each_point_defined_in_the_snapshot_formula()
      {
         _newTableFormula.AllPoints().Count().ShouldBeEqualTo(_tableFormula.AllPoints().Count());

         for (int i = 0; i < _newTableFormula.AllPoints().Count(); i++)
         {
            var p1 = _newTableFormula.AllPoints().ElementAt(i);
            var p2 = _tableFormula.AllPoints().ElementAt(i);
            p1.X.ShouldBeEqualTo(p2.X);
            p1.Y.ShouldBeEqualTo(p2.Y);
            p1.RestartSolver.ShouldBeEqualTo(p2.RestartSolver);
         }

      }
   }
}