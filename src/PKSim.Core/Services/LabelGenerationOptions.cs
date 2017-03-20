using System;
using System.Linq;
using System.Text.RegularExpressions;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Format;
using PKSim.Core.Model.PopulationAnalyses;

namespace PKSim.Core.Services
{
   public class LabelGenerationOptions
   {
      private const string _startPattern = "start";
      private const string _endPattern = "end";
      private const string _numberOfDigitsGroup = "digits";

      public const string ITERATION_PATTERN = "#";
      public static readonly string START_INTERVAL_PATTERN = intervalPattern(_startPattern,"n");
      public static readonly string END_INTERVAL_PATTERN = intervalPattern(_endPattern, "n");
      public static readonly string DEFAULT_NAMING_PATTERN = String.Format("Group_{0} from {1} to {2}", ITERATION_PATTERN, intervalPattern(_startPattern, 2), intervalPattern(_endPattern, 2));
      private readonly INumericFormatterOptions _numericFormatterOptions;
      private readonly NumericFormatter<double> _numericFormatter;
      private readonly Cache<string, Regex> _intervalRegexCache;
      private readonly INumericFormatterOptions _defaultFormatterOptions;

      public string Pattern { get; set; }
      public LabelGenerationStrategy Strategy { get; set; }

      public LabelGenerationOptions()
      {
         _intervalRegexCache = new Cache<string, Regex>
         {
            {_startPattern, new Regex(createIntervalRegexPattern(_startPattern))},
            {_endPattern, new Regex(createIntervalRegexPattern(_endPattern))}
         };

         _numericFormatterOptions = new NumericFormatterOptions();
         _defaultFormatterOptions = NumericFormatterOptions.Instance;
         _numericFormatter = new NumericFormatter<double>(_numericFormatterOptions);
      }

      private string createIntervalRegexPattern(string intervalPattern)
      {
         return "^" + //beginning of phrase
                ".*" + //zero or more characters
                "{" + //beginning of pattern
                intervalPattern +
                "(:(?<" +
                _numberOfDigitsGroup +
                ">\\d))?" + //optional : with a digit (optional) captured as value
                "}" + //end of pattern
                ".*" + //zero or more characters
                "$"; //end of phrase      
      }

      public bool HasIntervalPattern
      {
         get
         {
            return !string.IsNullOrEmpty(Pattern) && _intervalRegexCache.Any(regex => regex.IsMatch(Pattern));
         }
      }

      public bool HasIterationPattern
      {
         get { return !String.IsNullOrEmpty(Pattern) && Pattern.Contains(ITERATION_PATTERN); }
      }

      private static string intervalPattern(string limitName, uint? numberOfDigits = null)
      {
         string digitsPattern = string.Empty;
         if (numberOfDigits.HasValue)
            digitsPattern = numberOfDigits.ToString();

         return intervalPattern(limitName, digitsPattern);
      }

      private static string intervalPattern(string limitName, string numberOfDigits)
      {
         string digitsPattern = string.IsNullOrEmpty(numberOfDigits) ? "" : string.Format(":{0}", numberOfDigits);
         return string.Format("{{{0}{1}}}", limitName, digitsPattern);
      }

      public bool IsValid
      {
         get { return HasIntervalPattern || HasIterationPattern; }
      }

      public string ReplaceStartIntervalIn(string label, double startValue)
      {
         return replaceIntervalValue(label, startValue, _startPattern);
      }

      public string ReplaceEndIntervalIn(string label, double endValue)
      {
         return replaceIntervalValue(label, endValue, _endPattern);
      }

      private string replaceIntervalValue(string label, double value, string intervalPatternLimit)
      {
         var regex = _intervalRegexCache[intervalPatternLimit];
         if (!regex.IsMatch(label))
            return label;

         //default value to use if number was not specified
         _numericFormatterOptions.DecimalPlace = _defaultFormatterOptions.DecimalPlace;
         uint? numberOfDigits = null;
         var group = regex.Match(label).Groups[_numberOfDigitsGroup];
         if (group.Success)
         {
            numberOfDigits = uint.Parse(group.Value);
            _numericFormatterOptions.DecimalPlace = numberOfDigits.Value;
         }

         var intervalPatternToUse = intervalPattern(intervalPatternLimit, numberOfDigits);
         return label.Replace(intervalPatternToUse, _numericFormatter.Format(value));
      }
   }
}