using OSPSuite.Core.Commands.Core;
using OSPSuite.Presentation.Views;
using OSPSuite.Utility.Exceptions;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Snapshots;
using PKSim.Presentation;
using PKSim.Presentation.DTO.ExpressionProfiles;
using PKSim.Presentation.Mappers;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Services;

namespace PKSim.UI.Starter
{
   public static class ExpressionDatabaseQuery
   {
      public static object GetExpressionDatabaseQuery(IShell shell, string speciesName)
      {
         var container = ApplicationStartup.Initialize(shell);

         var expressionProfileProteinDatabaseTask = container.Resolve<IExpressionProfileProteinDatabaseTask>();
         

         if (!expressionProfileProteinDatabaseTask.CanQueryProteinExpressionsFor(speciesName))
            throw new OSPSuiteException(PKSimConstants.Error.NoProteinExpressionDatabaseAssociatedTo(speciesName));
        // var queryResults = _expressionProfileProteinDatabaseTask.QueryDatabase(_expressionProfile, _expressionProfileDTO.MoleculeName);

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