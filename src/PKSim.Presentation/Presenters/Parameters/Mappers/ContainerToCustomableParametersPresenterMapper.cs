using OSPSuite.Utility;
using OSPSuite.Core.Domain;

namespace PKSim.Presentation.Presenters.Parameters.Mappers
{
   public interface IContainerToCustomableParametersPresenterMapper : IMapper<IContainer, ICustomParametersPresenter>
   {
   }

   public class ContainerToCustomableParametersPresenterMapper : IContainerToCustomableParametersPresenterMapper
   {
      private readonly OSPSuite.Utility.Container.IContainer _ioc;

      public ContainerToCustomableParametersPresenterMapper(OSPSuite.Utility.Container.IContainer ioc)
      {
         _ioc = ioc;
      }

      public ICustomParametersPresenter MapFrom(IContainer container)
      {
         return _ioc.Resolve<IMultiParameterEditPresenter>();
      }
   }
}