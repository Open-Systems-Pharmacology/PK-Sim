using OSPSuite.Core.Commands.Core;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Presentation;
using PKSim.Presentation.Presenters.Individuals;

namespace PKSim.UI.Starter
{
   public static class IndividualCreator
   {
      public static object CreateIndividual()
      {
         var container = ApplicationStartup.Initialize();

         using (var presenter = container.Resolve<ICreateIndividualPresenter>())
         {
            presenter.Initialize();
            var workspace = container.Resolve<IWorkspace>();
            workspace.Project = new PKSimProject();

            if (presenter.Create().IsEmpty())
               return null;

            var mapper = container.Resolve<IIndividualToIndividualBuildingBlockMapper>();
            return mapper.MapFrom(presenter.Individual);
         }
      }
   }
}