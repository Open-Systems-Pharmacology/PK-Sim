using OSPSuite.Core.Domain;

namespace PKSim.Core.Model
{
   public class ParameterMetaData : ParameterInfo
   {
      public string ParameterName { get; set; }
      public int ContainerId { get; set; }
      public string ContainerType { get; set; }
      public string ContainerName { get; set; }
      public ParameterBuildMode BuildMode { get; set; }

      private string _dimension;
      public string Dimension
      {
         get { return _dimension; }
         set { _dimension = string.Equals(Constants.Dimension.DIMENSIONLESS, value) ? string.Empty : value; }
      }

      public string ParentContainerPath { get; set; }

      public override string ToString()
      {
         return string.Format("{0}-{1}", ContainerName, ParameterName);
      }
   }
}