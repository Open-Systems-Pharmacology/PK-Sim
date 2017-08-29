using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using SnapshotParameter = PKSim.Core.Snapshots.Parameter;
using SnapshotTableFormula = PKSim.Core.Snapshots.TableFormula;
using ModelTableFormula = OSPSuite.Core.Domain.Formulas.TableFormula;

namespace PKSim.Core.Snapshots.Mappers
{
   public class ParameterMapper : SnapshotMapperBase<IParameter, SnapshotParameter>
   {
      private readonly TableFormulaMapper _tableFormulaMapper;

      public ParameterMapper(TableFormulaMapper tableFormulaMapper)
      {
         _tableFormulaMapper = tableFormulaMapper;
      }

      public override SnapshotParameter MapToSnapshot(IParameter modelParameter)
      {
         var snapshotParameter = new SnapshotParameter();
         MapBaseProperties(modelParameter, snapshotParameter);
         //override description that might have been set since parameter in PK-Sim all come from DB
         snapshotParameter.Description = null;
         snapshotParameter.Value = modelParameter.ValueInDisplayUnit;
         snapshotParameter.Unit = SnapshotValueFor(modelParameter.DisplayUnit.Name);
         snapshotParameter.ValueDescription = SnapshotValueFor(modelParameter.ValueDescription);
         snapshotParameter.TableFormula = mapFormula(modelParameter.Formula);
         return snapshotParameter;
      }

      private SnapshotTableFormula mapFormula(IFormula formula)
      {
         var tableFormula = formula as ModelTableFormula;
         return tableFormula == null ? null : _tableFormulaMapper.MapToSnapshot(tableFormula);
      }
   }
}