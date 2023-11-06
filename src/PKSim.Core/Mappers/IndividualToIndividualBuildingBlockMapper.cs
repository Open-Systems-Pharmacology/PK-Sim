using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Format;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using static PKSim.Assets.PKSimConstants.UI;
using static PKSim.Core.CoreConstants.Groups;
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
      private readonly IIndividualParametersSameFormulaOrValueForAllSpeciesRepository _sameFormulaOrValueForAllSpeciesRepository;
      private readonly IDimensionRepository _dimensionRepository;
      private readonly NumericFormatter<double> _formatter;

      public IndividualToIndividualBuildingBlockMapper(IObjectBaseFactory objectBaseFactory,
         IEntityPathResolver entityPathResolver,
         IApplicationConfiguration applicationConfiguration,
         ILazyLoadTask lazyLoadTask,
         IRepresentationInfoRepository representationInfoRepository,
         ICalculationMethodCategoryRepository calculationMethodCategoryRepository,
         IFormulaFactory formulaFactory,
         IParameterQuery parameterQuery,
         IIndividualParametersSameFormulaOrValueForAllSpeciesRepository sameFormulaOrValueForAllSpeciesRepository,
         ICloner cloner,
         IDimensionRepository dimensionRepository) : base(objectBaseFactory, entityPathResolver, applicationConfiguration, lazyLoadTask, formulaFactory, cloner)
      {
         _representationInfoRepository = representationInfoRepository;
         _calculationMethodCategoryRepository = calculationMethodCategoryRepository;
         _parameterQuery = parameterQuery;
         _sameFormulaOrValueForAllSpeciesRepository = sameFormulaOrValueForAllSpeciesRepository;
         _dimensionRepository = dimensionRepository;
         _formatter = new NumericFormatter<double>(NumericFormatterOptions.Instance);
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
         bool shouldExportParameter(IParameter parameter)
         {
            //these parameters are exported separately
            if (parameter.GroupName == RELATIVE_EXPRESSION)
               return false;

            return !_sameFormulaOrValueForAllSpeciesRepository.IsSameFormulaOrValue(parameter);
         }

         //we only add parameters that either not defined in all species OR define in all species with a value or formula that is different between species
         return individual.GetAllChildren<IParameter>(shouldExportParameter);
      }

      protected override IndividualParameter CreateParameter(IParameter parameter, Individual individual)
      {
         var individualParameter = base.CreateParameter(parameter, individual);
         individualParameter.Info = parameter.Info.Clone();
         individualParameter.IsDefault = parameter.IsDefault;
         individualParameter.Origin = new ParameterOrigin
         {
            ParameterId = parameter.Id,
            BuilingBlockId = individual.Id,
         };

         return individualParameter;
      }

      protected override void MapFormulaOrValue(IParameter parameter, IndividualParameter builderParameter, Individual pkSimBuildingBlock, IFormulaCache formulaCache)
      {
         base.MapFormulaOrValue(parameter, builderParameter, pkSimBuildingBlock, formulaCache);
         switch (parameter.Formula)
         {
            case DistributionFormula distributionFormula:
               builderParameter.DistributionType = distributionFormula.DistributionType;
               break;
         }
      }

      public override IndividualBuildingBlock MapFrom(Individual individual)
      {
         var buildingBlock = base.MapFrom(individual);
         buildingBlock.Icon = individual.Icon;

         addOriginDataToBuildingBlock(buildingBlock, PKSimConstants.UI.DiseaseState, individual.OriginData.DiseaseState?.DisplayName);
         individual.OriginData.DiseaseStateParameters.Each(x => addOriginDataToBuildingBlock(buildingBlock, x.Name, x));
         addOriginDataToBuildingBlock(buildingBlock, PKSimConstants.UI.Species, individual.Species?.DisplayName);
         addOriginDataToBuildingBlock(buildingBlock, PKSimConstants.UI.Gender, individual.OriginData.Gender?.DisplayName);
         addOriginDataToBuildingBlock(buildingBlock, Age, individual.OriginData.Age);
         addOriginDataToBuildingBlock(buildingBlock, GestationalAge, individual.OriginData.GestationalAge);
         addOriginDataToBuildingBlock(buildingBlock, Height, individual.OriginData.Height);
         addOriginDataToBuildingBlock(buildingBlock, BMI, individual.OriginData.BMI);
         addOriginDataToBuildingBlock(buildingBlock, Weight, individual.OriginData.Weight);
         addOriginDataToBuildingBlock(buildingBlock, PKSimConstants.UI.Population, individual.OriginData.Population?.DisplayName);

         individual.OriginData.AllCalculationMethods().Where(cm => _calculationMethodCategoryRepository.HasMoreThanOneOption(cm, individual.Species))
            .Each(x => addCalculationMethodOriginDataToBuildingBlock(buildingBlock, x));

         return buildingBlock;
      }

      private void addCalculationMethodOriginDataToBuildingBlock(IndividualBuildingBlock buildingBlock, CalculationMethod calculationMethod)
      {
         var repInfo = _representationInfoRepository.InfoFor(RepresentationObjectType.CATEGORY, calculationMethod.Category);
         addOriginDataToBuildingBlock(buildingBlock, repInfo.DisplayName, calculationMethod.DisplayName);
      }

      private void addOriginDataToBuildingBlock(IndividualBuildingBlock buildingBlock, string key, OriginDataParameter parameter)
      {
         if (parameter == null)
            return;

         var displayValue = originDataFormattedForDisplay(parameter);
         addOriginDataToBuildingBlock(buildingBlock, keyForOriginDataParameter(key, parameter), $"{displayValue} {parameter.Unit}");
      }

      private string originDataFormattedForDisplay(OriginDataParameter parameter)
      {
         var dimension = _dimensionRepository.DimensionForUnit(parameter.Unit);
         var unit = dimension.FindUnit(parameter.Unit);
         return _formatter.Format(dimension.BaseUnitValueToUnitValue(unit, parameter.Value)).TrimEnd('0', '.');
      }

      private void addOriginDataToBuildingBlock(IndividualBuildingBlock buildingBlock, string key, string value)
      {
         if (string.IsNullOrEmpty(value))
            return;

         buildingBlock.OriginData.Add(new ExtendedProperty<string> { Name = key, Value = value });
      }

      private string keyForOriginDataParameter(string key, OriginDataParameter parameter) => string.IsNullOrEmpty(parameter.Name) ? key : parameter.Name;
   }
}