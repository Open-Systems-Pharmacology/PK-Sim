using OSPSuite.Core.Commands.Core;
using OSPSuite.Presentation.Core;
using PKSim.Assets;
using PKSim.Core.Commands;
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
      ///    return true if a protein expression database was defined for the species referenced in <paramref name="expressionProfile" />, otherwise
      ///    false
      /// </summary>
      bool CanQueryProteinExpressionsFor(ExpressionProfile expressionProfile);
   }

   public class ExpressionProfileProteinDatabaseTask : IExpressionProfileProteinDatabaseTask
   {
      private readonly IGeneExpressionsDatabasePathManager _geneExpressionsDatabasePathManager;
      private readonly IApplicationController _applicationController;
      private readonly IMoleculeToQueryExpressionSettingsMapper _queryExpressionSettingsMapper;
      private readonly IMoleculeExpressionTask<Individual> _moleculeExpressionTask;

      public ExpressionProfileProteinDatabaseTask(
         IGeneExpressionsDatabasePathManager geneExpressionsDatabasePathManager,
         IApplicationController applicationController,
         IMoleculeToQueryExpressionSettingsMapper queryExpressionSettingsMapper,
         IMoleculeExpressionTask<Individual> moleculeExpressionTask)
      {
         _geneExpressionsDatabasePathManager = geneExpressionsDatabasePathManager;
         _applicationController = applicationController;
         _queryExpressionSettingsMapper = queryExpressionSettingsMapper;
         _moleculeExpressionTask = moleculeExpressionTask;
      }

      public bool CanQueryProteinExpressionsFor(ExpressionProfile expressionProfile)
      {
         return _geneExpressionsDatabasePathManager.HasDatabaseFor(expressionProfile.Species);
      }

      public QueryExpressionResults QueryDatabase(ExpressionProfile expressionProfile, string moleculeName)
      {
         var (molecule, individual) = expressionProfile;
         using (_geneExpressionsDatabasePathManager.ConnectToDatabaseFor(individual.Species))
         using (var presenter = _applicationController.Start<IProteinExpressionsPresenter>())
         {
            presenter.InitializeSettings(_queryExpressionSettingsMapper.MapFrom(molecule, individual, moleculeName));
            presenter.Title = PKSimConstants.UI.EditProteinExpression;
            var success = presenter.Start();
            if (!success)
               return null;

            return presenter.GetQueryResults();

         }
      }
   }
}