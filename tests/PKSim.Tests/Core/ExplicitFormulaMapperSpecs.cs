using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.UnitSystem;
using PKSim.Core.Repositories;
using PKSim.Core.Snapshots.Mappers;

namespace PKSim.Core
{
   public abstract class concern_for_ExplicitFormulaMapper : ContextSpecificationAsync<ExplicitFormulaMapper>
   {
      protected ExplicitFormula _formula;
      protected IDimension _dimension1;
      private IDimensionRepository _dimensionRepository;
      private IDimension _dimension2;
      private FormulaUsablePathMapper _formulaUsablePathMapper;
      private IObjectBaseFactory _objectBaseFactory;

      protected override Task Context()
      {
         _dimensionRepository = A.Fake<IDimensionRepository>();
         _objectBaseFactory = A.Fake<IObjectBaseFactory>();
         _formulaUsablePathMapper = new FormulaUsablePathMapper(_dimensionRepository);
         sut = new ExplicitFormulaMapper(_objectBaseFactory, _formulaUsablePathMapper, _dimensionRepository);


         _dimension1 = DomainHelperForSpecs.ConcentrationDimensionForSpecs();
         _dimension2 = DomainHelperForSpecs.TimeDimensionForSpecs();
         _formula = new ExplicitFormula("P1+P2+P3+P4") { Dimension = _dimension1 };

         _formula.AddObjectPath(new FormulaUsablePath("A", "B", "C") {Alias = "P1", Dimension = _dimension1});
         _formula.AddObjectPath(new FormulaUsablePath("A", "B") {Alias = "P2", Dimension = _dimension2});
         _formula.AddObjectPath(new FormulaUsablePath("A") {Alias = "P3", Dimension = Constants.Dimension.NO_DIMENSION});
         _formula.AddObjectPath(new FormulaUsablePath("A") {Alias = "P4"});

         A.CallTo(() => _dimensionRepository.DimensionByName(_dimension1.Name)).Returns(_dimension1);
         A.CallTo(() => _objectBaseFactory.Create<ExplicitFormula>()).Returns(new ExplicitFormula());
         return _completed;
      }
   }

   public class When_mapping_an_explicit_formula_to_snapshot : concern_for_ExplicitFormulaMapper
   {
      private Snapshots.ExplicitFormula _result;

      protected override async Task Because()
      {
         _result = await sut.MapToSnapshot(_formula);
      }

      [Observation]
      public void should_return_a_valid_snapshot()
      {
         _result.Formula.ShouldBeEqualTo(_formula.FormulaString);
         _result.References.Length.ShouldBeEqualTo(_formula.ObjectPaths.Count);
      }
   }

   public class When_mapping_an_explicit_formula_snapshot_to_formula : concern_for_ExplicitFormulaMapper
   {
      private Snapshots.ExplicitFormula _snapshot;
      private ExplicitFormula _result;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_formula);
      }

      protected override async Task Because()
      {
         _result = await sut.MapToModel(_snapshot);
      }

      [Observation]
      public void should_return_a_valid_snapshot()
      {
         _result.FormulaString.ShouldBeEqualTo(_formula.FormulaString);
         _result.ObjectPaths.Count.ShouldBeEqualTo(_formula.ObjectPaths.Count);
         _result.Dimension.ShouldBeEqualTo(_dimension1);

         for (int i = 0; i < _formula.ObjectPaths.Count; i++)
         {
            var objectPath = _formula.ObjectPaths[i];
            var newObjectPath = _result.ObjectPaths[i];

            objectPath.Alias.ShouldBeEqualTo(newObjectPath.Alias);
            objectPath.PathAsString.ShouldBeEqualTo(newObjectPath.PathAsString);
         }
      }
   }
}