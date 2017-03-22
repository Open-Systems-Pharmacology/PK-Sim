using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Threading;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Infrastructure;
using PKSim.Presentation;

namespace PKSim.Matlab
{
   internal class ApplicationStartup
   {
      private static bool _initialized;

      public static void Initialize(string dimensionFilePath = null, string databaseFilePath = null, string pkParameterFilePath = null)
      {
         if (_initialized) return;

         redirectNHibernateAssembly();

         new ApplicationStartup().initializeForMatlab(dimensionFilePath, databaseFilePath, pkParameterFilePath);
         _initialized = true;
      }

      private void initializeForMatlab(string dimensionFilePath, string databaseFilePath, string pkParameterFilePath)
      {
         if (IoC.Container != null)
            return;

         InfrastructureRegister.Initialize();
         var container = IoC.Container;
         var pksimConfiguration = container.Resolve<IPKSimConfiguration>();

         //path was specified, update the default path
         if (!string.IsNullOrEmpty(dimensionFilePath))
            pksimConfiguration.DimensionFilePath = dimensionFilePath;

         //path was specified, update the default path
         if (!string.IsNullOrEmpty(databaseFilePath))
            pksimConfiguration.PKSimDb = databaseFilePath;

         //path was specified, update the default path
         if (!string.IsNullOrEmpty(pkParameterFilePath))
            pksimConfiguration.PKParametersFilePath = pkParameterFilePath;

         using (container.OptimizeDependencyResolution())
         {
            container.RegisterImplementationOf(new SynchronizationContext());
            container.AddRegister(x => x.FromType<MatlabRegister>());
            container.AddRegister(x => x.FromType<PresenterRegister>());
            container.AddRegister(x => x.FromType<CoreRegister>());
            container.AddRegister(x => x.FromType<InfrastructureRegister>());
            container.AddRegister(x => x.FromType<OSPSuite.Presentation.PresenterRegister>());

            //no computation required in matlab interface
            InfrastructureRegister.RegisterSerializationDependencies(registerSimModelSchema: false);
         }
      }

      private static void redirectNHibernateAssembly()
      {
         redirectAssembly("NHibernate", new Version(4, 1, 0, 4000), "aa95f207798dfdb4");
      }

      private static void redirectAssembly(string shortName, Version targetVersion, string publicKeyToken)
      {
         ResolveEventHandler handler = null;

         handler = (sender, args) =>
         {
            var requestedAssembly = new AssemblyName(args.Name);
            if (requestedAssembly.Name != shortName)
               return null;

            requestedAssembly.Version = targetVersion;
            requestedAssembly.SetPublicKeyToken(new AssemblyName("x, PublicKeyToken=" + publicKeyToken).GetPublicKeyToken());
            requestedAssembly.CultureInfo = CultureInfo.InvariantCulture;

            //once found, not need to react to event anymore
            AppDomain.CurrentDomain.AssemblyResolve -= handler;

            return Assembly.Load(requestedAssembly);
         };
         AppDomain.CurrentDomain.AssemblyResolve += handler;
      }
   }
}