using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Snapshots;
using Individual = PKSim.Core.Snapshots.Individual;
using OriginData = PKSim.Core.Snapshots.OriginData;
using ParameterRange = PKSim.Core.Snapshots.ParameterRange;

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
         var originData = new OriginData
         {
            Species = CoreConstants.Species.HUMAN,
            Population = CoreConstants.Population.ICRP,
         };

         originData.AddCalculationMethods("SurfaceAreaPlsInt_VAR1");

         _settings = new PopulationSettings
         {
            Individual = new Individual
            {
               OriginData = originData
            },
            Age = new ParameterRange
            {
               Min = 0,
               Max = 80,
               Unit = "year(s)",
            },
            Weight = new ParameterRange
            {
               Min = 70,
               Unit = "kg",
            },
            NumberOfIndividuals = 10,
            ProportionOfFemales = 70
         };
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

         var originData = new OriginData
         {
            Species = CoreConstants.Species.HUMAN,
            Population = CoreConstants.Population.PRETERM,
         };

         originData.AddCalculationMethods("SurfaceAreaPlsInt_VAR1");

         _settings = new PopulationSettings
         {
            Individual = new Individual
            {
               OriginData = originData
            },
            Age = new ParameterRange
            {
               Min = 0,
               Max = 80,
               Unit = "year(s)",
            },
            Weight = new ParameterRange
            {
               Min = 70,
               Unit = "kg",
            },
            GestationalAge = new ParameterRange
            {
               Min = 30,
               Max = 35,
               Unit = "week(s)",
            },

            NumberOfIndividuals = 10,
            ProportionOfFemales = 70
         };
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