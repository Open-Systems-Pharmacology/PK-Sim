using OSPSuite.Core.Serialization.SimModel.Services;
using OSPSuite.Utility.Container;
using PKSim.Core.Services;

namespace PKSim.Matlab
{
   public interface IMatlabHelper
   {
      /// <summary>
      ///    Loads the <paramref name="pkmlFileFullPath" /> and exports the corresponding simulation to SimModel xml file located
      ///    at <paramref name="simModelXmlFileFullPath" />
      /// </summary>
      /// <param name="pkmlFileFullPath">File containing the pkml simulation to load</param>
      /// <param name="simModelXmlFileFullPath">File where the SimModelXml will be exported</param>
      void SaveToSimModelXmlFile(string pkmlFileFullPath, string simModelXmlFileFullPath);
   }

   public class MatlabHelper : IMatlabHelper
   {
      private readonly ISimModelExporter _simModelExporter;
      private readonly ICoreLoader _coreLoader;

      static MatlabHelper()
      {
         ApplicationStartup.Initialize();
      }

      public MatlabHelper() : this(
            IoC.Resolve<ISimModelExporter>(),
            IoC.Resolve<ICoreLoader>()
         )
      {
      }

      internal MatlabHelper(ISimModelExporter simModelExporter, ICoreLoader coreLoader)
      {
         _simModelExporter = simModelExporter;
         _coreLoader = coreLoader;
      }

      public void SaveToSimModelXmlFile(string pkmlFileFullPath, string simModelXmlFileFullPath)
      {
         var simulationTransfer = _coreLoader.LoadSimulationTransfer(pkmlFileFullPath);
         _simModelExporter.Export(simulationTransfer.Simulation, simModelXmlFileFullPath);
      }
   }
}