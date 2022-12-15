using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using OSPSuite.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Services;
using PKSim.Core.Model;

namespace PKSim.Core.Mappers
{
   public interface IIndividualToIndividualBuildingBlockMapper : IPathAndValueBuildingBlockMapper<Individual, IndividualBuildingBlock>
   {
   }

   public class IndividualToIndividualBuildingBlockMapper : PathAndValueBuildingBlockMapper<Individual, IndividualBuildingBlock, IndividualParameter>, IIndividualToIndividualBuildingBlockMapper
   {
      private readonly ILazyLoadTask _lazyLoadTask;

      public IndividualToIndividualBuildingBlockMapper(IObjectBaseFactory objectBaseFactory, IEntityPathResolver entityPathResolver, IApplicationConfiguration applicationConfiguration, 
         ILazyLoadTask lazyLoadTask) : base(objectBaseFactory, entityPathResolver, applicationConfiguration)
      {
         _lazyLoadTask = lazyLoadTask;
      }

      protected override IReadOnlyList<IParameter> AllParametersFor(Individual sourcePKSimBuildingBlock)
      {
         return sourcePKSimBuildingBlock.GetAllChildren<IParameter>().Where(x => x.GroupName != CoreConstants.Groups.RELATIVE_EXPRESSION).ToList();
      }

      public override IndividualBuildingBlock MapFrom(Individual input)
      {
         _lazyLoadTask.Load(input);
         var buildingBlock = base.MapFrom(input);

         mapOriginData(buildingBlock, Assets.PKSimConstants.UI.Species, input.Species?.DisplayName);
         mapOriginData(buildingBlock, Assets.PKSimConstants.UI.DiseaseState, input.OriginData.DiseaseState?.DisplayName);
         mapOriginData(buildingBlock, Assets.PKSimConstants.UI.Population, input.OriginData.Population?.DisplayName);
         mapOriginData(buildingBlock, Assets.PKSimConstants.UI.Gender, input.OriginData.Gender?.DisplayName);
         mapOriginData(buildingBlock, Assets.PKSimConstants.UI.Age, input.OriginData.Age?.Value);
         mapOriginData(buildingBlock, Assets.PKSimConstants.UI.GestationalAge, input.OriginData.GestationalAge?.Value);
         mapOriginData(buildingBlock, Assets.PKSimConstants.UI.Height, input.OriginData.Height?.Value);
         mapOriginData(buildingBlock, Assets.PKSimConstants.UI.BMI, input.OriginData.BMI?.Value);
         mapOriginData(buildingBlock, Assets.PKSimConstants.UI.Weight, input.OriginData.Weight?.Value);

         buildingBlock.ValueOrigin = input.OriginData.ValueOrigin.Clone();
         buildingBlock.CalculationMethodCache = input.OriginData.CalculationMethodCache.Clone();

         return buildingBlock;
      }

      private void mapOriginData(IndividualBuildingBlock buildingBlock, string key, double? value)
      {
         mapOriginData(buildingBlock, key, value?.ToString(CultureInfo.InvariantCulture));
      }

      private void mapOriginData(IndividualBuildingBlock buildingBlock, string key, string value)
      {
         if(string.IsNullOrEmpty(value))
            return;

         buildingBlock.OriginData[key] = value;
      }
   }
}