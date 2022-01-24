using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Presentation.Core;
using PKSim.Core;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.ProteinExpression;
using PKSim.Presentation.Services;

namespace PKSim.Presentation
{
   public abstract class concern_for_ExpressionProfileProteinDatabaseTask : ContextSpecification<IExpressionProfileProteinDatabaseTask>
   {
      protected IGeneExpressionsDatabasePathManager _geneExpressionDatabasePathManager;
      protected IApplicationController _applicationController;
      protected IMoleculeToQueryExpressionSettingsMapper _querySettingsMapper;
      protected IMoleculeExpressionTask<Individual> _moleculeExpressionTask;
      protected IProteinExpressionsPresenter _proteinExpressionPresenter;

      protected override void Context()
      {
         _geneExpressionDatabasePathManager = A.Fake<IGeneExpressionsDatabasePathManager>();
         _applicationController = A.Fake<IApplicationController>();
         _querySettingsMapper = A.Fake<IMoleculeToQueryExpressionSettingsMapper>();
         _moleculeExpressionTask = A.Fake<IMoleculeExpressionTask<Individual>>();

         sut = new ExpressionProfileProteinDatabaseTask(_geneExpressionDatabasePathManager, _applicationController, _querySettingsMapper, _moleculeExpressionTask);


         _proteinExpressionPresenter = A.Fake<IProteinExpressionsPresenter>();

         A.CallTo(() => _applicationController.Start<IProteinExpressionsPresenter>()).Returns(_proteinExpressionPresenter);
      }
   }

   public class When_asked_to_edit_a_protein : concern_for_ExpressionProfileProteinDatabaseTask
   {
      private QueryExpressionSettings _querySettings;
      private QueryExpressionResults _queryResults;
      private ExpressionProfile _expressionProfile;
      private QueryExpressionResults _result;

      protected override void Context()
      {
         base.Context();
         _expressionProfile = DomainHelperForSpecs.CreateExpressionProfile<IndividualEnzyme>();
         _querySettings = A.Fake<QueryExpressionSettings>();
         _queryResults = A.Fake<QueryExpressionResults>();
         A.CallTo(() => _querySettingsMapper.MapFrom(_expressionProfile.Molecule, _expressionProfile.Individual, "NAME")).Returns(_querySettings);
         A.CallTo(() => _proteinExpressionPresenter.Start()).Returns(true);
         A.CallTo(() => _proteinExpressionPresenter.GetQueryResults()).Returns(_queryResults);
      }

      protected override void Because()
      {
         _result = sut.QueryDatabase(_expressionProfile, "NAME");
      }

      [Observation]
      public void should_leverage_query_expression_presenter_to_edit_the_protein_with_the_previous_configuration()
      {
         A.CallTo(() => _proteinExpressionPresenter.InitializeSettings(_querySettings)).MustHaveHappened();
      }

      [Observation]
      public void should_create_a_clone_of_the_input_protein_and_perform_an_update_value_by_value_of_the_edited_protein()
      {
         A.CallTo(() => _proteinExpressionPresenter.InitializeSettings(_querySettings)).MustHaveHappened();
      }

      [Observation]
      public void should_leverage_the_molecule_expression_task_to_edit_the_molecule()
      {
         _result.ShouldBeEqualTo(_queryResults);
      }
   }
}