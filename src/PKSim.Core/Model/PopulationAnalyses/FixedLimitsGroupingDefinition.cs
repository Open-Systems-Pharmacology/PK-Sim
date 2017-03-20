using System;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model.PopulationAnalyses
{
   public class FixedLimitsGroupingDefinition : IntervalGroupingDefinition
   {
      [Obsolete("For serialization")]
      public FixedLimitsGroupingDefinition() : this(null)
      {
      }

      public FixedLimitsGroupingDefinition(string fieldName) : base(fieldName)
      {
      }

      public void SetLimits(IOrderedEnumerable<double> limits)
      {
         Limits = limits.ToList();
      }

      public override void UpdatePropertiesFrom(IUpdatable source, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(source, cloneManager);
         var binSizeGroupingDefinition = source as FixedLimitsGroupingDefinition;
         if (binSizeGroupingDefinition == null) return;
         Limits = binSizeGroupingDefinition.Limits.ToList();
      }
   }
}