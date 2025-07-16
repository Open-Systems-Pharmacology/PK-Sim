using OSPSuite.Presentation;
using OSPSuite.Utility.Container;
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

         container.Register<IUserSettings, ICoreUserSettings, OSPSuite.Core.ICoreUserSettings, IPresentationUserSettings, BatchUserSettings>(LifeStyle.Singleton);
      }
   }
}