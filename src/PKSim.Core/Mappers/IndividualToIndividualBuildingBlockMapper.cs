using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Core.Mappers
{
   public interface IIndividualToIndividualBuildingBlockMapper : IPathAndValueBuildingBlockMapper<Individual, IndividualBuildingBlock>
   {
   }

   public class IndividualToIndividualBuildingBlockMapper : PathAndValueBuildingBlockMapper<Individual, IndividualBuildingBlock, IndividualParameter>, IIndividualToIndividualBuildingBlockMapper
   {
      private readonly IRepresentationInfoRepository _representationInfoRepository;
      private readonly ICalculationMethodCategoryRepository _calculationMethodCategoryRepository;

      public IndividualToIndividualBuildingBlockMapper(IObjectBaseFactory objectBaseFactory, IEntityPathResolver entityPathResolver, IApplicationConfiguration applicationConfiguration,
         ILazyLoadTask lazyLoadTask, IRepresentationInfoRepository representationInfoRepository, ICalculationMethodCategoryRepository calculationMethodCategoryRepository) : base(objectBaseFactory, entityPathResolver, applicationConfiguration, lazyLoadTask)
      {
         _representationInfoRepository = representationInfoRepository;
         _calculationMethodCategoryRepository = calculationMethodCategoryRepository;
      }

      protected override IReadOnlyList<IParameter> AllParametersFor(Individual individual)
      {
         return individual.GetAllChildren<IParameter>().Where(x => x.GroupName != CoreConstants.Groups.RELATIVE_EXPRESSION).ToList();
      }

      public override IndividualBuildingBlock MapFrom(Individual input)
      {
         var buildingBlock = base.MapFrom(input);
         buildingBlock.Icon = input.Icon;

         addOriginDataToBuildingBlock(buildingBlock, PKSimConstants.UI.DiseaseState, input.OriginData.DiseaseState?.DisplayName);
         addOriginDataToBuildingBlock(buildingBlock, PKSimConstants.UI.Species, input.Species?.DisplayName);
         addOriginDataToBuildingBlock(buildingBlock, PKSimConstants.UI.Gender, input.OriginData.Gender?.DisplayName);
         addOriginDataToBuildingBlock(buildingBlock, PKSimConstants.UI.Age, input.OriginData.Age);
         addOriginDataToBuildingBlock(buildingBlock, PKSimConstants.UI.GestationalAge, input.OriginData.GestationalAge);
         addOriginDataToBuildingBlock(buildingBlock, PKSimConstants.UI.Height, input.OriginData.Height);
         addOriginDataToBuildingBlock(buildingBlock, PKSimConstants.UI.BMI, input.OriginData.BMI);
         addOriginDataToBuildingBlock(buildingBlock, PKSimConstants.UI.Weight, input.OriginData.Weight);
         addOriginDataToBuildingBlock(buildingBlock, PKSimConstants.UI.Population, input.OriginData.Population?.DisplayName);

         input.OriginData.AllCalculationMethods().Where(cm => _calculationMethodCategoryRepository.HasMoreThanOneOption(cm, input.Species))
            .Each(x => addCalculationMethodOriginDataToBuildingBlock(buildingBlock, x));

         buildingBlock.OriginData.ValueOrigin = input.OriginData.ValueOrigin.Clone();

         return buildingBlock;
      }

      private void addCalculationMethodOriginDataToBuildingBlock(IndividualBuildingBlock buildingBlock, CalculationMethod calculationMethod)
      {
         var repInfo = _representationInfoRepository.InfoFor(RepresentationObjectType.CATEGORY, calculationMethod.Category);
         addOriginDataToBuildingBlock(buildingBlock, repInfo.DisplayName, calculationMethod.Category);
      }

      private void addOriginDataToBuildingBlock(IndividualBuildingBlock buildingBlock, string key, OriginDataParameter parameter)
      {
         if (parameter == null)
            return;

         addOriginDataToBuildingBlock(buildingBlock, keyForOriginDataParameter(key, parameter), $"{parameter.Value} {parameter.Unit}");
      }

      private string keyForOriginDataParameter(string key, OriginDataParameter parameter)
      {
         return string.IsNullOrEmpty(parameter.Name) ? key : parameter.Name;
      }

      private void addOriginDataToBuildingBlock(IndividualBuildingBlock buildingBlock, string key, string value)
      {
         if (string.IsNullOrEmpty(value))
            return;

         buildingBlock.OriginData.AddOriginDataItem(new OriginDataItem { Name = key, Value = value });
      }
   }
}