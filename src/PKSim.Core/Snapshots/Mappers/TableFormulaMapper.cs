using OSPSuite.Core.Domain.UnitSystem;
using PKSim.Core.Repositories;
using ModelTableFormula = OSPSuite.Core.Domain.Formulas.TableFormula;
using IFormulaFactory = PKSim.Core.Model.IFormulaFactory;

namespace PKSim.Core.Snapshots.Mappers
{
   public class TableFormulaMapper : OSPSuite.Core.Snapshots.Mappers.TableFormulaMapper
   {
      private readonly IFormulaFactory _formulaFactory;
      private readonly IDimensionRepository _dimensionRepository;

      public TableFormulaMapper(IFormulaFactory formulaFactory, IDimensionRepository dimensionRepository)
      {
         _formulaFactory = formulaFactory;
         _dimensionRepository = dimensionRepository;
      }

      protected override ModelTableFormula CreateNewTableFormula()
      {
         return _formulaFactory.CreateTableFormula();
      }

      protected override IDimension DimensionByName(string xDimension)
      {
         return _dimensionRepository.DimensionByName(xDimension);
      }
   }
}