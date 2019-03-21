using OSPSuite.Core.Domain.Builder;
using OSPSuite.Utility;
using PKSim.Presentation.DTO.Observers;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface IObservedToObserverDTOMapper : IMapper<IObserverBuilder, ObserverDTO>
   {
   }

   public class ObservedToObserverDTOMapper : IObservedToObserverDTOMapper
   {
      public ObserverDTO MapFrom(IObserverBuilder observer)
      {
         return new ObserverDTO(observer);
      }
   }
}