using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility;
using OSPSuite.Utility.Exceptions;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using IFormulaFactory = OSPSuite.Core.Domain.Formulas.IFormulaFactory;

namespace PKSim.Core.Services
{
   public interface IDiseaseStateImplementation : ISpecification<DiseaseState>
   {
      /// <summary>
      ///    Apply the disease state implementation to an individual in the context of a create individual (it will change the
      ///    underlying distributions)
      /// </summary>
      /// <param name="individual">Individual to update</param>
      /// <returns><c>true</c> if the algorithm could be applied otherwise <c>false</c> </returns>
      bool ApplyTo(Individual individual);

      /// <summary>
      ///    Apply the disease state implementation to an individual in the context of a population creation (it will not change
      ///    the underlying distributions or formula)
      /// </summary>
      /// <param name="individual">Individual to update</param>
      /// <returns><c>true</c> if the algorithm could be applied otherwise <c>false</c> </returns>
      bool ApplyForPopulationTo(Individual individual);

      /// <summary>
      ///    Returns an individual that can be used as based when creating a population with disease state
      /// </summary>
      /// <param name="originalIndividual">Original individual selected by the user as based individual</param>
      /// <returns></returns>
      Individual CreateBaseIndividualForPopulation(Individual originalIndividual);

      /// <summary>
      ///    Ensures that some parameters that might have been overwritten by the algorithm are reset (distributions or formula)
      /// </summary>
      void ResetParametersAfterPopulationIteration(Individual individual);

      /// <summary>
      ///    Validates that the parameters are compatible with the underlying disease state (age constraints etc..).
      ///    Throws an exception if the origin data is not valid
      /// </summary>
      void Validate(OriginData originData);

      /// <summary>
      ///    Returns <c>true</c> if the parameters are compatible with th underlying disease state otherwise <c>false</c>
      ///    If the <paramref name="originData" /> is not valid, the return value will contain the reason in error
      /// </summary>
      (bool isValid, string error) IsValid(OriginData originData);

      /// <summary>
      ///    Apply any change required to the disease factor parameter associated with the molecule
      /// </summary>
      void ApplyTo(ExpressionProfile expressionProfile, string moleculeName);

      /// <summary>
      ///    Returns <c>true</c> if the implementation modifies the expression profile otherwise <c>false</c>
      /// </summary>
      bool CanBeAppliedToExpressionProfile(QuantityType moleculeType);
   }

   public abstract class AbstractDiseaseStateImplementation : IDiseaseStateImplementation
   {
      private readonly IIndividualFactory _individualFactory;
      private readonly IContainerTask _containerTask;
      private readonly IParameterSetUpdater _parameterSetUpdater;
      private readonly IFormulaFactory _formulaFactory;
      private readonly IValueOriginRepository _valueOriginRepository;
      private readonly string _name;

      protected enum ParameterUpdateMode
      {
         //value will be use as is
         Value,

         //Value will be used to scale the current value of the parameters
         Factor
      }

      protected class ParameterUpdate
      {
         public IParameter Parameter { get; }
         public double Value { get; }
         public ParameterUpdateMode Mode { get; }

         /// <summary>
         ///    Create a parameter update: Default mode is factor
         /// </summary>
         public ParameterUpdate(IParameter parameter, double value, ParameterUpdateMode mode = ParameterUpdateMode.Factor)
         {
            Parameter = parameter;
            Value = value;
            Mode = mode;
         }

         public double ValueToUse => Mode == ParameterUpdateMode.Value ? Value : Parameter.Value * Value;

         public void UpdateValue()
         {
            Parameter.Value = ValueToUse;
         }
      }

      protected AbstractDiseaseStateImplementation(
         IValueOriginRepository valueOriginRepository,
         IFormulaFactory formulaFactory,
         IIndividualFactory individualFactory,
         IContainerTask containerTask,
         IParameterSetUpdater parameterSetUpdater,
         string name)
      {
         _individualFactory = individualFactory;
         _containerTask = containerTask;
         _parameterSetUpdater = parameterSetUpdater;
         _formulaFactory = formulaFactory;
         _valueOriginRepository = valueOriginRepository;
         _name = name;
      }

      public bool IsSatisfiedBy(DiseaseState diseaseState) => diseaseState.IsNamed(_name);

      public abstract bool ApplyTo(Individual individual);

      public abstract bool ApplyForPopulationTo(Individual individual);

