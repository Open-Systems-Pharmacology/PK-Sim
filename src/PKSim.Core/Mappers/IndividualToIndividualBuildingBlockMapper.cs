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
using PKSim.Core.Services;
using ILazyLoadTask = OSPSuite.Core.Domain.Services.ILazyLoadTask;

namespace PKSim.Core.Mappers
{
   public interface IIndividualToIndividualBuildingBlockMapper : IPathAndValueBuildingBlockMapper<Individual, IndividualBuildingBlock, IndividualParameter>
   {
   }

   public class IndividualToIndividualBuildingBlockMapper : PathAndValueBuildingBlockMapper<Individual, IndividualBuildingBlock, IndividualParameter>, IIndividualToIndividualBuildingBlockMapper
   {
      private readonly IRepresentationInfoRepository _representationInfoRepository;
      private readonly ICalculationMethodCategoryRepository _calculationMethodCategoryRepository;

      public IndividualToIndividualBuildingBlockMapper(IObjectBaseFactory objectBaseFactory, IEntityPathResolver entityPathResolver, IApplicationConfiguration applicationConfiguration,
         ILazyLoadTask lazyLoadTask, IRepresentationInfoRepository representationInfoRepository, ICalculationMethodCategoryRepository calculationMethodCategoryRepository, ICloner cloner) : base(objectBaseFactory, entityPathResolver, applicationConfiguration, lazyLoadTask, cloner)
      {
         _representationInfoRepository = representationInfoRepository;
         _calculationMethodCategoryRepository = calculationMethodCategoryRepository;
      }

      protected override IReadOnlyList<IParameter> AllParametersFor(Individual individual)
      {
         return individual.GetAllChildren<IParameter>().Where(x => x.GroupName != CoreConstants.Groups.RELATIVE_EXPRESSION).ToList();
      }

      public override IndividualBuildingBlock MapFrom(Individual individual)
      {
         var buildingBlock = base.MapFrom(individual);
         buildingBlock.Icon = individual.Icon;

         addOriginDataToBuildingBlock(buildingBlock, PKSimConstants.UI.DiseaseState, individual.OriginData.DiseaseState?.DisplayName);
         addOriginDataToBuildingBlock(buildingBlock, PKSimConstants.UI.Species, individual.Species?.DisplayName);
         addOriginDataToBuildingBlock(buildingBlock, PKSimConstants.UI.Gender, individual.OriginData.Gender?.DisplayName);
         addOriginDataToBuildingBlock(buildingBlock, PKSimConstants.UI.Age, individual.OriginData.Age);
         addOriginDataToBuildingBlock(buildingBlock, PKSimConstants.UI.GestationalAge, individual.OriginData.GestationalAge);
         addOriginDataToBuildingBlock(buildingBlock, PKSimConstants.UI.Height, individual.OriginData.Height);
         addOriginDataToBuildingBlock(buildingBlock, PKSimConstants.UI.BMI, individual.OriginData.BMI);
         addOriginDataToBuildingBlock(buildingBlock, PKSimConstants.UI.Weight, individual.OriginData.Weight);
         addOriginDataToBuildingBlock(buildingBlock, PKSimConstants.UI.Population, individual.OriginData.Population?.DisplayName);

         individual.OriginData.AllCalculationMethods().Where(cm => _calculationMethodCategoryRepository.HasMoreThanOneOption(cm, individual.Species))
            .Each(x => addCalculationMethodOriginDataToBuildingBlock(buildingBlock, x));

         buildingBlock.OriginData.ValueOrigin = individual.OriginData.ValueOrigin.Clone();

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

         buildingBlock.OriginData.AddOriginDataItem(new OriginDataItem {Name = key, Value = value});
      }
   }
}