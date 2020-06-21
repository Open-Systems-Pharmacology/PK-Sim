using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Core.Services
{
   public class FullPathDisplayResolver : OSPSuite.Core.Domain.Services.FullPathDisplayResolver
   {
      private readonly IRepresentationInfoRepository _representationInfoRepository;

      public FullPathDisplayResolver(IQuantityPathToQuantityDisplayPathMapper quantityDisplayPathMapper, IRepresentationInfoRepository representationInfoRepository) : base(quantityDisplayPathMapper)
      {
         _representationInfoRepository = representationInfoRepository;
      }

      public override string FullPathFor(IObjectBase objectBase, bool addSimulationName = false)
      {
         switch (objectBase)
         {
            case ParameterAlternativeGroup parameterAlternativeGroup:
               return DisplayFor(parameterAlternativeGroup);
         }

         return base.FullPathFor(objectBase, addSimulationName);
      }

      protected override string DisplayFor(IObjectBase objectBase) => _representationInfoRepository.DisplayNameFor(objectBase);
   }
}