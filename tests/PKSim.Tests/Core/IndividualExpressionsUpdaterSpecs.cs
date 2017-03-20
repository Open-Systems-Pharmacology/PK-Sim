using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;

namespace PKSim.Core
{
   public abstract class concern_for_IndividualExpressionsUpdater : ContextSpecification<IIndividualExpressionsUpdater>
   {
      protected Individual _targetIndividual;
      protected Individual _sourceIndividual;
      protected IOntogenyTask<Individual> _ontogenyTask;
      protected ICloner _cloner;

      protected override void Context()
      {
         _sourceIndividual = A.Fake<Individual>();
         _ontogenyTask = A.Fake<IOntogenyTask<Individual>>();
         _cloner = A.Fake<ICloner>();
         _targetIndividual = DomainHelperForSpecs.CreateIndividual();

         sut = new IndividualExpressionsUpdater(_ontogenyTask, _cloner);
      }
   }

   public class When_updating_the_enzyme_expression_from_one_individual_to_another : concern_for_IndividualExpressionsUpdater
   {
      private IEnumerable<IndividualMolecule> _allEnzymesExpression;
      private IndividualMolecule _enzymeExpression1;
      private IndividualMolecule _enzymeExpression2;
      private IndividualMolecule _enzymeExpressionClone1;
      private IndividualMolecule _enzymeExpressionClone2;

      protected override void Context()
      {
         base.Context();
         _enzymeExpression1 = A.Fake<IndividualMolecule>().WithName("meta1");
         _enzymeExpression2 = A.Fake<IndividualMolecule>().WithName("meta2");
         _enzymeExpressionClone1 = A.Fake<IndividualMolecule>().WithName("meta1");
         _enzymeExpressionClone2 = A.Fake<IndividualMolecule>().WithName("meta2");
         _enzymeExpressionClone1.Ontogeny = new DatabaseOntogeny();
         _enzymeExpressionClone2.Ontogeny = new DatabaseOntogeny();
         _allEnzymesExpression = new List<IndividualMolecule> {_enzymeExpression1, _enzymeExpression2};
         A.CallTo(() => _sourceIndividual.AllMolecules()).Returns(_allEnzymesExpression);

         A.CallTo(() => _cloner.Clone(_enzymeExpression1)).Returns(_enzymeExpressionClone1);
         A.CallTo(() => _cloner.Clone(_enzymeExpression2)).Returns(_enzymeExpressionClone2);
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
   }
}