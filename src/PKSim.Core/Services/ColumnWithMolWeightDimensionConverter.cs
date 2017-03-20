using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.UnitSystem;

namespace PKSim.Core.Services
{
   public abstract class ColumnWithMolWeightDimensionConverter : DimensionConverterBase
   {
      protected readonly DataColumn _column;

      protected ColumnWithMolWeightDimensionConverter(DataColumn column, IDimension sourceDimension, IDimension targetDimension) :
         base(sourceDimension, targetDimension)
      {
         _column = column;
      }

      public override bool CanResolveParameters()
      {
         return _column.DataInfo.MolWeight != null;
      }

      protected override double MolWeight
      {
         get { return _column.DataInfo.MolWeight.HasValue ? _column.DataInfo.MolWeight.Value : double.NaN; }
      }
   }
}