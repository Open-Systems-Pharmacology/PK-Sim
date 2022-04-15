using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Repositories;
using SnapshotExplicitFormula = PKSim.Core.Snapshots.ExplicitFormula;
using ModelExplicitFormula = OSPSuite.Core.Domain.Formulas.ExplicitFormula;

namespace PKSim.Core.Snapshots.Mappers
{
   public class ExplicitFormulaMapper : ObjectBaseSnapshotMapperBase<ModelExplicitFormula, SnapshotExplicitFormula>
   {
      private readonly IObjectBaseFactory _objectBaseFactory;
      private readonly FormulaUsablePathMapper _formulaUsablePathMapper;
      private readonly IDimensionRepository _dimensionRepository;

      public ExplicitFormulaMapper(
         IObjectBaseFactory objectBaseFactory,
         FormulaUsablePathMapper formulaUsablePathMapper,
         IDimensionRepository dimensionRepository)
      {
         _objectBaseFactory = objectBaseFactory;
         _formulaUsablePathMapper = formulaUsablePathMapper;
         _dimensionRepository = dimensionRepository;
      }

      public override async Task<SnapshotExplicitFormula> MapToSnapshot(ModelExplicitFormula formula)
      {
         var snapshot = await SnapshotFrom(formula, x => x.Formula = formula.FormulaString);
         snapshot.References = await _formulaUsablePathMapper.MapToSnapshots(formula.ObjectPaths);
         snapshot.Dimension = formula.Dimension?.Name;
         return snapshot;
      }

      public override async Task<ModelExplicitFormula> MapToModel(SnapshotExplicitFormula snapshot, SnapshotContext snapshotContext)
      {
         var formula = _objectBaseFactory.Create<ModelExplicitFormula>();
         MapSnapshotPropertiesToModel(snapshot, formula);
         formula.FormulaString = snapshot.Formula;
         formula.Dimension = _dimensionRepository.DimensionByName(snapshot.Dimension);
         var objectsPaths = await _formulaUsablePathMapper.MapToModels(snapshot.References, snapshotContext);
         objectsPaths?.Each(formula.AddObjectPath);
         return formula;
      }
   }
}