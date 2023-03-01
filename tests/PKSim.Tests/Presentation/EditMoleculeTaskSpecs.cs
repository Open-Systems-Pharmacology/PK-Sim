using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Core;
using PKSim.Core;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Services;

namespace PKSim.Presentation
{
   public abstract class concern_for_EditMoleculeTask : ContextSpecification<IEditMoleculeTask<Individual>>
   {
      protected IMoleculeExpressionTask<Individual> _moleculeExpressionTask;
      protected IMoleculeToQueryExpressionSettingsMapper _querySettingsMapper;
      protected IGeneExpressionsDatabasePathManager _geneExpressionsDatabasePathManager;
      protected IApplicationController _applicationController;
      protected MoleculeExpressionContainer _moleculeContainer1;
      protected MoleculeExpressionContainer _moleculeContainer2;
      protected IndividualMolecule _molecule;
      protected Individual _individual;
      protected IExpressionProfileSelectionPresenter _expressionProfileSelectionPresenter;

      protected override void Context()
      {
         _moleculeExpressionTask = A.Fake<IMoleculeExpressionTask<Individual>>();
         _querySettingsMapper = A.Fake<IMoleculeToQueryExpressionSettingsMapper>();
         _geneExpressionsDatabasePathManager = A.Fake<IGeneExpressionsDatabasePathManager>();
         _applicationController = A.Fake<IApplicationController>();

         sut = new EditMoleculeTask<Individual>(_moleculeExpressionTask, _applicationController);

         _expressionProfileSelectionPresenter = A.Fake<IExpressionProfileSelectionPresenter>();
         A.CallTo(() => _applicationController.Start<IExpressionProfileSelectionPresenter>()).Returns(_expressionProfileSelectionPresenter);


         _moleculeContainer1 = new MoleculeExpressionContainer().WithName("C1");
         _moleculeContainer1.Add(DomainHelperForSpecs.ConstantParameterWithValue(5).WithName(Constants.Parameters.REL_EXP));
         _moleculeContainer2 = new MoleculeExpressionContainer().WithName("C2");
         _moleculeContainer2.Add(DomainHelperForSpecs.ConstantParameterWithValue(5).WithName(Constants.Parameters.REL_EXP));

         _individual = new Individual {OriginData = new OriginData {Species = new Species {Name = "Human", DisplayName = "Human"}}};


         _molecule = new IndividualEnzyme {Name = "CYP3A4"};
         _molecule.Add(_moleculeContainer1);
         _molecule.Add(_moleculeContainer2);
      }
   }

   public class When_asked_to_add_an_expression_profile_to_an_individual : concern_for_EditMoleculeTask
   {
      private ICommand _resultCommand;
      private ICommand _addCommand;
      private ExpressionProfile _expressionProfile;

      protected override void Context()
      {
         base.Context();
         _addCommand = A.Fake<ICommand>();
         _expressionProfile = DomainHelperForSpecs.CreateExpressionProfile<IndividualEnzyme>();
         A.CallTo(() => _expressionProfileSelectionPresenter.SelectExpressionProfile<IndividualProtein>(_individual)).Returns(_expressionProfile);
         A.CallTo(() => _moleculeExpressionTask.AddExpressionProfile(_individual, _expressionProfile)).Returns(_addCommand);
      }

      protected override void Because()
      {
         _resultCommand = sut.AddExpressionProfile<IndividualProtein>(_individual);
      }

      [Observation]
      public void should_ask_the_user_to_select_an_expression_profile()
      {
         A.CallTo(() => _expressionProfileSelectionPresenter.SelectExpressionProfile<IndividualProtein>(_individual)).MustHaveHappened();
      }

      [Observation]
      public void should_update_the_default_parameters_in_the_newly_added_molecule()
      {
         A.CallTo(() => _moleculeExpressionTask.AddExpressionProfile(_individual, _expressionProfile)).MustHaveHappened();
      }

      [Observation]
      public void the_resulting_command_should_be_an_instance_of_add_protein_to_individual_command()
      {
         _resultCommand.ShouldBeEqualTo(_addCommand);
      }
   }

}