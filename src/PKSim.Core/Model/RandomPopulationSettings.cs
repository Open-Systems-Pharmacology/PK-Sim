using System.Collections.Generic;
using PKSim.Assets;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public class RandomPopulationSettings
   {
      private readonly ICache<string, ParameterRange> _parameterRanges = new Cache<string, ParameterRange>(x => x.ParameterName);
      private readonly ICache<Gender, GenderRatio> _genderRatios = new Cache<Gender, GenderRatio>(x => x.Gender);
      public Individual BaseIndividual { get; set; }
      public int NumberOfIndividuals { get; set; }

      public IEnumerable<GenderRatio> GenderRatios => _genderRatios;

      public IEnumerable<ParameterRange> ParameterRanges => _parameterRanges;

      public bool ContainsParameterRangeFor(string parameterName)
      {
         return _parameterRanges.Contains(parameterName);
      }

      public void AddGenderRatio(GenderRatio genderRatio)
      {
         _genderRatios.Add(genderRatio);
      }

      public GenderRatio GenderRatio(Gender gender)
      {
         return _genderRatios[gender];
      }

      public void AddParameterRange(ParameterRange parameterRange)
      {
         _parameterRanges.Add(parameterRange);
      }

      public ParameterRange ParameterRange(string parameterName)
      {
         return _parameterRanges[parameterName];
      }

      public RandomPopulationSettings Clone(ICloneManager cloneManager)
      {
         var clone = new RandomPopulationSettings {BaseIndividual = cloneManager.Clone(BaseIndividual), NumberOfIndividuals = NumberOfIndividuals};
         _genderRatios.Each(gr => clone.AddGenderRatio(gr.Clone()));
         _parameterRanges.Each(pr => clone.AddParameterRange(pr.Clone()));
         return clone;
      }

      public override string ToString()
      {
         return PKSimConstants.UI.PopulationSettings;
      }
   }
}