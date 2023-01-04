using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core;
using PKSim.Presentation.DTO.Mappers;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Extensions;

namespace PKSim.Presentation.Mappers
{
   public interface IIndividualToIndividualBuildingBlockMapper : IPathAndValueBuildingBlockMapper<Individual, IndividualBuildingBlock>
   {
   }

   public class IndividualToIndividualBuildingBlockMapper : PathAndValueBuildingBlockMapper<Individual, IndividualBuildingBlock, IndividualParameter>, IIndividualToIndividualBuildingBlockMapper
   {
      private readonly ICalculationMethodToCategoryCalculationMethodDTOMapper _calculationMethodDTOMapper;

      public IndividualToIndividualBuildingBlockMapper(IObjectBaseFactory objectBaseFactory, IEntityPathResolver entityPathResolver, IApplicationConfiguration applicationConfiguration,
         ILazyLoadTask lazyLoadTask, ICalculationMethodToCategoryCalculationMethodDTOMapper calculationMethodDTOMapper) : base(objectBaseFactory, entityPathResolver, applicationConfiguration, lazyLoadTask)
      {
         _calculationMethodDTOMapper = calculationMethodDTOMapper;
      }

      protected override IReadOnlyList<IParameter> AllParametersFor(Individual individual)
      {
         return individual.GetAllChildren<IParameter>().Where(x => x.GroupName != CoreConstants.Groups.RELATIVE_EXPRESSION).ToList();
      }

      public override IndividualBuildingBlock MapFrom(Individual input)
      {
         var buildingBlock = base.MapFrom(input);
         buildingBlock.Icon = input.Icon;

         addOriginDataToBuildingBlock(buildingBlock, Assets.PKSimConstants.UI.DiseaseState, input.OriginData.DiseaseState?.DisplayName);
         addOriginDataToBuildingBlock(buildingBlock, Assets.PKSimConstants.UI.Species, input.Species?.DisplayName);
         addOriginDataToBuildingBlock(buildingBlock, Assets.PKSimConstants.UI.Gender, input.OriginData.Gender?.DisplayName);
         addOriginDataToBuildingBlock(buildingBlock, Assets.PKSimConstants.UI.Age, input.OriginData.Age);
         addOriginDataToBuildingBlock(buildingBlock, Assets.PKSimConstants.UI.GestationalAge, input.OriginData.GestationalAge);
         addOriginDataToBuildingBlock(buildingBlock, Assets.PKSimConstants.UI.Height, input.OriginData.Height);
         addOriginDataToBuildingBlock(buildingBlock, Assets.PKSimConstants.UI.BMI, input.OriginData.BMI);
         addOriginDataToBuildingBlock(buildingBlock, Assets.PKSimConstants.UI.Weight, input.OriginData.Weight);
         addOriginDataToBuildingBlock(buildingBlock, Assets.PKSimConstants.UI.Population, input.OriginData.Population?.DisplayName);

         input.OriginData.AllCalculationMethods().MapAllUsing(_calculationMethodDTOMapper)
            .Each(x => addOriginDataToBuildingBlock(buildingBlock, x.DisplayName, x.CategoryItem.DisplayName));

         buildingBlock.OriginData.ValueOrigin = input.OriginData.ValueOrigin.Clone();

         return buildingBlock;
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
