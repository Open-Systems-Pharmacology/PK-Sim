using OSPSuite.Utility.Container;
using PKSim.Presentation;
using PKSim.Presentation.Presenters.Individuals;

namespace PKSim.UI.Starter
{
   public class PKSimStarterPresenterRegister : BasePresenterRegister
   {
      public override void RegisterInContainer(IContainer container)
      {
         base.RegisterInContainer(container);
         container.Register<ICreateIndividualPresenter, CreateIndividualPresenterForMoBi>();
      }

      protected override void ExcludeTypes(IAssemblyScanner scan)
      {
         base.ExcludeTypes(scan);
         scan.ExcludeType<CreateIndividualPresenter>();
      }
      
      protected override void RegisterMainViewPresenters(IContainer container)
      {
         
      }
   }
}