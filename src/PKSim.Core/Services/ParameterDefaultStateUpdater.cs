using System.Linq;
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
         var organism = spatialStructure.TopContainers.FindByName(Constants.ORGANISM);
         if (organism == null) return;
         updateDefaultStateForOriginDataParameters(organism);
      }

      private void updateDefaultStateForOriginDataParameters(IContainer organism)
      {
         var parameterNames = new[]
         {
            CoreConstants.Parameters.AGE,
            Constants.Parameters.GESTATIONAL_AGE,
            CoreConstants.Parameters.HEIGHT
         };

         parameterNames
            .Select(organism.Parameter)
            .Where(p => p != null)
            .Each(resetDefault);
      }

      public void UpdateDefaultFor(IEventGroupBuildingBlock eventGroupBuildingBlock)
      {
         eventGroupBuildingBlock.Each(updateDefaultStateForEventParameters);
      }

      private void updateDefaultStateForEventParameters(IEventGroupBuilder eventGroupBuilder)
      {
         var parameterNames = new[]
         {
            CoreConstants.Parameters.DOSE,
            CoreConstants.Parameters.DOSE_PER_BODY_WEIGHT,
            CoreConstants.Parameters.DOSE_PER_BODY_SURFACE_AREA,
            Constants.Parameters.START_TIME
         };

         eventGroupBuilder.GetAllChildren<IParameter>(x => x.NameIsOneOf(parameterNames)).Each(resetDefault);
      }

      private void resetDefault(IParameter parameter) => parameter.IsDefault = true;
   }
}