      public Individual CreateBaseIndividualForPopulation(Individual originalIndividual)
      {
         //we need to create a new individual WITHOUT disease state (e.g. HEALTHY) and set all percentiles as in the original individuals. Other parameters, value wil be taken as is
         var originData = originalIndividual.OriginData.Clone();

         //remove the disease state to create a healthy Individual
         originData.DiseaseState = null;
         var healthyIndividual = _individualFactory.CreateAndOptimizeFor(originData, originalIndividual.Seed);

         var allParametersChangedByDiseaseStateImplementation = ParameterChangedByDiseaseStateAsList(healthyIndividual);

         //Make sure we update the flags that might not be set coming from the database
         allParametersChangedByDiseaseStateImplementation.Each(x => x.IsChangedByCreateIndividual = true);

         //do not update parameters changed by the disease state algorithm or that are not visible
         var allHealthyParameters = _containerTask.CacheAllChildrenSatisfying<IParameter>(healthyIndividual, x => !allParametersChangedByDiseaseStateImplementation.Contains(x) && x.Visible);
         var allOriginalParameters = _containerTask.CacheAllChildren<IParameter>(originalIndividual);
         _parameterSetUpdater.UpdateValues(allOriginalParameters, allHealthyParameters);

         //we have a healthy individuals based on the disease state individual where all changes were all manual changes were accounted for
         //we now need to add the disease state contributions from the original individual
         originData.DiseaseState = originalIndividual.OriginData.DiseaseState;
         originalIndividual.OriginData.DiseaseStateParameters.Each(x => originData.AddDiseaseStateParameter(x.Clone()));

         return healthyIndividual;
      }

      public void ResetParametersAfterPopulationIteration(Individual individual)
      {
         //ensures that formula parameters are reset so that they can be reused in next iteration
         var allDiseaseStateParameters = ParameterChangedByDiseaseStateAsList(individual).Where(x => x.IsFixedValue);
         allDiseaseStateParameters.Each(x => x.ResetToDefault());
      }

      public void Validate(OriginData originData)
      {
         var (valid, error) = IsValid(originData);
         if (valid)
            return;

         throw new OSPSuiteException(error);
      }

      public abstract (bool isValid, string error) IsValid(OriginData originData);

      public virtual void ApplyTo(ExpressionProfile expressionProfile, string moleculeName)
      {
         //Override in specific implementation if needed
      }

      public virtual bool CanBeAppliedToExpressionProfile(QuantityType moleculeType) => false;

      protected abstract IReadOnlyList<IParameter> ParameterChangedByDiseaseStateAsList(Individual individual);

      protected void UpdateParameterValue(ParameterUpdate parameterUpdate) => parameterUpdate.UpdateValue();

      protected Action<ParameterUpdate> UpdateParameter(int valueOriginId) => (parameterUpdate) =>
      {
         var parameter = parameterUpdate.Parameter;
         var valueToUse = parameterUpdate.ValueToUse;
         UpdateValueOriginsFor(parameter, valueOriginId);
         if (parameter is IDistributedParameter distributedParameter)
         {
            distributedParameter.ScaleDistributionBasedOn(valueToUse / distributedParameter.Value);
            return;
         }

         //We are using a formula, we override with a constant
         if (parameter.Formula.IsExplicit())
         {
            parameter.Formula = _formulaFactory.ConstantFormula(valueToUse, parameter.Dimension);
            //Make sure the formula is indeed used in case the value was overwritten before as fixed value
            parameter.IsFixedValue = false;
            return;
         }

         //constant formula
         UpdateParameterValue(parameterUpdate);
         parameter.DefaultValue = valueToUse;
         parameter.IsFixedValue = false;
      };

      protected void UpdateValueOriginsFor(IParameter parameter, int valueOriginId)
      {
         parameter.ValueOrigin.UpdateAllFrom(_valueOriginRepository.FindBy(valueOriginId));
         //Make sure we mark this parameter as changed by create individual. It might already be the case but in that case, it does not change anything
         parameter.IsChangedByCreateIndividual = true;
      }
   }

   public class HealthyDiseaseStateImplementation : IDiseaseStateImplementation
   {
      public bool ApplyTo(Individual individual)
      {
         //nothing to do here
         return true;
      }

      public bool ApplyForPopulationTo(Individual individual) => ApplyTo(individual);

      public Individual CreateBaseIndividualForPopulation(Individual originalIndividual)
      {
         //for healthy population, we use this individual as is
         return originalIndividual;
      }

      public void ResetParametersAfterPopulationIteration(Individual individual)
      {
         //nothing to do here
      }

      public void Validate(OriginData originData)
      {
         //nothing to do here
      }

      public (bool isValid, string error) IsValid(OriginData originData)
      {
         return (true, string.Empty);
      }

      public void ApplyTo(ExpressionProfile expressionProfile, string moleculeName)
      {
         //nothing to do here
      }

      public bool CanBeAppliedToExpressionProfile(QuantityType moleculeType)
      {
         return false;
      }

      public bool IsSatisfiedBy(DiseaseState item) => false;
   }
}