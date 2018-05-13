using System;
using System.Collections.Generic;
using System.Linq;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Services;
using OSPSuite.TeXReporting.Items;

namespace PKSim.Core.Model
{
   public interface IParameterFactory
   {
      /// <summary>
      ///    Returns a default parameter with the name set to <paramref name="parameterName" /> and the buildingBlockType set to
      ///    <paramref name="buildingBlockType" />
      /// </summary>
      IParameter CreateFor(string parameterName, PKSimBuildingBlockType buildingBlockType);

      /// <summary>
      ///    Returns a default parameter with the name set to <paramref name="parameterName" />, the value set to
      ///    <paramref name="defaultValue" />
      ///    and the buildingBlockType set to <paramref name="buildingBlockType" />
      /// </summary>
      IParameter CreateFor(string parameterName, double defaultValue, PKSimBuildingBlockType buildingBlockType);

      /// <summary>
      ///    Returns a default parameter with the name set to <paramref name="parameterName" /> , the value set to
      ///    <paramref name="defaultValue" />
      ///    ,
      ///    the dimension set to  <paramref name="dimensionName" /> and the buildingBlockType set to
      ///    <paramref name="buildingBlockType" />
      /// </summary>
      /// <exception cref="NotFoundException">
      ///    is thrown if the no dimension can be found in the dimension repositry with the given <paramref name="dimensionName" />
      /// </exception>
      IParameter CreateFor(string parameterName, double defaultValue, string dimensionName, PKSimBuildingBlockType buildingBlockType);

      /// <summary>
      ///    Returns a parameter with a formula rate defined according to <paramref name="parameterRateDefinition" /> and add the formula in the
      ///    <paramref name="formulaCache" />
      ///    if the formula was not added alraedy
      /// </summary>
      IParameter CreateFor(ParameterRateMetaData parameterRateDefinition, IFormulaCache formulaCache);

      /// <summary>
      ///    Returns a parameter with a constant formula defined according to <paramref name="parameterValueDefinition" />
      /// </summary>
      IParameter CreateFor(ParameterValueMetaData parameterValueDefinition);

      /// <summary>
      ///    Returns a distributed parameter with a distributed formula created according to the distributions definition
      ///    <paramref name="distributions" />
      /// </summary>
      IDistributedParameter CreateFor(IEnumerable<ParameterDistributionMetaData> distributions, OriginData originData);

      IDistributedParameter CreateFor(ParameterDistributionMetaData distributionMetaData);

      /// <summary>
      ///    Create a concentration parameter with the concentration formula equal to amount / volume of parent compartment
      ///    The created formula will be added in the <paramref name="formulaCache" />if it does not exist already
      /// </summary>
      IParameter CreateConcentrationParameterIn(IFormulaCache formulaCache);
   }

   public class ParameterFactory : IParameterFactory
   {
      private readonly IPKSimObjectBaseFactory _objectBaseFactory;
      private readonly IFormulaFactory _formulaFactory;
      private readonly IDimensionRepository _dimensionRepository;
      private readonly IDisplayUnitRetriever _displayUnitRetriever;

      public ParameterFactory(IPKSimObjectBaseFactory objectBaseFactory, IFormulaFactory formulaFactory, IDimensionRepository dimensionRepository, IDisplayUnitRetriever displayUnitRetriever )
      {
         _objectBaseFactory = objectBaseFactory;
         _formulaFactory = formulaFactory;
         _dimensionRepository = dimensionRepository;
         _displayUnitRetriever = displayUnitRetriever;
      }

      public IParameter CreateFor(ParameterRateMetaData parameterRateDefinition, IFormulaCache formulaCache)
      {
         var parameter = _objectBaseFactory.CreateParameter();
         parameter.Formula = _formulaFactory.RateFor(parameterRateDefinition, formulaCache);

         if (!string.IsNullOrEmpty(parameterRateDefinition.RHSRate))
            parameter.RHSFormula = _formulaFactory.RHSRateFor(parameterRateDefinition, formulaCache);

         setParameterProperties(parameter, parameterRateDefinition);
         updateDefaultValueFor(parameter);
         synchronizeFormulaDimension(parameter);
         return parameter;
      }

      public IParameter CreateFor(string parameterName, PKSimBuildingBlockType buildingBlockType)
      {
         return CreateFor(parameterName, double.NaN, buildingBlockType);
      }

      public IParameter CreateFor(string parameterName, double defaultValue, PKSimBuildingBlockType buildingBlockType)
      {
         return CreateFor(parameterName, defaultValue, Constants.Dimension.DIMENSIONLESS, buildingBlockType);
      }

      public IParameter CreateFor(string parameterName, double defaultValue, string dimensionName, PKSimBuildingBlockType buildingBlockType)
      {
         var parameterValueDefinition = new ParameterValueMetaData
            {
               ParameterName = parameterName,
               DefaultValue = defaultValue,
               Dimension = dimensionName,
               BuildingBlockType = buildingBlockType,
            };

         return CreateFor(parameterValueDefinition);
      }

      public IParameter CreateFor(ParameterValueMetaData parameterValueDefinition)
      {
         var parameter = _objectBaseFactory.CreateParameter();
         parameter.Formula = _formulaFactory.ValueFor(parameterValueDefinition);
         setParameterProperties(parameter, parameterValueDefinition);
         updateDefaultValueFor(parameter);
         synchronizeFormulaDimension(parameter);
         return parameter;
      }

      public IDistributedParameter CreateFor(IEnumerable<ParameterDistributionMetaData> distributions, OriginData originData)
      {
         var dist = distributions.ToList();
         return create(dist.First(), p => _formulaFactory.DistributionFor(dist, p, originData));
      }

