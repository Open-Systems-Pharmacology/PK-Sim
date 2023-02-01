using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Services;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Maths;

namespace PKSim.Core
{
   public abstract class concern_for_StatisticalDataCalculator : ContextSpecification<IStatisticalDataCalculator>
   {
      protected FloatMatrix _floatMatrix;
      protected StatisticalAggregation _selection;
      protected List<float[]> _results;
      protected float[] _values1 = {1, 10, 100};
      protected float[] _values2 = {2, 20, 200};
      protected float[] _values3 = {3, 30, 300};

      protected override void Context()
      {
         sut = new StatisticalDataCalculator();
         _floatMatrix = new FloatMatrix();
         _floatMatrix.AddSortedValues(_values1);
         _floatMatrix.AddSortedValues(_values2);
         _floatMatrix.AddSortedValues(_values3);
      }

      protected override void Because()
      {
         _results = sut.StatisticalDataFor(_floatMatrix, _selection).ToList();
      }
   }

   public class When_calculating_the_statistics_for_a_given_percentile : concern_for_StatisticalDataCalculator
   {
      protected override void Context()
      {
         base.Context();
         _selection = new PercentileStatisticalAggregation {Percentile = 25};
      }

      [Observation]
      public void should_return_the_expected_values()
      {
         _results.Count.ShouldBeEqualTo(1);
         _results[0].ShouldOnlyContainInOrder(new SortedFloatArray(_values1,alreadySorted: true).Percentile(25), new SortedFloatArray(_values2, alreadySorted: true).Percentile(25), new SortedFloatArray(_values3, alreadySorted: true).Percentile(25));
      }
   }

   public abstract class When_calculating_the_statistics_using_a_predefined_method : concern_for_StatisticalDataCalculator
   {
      protected readonly StatisticalAggregationType _statisticalAggregationType;
      protected readonly Func<float[], float> _method;

      protected When_calculating_the_statistics_using_a_predefined_method(StatisticalAggregationType statisticalAggregationType, Func<float[], float> method)
      {
         _statisticalAggregationType = statisticalAggregationType;
         _method = method;
      }

      protected override void Context()
      {
         base.Context();
         _selection = new PredefinedStatisticalAggregation {Method = _statisticalAggregationType};
      }

      [Observation]
      public virtual void should_return_the_expected_values()
      {
         _results[0].ShouldOnlyContainInOrder(_method(_values1), _method(_values2), _method(_values3));
      }
   }

   public class When_calculating_the_statistics_using_the_arithmetical_mean : When_calculating_the_statistics_using_a_predefined_method
   {
      public When_calculating_the_statistics_using_the_arithmetical_mean()
         : base(StatisticalAggregationType.ArithmeticMean, x => x.ArithmeticMean())
      {
      }
   }

   public class When_calculating_the_statistics_using_the_geometrical_mean : When_calculating_the_statistics_using_a_predefined_method
   {
      public When_calculating_the_statistics_using_the_geometrical_mean()
         : base(StatisticalAggregationType.GeometricMean, x => x.GeometricMean())
      {
      }
   }

   public class When_calculating_the_statistics_using_the_min : When_calculating_the_statistics_using_a_predefined_method
   {
      public When_calculating_the_statistics_using_the_min()
         : base(StatisticalAggregationType.Min, x => x.First())
      {
      }
   }

   public class When_calculating_the_statistics_using_the_max : When_calculating_the_statistics_using_a_predefined_method
   {
      public When_calculating_the_statistics_using_the_max()
         : base(StatisticalAggregationType.Max, x => x.Last())
      {
      }
   }

   public class When_calculating_the_statistics_using_the_median : When_calculating_the_statistics_using_a_predefined_method
   {
      public When_calculating_the_statistics_using_the_median()
         : base(StatisticalAggregationType.Median, x => new SortedFloatArray(x, alreadySorted: true).Median())
      {
      }
   }

   public abstract class When_calculating_the_statistics_using_a_predefined_range_method : When_calculating_the_statistics_using_a_predefined_method
   {
      private readonly Func<float[], float> _upperRange;

      protected When_calculating_the_statistics_using_a_predefined_range_method(StatisticalAggregationType statisticalAggregationType, Func<float[], float> lowerRange, Func<float[], float> upperRange)
         : base(statisticalAggregationType, lowerRange)
      {
         _upperRange = upperRange;
      }

      protected override void Context()
      {
         base.Context();
         _selection = new PredefinedStatisticalAggregation {Method = _statisticalAggregationType};
      }

      [Observation]
      public override void should_return_the_expected_values()
      {
         base.should_return_the_expected_values();
         _results[1].ShouldOnlyContainInOrder(_upperRange(_values1), _upperRange(_values2), _upperRange(_values3));
      }
   }

   public class When_calculating_the_statistics_using_the_range_2_5_to_97_5 : When_calculating_the_statistics_using_a_predefined_range_method
   {
      public When_calculating_the_statistics_using_the_range_2_5_to_97_5()
         : base(StatisticalAggregationType.Range95, x => new SortedFloatArray(x, alreadySorted: true).Percentile(2.5), x => new SortedFloatArray(x, alreadySorted: true).Percentile(97.5))
      {
      }
   }

   public class When_calculating_the_statistics_using_the_range_5_to_95 : When_calculating_the_statistics_using_a_predefined_range_method
   {
      public When_calculating_the_statistics_using_the_range_5_to_95()
         : base(StatisticalAggregationType.Range90, x => new SortedFloatArray(x, alreadySorted: true).Percentile(5), x => new SortedFloatArray(x, alreadySorted: true).Percentile(95))
      {
      }
   }

   public class When_calculating_the_statistics_using_the_arithmetical_standard_deviation : When_calculating_the_statistics_using_a_predefined_range_method
   {
      public When_calculating_the_statistics_using_the_arithmetical_standard_deviation()
         : base(StatisticalAggregationType.ArithmeticStandardDeviation, x =>
         {
            var mean = x.ArithmeticMean();
            var std = x.ArithmeticStandardDeviation();
            return mean - std;
         },
            x =>
            {
               var mean = x.ArithmeticMean();
               var std = x.ArithmeticStandardDeviation();
               return mean + std;
            })
      {
      }
   }

   public class When_calculating_the_statistics_using_the_geometrical_standard_deviation : When_calculating_the_statistics_using_a_predefined_range_method
   {
      public When_calculating_the_statistics_using_the_geometrical_standard_deviation()
         : base(StatisticalAggregationType.GeometricStandardDeviation, x =>
         {
            var mean = x.GeometricMean();
            var std = x.GeometricStandardDeviation();
            return mean / std;
         },
            x =>
            {
               var mean = x.GeometricMean();
               var std = x.GeometricStandardDeviation();
               return mean * std;
            })
      {
      }
   }
}