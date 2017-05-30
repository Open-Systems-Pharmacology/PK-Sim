using System;
using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model.Extensions;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public interface IFormulaFactory
   {
      /// <summary>
      ///    Creates and returns a formula based on the <paramref name="rateDefinition" /> and add it to the
      ///    <paramref
      ///       name="formulaCache" />
      ///    If a formula with the same combo "Rate-CalculationMethods" already exists in the formulaCache, the formula will be
      ///    returned instead
      /// </summary>
      IFormula RateFor(ParameterRateMetaData rateDefinition, IFormulaCache formulaCache);

      /// <summary>
      ///    Returns the concentration formula for a parameter defined in a molecule amount. if the formula was already defined
      ///    in the formula cache, simply returns the same reference
      ///    <remarks>
      ///       This cannot be defined in the database so far, since the structure does not allow parameter paths referenciation
      ///       as needed (..)
      ///    </remarks>
      /// </summary>
      IFormula ConcentrationFormulaFor(IFormulaCache formulaCache);

      /// <summary>
      ///    Create and returns a distribution formula based on the given <paramref name="distributions" /> .
      /// </summary>
      IDistributionFormula DistributionFor(IEnumerable<ParameterDistributionMetaData> distributions, IDistributedParameter parameterWithDistribution, OriginData originData);

      /// <summary>
      ///    Create and returns a distribution formula based on the given <paramref name="distributionMetaData" /> .
      /// </summary>
      IDistributionFormula DistributionFor(ParameterDistributionMetaData distributionMetaData, IDistributedParameter parameterWithDistribution);

      /// <summary>
      ///    Creates and returns a constant formula based on the <paramref name="valueDefinition" />
      /// </summary>
      IFormula ValueFor(ParameterValueMetaData valueDefinition);

      /// <summary>
      ///    Return the formula defined for the rate key and add it to the formula cache, if the formula did not exist already
      /// </summary>
      IFormula RateFor(RateKey rateKey, IFormulaCache formulaCache);

      /// <summary>
      ///    creates and returns a table formula
      /// </summary>
      TableFormula CreateTableFormula();

      /// <summary>
      ///    creates and returns a distributed table formula
      /// </summary>
      DistributedTableFormula CreateDistributedTableFormula();

      /// <summary>
      ///    Return the formula defined for the combination calculation method - rate and add it to the formula cache, if the
      ///    formula did not exist already
      /// </summary>
      IFormula RateFor(string calculationMethod, string rate, IFormulaCache formulaCache);

      IFormula RateFor(IWithFormula objectWithFormula, IFormulaCache formulaCache);

      /// <summary>
      ///    Creates and returns RHS formula based on the <paramref name="parameterRateDefinition" /> and add it to the
      ///    <paramref
      ///       name="formulaCache" />
      /// </summary>
      IFormula RHSRateFor(ParameterRateMetaData parameterRateDefinition, IFormulaCache formulaCache);

      /// <summary>
      ///    Creates a formula for BMI referencing the two parameters weight and height
      /// </summary>
      IFormula BMIFormulaFor(IParameter weightParameter, IParameter heightParameter);

      /// <summary>
      ///    Creates a formula for age referencing the time in the simulation. This is required for aging simulation.
      /// </summary>
      /// <param name="age0Parameter">
      ///    Default age parameter (typically with a constant value that will be used to create the
      ///    formula)
      /// </param>
      /// <param name="minToYearFactorParameter">Factor to convert time in min to year</param>
      IFormula AgeFormulaFor(IParameter age0Parameter, IParameter minToYearFactorParameter);
   }

   public class FormulaFactory : IFormulaFactory
   {
      private readonly IObjectBaseFactory _objectBaseFactory;
      private readonly IRateObjectPathsRepository _rateObjectPathsRepository;
      private readonly IRateFormulaRepository _rateFormulaRepository;
      private readonly IDistributionFormulaFactory _distributionFactory;
      private readonly IObjectPathFactory _objectPathFactory;
      private readonly IDimensionRepository _dimensionRepository;
      private readonly IIdGenerator _idGenerator;
      private readonly IDynamicFormulaCriteriaRepository _dynamicFormulaCriteriaRepository;

      public FormulaFactory(IObjectBaseFactory objectBaseFactory, IRateObjectPathsRepository rateObjectPathsRepository,
         IRateFormulaRepository rateFormulaRepository, IDistributionFormulaFactory distributionFactory, IObjectPathFactory objectPathFactory,
         IDimensionRepository dimensionRepository, IIdGenerator idGenerator,
         IDynamicFormulaCriteriaRepository dynamicFormulaCriteriaRepository)
      {
         _objectBaseFactory = objectBaseFactory;
         _rateObjectPathsRepository = rateObjectPathsRepository;
         _rateFormulaRepository = rateFormulaRepository;
         _distributionFactory = distributionFactory;
         _objectPathFactory = objectPathFactory;
         _dimensionRepository = dimensionRepository;
         _idGenerator = idGenerator;
         _dynamicFormulaCriteriaRepository = dynamicFormulaCriteriaRepository;
      }

      public IFormula RateFor(ParameterRateMetaData rateDefinition, IFormulaCache formulaCache)
      {
         return RateFor(rateDefinition.CalculationMethod, rateDefinition.Rate, formulaCache);
      }

      public IFormula BMIFormulaFor(IParameter weightParameter, IParameter heightParameter)
      {
         const string height = "Height";
         const string weight = "BW";

         var formula = _objectBaseFactory.Create<ExplicitFormula>()
            .WithName(CoreConstants.Parameter.BMI)
            .WithFormulaString($"{height}>0 ? {weight} / ({height})^2 : 0")
            .WithDimension(_dimensionRepository.BMI);

         formula.AddObjectPath(pathInParentContainerFor(weightParameter, weight));
         formula.AddObjectPath(pathInParentContainerFor(heightParameter, height));

         return formula;
      }

      public IFormula AgeFormulaFor(IParameter age0Parameter, IParameter minToYearFactorParameter)
      {
         const string age0 = "Age_0";
         const string minToYearFactor = "minToYearFactor";

         //Age is in year but time is in min
         var formula = _objectBaseFactory.Create<ExplicitFormula>()
            .WithName(CoreConstants.Parameter.AGE)
            .WithFormulaString($"{age0} + {Constants.TIME} * {minToYearFactor}")
            .WithDimension(_dimensionRepository.AgeInYears);

         formula.AddObjectPath(pathInParentContainerFor(age0Parameter, age0));
         formula.AddObjectPath(pathInParentContainerFor(minToYearFactorParameter, minToYearFactor));

         addTimeReferenceIfNeeded(formula);

         return formula;
      }

      private IFormulaUsablePath pathInParentContainerFor(IParameter parameter, string alias)
      {
         return _objectPathFactory.CreateFormulaUsablePathFrom(ObjectPath.PARENT_CONTAINER, parameter.Name)
            .WithAlias(alias)
            .WithDimension(parameter.Dimension);
      }

      public IFormula ConcentrationFormulaFor(IFormulaCache formulaCache)
      {
         if (formulaCache.Contains(CoreConstants.Formula.Concentration))
            return formulaCache[CoreConstants.Formula.Concentration];

         var formula = _objectBaseFactory.Create<ExplicitFormula>()
            .WithId(CoreConstants.Formula.Concentration)
            .WithName(CoreConstants.Formula.Concentration)
            .WithFormulaString("V>0 ? M/V : 0");

         formula.AddObjectPath(_objectPathFactory.CreateFormulaUsablePathFrom(ObjectPath.PARENT_CONTAINER).WithAlias("M").WithDimension(_dimensionRepository.Amount));
         formula.AddObjectPath(_objectPathFactory.CreateFormulaUsablePathFrom(ObjectPath.PARENT_CONTAINER, ObjectPath.PARENT_CONTAINER, CoreConstants.Parameter.VOLUME).WithAlias("V").WithDimension(_dimensionRepository.Volume));

         formulaCache.Add(formula);
         formula.Dimension = _dimensionRepository.MolarConcentration;
         return formula;
      }

      public IFormula RateFor(RateKey rateKey, IFormulaCache formulaCache)
      {
         if (rateKey == null)
            return null;

         //no formula cache defined. Formula should be unique by id
         if (formulaCache == null)
            return createFormula(rateKey).WithId(_idGenerator.NewId());

         if (!formulaCache.Contains(rateKey))
         {
            var formula = createFormula(rateKey);
            if (formula.IsConstant())
               return formula;

            formulaCache.Add(formula);
         }

         return formulaCache[rateKey];
      }

      public TableFormula CreateTableFormula()
      {
         var tableFormula = _objectBaseFactory.Create<TableFormula>().WithName(CoreConstants.Formula.TableFormula);
         setupDefaultFor(tableFormula);
         return tableFormula;
      }

      private void setupDefaultFor(TableFormula tableFormula)
      {
         tableFormula.XDimension = _dimensionRepository.Time;
         tableFormula.XName = Constants.TIME;
         tableFormula.Dimension = _dimensionRepository.Fraction;
      }

      public DistributedTableFormula CreateDistributedTableFormula()
      {
         var tableFormula = _objectBaseFactory.Create<DistributedTableFormula>().WithName(CoreConstants.Formula.TableFormula);
         setupDefaultFor(tableFormula);
         return tableFormula;
      }

      public IFormula RateFor(string calculationMethod, string rate, IFormulaCache formulaCache)
      {
         return RateFor(new RateKey(calculationMethod, rate), formulaCache);
      }

      public IFormula RateFor(IWithFormula objectWithFormula, IFormulaCache formulaCache)
      {
         return RateFor(objectWithFormula.CalculationMethod, objectWithFormula.Rate, formulaCache);
      }

      public IFormula RHSRateFor(ParameterRateMetaData parameterRateDefinition, IFormulaCache formulaCache)
      {
         if (string.IsNullOrEmpty(parameterRateDefinition.RHSRate))
            return null;

         return RateFor(parameterRateDefinition.CalculationMethod, parameterRateDefinition.RHSRate, formulaCache);
      }

      private IFormula createFormula(RateKey rateKey)
      {
         if (rateKey.IsBlackBoxFormula)
         {
            return _objectBaseFactory.Create<BlackBoxFormula>()
               .WithId(rateKey)
               .WithName(rateKey.Rate);
         }

         if (rateKey.IsTableWithOffsetFormula)
            return createTableFormulaWithOffset(rateKey);

         //now it can be either dynamic formula or explicit formula
         FormulaWithFormulaString formula;

         if (rateKey.IsDynamicSumFormula)
         {
            var sumFormula = _objectBaseFactory.Create<SumFormula>();
            sumFormula.Criteria = _dynamicFormulaCriteriaRepository.CriteriaFor(rateKey).Clone();
            formula = sumFormula;
         }
         else
         {
            formula = _objectBaseFactory.Create<ExplicitFormula>();
         }

         formula.WithFormulaString(_rateFormulaRepository.FormulaFor(rateKey))
            .WithId(rateKey)
            .WithName(rateKey.Rate);

         foreach (var rateObjectPath in _rateObjectPathsRepository.ObjectPathsFor(rateKey))
         {
            formula.AddObjectPath(rateObjectPath.Clone<IFormulaUsablePath>());
         }

         addTimeReferenceIfNeeded(formula);

         //set formula dimension if available
         var dimensionName = _rateFormulaRepository.DimensionNameFor(rateKey);
         var dimension = _dimensionRepository.DimensionByName(dimensionName);
         formula.Dimension = dimension;

         if (formula.ObjectPaths.Any() || formula.IsAnImplementationOf<DynamicFormula>())
            return formula;

         //this is actually a constant formula! Evaluate the function and return a contant formula
         return constantFormula(formula.Calculate(null)).WithDimension(dimension);
      }

      private IFormula createTableFormulaWithOffset(RateKey rateKey)
      {
         var formula = _objectBaseFactory.Create<TableFormulaWithOffset>()
            .WithId(rateKey)
            .WithName(rateKey.Rate);

         var tableObjectPath = _rateObjectPathsRepository.PathWithAlias(rateKey, CoreConstants.Alias.TABLE);
         var offsetObjectPath = _rateObjectPathsRepository.PathWithAlias(rateKey, CoreConstants.Alias.OFFSET);

         if ((tableObjectPath == null) || (offsetObjectPath == null))
            throw new ArgumentException(
               PKSimConstants.Error.TableFormulaWithOffsetMissingRefs(rateKey.ToString(), CoreConstants.Alias.TABLE, CoreConstants.Alias.OFFSET));

         formula.AddTableObjectPath(tableObjectPath.Clone<IFormulaUsablePath>());
         formula.AddOffsetObjectPath(offsetObjectPath.Clone<IFormulaUsablePath>());

         //Table formula with offest has the same dimension as its referenced table object
         formula.Dimension = tableObjectPath.Dimension;

         return formula;
      }

      /// <summary>
      ///    Checks if formula equation is time dependent and add reference to
      ///    <para></para>
      ///    Time (if not already done)
      /// </summary>
      private void addTimeReferenceIfNeeded(FormulaWithFormulaString formula)
      {
         if (string.IsNullOrEmpty(formula.FormulaString))
            return;

         // check if formula string contains "Time" as a substring
         if (!formula.FormulaString.Contains(Constants.TIME))
            return;

         // check if time reference is already created
         if (formula.ContainsTimePath())
            return;

         // add time reference
         formula.AddObjectPath(_objectPathFactory.CreateTimePath(_dimensionRepository.Time));
      }

      public IDistributionFormula DistributionFor(ParameterDistributionMetaData distributionMetaData, IDistributedParameter parameterWithDistribution)
      {
         return _distributionFactory.CreateFor(distributionMetaData, parameterWithDistribution);
      }

      public IFormula ValueFor(ParameterValueMetaData valueDefinition)
      {
         return constantFormula(valueDefinition);
      }

      public IDistributionFormula DistributionFor(IEnumerable<ParameterDistributionMetaData> distributions, IDistributedParameter parameterWithDistribution, OriginData originData)
      {
         return _distributionFactory.CreateFor(distributions, parameterWithDistribution, originData);
      }

      private IFormula constantFormula(ParameterValueMetaData parameterInfo)
      {
         return constantFormula(parameterInfo.DefaultValue);
      }

      private IFormula constantFormula(double defaultValue)
      {
         var constantFormula = _objectBaseFactory.Create<ConstantFormula>();
         constantFormula.Value = defaultValue;
         return constantFormula;
      }
   }
}