using OSPSuite.BDDHelper;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;

namespace PKSim.Core
{
   public abstract class concern_for_UnregisterObjectVisitor : ContextSpecification<IUnregisterObjectVisitor>
   {
      protected IWithIdRepository _withIdRepository;
      protected IContainer _objectToUnregister;
      protected IEntity _child2;
      protected IEntity _child1;

      protected override void Context()
      {
         _withIdRepository = A.Fake<IWithIdRepository>();
         _objectToUnregister = new Compartment().WithName("tutu").WithId("tutu");
         _child1 = new PKSimParameter().WithName("toto").WithId("toto").WithFormula(new ExplicitFormula("5"));
         _child2 = new PKSimParameter().WithName("tata").WithId("tata").WithFormula(new ExplicitFormula("6"));
         _objectToUnregister.Add(_child1);
         _objectToUnregister.Add(_child2);
         sut = new UnregisterObjectVisitor(_withIdRepository);
      }
   }

   public class When_unregistering_an_object_from_the_object_factory : concern_for_UnregisterObjectVisitor
   {
      protected override void Because()
      {
         sut.Unregister(_objectToUnregister);
      }

      [Observation]
      public void should_unregister_the_object_in_the_factory()
      {
         A.CallTo(() => _withIdRepository.Unregister(_objectToUnregister.Id)).MustHaveHappened();
      }

      [Observation]
      public void should_unregister_all_children_from_the_object_to_unregister_as_well()
      {
         A.CallTo(() => _withIdRepository.Unregister(_child1.Id)).MustHaveHappened();
         A.CallTo(() => _withIdRepository.Unregister(_child2.Id)).MustHaveHappened();
      }
   }
}