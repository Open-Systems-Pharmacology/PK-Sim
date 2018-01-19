using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Utility.Extensions;

namespace PKSim.Core.Services
{
   public interface IParameterDefaultStateUpdater
   {
      void UpdateDefaultFor(ISpatialStructure spatialStructure);
      void UpdateDefaultFor(IEventGroupBuildingBlock eventGroupBuildingBlock);
   }

   public class ParameterDefaultStateUpdater : IParameterDefaultStateUpdater
   {
      public void UpdateDefaultFor(ISpatialStructure spatialStructure)
      {
         //TODO implement
      }


      private void updateDefaultStateForOriginDataParameters(IContainer spatialStructureOrganism)
      {
         new[] { CoreConstants.Parameter.AGE, Constants.Parameters.GESTATIONAL_AGE, CoreConstants.Parameter.HEIGHT }.Each(x =>
         {
            spatialStructureOrganism.Parameter(x).IsDefault = true;
         });
      }


      public void UpdateDefaultFor(IEventGroupBuildingBlock eventGroupBuildingBlock)
      {
         //TODO implement
      }
   }
}