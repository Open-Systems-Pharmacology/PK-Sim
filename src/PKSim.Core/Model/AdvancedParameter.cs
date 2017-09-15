using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using PKSim.Core.Model.Extensions;
using OSPSuite.Core.Maths.Random;

namespace PKSim.Core.Model
{
   public class RandomValue
   {
      public double Value { get; set; }
      public double Percentile { get; set; }
   }

 public class AdvancedParameter : Container
   {
      private RandomGenerator _randomGenerator;

      /// <summary>
      ///    Path of parameter that is being distributed
      /// </summary>
      public string ParameterPath { get; set; }

      /// <summary>
      ///    Full path of parameter in tree hierarchy. This value will always be set at runtime and does not need to be serialized.
      /// </summary>
      public string FullDisplayName { get; set; }

      private int _seed;

      public AdvancedParameter()
      {
         Seed = Environment.TickCount;
      }

      /// <summary>
      ///    Seed value used to generate the random numbers
      /// </summary>
      public int Seed
      {
         get => _seed;
         set
         {
            _seed = value;
            ResetGenerator();
         }
      }

      /// <summary>
      ///    Generates a new random value based on the distribution type and the defined distribution parameters
      /// </summary>
      public double GenerateRandomValue => DistributedParameter.RandomDeviateIn(_randomGenerator);

      /// <summary>
      ///    Returns the percentile corresponding to the generated value
      /// </summary>
      public double GetPercentileForValue(double value)
      {
         DistributedParameter.Value = value;
         return DistributedParameter.Percentile;
      }

      /// <summary>
      ///    Generates an array of random values. The dimension of the array is equal to numberOfValues
      /// </summary>
      /// <param name="numberOfValues">Number of values to generate</param>
      public virtual IEnumerable<RandomValue> GenerateRandomValues(int numberOfValues)
      {
         ResetGenerator();
         var randomValues = new List<RandomValue>();
         for (int i = 0; i < numberOfValues; i++)
         {
            var randomValue = new RandomValue {Value = GenerateRandomValue};
            randomValue.Percentile = GetPercentileForValue(randomValue.Value);
            randomValues.Add(randomValue);
         }
         return randomValues;
      }

      /// <summary>
      ///    Returns the distribution type of the advanced parameter
      /// </summary>
      public virtual DistributionType DistributionType => DistributedParameter.Formula.DistributionType();

      /// <summary>
      ///    Returns the parameter of the underlying distribution
      /// </summary>
      public IEnumerable<IParameter> AllParameters => DistributedParameter.AllParameters().Where(canBeEdited);

      private bool canBeEdited(IParameter parameter)
      {
         return !string.Equals(parameter.Name, Constants.Distribution.PERCENTILE);
      }

      public override void UpdatePropertiesFrom(IUpdatable sourceObject, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(sourceObject, cloneManager);
         var sourceAdvancedParameter = sourceObject as AdvancedParameter;
         if (sourceAdvancedParameter == null) return;
         Seed = sourceAdvancedParameter.Seed;
         ParameterPath = sourceAdvancedParameter.ParameterPath;
         FullDisplayName = sourceAdvancedParameter.FullDisplayName;
         //No need to update distributed parameter which is saved as a child of the container
      }

      /// <summary>
      ///    The underlying distributed parameter in charge of calculating the distribution values. This is not the parameter
      ///    targeted by the ParameterPath
      /// </summary>
      public virtual IDistributedParameter DistributedParameter
      {
         get { return this.GetSingleChild<IDistributedParameter>(x => true); }
         set
         {
            var distributedParameter = DistributedParameter;
            if (distributedParameter != null)
               RemoveChild(distributedParameter);
            Add(value);
         }
      }
      
      /// <summary>
      ///    Reset the generator with the value of the seed.
      ///    This is necessary if we want to be able to generate the same sequence of random numbers
      /// </summary>
      public void ResetGenerator()
      {
         _randomGenerator = new RandomGenerator(Seed);
      }
   }
}