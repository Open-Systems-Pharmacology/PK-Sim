using OSPSuite.Core.Domain.UnitSystem;
using PKSim.Core.Snapshots;

namespace PKSim.Matlab.Mappers
{
   public interface IMatlabParameterToSnapshotParameterMapper
   {
      Parameter MapFrom(double value, IDimension dimension);
   }

   public class MatlabParameterToSnapshotParameterMapper : IMatlabParameterToSnapshotParameterMapper
   {
      public Parameter MapFrom(double value, IDimension dimension)
      {
         if (double.IsNaN(value))
            return null;

         return new Parameter
         {
            Value = value,
            Unit = dimension.BaseUnit.Name,
         };
      }
   }
}