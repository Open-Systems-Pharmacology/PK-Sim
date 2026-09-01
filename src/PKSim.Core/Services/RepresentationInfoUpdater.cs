using OSPSuite.Core.Domain;
using OSPSuite.Utility.Visitor;
using PKSim.Core.Repositories;

namespace PKSim.Core.Services
{
   public interface IRepresentationInfoUpdater
   {
      void UpdateRepresentationInfoIn(IVisitable<IVisitor> visitable);
   }

   public class RepresentationInfoUpdater : IRepresentationInfoUpdater, IVisitor<IObjectBase>
   {
      private readonly IRepresentationInfoRepository _representationInfoRepository;

      public RepresentationInfoUpdater(IRepresentationInfoRepository representationInfoRepository)
      {
         _representationInfoRepository = representationInfoRepository;
      }

      public void UpdateRepresentationInfoIn(IVisitable<IVisitor> visitable)
      {
         _representationInfoRepository.Start();
         visitable.AcceptVisitor(this);
      }

      public void Visit(IObjectBase objToVisit)
      {
         if (!_representationInfoRepository.ContainsInfoFor(objToVisit))
            return;

         var repInfo = _representationInfoRepository.InfoFor(objToVisit);
         objToVisit.Icon = repInfo.IconName;

         if (string.IsNullOrEmpty(objToVisit.Description))
            objToVisit.Description = repInfo.Description;
      }
   }
}
