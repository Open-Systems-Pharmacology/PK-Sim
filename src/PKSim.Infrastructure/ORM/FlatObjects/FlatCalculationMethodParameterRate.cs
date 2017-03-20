using OSPSuite.Core.Domain;

namespace PKSim.Infrastructure.ORM.FlatObjects
{
   public class FlatCalculationMethodParameterRate : ParameterInfo
   {
      public int ParameterId { get; set; }
      public string ParameterName { get; set; }
      public string CalculationMethod { get; set; }
      public string Rate { get; set; }
      public bool IsOutputParameter { get; set; }

      private string _dimension;

      public string Dimension
      {
         get { return _dimension; }
         set { _dimension = string.Equals(Constants.Dimension.DIMENSIONLESS, value) ? string.Empty : value; }
      }
   }
}