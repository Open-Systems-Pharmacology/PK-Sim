using OSPSuite.BDDHelper;
using FakeItEasy;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;

namespace PKSim.Core
{
   public abstract class concern_for_RegistrationTask : ContextSpecification<IRegistrationTask>
   {
      protected IUnregisterObjectVisitor _uregisterObjectVisitor;
      protected IRegisterObjectVisitor _registerObjectVisitor;
      protected IObjectBase _oneObject;

      protected override void Context()
      {
         _uregisterObjectVisitor = A.Fake<IUnregisterObjectVisitor>();
         _registerObjectVisitor = A.Fake<IRegisterObjectVisitor>();
         _oneObject = A.Fake<IObjectBase>();
         sut = new RegistrationTask(_registerObjectVisitor, _uregisterObjectVisitor);
      }
   }

   public class When_asked_to_register_an_object : concern_for_RegistrationTask
   {
      protected override void Because()
      {
         sut.Register(_oneObject);
      }

      [Observation]
      public void should_leverate_the_register_object_visitor()
      {
         A.CallTo(() => _registerObjectVisitor.Register(_oneObject)).MustHaveHappened();
      }
   }

   public class When_asked_to_unregister_an_object : concern_for_RegistrationTask
   {
      protected override void Because()
      {
         sut.Unregister(_oneObject);
      }

      [Observation]
      public void should_leverate_the_register_object_visitor()
      {
         A.CallTo(() => _uregisterObjectVisitor.Unregister(_oneObject)).MustHaveHappened();
      }
   }
}