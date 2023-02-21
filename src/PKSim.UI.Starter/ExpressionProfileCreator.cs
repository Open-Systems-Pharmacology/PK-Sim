using OSPSuite.Core.Commands.Core;
using OSPSuite.Presentation.Views;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Presentation;
using PKSim.Presentation.Presenters.ExpressionProfiles;

namespace PKSim.UI.Starter
{
   public static class ExpressionProfileCreator
   {
      public static object CreateIndividualEnzymeExpressionProfile()
      {
         return createExpressionProfile<IndividualEnzyme>();
      }

      public static object CreateTransporterExpressionProfile()
      {
         return createExpressionProfile<IndividualTransporter>();
      }

      public static object CreateBindingPartnerExpressionProfile()
      {
         return createExpressionProfile<IndividualOtherProtein>();
      }

      private static object createExpressionProfile<T>() where T : IndividualMolecule
      {
         var container = ApplicationStartup.Initialize();

         using (var presenter = container.Resolve<ICreateExpressionProfilePresenter>())
         {
            var workspace = container.Resolve<IWorkspace>();
            workspace.Project = new PKSimProject();
            var mapper = container.Resolve<IExpressionProfileToExpressionProfileBuildingBlockMapper>();


            return presenter.Create<T>().IsEmpty() ? null : mapper.MapFrom(presenter.ExpressionProfile);
         }
      }
   }
}