﻿using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Model.Extensions;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using IFormulaFactory = PKSim.Core.Model.IFormulaFactory;
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
      private readonly IParameterQuery _parameterQuery;

      public IndividualToIndividualBuildingBlockMapper(
         IObjectBaseFactory objectBaseFactory,
         IEntityPathResolver entityPathResolver,
         IApplicationConfiguration applicationConfiguration,
         ILazyLoadTask lazyLoadTask,
         IRepresentationInfoRepository representationInfoRepository,
         ICalculationMethodCategoryRepository calculationMethodCategoryRepository,
         IFormulaFactory formulaFactory,
         IParameterQuery parameterQuery) : base(objectBaseFactory, entityPathResolver, applicationConfiguration, lazyLoadTask, formulaFactory)
      {
         _representationInfoRepository = representationInfoRepository;
         _calculationMethodCategoryRepository = calculationMethodCategoryRepository;
         _parameterQuery = parameterQuery;
      }

      protected override IFormula TemplateFormulaFor(IParameter parameter, IFormulaCache formulaCache, Individual individual)
      {
         bool isMetaDataForParameter(ParameterMetaData p) => p.BuildingBlockType == PKSimBuildingBlockType.Individual && string.Equals(p.ParameterName, parameter.Name);


         //for individual, the CM to use depends on the CM available in the origin data as well as the container where the parameter resides.
         var calculationMethods = individual.OriginData.AllCalculationMethods().AllNames();

         var parameterRate = _parameterQuery.ParameterRatesFor(parameter.ParentContainer, calculationMethods, isMetaDataForParameter).ToList();

         //this is not possible?
         if (parameterRate.Count != 1)
            return null;

         var cloneFormula = _formulaFactory.RateFor(parameterRate[0], formulaCache);
         cloneFormula.ObjectPaths.Each(x => x.Remove(Constants.ROOT));
         return cloneFormula;
      }

      protected override IReadOnlyList<IParameter> AllParametersFor(Individual individual)
      {
         return individual.GetAllChildren<IParameter>().Where(x => x.GroupName != CoreConstants.Groups.RELATIVE_EXPRESSION).ToList();
      }

      public override IndividualParameter MapParameter(IParameter parameter, Individual individual)
      {
         var individualParameter = base.MapParameter(parameter, individual);
         individualParameter.Info = parameter.Info.Clone();
         individualParameter.Origin = new ParameterOrigin
         {
            ParameterId = parameter.Id,
            BuilingBlockId = individual.Id,
         };

         return individualParameter;
      }

      protected override void MapFormulaOrValue(IParameter parameter, IndividualParameter builderParameter, Individual pkSimBuildingBlock, BuildingBlockFormulaCache formulaCache)
      {
         base.MapFormulaOrValue(parameter, builderParameter, pkSimBuildingBlock, formulaCache);
         switch (parameter.Formula)
         {
            case DistributionFormula distributionFormula:
               builderParameter.DistributionType = distributionFormula.DistributionType().Id;
               break;
         }
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

         buildingBlock.OriginData.Add(new ExtendedProperty<string> {Name = key, Value = value});
      }
   }
}