using System;
using System.Reflection;
using OSPSuite.CLI.Core.RunOptions;
using OSPSuite.CLI.Core.Services;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.CLI.Core.RunOptions;
using PKSim.CLI.Core.Services;
using PKSim.R.Bootstrap;
using PKSim.R.Services;
using IIndividualFactory = PKSim.R.Services.IIndividualFactory;

namespace PKSim.R
{
   public static class Api
   {
      private static IContainer _container;

      public static void InitializeOnce()
      {
         try
         {
            _container = ApplicationStartup.Initialize();
         }
         catch (ReflectionTypeLoadException e)
         {
            e.LoaderExceptions?.Each(x => Console.WriteLine(e.FullMessage()));
            throw;
         }
      }

      public static IIndividualFactory GetIndividualFactory() => ResolveTask<IIndividualFactory>();

      public static IPopulationFactory GetPopulationFactory() => ResolveTask<IPopulationFactory>();

      public static void RunSnapshot(SnapshotRunOptions runOptions) => ResolveTask<IBatchRunner<SnapshotRunOptions>>().RunBatchAsync(runOptions).Wait();

      public static void RunExport(ExportRunOptions runOptions) => ResolveTask<IBatchRunner<ExportRunOptions>>().RunBatchAsync(runOptions).Wait();

      public static void RunQualification(QualificationRunOptions runOptions) => ResolveTask<IBatchRunner<QualificationRunOptions>>().RunBatchAsync(runOptions).Wait();

      public static void RunJson(JsonRunOptions runOptions) => ResolveTask<IBatchRunner<JsonRunOptions>>().RunBatchAsync(runOptions).Wait();

      public static void RunSimulationExport(ExportRunOptions runOptions) => ResolveTask<IExportSimulationRunner>().RunBatchAsync(runOptions).Wait();

      internal static T ResolveTask<T>()
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