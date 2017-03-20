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

   public interface IAdvancedParameter : IEntity
   {
      /// <summary>
      ///    Path of parameter that is being distributed
      /// </summary>
      string ParameterPath { get; set; }

      /// <summary>
      ///    Seed value used to generate the random numbers
      /// </summary>
      int Seed { get; set; }

      /// <summary>
      ///    Generates a new random value based on the distribution type and the defined distribution parameters
      /// </summary>
      double GenerateRandomValue { get; }

      /// <summary>
      ///    Returns the percentile corresponding to the generated value
      /// </summary>
      double GetPercentileForValue(double value);

      /// <summary>
      ///    Generates an array of random values. The dimension of the array is equal to numberOfValues
      /// </summary>
      /// <param name="numberOfValues">Number of values to generate</param>
      IEnumerable<RandomValue> GenerateRandomValues(int numberOfValues);

      /// <summary>
      ///    Returns the distribution type of the advanced parameter
      /// </summary>
      DistributionType DistributionType { get; }

      /// <summary>
      ///    Returns the parameter of the underlying distribution
      /// </summary>
      IEnumerable<IParameter> AllParameters { get; }

      /// <summary>
      ///    The underlying distributed parameter in charge of calculating the distribution values. This is not the parameter
      ///    targeted by the ParameterPath
      /// </summary>
      IDistributedParameter DistributedParameter { get; set; }

      /// <summary>
      ///    Reset the genrator with the value of the seed.
      ///    This is necessary if we want to be able to generate the same sequence of random numbers
      /// </summary>
      void ResetGenerator();

      /// <summary>
      ///    Full path of parameter in tree hierarchy. This value will always be set at runtime and does not need to be serialized.
      /// </summary>
      string FullDisplayName { get; set; }
   }

   public class AdvancedParameter : Container, IAdvancedParameter
   {
      private RandomGenerator _randomGenerator;
      public string ParameterPath { get; set; }
      public string FullDisplayName { get; set; }
      private int _seed;

      public AdvancedParameter()
      {
         Seed = Environment.TickCount;
      }

      public int Seed
      {
         get { return _seed; }
         set
         {
            _seed = value;
            ResetGenerator();
         }
      }

      public double GenerateRandomValue
      {
         get { return DistributedParameter.RandomDeviateIn(_randomGenerator); }
      }

      public double GetPercentileForValue(double value)
      {
         DistributedParameter.Value = value;
         return DistributedParameter.Percentile;
      }

      public IEnumerable<RandomValue> GenerateRandomValues(int numberOfValues)
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

      public DistributionType DistributionType
      {
         get { return DistributedParameter.Formula.DistributionType(); }
      }

      public IEnumerable<IParameter> AllParameters
      {
         get { return DistributedParameter.AllParameters().Where(canBeEdited); }
      }

      private bool canBeEdited(IParameter parameter)
      {
         return !string.Equals(parameter.Name, Constants.Distribution.PERCENTILE);
      }

      public override void UpdatePropertiesFrom(IUpdatable sourceObject, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(sourceObject, cloneManager);
         var sourceAdvancedParameter = sourceObject as IAdvancedParameter;
         if (sourceAdvancedParameter == null) return;
         Seed = sourceAdvancedParameter.Seed;
         ParameterPath = sourceAdvancedParameter.ParameterPath;
         FullDisplayName = sourceAdvancedParameter.FullDisplayName;
         //No need to update distributed parameter which is saved as a child of the container
      }

      public IDistributedParameter DistributedParameter
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

      public void ResetGenerator()
      {
         _randomGenerator = new RandomGenerator(Seed);
      }
   }
}