using System.Collections.Generic;
using OSPSuite.Utility.Collections;
using PKSim.Core.Model.PopulationAnalyses;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;

namespace PKSim.Core.Repositories
{
   public interface IRepresentationInfoRepository : IStartableRepository<RepresentationInfo>
   {
      IEnumerable<RepresentationInfo> AllOfType(RepresentationObjectType objectType);
      RepresentationInfo InfoFor(RepresentationObjectType objectType, string objectName);
      RepresentationInfo ContainerInfoFor(string objectName);
      RepresentationInfo InfoFor(IObjectBase objectBase);
      string DisplayNameFor(IObjectBase objectBase);
      string DisplayNameFor(StatisticalAggregation statisticalAggregation);
      string DescriptionFor(IObjectBase objectBase);
      string DescriptionFor(RepresentationObjectType objectType, string objectName);
      string DisplayNameFor(RepresentationObjectType objectType, string objectName);
      bool ContainsInfoFor(IObjectBase objectBase);
   }
}