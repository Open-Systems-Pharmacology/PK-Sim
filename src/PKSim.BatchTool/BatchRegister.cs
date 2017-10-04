using OSPSuite.Core.Diagram;
using OSPSuite.Core.Journal;
using OSPSuite.Core.Serialization.Diagram;
using OSPSuite.Presentation;
using OSPSuite.Utility.Container;
using PKSim.BatchTool.Mappers;
using PKSim.BatchTool.Services;
using PKSim.BatchTool.Views;
using PKSim.Core;
using PKSim.Presentation;

namespace PKSim.BatchTool
{
   public class BatchRegister : Register
   {
      public override void RegisterInContainer(IContainer container)
      {
         container.AddScanner(scan => scan.AssemblyContainingType<BatchRegister>());


         container.Register<IUserSettings, ICoreUserSettings, OSPSuite.Core.ICoreUserSettings, IPresentationUserSettings, BatchUserSettings>(LifeStyle.Singleton);
         container.Register<IDiagramModelToXmlMapper, BatchDiagramModelToXmlMapper>(LifeStyle.Singleton);
         container.Register<IDiagramModel, BatchDiagramModel>(LifeStyle.Singleton);
         container.Register<IJournalDiagramManagerFactory, BatchJournalDiagramManagerFactory>(LifeStyle.Singleton);

         container.Register<JsonSimulationRunner, JsonSimulationRunner>();
         container.Register<ProjectComparisonRunner, ProjectComparisonRunner>();
         container.Register<ProjectOverviewRunner, ProjectOverviewRunner>();
         container.Register<SnapshotsRunner, SnapshotsRunner>();

         container.Register(typeof(IInputAndOutputBatchView<>), typeof(InputAndOutputBatchView<>));
      }
   }
}