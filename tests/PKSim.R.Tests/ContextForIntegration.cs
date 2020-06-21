using System;
using NUnit.Framework;
using OSPSuite.BDDHelper;

namespace PKSim.R
{
   [IntegrationTests]
   [Category("R")]
   public abstract class ContextForIntegration<T> : ContextSpecification<T>
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         Api.InitializeOnce();

         Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
      }
   }
}