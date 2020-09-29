using OSPSuite.Utility.Container;

namespace PKSim.UI.Starter
{
  
   public class StarterRegister : Register
   {
      public override void RegisterInContainer(IContainer container)
      {
         container.AddScanner(scan =>
         {
            scan.AssemblyContainingType<StarterRegister>();
            scan.WithDefaultConvention();
         });

     

      }
   }
}