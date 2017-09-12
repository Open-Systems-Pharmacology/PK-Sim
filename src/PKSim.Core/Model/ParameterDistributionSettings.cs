using System.Collections.Generic;
using OSPSuite.Utility.Collections;
using PKSim.Core.Chart;

namespace PKSim.Core.Model
{
   public class ParameterDistributionSettingsCache : Cache<string, ParameterDistributionSettings>
   {
      public ParameterDistributionSettingsCache() : base(x => x.ParameterPath, x => null)
      {
      }

      public IEnumerable<ParameterDistributionSettings> AllParameterDistributions => this;

      public virtual ParameterDistributionSettingsCache Clone()
      {
         var clone = new ParameterDistributionSettingsCache();
         foreach (var parameterDistributionSettings in this)
         {
            clone.Add(parameterDistributionSettings.Clone());
         }
         return clone;
      }
   }

   public class ParameterDistributionSettings
   {
      public virtual string ParameterPath { get; set; }
      public virtual DistributionSettings Settings { get; set; } = new DistributionSettings();

      public ParameterDistributionSettings Clone()
      {
         return new ParameterDistributionSettings
         {
            ParameterPath = ParameterPath,
            Settings = Settings.Clone()
         };
      }

      public bool UseInReport
      {
         get => Settings.UseInReport;
         set => Settings.UseInReport = value;
      }
   }
}