      public IDistributedParameter CreateFor(ParameterDistributionMetaData distributionMetaData)
      {
         return create(distributionMetaData, p => _formulaFactory.DistributionFor(distributionMetaData, p));
      }

      private IDistributedParameter create(ParameterDistributionMetaData distributionMetaData, Func<IDistributedParameter, IDistributionFormula> createFormula)
      {
         var parameter = _objectBaseFactory.CreateDistributedParameter();
         setParameterProperties(parameter, distributionMetaData);
         addParametersToDistributedParameter(parameter, distributionMetaData);
         parameter.Formula = createFormula(parameter);
         updateDefaultValueFor(parameter);
         synchronizeFormulaDimension(parameter);
         return parameter;
      }

      private void updateDefaultValueFor(IParameter parameter)
      {
         if (!parameter.NeedsDefault())
            return;

         parameter.DefaultValue = parameter.Value;
      }

      private void synchronizeFormulaDimension(IParameter parameter)
      {
         if (parameter.Formula == null)
            return;

         parameter.Formula.Dimension = parameter.Dimension;
      }

      private void addParametersToDistributedParameter(IDistributedParameter param, ParameterDistributionMetaData distributionMetaData)
      {
         var percentile = createHiddenParameterBasedOn(Constants.Distribution.PERCENTILE, distributionMetaData)
            .WithDimension(_dimensionRepository.NoDimension);

         percentile.Value = CoreConstants.DEFAULT_PERCENTILE;
         param.Add(percentile);
         if (distributionMetaData.Distribution == DistributionTypes.Normal)
         {
            param.Add(createHiddenParameterBasedOn(Constants.Distribution.MEAN, distributionMetaData));
            param.Add(createHiddenParameterBasedOn(Constants.Distribution.DEVIATION, distributionMetaData));
         }
         else if (distributionMetaData.Distribution == DistributionTypes.LogNormal)
         {
            //log normal=> we need to reset dimension 
            param.Add(createHiddenParameterBasedOn(Constants.Distribution.MEAN, distributionMetaData));
            var gsd = createHiddenParameterBasedOn(Constants.Distribution.GEOMETRIC_DEVIATION, distributionMetaData)
               .WithDimension(_dimensionRepository.NoDimension);
            gsd.Info.MinIsAllowed = true;
            gsd.Info.MinValue = 1;
            gsd.DisplayUnit = _dimensionRepository.NoDimension.DefaultUnit;
            param.Add(gsd);
         }
         else if (distributionMetaData.Distribution == DistributionTypes.Uniform)
         {
            param.Add(createHiddenParameterBasedOn(Constants.Distribution.MINIMUM, distributionMetaData));
            param.Add(createHiddenParameterBasedOn(Constants.Distribution.MAXIMUM, distributionMetaData));
         }
         else if (distributionMetaData.Distribution == DistributionTypes.Discrete)
         {
            param.Add(createHiddenParameterBasedOn(Constants.Distribution.MEAN, distributionMetaData));
         }
         else if (distributionMetaData.Distribution == DistributionTypes.Unknown)
         {
            /*nothing to do*/
         }
         else
         {
            throw new DistributionNotFoundException(distributionMetaData);
         }
         updateRangeValueForMean(param);
      }

      private void updateRangeValueForMean(IDistributedParameter parameter)
      {
         var meanParameter = parameter.MeanParameter;
         if (meanParameter == null) return;
         meanParameter.Info.MinValue = parameter.MinValue;
         meanParameter.Info.MinIsAllowed = parameter.MinIsAllowed;
         meanParameter.Info.MaxValue = parameter.MaxValue;
         meanParameter.Info.MaxIsAllowed = parameter.MaxIsAllowed;
      }

      public IParameter CreateConcentrationParameterIn(IFormulaCache formulaCache)
      {
         var parameter = CreateFor(CoreConstants.Parameters.CONCENTRATION, PKSimBuildingBlockType.Simulation);
         parameter.BuildMode = ParameterBuildMode.Local;
         parameter.Info.Visible = false;
         parameter.Formula = _formulaFactory.ConcentrationFormulaFor(formulaCache);
         parameter.Dimension = _dimensionRepository.MolarConcentration;
         return parameter;
      }

      private void setParameterProperties(IParameter parameter, ParameterMetaData parameterMetaData)
      {
         parameter.Name = parameterMetaData.ParameterName;
         parameter.BuildMode = parameterMetaData.BuildMode;
         parameter.Info = parameterMetaData.Clone();
         parameter.Dimension = _dimensionRepository.DimensionByName(parameterMetaData.Dimension);
         parameter.IsDefault = !parameterMetaData.IsInput;
         parameter.ValueOrigin.UpdateAllFrom(parameterMetaData.ValueOrigin);

         if (!string.IsNullOrEmpty(parameterMetaData.DefaultUnit))
            parameter.DisplayUnit = parameter.Dimension.Unit(parameterMetaData.DefaultUnit);

         parameter.DisplayUnit = _displayUnitRetriever.PreferredUnitFor(parameter);
      }

      private IParameter createHiddenParameterBasedOn(string parameterName, ParameterMetaData parameterMetaData)
      {
         var parameterDefinition = new ParameterValueMetaData
            {
               ParameterName = parameterName,
               CanBeVaried = true,
               CanBeVariedInPopulation = false,
               Visible = false,
               GroupName = parameterMetaData.GroupName,
               BuildingBlockType = parameterMetaData.BuildingBlockType,
               Dimension = parameterMetaData.Dimension,
               DefaultUnit = parameterMetaData.DefaultUnit
            };

         return CreateFor(parameterDefinition);
      }
   }
}