using OSPSuite.Core.Domain;
using OSPSuite.Utility;
using PKSim.Infrastructure.ORM.FlatObjects;

namespace PKSim.Infrastructure.ORM.Mappers
{
   public interface IFlatValueOriginToValueOriginMapper : IMapper<FlatValueOrigin, ValueOrigin>
   {
   }

   public class FlatValueOriginToValueOriginMapper : IFlatValueOriginToValueOriginMapper
   {
      public ValueOrigin MapFrom(FlatValueOrigin flatValueOrigin)
      {
         return new ValueOrigin
         {
            Description = flatValueOrigin.Description,
            Type = ValueOriginTypes.ManualFit
         };
      }
   }
}