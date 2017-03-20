using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;

namespace PKSim.Matlab
{
   public class MatlabFullPathDisplayResolver : IFullPathDisplayResolver
   {
      private readonly IObjectPathFactory _objectPathFactory;

      public MatlabFullPathDisplayResolver(IObjectPathFactory objectPathFactory)
      {
         _objectPathFactory = objectPathFactory;
      }

      public string FullPathFor(IObjectBase objectBase, bool addSimulationName = false)
      {
         var entity = objectBase as IEntity;
         if (entity != null)
            return _objectPathFactory.CreateAbsoluteObjectPath(entity).PathAsString;

         return objectBase.Name;
      }

   }
}