using OSPSuite.Core.Diagram;
using OSPSuite.Core.Journal;
using OSPSuite.Core.Serialization.Diagram;
using OSPSuite.Presentation;
using OSPSuite.Utility.Container;
using PKSim.BatchTool.Views;
using PKSim.CLI.Core.MinimalImplementations;
using PKSim.Core;
using PKSim.Presentation;

namespace PKSim.BatchTool
{
   public class BatchRegister : Register
   {
      public override void RegisterInContainer(IContainer container)
      {
         container.AddScanner(scan =>
         {
            scan.AssemblyContainingType<BatchRegister>();
            scan.WithDefaultConvention();
         });

         container.Register<IUserSettings, ICoreUserSettings, OSPSuite.Core.ICoreUserSettings, IPresentationUserSettings, CLIUserSettings>(LifeStyle.Singleton);
         container.Register<IDiagramModelToXmlMapper, CLIDiagramModelToXmlMapper>();
         container.Register<IDiagramModel, CLIDiagramModel>();
         container.Register<IJournalDiagramManagerFactory, CLIJournalDiagramManagerFactory>();


         container.Register(typeof(IInputAndOutputBatchView<>), typeof(InputAndOutputBatchView<>));
      }
   }
}