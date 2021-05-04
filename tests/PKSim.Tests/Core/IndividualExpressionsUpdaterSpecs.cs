using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core
{
   public abstract class concern_for_IndividualExpressionsUpdater : ContextSpecification<IIndividualExpressionsUpdater>
   {
      protected Individual _targetIndividual;
      protected Individual _sourceIndividual;
      protected IOntogenyTask<Individual> _ontogenyTask;
      private IMoleculeExpressionTask<Individual> _moleculeExpressionTask;
      private IEntityPathResolver _entityPathResolver;
      private ICloner _cloner;
      private IParameterUpdater _parameterUpdater;

      protected override void Context()
      {
         _sourceIndividual = A.Fake<Individual>();
         _ontogenyTask = A.Fake<IOntogenyTask<Individual>>();
         _targetIndividual = DomainHelperForSpecs.CreateIndividual();
         _moleculeExpressionTask= A.Fake<IMoleculeExpressionTask<Individual>>();
         _entityPathResolver= A.Fake<IEntityPathResolver>();

         sut = new IndividualExpressionsUpdater(_ontogenyTask,_moleculeExpressionTask,_entityPathResolver,_cloner,_parameterUpdater );
      }
   }

   public class When_updating_the_enzyme_expression_from_one_individual_to_another : concern_for_IndividualExpressionsUpdater
   {
      private IEnumerable<IndividualMolecule> _allEnzymesExpression;
      private IndividualMolecule _enzymeExpression1;
      private IndividualMolecule _enzymeExpression2;
      private IndividualMolecule _enzymeExpressionClone1;
      private IndividualMolecule _enzymeExpressionClone2;
      private IParameter _userDefinedParameter;
      private double _defaultValue = 20;

      protected override void Context()
      {
         base.Context();
         _enzymeExpression1 = new IndividualEnzyme().WithName("meta1");
         _enzymeExpression2 = new IndividualEnzyme().WithName("meta2");
         _enzymeExpressionClone1 = new IndividualEnzyme().WithName("meta1");

         //Create a parameter that changed!
         _userDefinedParameter = DomainHelperForSpecs.ConstantParameterWithValue(10);
         _userDefinedParameter.DefaultValue = _defaultValue;

         _enzymeExpressionClone1.Add(_userDefinedParameter);
         _enzymeExpressionClone2 = new IndividualEnzyme().WithName("meta2");
         _enzymeExpressionClone1.Ontogeny = new DatabaseOntogeny();
         _enzymeExpressionClone2.Ontogeny = new DatabaseOntogeny();
         _allEnzymesExpression = new List<IndividualMolecule> {_enzymeExpression1, _enzymeExpression2};
         A.CallTo(() => _sourceIndividual.AllMolecules()).Returns(_allEnzymesExpression);
    }

      protected override void Because()
      {
         sut.Update(_sourceIndividual, _targetIndividual);
      }

      [Observation]
      public void should_add_a_clone_of_the_source_protein_in_the_target_individual()
      {
         _targetIndividual.MoleculeByName<IndividualMolecule>("meta1").ShouldBeEqualTo(_enzymeExpressionClone1);
         _targetIndividual.MoleculeByName<IndividualMolecule>("meta2").ShouldBeEqualTo(_enzymeExpressionClone2);
      }

      [Observation]
      public void should_have_updated_the_ontogeny_of_the_new_molecule()
      {
         A.CallTo(() => _ontogenyTask.SetOntogenyForMolecule(_enzymeExpressionClone1, _enzymeExpressionClone1.Ontogeny, _targetIndividual)).MustHaveHappened();
         A.CallTo(() => _ontogenyTask.SetOntogenyForMolecule(_enzymeExpressionClone2, _enzymeExpressionClone2.Ontogeny, _targetIndividual)).MustHaveHappened();
      }

      [Observation]
      public void should_reset_all_molecule_user_defined_parameters_to_their_default_value()
      {
         _userDefinedParameter.Value.ShouldBeEqualTo(_defaultValue);
      }
   }
}