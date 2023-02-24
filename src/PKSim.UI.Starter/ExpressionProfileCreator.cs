using System.Linq;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Exceptions;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation;
using PKSim.Presentation.Presenters.ExpressionProfiles;
using PKSim.Presentation.Services;

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

      public static object GetExpressionDatabaseQuery(ExpressionProfileBuildingBlockUpdate buildingBlockUpdate)
      {
         var container = ApplicationStartup.Initialize();

         var expressionProfileProteinDatabaseTask = container.Resolve<IExpressionProfileProteinDatabaseTask>();
         loadApplicationSettings(container);

         if (!expressionProfileProteinDatabaseTask.CanQueryProteinExpressionsFor(buildingBlockUpdate))
            throw new OSPSuiteException(PKSimConstants.Error.NoProteinExpressionDatabaseAssociatedTo(buildingBlockUpdate.Species));

         var queryResults = expressionProfileProteinDatabaseTask.QueryDatabase(buildingBlockUpdate);
         
         return queryResults == null ? null : queryResultsToExpressionParameter(buildingBlockUpdate, queryResults);
      }

      private static void loadApplicationSettings(IContainer container)
      {
         container.Resolve<IApplicationSettingsPersistor>().Load();
      }

      private static ExpressionProfileBuildingBlockUpdate queryResultsToExpressionParameter(ExpressionProfileBuildingBlockUpdate buildingBlock, QueryExpressionResults queryResults)
      {
         buildingBlock.ExpressionParameters.Where(x => x.IsExpression()).Each(expressionParameter =>
         {
            var result = queryResults.ExpressionResultFor(expressionParameter.ContainerNameForRelativeExpressionParameter());
            if (result != null)
               expressionParameter.UpdatedValue = result.RelativeExpression;
         });

         return buildingBlock;
      }
   }
}