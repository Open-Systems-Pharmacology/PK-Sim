using System;
using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.Assets;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Views.Individuals;

namespace PKSim.Presentation
{
   public abstract class concern_for_ExpressionProfileSelectionPresenter : ContextSpecification<IExpressionProfileSelectionPresenter>
   {
      protected IExpressionProfileSelectionView _view;
      protected IMoleculePropertiesMapper _moleculePropertiesMapper;
      protected IBuildingBlockRepository _buildingBlockRepository;
      protected IExpressionProfileTask _expressionProfileTask;
      protected ExpressionProfileSelectionDTO _dto;
      protected List<ExpressionProfile> _allExpressionProfiles;
      protected ExpressionProfile _expressionProfile1;
      protected ExpressionProfile _expressionProfile2;
      protected ExpressionProfile _expressionProfile3;
      protected ExpressionProfile _expressionProfile4;
      protected ExpressionProfile _expressionProfile5;
      protected Func<ExpressionProfile, bool> _predicate;
      protected Individual _individual;

      protected override void Context()
      {
         _view = A.Fake<IExpressionProfileSelectionView>();
         _moleculePropertiesMapper = A.Fake<IMoleculePropertiesMapper>();
         _buildingBlockRepository = A.Fake<IBuildingBlockRepository>();
         _expressionProfileTask = A.Fake<IExpressionProfileTask>();


         sut = new ExpressionProfileSelectionPresenter(_view, _moleculePropertiesMapper, _buildingBlockRepository, _expressionProfileTask);

         A.CallTo(() => _view.BindTo(A<ExpressionProfileSelectionDTO>._))
            .Invokes(x => _dto = x.GetArgument<ExpressionProfileSelectionDTO>(0));

         _individual = DomainHelperForSpecs.CreateIndividual(speciesName: "Human");
         _expressionProfile1 = DomainHelperForSpecs.CreateExpressionProfile<IndividualEnzyme>(speciesName:"Human");
         _expressionProfile2 = DomainHelperForSpecs.CreateExpressionProfile<IndividualEnzyme>(speciesName:"Human");
         _expressionProfile3 = DomainHelperForSpecs.CreateExpressionProfile<IndividualTransporter>(speciesName:"Human");
         _expressionProfile4 = DomainHelperForSpecs.CreateExpressionProfile<IndividualEnzyme>(speciesName:"Dog");
         _expressionProfile5 = DomainHelperForSpecs.CreateExpressionProfile<IndividualEnzyme>(speciesName: "Human");

         _allExpressionProfiles = new List<ExpressionProfile> {_expressionProfile1, _expressionProfile2};

         A.CallTo(() => _buildingBlockRepository.All(A<Func<ExpressionProfile, bool>>._))
            .Invokes(x => _predicate = x.GetArgument<Func<ExpressionProfile, bool>>(0))
            .Returns(_allExpressionProfiles);

         _individual.AddExpressionProfile(_expressionProfile5);
      }
   }

   public class When_selecting_an_expression_profile_for_a_given_molecule_type_and_simulation_subject : concern_for_ExpressionProfileSelectionPresenter
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _moleculePropertiesMapper.MoleculeDisplayFor<IndividualEnzyme>()).Returns("enzyme");
         A.CallTo(() => _moleculePropertiesMapper.MoleculeIconFor<IndividualEnzyme>()).Returns(ApplicationIcons.Molecule);
      }
      protected override void Because()
      {
         sut.SelectExpressionProfile<IndividualEnzyme>(_individual);
      }

      [Observation]
      public void should_only_propose_expression_profile_for_the_molecule_type_and_for_the_species()
      {
         sut.AllExpressionProfiles().ShouldOnlyContain(_expressionProfile1, _expressionProfile2);
      }

      [Observation]
      public void should_filter_out_all_expression_profile_not_valid_for_the_current_species_selection()
      {
         _predicate(_expressionProfile1).ShouldBeTrue();
         _predicate(_expressionProfile2).ShouldBeTrue();
         _predicate(_expressionProfile3).ShouldBeFalse();
         _predicate(_expressionProfile4).ShouldBeFalse();
      }


      [Observation]
      public void should_filter_out_all_expression_profile_already_added_to_the_simulation_subject()
      {
         _predicate(_expressionProfile5).ShouldBeFalse();
      }

      [Observation]
      public void should_update_the_view_with_the_type_of_the_molecule()
      {
         _view.Caption.ShouldBeEqualTo(PKSimConstants.UI.AddMolecule("enzyme"));
      }

      [Observation]
      public void should_update_the_view_with_the_icon_of_the_molecule()
      {
         _view.ApplicationIcon.ShouldBeEqualTo(ApplicationIcons.Molecule);
      }
   }
}