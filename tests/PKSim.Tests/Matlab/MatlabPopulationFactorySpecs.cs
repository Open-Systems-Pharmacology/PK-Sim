using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using NUnit.Framework;
using PKSim.Core;
using PKSim.Core.Batch;
using PKSim.Core.Model;

namespace PKSim.Matlab
{
   [IntegrationTests]
   [Category("Matlab")]
   public abstract class concern_for_MatlabPopulationFactory : ContextSpecification<IMatlabPopulationFactory>
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         ApplicationStartup.Initialize();
      }

      protected override void Context()
      {
         sut = new MatlabPopulationFactory();
      }
   }

   public class When_creating_a_non_preterm_population_from_matlab : concern_for_MatlabPopulationFactory
   {
      private PopulationSettings _settings;
      private IParameterValueCache _result;

      protected override void Context()
      {
         base.Context();
         _settings = new PopulationSettings
         {
            Species = CoreConstants.Species.Human,
            Population = CoreConstants.Population.ICRP,
            MinAge = 0,
            MaxAge = 80,
            MinWeight = 70,
            NumberOfIndividuals = 10,
            ProportionOfFemales = 70
         };
         _settings.AddCalculationMethod("SurfaceAreaPlsInt", "SurfaceAreaPlsInt_VAR1");
      }

      protected override void Because()
      {
         _result = sut.CreatePopulation(_settings, new List<string>().ToArray());
      }

      [Observation]
      public void should_be_able_to_generate_a_basic_population()
      {
         _result.AllParameterValues.First().Values.Count.ShouldBeEqualTo(_settings.NumberOfIndividuals);
      }
   }

   public class When_creating_a_preterm_population_from_matlab : concern_for_MatlabPopulationFactory
   {
      private PopulationSettings _settings;
      private IParameterValueCache _result;

      protected override void Context()
      {
         base.Context();
         _settings = new PopulationSettings
         {
            Species = CoreConstants.Species.Human,
            Population = CoreConstants.Population.Preterm,
            MinAge = 0,
            MaxAge = 80,
            MinGestationalAge = 30,
            MaxGestationalAge = 35,
            MinWeight = 70,
            NumberOfIndividuals = 10,
            ProportionOfFemales = 70
         };
         _settings.AddCalculationMethod("SurfaceAreaPlsInt", "SurfaceAreaPlsInt_VAR1");
      }

      protected override void Because()
      {
         _result = sut.CreatePopulation(_settings, new List<string>().ToArray());
      }

      [Observation]
      public void should_be_able_to_generate_a_basic_population()
      {
         _result.AllParameterValues.First().Values.Count.ShouldBeEqualTo(_settings.NumberOfIndividuals);
      }
   }
}