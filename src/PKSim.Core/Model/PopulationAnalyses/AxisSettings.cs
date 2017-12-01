using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model.PopulationAnalyses
{
   public class AxisSettings : IUpdatable
   {
      public double? Min { set; get; }
      public double? Max { set; get; }

      /// <summary>
      /// Indicates that the axis range is automatically configured by the control to fit all the content
      /// In other words, Min and Max would not be applied.
      /// </summary>
      public bool AutoRange { get; set; }

      public void UpdatePropertiesFrom(IUpdatable source, ICloneManager cloneManager)
      {
         var sourceSettings = source as AxisSettings;
         if (sourceSettings == null)
            return;

         Min = sourceSettings.Min;
         Max = sourceSettings.Max;
         AutoRange = sourceSettings.AutoRange;
      }

      public bool HasRange => Max.HasValue && Min.HasValue;
   }
}