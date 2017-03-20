using OSPSuite.Utility;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Presentation.DTO.PopulationAnalyses;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface IObservedDataCurveOptionsToObservedDataCurveOptionsDTOMapper:IMapper<ObservedDataCurveOptions,ObservedDataCurveOptionsDTO>
   {
   }

   public class ObservedDataCurveOptionsToObservedDataCurveOptionsDTOMapper : IObservedDataCurveOptionsToObservedDataCurveOptionsDTOMapper
   {
      public ObservedDataCurveOptionsDTO MapFrom(ObservedDataCurveOptions observedDataCurveOptions)
      {
         return new ObservedDataCurveOptionsDTO(observedDataCurveOptions);
      }
   }
}