using OSPSuite.Core.Commands.Core;
using OSPSuite.Presentation.Views;
using PKSim.Core.Model;
using PKSim.Presentation;
using PKSim.Presentation.Mappers;
using PKSim.Presentation.Presenters.Individuals;

namespace PKSim.UI.Starter
{
   public static class IndividualCreator
   {
      public static object CreateIndividual(IShell shell)
      {
         var container = ApplicationStartup.Initialize(shell);

         using (var presenter = container.Resolve<ICreateIndividualPresenter>())
         {
            presenter.Initialize();
            var workspace = container.Resolve<IWorkspace>();
            workspace.Project = new PKSimProject();
            var mapper = container.Resolve<IIndividualToIndividualBuildingBlockMapper>();

            return presenter.Create().IsEmpty() ? null : mapper.MapFrom(presenter.Individual);
         }
      }
   }
}