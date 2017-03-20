using System;
using NUnit.Framework;

namespace PKSim.IntegrationTests
{
   /// <summary>
   /// Attribute should be use on TestFixture class that should be recognized as Test to run on nightly only
   /// </summary>
   [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
   public class NightlyOnlyAttribute : CategoryAttribute
   {
      /// <summary>
      /// Constructor setting the name to "IntegrationTests"
      /// </summary>
      public NightlyOnlyAttribute() : base("NightlyOnly")
      {
      }
   }
}