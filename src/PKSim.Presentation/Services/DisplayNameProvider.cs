using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Services;

namespace PKSim.Presentation.Services
{
   public class DisplayNameProvider : IDisplayNameProvider
   {
      private readonly IRepresentationInfoRepository _representationInfoRepository;
      private readonly IObjectTypeResolver _objectTypeResolver;

      public DisplayNameProvider(IRepresentationInfoRepository representationInfoRepository, IObjectTypeResolver objectTypeResolver)
      {
         _representationInfoRepository = representationInfoRepository;
         _objectTypeResolver = objectTypeResolver;
      }

      public string DisplayNameFor(object objectToDisplay)
      {
         if (objectToDisplay == null)
            return string.Empty;

         var withName = objectToDisplay as IWithName;
         if (withName == null)
            return _objectTypeResolver.TypeFor(objectToDisplay);

         var objectBase = objectToDisplay as IObjectBase;
         if (objectBase == null)
            return withName.Name;

         return _representationInfoRepository.DisplayNameFor(objectBase);
      }
   }
}