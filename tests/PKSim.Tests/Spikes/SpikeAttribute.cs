using System;
using NUnit.Framework;

namespace PKSim.Spikes
{
   /// <summary>
   /// Attribute should be use on TestFixture class that should be recognized as Spike Test.
   /// </summary>
   [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
   public class SpikeAttribute : CategoryAttribute
   {
      /// <summary>
      /// Constructor setting the name to "IntegrationTests"
      /// </summary>
      public SpikeAttribute(): base("Spike")
      {
      }
   }
}