using OSPSuite.Core.Commands.Core;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation;
using PKSim.Presentation.Presenters.Individuals;
using static PKSim.Starter.ExchangeSerializer;

namespace PKSim.Starter
{
   public static class IndividualCreator
   {
      public static object CreateIndividual()
      {
         var container = UIApplicationStartup.Initialize();

         using (var presenter = container.Resolve<ICreateIndividualPresenter>())
         {
            presenter.Initialize();
            var workspace = container.Resolve<IWorkspace>();
            workspace.Project = new PKSimProject();

            if (presenter.Create().IsEmpty())
               return null;

            var mapper = container.Resolve<IIndividualToIndividualBuildingBlockMapper>();
            var individualBuildingBlock = mapper.MapFrom(presenter.Individual);
            container.Resolve<ISnapshotUpdater>().AddSnapshotTo(individualBuildingBlock, presenter.Individual);
            return Serialize(individualBuildingBlock, container);
         }
      }
   }
}