using System;
using PKSim.Core.Model;
using PKSim.Core.Model.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Maths.Random;

namespace PKSim.Core.Services
{
   public interface IMuSigma
   {
      /// <summary>
      ///    return the mean of the distribution
      /// </summary>
      double Mean { get; set; }

      /// <summary>
      ///    return the deviation of the distributin
      /// </summary>
      double Deviation { get; set; }

      /// <summary>
      ///    return the default value for the distribution
      /// </summary>
      double DefaultValue { get; }

      /// <summary>
      ///    Generate a random value for the distribution around the mean
      /// </summary>
      /// <param name="randomGenerator"> random generator used to generate random values </param>
      double GenerateRandomValueForIndividual(RandomGenerator randomGenerator);

      /// <summary>
      ///    Generate a random value for the distribution around the mean
      /// </summary>
      /// <param name="randomGenerator"> random generator used to generate random values </param>
      double GenerateRandomValueForPopulation(RandomGenerator randomGenerator);

      /// <summary>
      ///    Scale mu and sigma with the given hrel (relative height) and apha (AllometricScaleFactor)
      /// </summary>
      /// <param name="hrel"> Relative height </param>
      /// <param name="alpha"> Allometric Scale Factor </param>
      void ScaleWith(double hrel, double alpha);

      /// <summary>
      ///    return the probability value for the given value
      /// </summary>
      /// <param name="value"> value (e.g. of the volume) </param>
      double ProbabilityFor(double value);
   }

   public static class MuSigma
   {
      public static IMuSigma From(IParameter parameter)
      {
         var distributedParameter = parameter as IDistributedParameter;
         if (distributedParameter != null)
         {
            return new DistributedMuSigma(distributedParameter);
         }

         return new ConstantMuSigma(parameter.Value);
      }
   }

   public class DistributedMuSigma : IMuSigma
   {
      private readonly IDistributedParameter _parameter;
      private readonly IParameter _deviationParameter;
      private readonly IParameter _meanParameter;

      public DistributedMuSigma(IDistributedParameter parameter)
      {
         _parameter = parameter;
         _meanParameter = _parameter.MeanParameter;
         _deviationParameter = _parameter.DeviationParameter;
      }

      public double Mean
      {
         get => _meanParameter.Value;
         set
         {
            if (_meanParameter.Value == value) return;
            _meanParameter.Value = value;
         }
      }

      public double Deviation
      {
         get => _deviationParameter.Value;
         set
         {
            if (_deviationParameter.Value == value) return;
            _deviationParameter.Value = value;
         }
      }

      public DistributionType DistributionType => _parameter.Formula.DistributionType();

      public double DefaultValue => generateValue(0);

      public double GenerateRandomValueForIndividual(RandomGenerator randomGenerator)
      {
         return generateValue(randomGenerator.NextDouble());
      }

      public double GenerateRandomValueForPopulation(RandomGenerator randomGenerator)
      {
         return _parameter.RandomDeviateIn(randomGenerator);
      }

      private double generateValue(double perturbation)
      {
         if (DistributionType == DistributionTypes.Normal)
            return Mean + Deviation * perturbation;

         if (DistributionType == DistributionTypes.LogNormal)
            return Mean * Math.Exp(Math.Log(Deviation) * perturbation);

         return Mean;
      }

      public void ScaleWith(double hrel, double alpha)
      {
         var scale = Math.Pow(hrel, alpha);
         Mean *= scale;
         if (DistributionType == DistributionTypes.Normal)
         {
            Deviation *= scale;
         }
      }

      public double ProbabilityFor(double value)
      {
         return _parameter.ProbabilityDensityFor(value);
      }
   }

   public class ConstantMuSigma : IMuSigma
   {
      private readonly double _value;
      public double Mean { get; set; }
      public double Deviation { get; set; }

      public ConstantMuSigma(double value)
      {
         _value = value;
      }

      public double DefaultValue => _value;

      public double GenerateRandomValueForIndividual(RandomGenerator randomGenerator)
      {
         return _value;
      }

      public double GenerateRandomValueForPopulation(RandomGenerator randomGenerator)
      {
         return _value;
      }

      public void ScaleWith(double hrel, double alpha)
      {
         //nothign to do 
      }

      public double ProbabilityFor(double value)
      {
         return value.EqualsByTolerance(_value, 1e-6) ? 1 : 0.001;
      }
   }
}