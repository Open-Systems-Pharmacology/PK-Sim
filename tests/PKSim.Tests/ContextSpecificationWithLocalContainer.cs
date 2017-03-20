using OSPSuite.BDDHelper;
using OSPSuite.Utility.Container;
using FakeItEasy;
using NUnit.Framework;

namespace PKSim
{
   [Category("LocalOnly")]
   public abstract class ContextSpecificationWithLocalContainer<T> : ContextSpecification<T>
   {
      private IContainer _oldContainer;
      protected IContainer _container;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _container = A.Fake<IContainer>();
         _oldContainer = IoC.Container;
         IoC.InitializeWith(_container);
      }

      public override void GlobalCleanup()
      {
         base.GlobalCleanup();
         IoC.InitializeWith(_oldContainer);
      }
   }
}