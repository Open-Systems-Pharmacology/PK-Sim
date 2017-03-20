using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.UnitSystem;

namespace PKSim.Core.Services
{
   public abstract class ConcentrationDimensionConverter : ColumnWithMolWeightDimensionConverter
   {
      protected ConcentrationDimensionConverter(DataColumn column, IDimension sourceDimension, IDimension targetDimension) : base(column, sourceDimension, targetDimension)
      {
      }

      public override bool CanResolveParameters()
      {
         return isConcentration(_column) && _column.DataInfo.MolWeight != null;
      }

      private bool isConcentration(DataColumn column)
      {
         if (column.DataInfo.Origin == ColumnOrigins.Calculation)
            return true;

         if (column.DataInfo.Origin == ColumnOrigins.CalculationAuxiliary)
            return true;

         if (column.DataInfo.Origin == ColumnOrigins.Observation)
            return true;

         return (column.DataInfo.Origin == ColumnOrigins.ObservationAuxiliary &&
                 column.DataInfo.AuxiliaryType == AuxiliaryType.ArithmeticStdDev);
      }
   }
}