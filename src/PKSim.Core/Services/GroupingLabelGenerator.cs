using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using OSPSuite.Core.Extensions;

namespace PKSim.Core.Services
{
   public interface IGroupingLabelGenerator
   {
      /// <summary>
      /// Generates labels using the <paramref name="options"/> and the given <paramref name="intervalLimitsInDisplayUnit"/>. 
      /// </summary>
      /// <param name="options">Option used to generate the labels</param>
      /// <param name="intervalLimitsInDisplayUnit">Limits in display unit. The number of labels generated will be equal to the count -1</param>
      IReadOnlyList<string> GenerateLabels(LabelGenerationOptions options, IReadOnlyList<double> intervalLimitsInDisplayUnit);

      /// <summary>
      /// Generates labels using the settins defined in <paramref name="groupingDefinition"/>. The <paramref name="populationDataCollector"/> and <paramref name="numericField"/>
      /// are used to calculate the min and max of the definiion interval. It is assumes that limits were calculated already for the <paramref name="groupingDefinition"/>
      /// </summary>
      IReadOnlyList<string> GenerateLabels(IPopulationDataCollector populationDataCollector, PopulationAnalysisNumericField numericField, NumberOfBinsGroupingDefinition groupingDefinition);


      /// <summary>
      /// Generates labels using the settins defined in <paramref name="options"/>. The <paramref name="populationDataCollector"/>, <paramref name="numericField"/> and <paramref name="numberOfLabels"/>
      /// are used to calculate the interval limits. 
      /// </summary>
      IReadOnlyList<string> GenerateLabels(IPopulationDataCollector populationDataCollector, PopulationAnalysisNumericField numericField, LabelGenerationOptions options, int numberOfLabels);
   }

   public class GroupingLabelGenerator : IGroupingLabelGenerator
   {
      public IReadOnlyList<string> GenerateLabels(LabelGenerationOptions options, IReadOnlyList<double> intervalLimitsInDisplayUnit)
      {
         //1.. 2.. 3 => 3 limits but two labels
         int numberOfLabels = intervalLimitsInDisplayUnit.Count - 1;
         string pattern = options.Pattern;

         if (!options.IsValid)
            return new string[numberOfLabels].InitializeWith(pattern);

         var nextPattern = getGenerator(options.Strategy);
         var labels = new List<string>();

         //starts at 1 as it makes label generation easier to read for user
         for (int i = 1; i <= numberOfLabels; i++)
         {
            var currentPattern = nextPattern(i);
            labels.Add(pattern.Replace(LabelGenerationOptions.ITERATION_PATTERN, currentPattern));
         }

         updateLabelsWithLimits(labels, intervalLimitsInDisplayUnit, options);

         return labels;
      }

      public IReadOnlyList<string> GenerateLabels(IPopulationDataCollector populationDataCollector, PopulationAnalysisNumericField numericField, NumberOfBinsGroupingDefinition groupingDefinition)
      {
         var limitsInDisplayUnit = groupingDefinition.Limits.Select(v => convertedValue(numericField, v)).ToList();
         var values = numericField.GetValues(populationDataCollector);
         //add min and max to the limits
         limitsInDisplayUnit.Insert(0, convertedValue(numericField, values.Min()));
         limitsInDisplayUnit.Add(convertedValue(numericField, values.Max()));

         return GenerateLabels(new LabelGenerationOptions
         {
            Pattern = groupingDefinition.NamingPattern,
            Strategy = groupingDefinition.Strategy
         }, limitsInDisplayUnit);
      }

      private double convertedValue(PopulationAnalysisNumericField numericField, double valueInBaseUnit)
      {
         return numericField.ValueInDisplayUnit(valueInBaseUnit);
      }

      public IReadOnlyList<string> GenerateLabels(IPopulationDataCollector populationDataCollector, PopulationAnalysisNumericField numericField, LabelGenerationOptions options, int numberOfLabels)
      {
         var groupingDefinition = new NumberOfBinsGroupingDefinition(numericField.Name)
         {
            NumberOfBins = numberOfLabels,
            NamingPattern = options.Pattern,
            Strategy = options.Strategy
         };

         groupingDefinition.CreateLimits(populationDataCollector, numericField);
         return GenerateLabels(populationDataCollector, numericField, groupingDefinition);
      }

      private void updateLabelsWithLimits(List<string> labels, IReadOnlyList<double> limits, LabelGenerationOptions options)
      {
         if (!options.HasIntervalPattern)
            return;

         for (int i = 0; i < labels.Count; i++)
         {
            var label = labels[i];
            label = options.ReplaceStartIntervalIn(label, limits[i]);
            label = options.ReplaceEndIntervalIn(label, limits[i + 1]);
            labels[i] = label;
         }
      }

      private Func<int, string> getGenerator(LabelGenerationStrategy generationStrategy)
      {
         if (generationStrategy == LabelGenerationStrategies.Numeric)
            return i => i.ToString(NumberFormatInfo.InvariantInfo);

         if (generationStrategy == LabelGenerationStrategies.Alpha)
            return toAlpha;

         if (generationStrategy == LabelGenerationStrategies.Roman)
            return romanCharactersFrom;

         throw new Exception("Unknow LabelGenerationStrategy '{0}'".FormatWith(generationStrategy));
      }

      private string romanCharactersFrom(int i)
      {
         return toRoman(i);
      }

      /// <summary>
      ///    This method converts integers to letters
      /// </summary>
      private static string toAlpha(int number)
      {
         const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

         number--; // to start with the A
         var value = string.Empty;

         if (number >= letters.Length)
            value += letters[number / letters.Length - 1];

         value += letters[number % letters.Length];

         return value;
      }

      private static string toRoman(int number)
      {
         //taken from stack overflow http://stackoverflow.com/questions/7040289/converting-integers-to-roman-numerals
         if ((number < 0) || (number > 3999))
            return number.ToString(NumberFormatInfo.InvariantInfo);

         if (number == 0) return string.Empty;
         if (number >= 1000) return "M" + toRoman(number - 1000);
         if (number >= 900) return "CM" + toRoman(number - 900);
         if (number >= 500) return "D" + toRoman(number - 500);
         if (number >= 400) return "CD" + toRoman(number - 400);
         if (number >= 100) return "C" + toRoman(number - 100);
         if (number >= 90) return "XC" + toRoman(number - 90);
         if (number >= 50) return "L" + toRoman(number - 50);
         if (number >= 40) return "XL" + toRoman(number - 40);
         if (number >= 10) return "X" + toRoman(number - 10);
         if (number >= 9) return "IX" + toRoman(number - 9);
         if (number >= 5) return "V" + toRoman(number - 5);
         if (number >= 4) return "IV" + toRoman(number - 4);
         if (number >= 1) return "I" + toRoman(number - 1);

         return number.ToString(NumberFormatInfo.InvariantInfo);
      }
   }
}