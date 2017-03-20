using System.Drawing;
using PKSim.Assets;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Validation;
using PKSim.Core;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Core;
using OSPSuite.Core.Chart;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.DTO.PopulationAnalyses
{
   public class BinSizeGroupingDTO : ValidatableDTO
   {
      private int _numberOfBins;
      private string _namingPattern;
      private Color _startColor;
      private Color _endColor;
      private Symbols _symbol;

      public NotifyList<GroupingItemDTO> Labels { get; private set; }
      public LabelGenerationStrategy Strategy { get; set; }

      public BinSizeGroupingDTO()
      {
         NumberOfBins = 3;
         NamingPattern = LabelGenerationOptions.DEFAULT_NAMING_PATTERN;
         Labels = new NotifyList<GroupingItemDTO>();
         Strategy = LabelGenerationStrategies.Numeric;
         Symbol = Symbols.Circle;
         StartColor = PKSimColors.StartGroupingColor;
         EndColor = PKSimColors.EndGroupingColor;
         Rules.AddRange(new[] {atLeastTwoBins, maximumOfBins});
      }

      public int NumberOfBins
      {
         get { return _numberOfBins; }
         set
         {
            _numberOfBins = value;
            OnPropertyChanged(() => NumberOfBins);
         }
      }

      public string NamingPattern
      {
         get { return _namingPattern; }
         set
         {
            _namingPattern = value;
            OnPropertyChanged(() => NamingPattern);
         }
      }

      public virtual Color StartColor
      {
         get { return _startColor; }
         set
         {
            _startColor = value;
            OnPropertyChanged(() => StartColor);
         }
      }

      public virtual Color EndColor
      {
         get { return _endColor; }
         set
         {
            _endColor = value;
            OnPropertyChanged(() => EndColor);
         }
      }

      public virtual Symbols Symbol
      {
         get { return _symbol; }
         set
         {
            _symbol = value;
            OnPropertyChanged(() => Symbol);
         }
      }

      private static IBusinessRule atLeastTwoBins
      {
         get
         {
            return CreateRule.For<BinSizeGroupingDTO>()
               .Property(x => x.NumberOfBins)
               .WithRule((o, bins) => bins >= 2)
               .WithError(PKSimConstants.Rules.AtLeastTwoBinsRequired);
         }
      }

      private static IBusinessRule maximumOfBins
      {
         get
         {
            return CreateRule.For<BinSizeGroupingDTO>()
               .Property(x => x.NumberOfBins)
               .WithRule((o, bins) => bins <= 97)
               .WithError(PKSimConstants.Rules.ExceededMaximumOfBins);
         }
      }
   }
}