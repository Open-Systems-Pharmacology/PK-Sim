using System;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.R.Bootstrap;
using PKSim.R.Services;

namespace PKSim.R
{
   public static class Api
   {
      private static IContainer _container;

      public static void InitializeOnce()
      {
         _container = ApplicationStartup.Initialize();
      }

      public static IIndividualFactory GetIndividualFactory() => resolveTask<IIndividualFactory>();

      private static T resolveTask<T>()
      {
         try
         {
            return _container.Resolve<T>();
         }
         catch (Exception e)
         {
            Console.WriteLine(e.FullMessage());
            throw;
         }
      }
   }
}