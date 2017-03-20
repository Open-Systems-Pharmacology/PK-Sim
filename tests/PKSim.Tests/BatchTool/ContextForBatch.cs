using OSPSuite.Utility.Container;
using PKSim.BatchTool.Services;
using PKSim.Core;
using PKSim.Core.Batch;
using PKSim.IntegrationTests;
using Simulation = PKSim.Core.Model.Simulation;

namespace PKSim.BatchTool
{
   public abstract class ContextForBatch : ContextForIntegration<ISimulationLoader>
   {
      protected Simulation _simulation;
      protected SimulationForBatch _simulationForBatch;

      public override void GlobalContext()
      {
         base.GlobalContext();
         sut = IoC.Resolve<ISimulationLoader>();
      }

      public void Load(string jsonFileName)
      {
         var projectFile = DomainHelperForSpecs.DataFilePathFor(string.Format("BatchFiles\\{0}.json", jsonFileName));
         _simulationForBatch = sut.LoadSimulationFrom(projectFile);
         _simulation = _simulationForBatch.Simulation;
      }
   }
}