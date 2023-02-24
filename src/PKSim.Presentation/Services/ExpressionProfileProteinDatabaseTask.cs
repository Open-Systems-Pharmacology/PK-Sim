using OSPSuite.Core.Domain.Builder;
using OSPSuite.Presentation.Core;
using PKSim.Assets;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.ProteinExpression;

namespace PKSim.Presentation.Services
{
   public interface IExpressionProfileProteinDatabaseTask
   {
      /// <summary>
      ///    Edit the given molecule defined in the simulationSubject
      /// </summary>
      /// <param name="expressionProfile">Edited expression profile</param>
      /// <param name="moleculeName">Predefined name for the query</param>
      QueryExpressionResults QueryDatabase(ExpressionProfile expressionProfile, string moleculeName);

      /// <summary>
      ///    return true if a protein expression database was defined for the species referenced in
      ///    <paramref name="expressionProfile" />, otherwise
      ///    false
      /// </summary>
      bool CanQueryProteinExpressionsFor(ExpressionProfile expressionProfile);

      /// <summary>
      ///    return true if a protein expression database was defined for the species and molecule referenced in
      ///    <paramref name="expressionProfile" />, otherwise
      ///    false
      /// </summary>
      bool CanQueryProteinExpressionsFor(ExpressionProfileBuildingBlockUpdate expressionProfile);

      /// <summary>
      ///    Edit the given molecule defined in the <paramref name="expressionProfile" />
      /// </summary>
      /// <param name="expressionProfile">Edited expression profile</param>
      QueryExpressionResults QueryDatabase(ExpressionProfileBuildingBlockUpdate expressionProfile);
   }

   public class ExpressionProfileProteinDatabaseTask : IExpressionProfileProteinDatabaseTask
   {
      private readonly IGeneExpressionsDatabasePathManager _geneExpressionsDatabasePathManager;
      private readonly IApplicationController _applicationController;
      private readonly IMoleculeToQueryExpressionSettingsMapper _queryExpressionSettingsMapper;

      public ExpressionProfileProteinDatabaseTask(
         IGeneExpressionsDatabasePathManager geneExpressionsDatabasePathManager,
         IApplicationController applicationController,
         IMoleculeToQueryExpressionSettingsMapper queryExpressionSettingsMapper)
      {
         _geneExpressionsDatabasePathManager = geneExpressionsDatabasePathManager;
         _applicationController = applicationController;
         _queryExpressionSettingsMapper = queryExpressionSettingsMapper;
      }

      public bool CanQueryProteinExpressionsFor(ExpressionProfile expressionProfile)
      {
         return _geneExpressionsDatabasePathManager.HasDatabaseFor(expressionProfile.Species);
      }

      public bool CanQueryProteinExpressionsFor(ExpressionProfileBuildingBlockUpdate expressionProfile)
      {
         return _geneExpressionsDatabasePathManager.HasDatabaseFor(expressionProfile.Species);
      }

      public QueryExpressionResults QueryDatabase(ExpressionProfileBuildingBlockUpdate expressionProfile)
      {
         using (_geneExpressionsDatabasePathManager.ConnectToDatabaseFor(expressionProfile.Species))
         using (var presenter = _applicationController.Start<IProteinExpressionsPresenter>())
         {
            presenter.InitializeSettings(_queryExpressionSettingsMapper.MapFrom(expressionProfile));
            return getQueryResults(presenter);
         }
      }

      public QueryExpressionResults QueryDatabase(ExpressionProfile expressionProfile, string moleculeName)
      {
         var (molecule, individual) = expressionProfile;
         using (_geneExpressionsDatabasePathManager.ConnectToDatabaseFor(individual.Species))
         using (var presenter = _applicationController.Start<IProteinExpressionsPresenter>())
         {
            presenter.InitializeSettings(_queryExpressionSettingsMapper.MapFrom(molecule, individual, moleculeName));
            return getQueryResults(presenter);
         }
      }

      private static QueryExpressionResults getQueryResults(IProteinExpressionsPresenter presenter)
      {
         presenter.Title = PKSimConstants.UI.EditProteinExpression;
         var success = presenter.Start();
         if (!success)
            return null;

         return presenter.GetQueryResults();
      }
   }
}