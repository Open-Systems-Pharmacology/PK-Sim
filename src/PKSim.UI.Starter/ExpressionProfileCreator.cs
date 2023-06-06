using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Utility.Exceptions;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation;
using PKSim.Presentation.Presenters.ExpressionProfiles;
using PKSim.Presentation.Services;
using IContainer = OSPSuite.Utility.Container.IContainer;

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

      public static object GetExpressionDatabaseQuery(ExpressionProfileBuildingBlock buildingBlock)
      {
         var container = ApplicationStartup.Initialize();

         var expressionProfileProteinDatabaseTask = container.Resolve<IExpressionProfileProteinDatabaseTask>();
         loadApplicationSettings(container);

         if (!expressionProfileProteinDatabaseTask.CanQueryProteinExpressionsFor(buildingBlock))
            throw new OSPSuiteException(PKSimConstants.Error.NoProteinExpressionDatabaseAssociatedTo(buildingBlock.Species));

         var queryResults = expressionProfileProteinDatabaseTask.QueryDatabase(buildingBlock);

         return queryResults == null ? null : queryResultsToExpressionParameter(buildingBlock, queryResults);
      }

      private static void loadApplicationSettings(IContainer container)
      {
         container.Resolve<IApplicationSettingsPersistor>().Load();
      }

      private static List<ExpressionParameterValueUpdate> queryResultsToExpressionParameter(ExpressionProfileBuildingBlock buildingBlock, QueryExpressionResults queryResults)
      {
         var returnList = new List<ExpressionParameterValueUpdate>();

         buildingBlock.ExpressionParameters.Where(x => x.HasExpressionName()).Each(expressionParameter =>
         {
            var result = queryResults.ExpressionResultFor(expressionParameter.ContainerNameForRelativeExpressionParameter());
            if (result != null && !Equals(result.RelativeExpression, expressionParameter.Value))
               returnList.Add(new ExpressionParameterValueUpdate(expressionParameter.Path) { UpdatedValue = result.RelativeExpression });
         });

         return returnList;
      }
   }
}