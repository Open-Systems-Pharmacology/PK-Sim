using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Descriptors;

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
      public DescriptorCriteria ContainerCriteria { get; set; }

      /// <summary>
      ///    Species if the parameter is a parameter set by the user or a input parameter parameter set in the DB. Default value
      ///    is <c>false</c>
      /// </summary>
      public bool IsInput { get; set; } = false;

      /// <summary>
      ///    Species if the parameter is a parameter set by the user or a default parameter parameter set in the DB. Default
      ///    value is <c>true</c>
      /// </summary>
      public bool IsDefault
      {
         get => !IsInput;
         set => IsInput = !value;
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

      public virtual void UpdatePropertiesFrom(ParameterMetaData parameterMetaData)
      {
         ParameterName = parameterMetaData.ParameterName;
         ContainerId = parameterMetaData.ContainerId;
         ContainerType = parameterMetaData.ContainerType;
         ContainerName = parameterMetaData.ContainerName;
         BuildMode = parameterMetaData.BuildMode;
         ValueOriginId = parameterMetaData.ValueOriginId;
         ValueOrigin = parameterMetaData.ValueOrigin;
         ContainerCriteria = parameterMetaData.ContainerCriteria;
         Dimension = parameterMetaData.Dimension;
         base.UpdatePropertiesFrom(parameterMetaData);
      }
   }
}