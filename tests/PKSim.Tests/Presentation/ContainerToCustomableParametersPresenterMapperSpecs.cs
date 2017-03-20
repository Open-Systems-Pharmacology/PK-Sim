using OSPSuite.Utility.Container;
using OSPSuite.BDDHelper;
using FakeItEasy;
using PKSim.Presentation.Presenters.Parameters.Mappers;

namespace PKSim.Presentation
{
   public abstract class concern_for_ContainerToCustomableParametersPresenterMapper    : ContextSpecification<IContainerToCustomableParametersPresenterMapper>
   {
      private IContainer _container;

      protected override void Context()
      {
         _container = A.Fake<IContainer>();
         sut = new ContainerToCustomableParametersPresenterMapper(_container);
      }
   }
}	