using OSPSuite.Presentation.Views;
using OSPSuite.Utility.Container;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Presentation;
using PKSim.Presentation.Presenters.ExpressionProfiles;

namespace PKSim.UI.Starter
{
   public static class ExpressionProfileCreator
   {
      public static object CreateIndividualEnzymeExpressionProfile(IShell shell)
      {
         return createExpressionProfile<IndividualEnzyme>(shell);
      }

      public static object CreateTransporterExpressionProfile(IShell shell)
      {
         return createExpressionProfile<IndividualTransporter>(shell);
      }

      public static object CreateBindingPartnerExpressionProfile(IShell shell)
      {
         return createExpressionProfile<IndividualOtherProtein>(shell);
      }

      private static object createExpressionProfile<T>(IShell shell) where T : IndividualMolecule
      {
         var container = ApplicationStartup.Initialize(shell);

         using (var presenter = container.Resolve<ICreateExpressionProfilePresenter>())
         {
            var workspace = container.Resolve<IWorkspace>();
            workspace.Project = new PKSimProject();
            var mapper = IoC.Resolve<IExpressionProfileToExpressionProfileBuildingBlockMapper>();
            presenter.Create<T>();

            return mapper.MapFrom(presenter.ExpressionProfile);
         }
      }
   }
}