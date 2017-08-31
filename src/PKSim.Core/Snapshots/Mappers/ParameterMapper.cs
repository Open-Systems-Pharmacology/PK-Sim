using System;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
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
         return new SnapshotParameter
         {
            Name = modelParameter.Name,
            Value = modelParameter.ValueInDisplayUnit,
            Unit = SnapshotValueFor(modelParameter.DisplayUnit.Name),
            ValueDescription = SnapshotValueFor(modelParameter.ValueDescription),
            TableFormula = mapFormula(modelParameter.Formula)
         };
      }

      public override IParameter MapToModel(SnapshotParameter snapshot)
      {
         throw new NotSupportedException("Parameter should not be created from snapshot. Instead use the update method");
      }

      public virtual void UpdateParameterFromSnapshot(IParameter parameter, SnapshotParameter snapshotParameter)
      {
         parameter.ValueDescription = snapshotParameter.ValueDescription;
         parameter.DisplayUnit = parameter.Dimension.Unit(UnitValueFor(snapshotParameter.Unit));

         //only update formula if required
         if (snapshotParameter.TableFormula != null)
            parameter.Formula = _tableFormulaMapper.MapToModel(snapshotParameter.TableFormula);

         //This needs to come AFTER formula update so that the base value is accurate
         var baseValue = parameter.Value;
         var snapshotValueInBaseUnit = parameter.ConvertToBaseUnit(snapshotParameter.Value);
         
         if (!ValueComparer.AreValuesEqual(baseValue, snapshotValueInBaseUnit))
            parameter.Value = snapshotValueInBaseUnit;
      }

      private SnapshotTableFormula mapFormula(IFormula formula)
      {
         var tableFormula = formula as ModelTableFormula;
         return tableFormula == null ? null : _tableFormulaMapper.MapToSnapshot(tableFormula);
      }
   }
}