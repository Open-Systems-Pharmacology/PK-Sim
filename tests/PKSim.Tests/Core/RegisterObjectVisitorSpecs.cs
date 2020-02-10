using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_RegisterObjectVisitor : ContextSpecification<IRegisterObjectVisitor>
   {
      protected IWithIdRepository _objectBaseRepository;
      protected IContainer _objectToRegister;
      protected IEntity _child2;
      protected IEntity _child1;

      protected override void Context()
      {
         _objectBaseRepository = A.Fake<IWithIdRepository>();
         _objectToRegister = new Compartment().WithName("tutu").WithId("tutu");
         _child1 = new PKSimParameter().WithName("toto").WithId("toto").WithFormula(new ExplicitFormula("5"));
         _child2 = new PKSimParameter().WithName("tata").WithId("tata").WithFormula(new ExplicitFormula("6"));
         _objectToRegister.Add(_child1);
         _objectToRegister.Add(_child2);
         sut = new RegisterObjectVisitor(_objectBaseRepository);
      }
   }

   public class When_registering_an_object_in_the_object_factory : concern_for_RegisterObjectVisitor
   {
      protected override void Because()
      {
         sut.Register(_objectToRegister);
      }

      [Observation]
      public void should_remove_the_object_from_the_factory()
      {
         A.CallTo(() => _objectBaseRepository.Unregister(_objectToRegister.Id)).MustHaveHappened();
      }

      [Observation]
      public void should_register_the_object_in_the_factory()
      {
         A.CallTo(() => _objectBaseRepository.Register(_objectToRegister)).MustHaveHappened();
      }

      [Observation]
      public void should_register_all_children_from_the_object_to_register_as_well()
      {
         A.CallTo(() => _objectBaseRepository.Register(_child1)).MustHaveHappened();
         A.CallTo(() => _objectBaseRepository.Register(_child2)).MustHaveHappened();
      }
   }
}