using System.Collections.Generic;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;

namespace PKSim.Core.Model
{
   public class RandomPopulationSettings
   {
      private readonly Cache<string, ParameterRange> _parameterRanges = new Cache<string, ParameterRange>(x => x.ParameterName, x => null);
      private readonly Cache<Gender, GenderRatio> _genderRatios = new Cache<Gender, GenderRatio>(x => x.Gender, x=>null);

      public Individual BaseIndividual { get; set; }
      public int NumberOfIndividuals { get; set; }

      public IEnumerable<GenderRatio> GenderRatios => _genderRatios;

      public IEnumerable<ParameterRange> ParameterRanges => _parameterRanges;

      public bool ContainsParameterRangeFor(string parameterName) => _parameterRanges.Contains(parameterName);

      public void AddGenderRatio(GenderRatio genderRatio) => _genderRatios.Add(genderRatio);

      public GenderRatio GenderRatio(Gender gender) => _genderRatios[gender];

      public void AddParameterRange(ParameterRange parameterRange) => _parameterRanges.Add(parameterRange);

      public ParameterRange ParameterRange(string parameterName) => _parameterRanges[parameterName];

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