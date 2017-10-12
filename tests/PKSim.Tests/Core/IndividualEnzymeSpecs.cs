using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core
{
   public abstract class concern_for_IndividualEnzyme : ContextSpecification<IndividualEnzyme>
   {
      protected MoleculeExpressionContainer _expressionContainer;
      protected MoleculeExpressionContainer _anotherContainer;
      protected IParameter _referenceConcentration;

      protected override void Context()
      {
         _expressionContainer = new MoleculeExpressionContainer().WithName("tralal");
         _expressionContainer.Add(A.Fake<IParameter>().WithName(CoreConstants.Parameter.REL_EXP));
         _expressionContainer.Add(A.Fake<IParameter>().WithName(CoreConstants.Parameter.REL_EXP_NORM));
         _anotherContainer = new MoleculeExpressionContainer().WithName("AnotherContainer");
         sut = new IndividualEnzyme();
         _referenceConcentration = A.Fake<IParameter>().WithName(CoreConstants.Parameter.REFERENCE_CONCENTRATION);
         sut.Add(_referenceConcentration);
         sut.Add(_expressionContainer);
         sut.Add(_anotherContainer);
      }
   }

   public class When_retrieving_the_list_of_active_container_for_an_enzyme : concern_for_IndividualEnzyme
   {
      [Observation]
      public void should_only_return_the_active_container()
      {
         sut.AllExpressionsContainers().ShouldOnlyContain(_expressionContainer, _anotherContainer);
      }
   }

   public class When_asked_for_the_activity_factor_parameter : concern_for_IndividualEnzyme
   {
      [Observation]
      public void should_return_the_predefined_parameter()
      {
         sut.ReferenceConcentration.ShouldBeEqualTo(_referenceConcentration);
      }
   }

   public class When_updating_a_molecule_from_another_one_using_an_ontogeny_coming_from_the_database : concern_for_IndividualEnzyme
   {
      private IndividualEnzyme _sourceMolecule;

      protected override void Context()
      {
         base.Context();
         _sourceMolecule = new IndividualEnzyme {Ontogeny = new DatabaseOntogeny()};
      }

      protected override void Because()
      {
         sut.UpdatePropertiesFrom(_sourceMolecule, A.Fake<ICloneManager>());
      }

      [Observation]
      public void should_simply_copy_the_reference_to_the_ontogeny()
      {
         sut.Ontogeny.ShouldBeEqualTo(_sourceMolecule.Ontogeny);   
      }
   }

   public class When_updating_a_molecule_from_another_one_using_a_user_defined_ontogeny_: concern_for_IndividualEnzyme
   {
      private IndividualEnzyme _sourceMolecule;
      private ICloneManager _cloneManager;
      private Ontogeny _clonedOntogeny;

      protected override void Context()
      {
         base.Context();
         _cloneManager= A.Fake<ICloneManager>();
         _sourceMolecule = new IndividualEnzyme { Ontogeny = new UserDefinedOntogeny() };
         _clonedOntogeny=new UserDefinedOntogeny();
         A.CallTo(() => _cloneManager.Clone(_sourceMolecule.Ontogeny)).Returns(_clonedOntogeny);
      }

      protected override void Because()
      {
         sut.UpdatePropertiesFrom(_sourceMolecule, _cloneManager);
      }

      [Observation]
      public void should_have_cloned_the_source_ontogeny_and_use_it_in_the_molecule()
      {
         sut.Ontogeny.ShouldBeEqualTo(_clonedOntogeny);
      }
   }
}