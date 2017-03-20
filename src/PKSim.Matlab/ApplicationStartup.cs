using System.Threading;
using PKSim.Core;
using PKSim.Infrastructure;
using PKSim.Presentation;
using OSPSuite.Utility.Container;

namespace PKSim.Matlab
{
   internal class ApplicationStartup
   {
      private static bool _initialized;

      public static void Initialize(string dimensionFilePath = null, string databaseFilePath = null, string pkParameterFilePath = null)
      {
         if (_initialized) return;
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
}