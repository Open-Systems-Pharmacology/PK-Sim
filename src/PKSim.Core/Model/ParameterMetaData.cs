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
      public int? ValueOriginId { get; set; }
      public ValueOrigin ValueOrigin { get; set; }
      private bool _isInput = false;

      /// <summary>
      /// Species if the parameter is a parameter set by the user or a input parameter parameter set in the DB. Default value is <c>false</c>
      /// </summary>
      public bool IsInput
      {
         get => _isInput;
         set => _isInput = value;
      }

      /// <summary>
      /// Species if the parameter is a parameter set by the user or a default parameter parameter set in the DB. Default value is <c>true</c>
      /// </summary>
      public bool IsDefault
      {
         get => !_isInput;
         set => _isInput = !value;
      }

      private string _dimension;

      public string Dimension
      {
         get => _dimension;
         set => _dimension = string.Equals(Constants.Dimension.DIMENSIONLESS, value) ? string.Empty : value;
      }

      public string ParentContainerPath { get; set; }

      public override string ToString()
      {
         return $"{ContainerName}-{ParameterName}";
      }
   }
}