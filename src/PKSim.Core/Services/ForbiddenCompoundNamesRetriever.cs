using System.Collections.Generic;
using System.Linq;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Core.Services
{
   /// <summary>
   ///    Retrieves the names that cannot be used for a compound because they are the internal names of model elements
   ///    (containers, parameters, molecules, observers or processes). A compound using one of these names would clash
   ///    with the corresponding element during simulation construction.
   /// </summary>
   public interface IForbiddenCompoundNamesRetriever
   {
      IReadOnlyList<string> All();
   }

   public class ForbiddenCompoundNamesRetriever : IForbiddenCompoundNamesRetriever
   {
      private static readonly RepresentationObjectType[] _forbiddenObjectTypes =
      {
         RepresentationObjectType.CONTAINER,
         RepresentationObjectType.PARAMETER,
         RepresentationObjectType.MOLECULE,
         RepresentationObjectType.OBSERVER,
         RepresentationObjectType.PROCESS,
      };

      private readonly IRepresentationInfoRepository _representationInfoRepository;

      public ForbiddenCompoundNamesRetriever(IRepresentationInfoRepository representationInfoRepository)
      {
         _representationInfoRepository = representationInfoRepository;
      }

      public IReadOnlyList<string> All() => _forbiddenObjectTypes
         .SelectMany(_representationInfoRepository.AllOfType)
         .Select(x => x.Name)
         .Distinct()
         .ToList();
   }
}
