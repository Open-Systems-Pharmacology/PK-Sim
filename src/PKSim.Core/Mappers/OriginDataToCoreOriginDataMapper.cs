using OSPSuite.Core.Domain;
using OSPSuite.Utility;
using PKSim.Core.Model;

namespace PKSim.Core.Mappers
{
   public interface IOriginDataToCoreOriginDataMapper : IMapper<OriginData, CoreOriginData>
   {
   }

   public class OriginDataToCoreOriginDataMapper : IOriginDataToCoreOriginDataMapper
   {
      public CoreOriginData MapFrom(OriginData input)
      {
         var coreOriginData = new CoreOriginData
         {
            Species = input.Species.DisplayName,
            Age = input.Age?.Clone(),
            BMI = input.BMI?.Clone(),
            Comment = input.Comment,
            Height = input.Height?.Clone(),
            Weight = input.Weight?.Clone()
         };
         coreOriginData.ValueOrigin.UpdateFrom(input.ValueOrigin);

         return coreOriginData;
      }
   }
}