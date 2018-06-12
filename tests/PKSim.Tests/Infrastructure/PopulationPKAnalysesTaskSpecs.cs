using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.PKAnalyses;
using OSPSuite.Core.Domain.Services;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using ILazyLoadTask = PKSim.Core.Services.ILazyLoadTask;
using IPKAnalysesTask = PKSim.Core.Services.IPKAnalysesTask;
using IPKCalculationOptionsFactory = PKSim.Core.Services.IPKCalculationOptionsFactory;
using PKAnalysesTask = PKSim.Core.Services.PKAnalysesTask;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_PopulationPKAnalysesTask : ContextSpecification<IPKAnalysesTask>
   {
      protected ILazyLoadTask _lazyLoadTask;
      protected IPKValuesCalculator _pkValuesCalculator;
      protected IPKParameterRepository _pkParameterRepository;
      protected IPKCalculationOptionsFactory _pkCalculationOptionsFactory;
      private IEntityPathResolver _entityPathResolver;
      protected PopulationSimulation _populationSimulation;
      protected List<double> _allBodyWeights;
      protected IParameter _bodyWeight;
      private SimulationResults _simulationResults;
      private IndividualResults _individualResult0;
      private IndividualResults _individualResult1;
      protected string _quantityPath1 = "QuantityPath1|Drug|Concentration";
      protected string _quantityPath2 = "QuantityPath2|Drug|Concentration";
      protected OutputSelections _outputSelections;
      private PKParameter _pkParameter1;
      private PKParameter _pkParameter2;
      private IPKValuesToPKAnalysisMapper _pkMapper;
      private IDimensionRepository _dimensionRepository;

      protected override void Context()
      {
         _lazyLoadTask = A.Fake<ILazyLoadTask>();
         _pkValuesCalculator = A.Fake<IPKValuesCalculator>();
         _pkParameterRepository = A.Fake<IPKParameterRepository>();
         _pkCalculationOptionsFactory = A.Fake<IPKCalculationOptionsFactory>();
         _entityPathResolver = A.Fake<IEntityPathResolver>();
         _pkMapper= A.Fake<IPKValuesToPKAnalysisMapper>();
         _dimensionRepository= A.Fake<IDimensionRepository>();
         sut = new PKAnalysesTask(_lazyLoadTask, _pkValuesCalculator, _pkParameterRepository, _pkCalculationOptionsFactory, _entityPathResolver,_pkMapper, _dimensionRepository);

         _populationSimulation = A.Fake<PopulationSimulation>();
         _outputSelections = new OutputSelections();
         A.CallTo(() => _populationSimulation.OutputSelections).Returns(_outputSelections);
         _allBodyWeights = new List<double>();
         _bodyWeight = A.Fake<IParameter>();
         var bodyWeightPath = "PATH";
         A.CallTo(() => _populationSimulation.BodyWeight).Returns(_bodyWeight);
         A.CallTo(() => _entityPathResolver.PathFor(_bodyWeight)).Returns(bodyWeightPath);
         A.CallTo(() => _populationSimulation.AllValuesFor(bodyWeightPath)).Returns(_allBodyWeights);
         A.CallTo(() => _populationSimulation.NumberOfItems).Returns(2);
         _individualResult0 = new IndividualResults {IndividualId = 0, Time = new QuantityValues {Values = new[] {1f, 2f}}};
         _individualResult0.Add(new QuantityValues {QuantityPath = _quantityPath1, Values = new[] {10f, 20f}});
         _individualResult0.Add(new QuantityValues {QuantityPath = _quantityPath2, Values = new[] {11f, 21f}});
         _individualResult1 = new IndividualResults {IndividualId = 1, Time = new QuantityValues {Values = new[] {3f, 4f}}};
         _individualResult1.Add(new QuantityValues {QuantityPath = _quantityPath1, Values = new[] {30f, 40f}});
         _individualResult1.Add(new QuantityValues {QuantityPath = _quantityPath2, Values = new[] {31f, 41f}});
         _simulationResults = new SimulationResults {_individualResult0, _individualResult1};
         _populationSimulation.Results = _simulationResults;

         _pkParameter1 = new PKParameter {Mode = PKParameterMode.Always, Name = "Cmax"};
         _pkParameter2 = new PKParameter {Mode = PKParameterMode.Always, Name = "tMax"};

         A.CallTo(() => _pkParameterRepository.All()).Returns(new[] {_pkParameter1, _pkParameter2});
      }
   }

   public class When_calculating_the_population_pk_analyses_for_a_population_simulation : concern_for_PopulationPKAnalysesTask
   {
      private PopulationSimulationPKAnalyses _results;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _populationSimulation.HasResults).Returns(true);
         _outputSelections.AddOutput(new QuantitySelection(_quantityPath1, QuantityType.Drug));
         _outputSelections.AddOutput(new QuantitySelection(_quantityPath2, QuantityType.Drug));
         _allBodyWeights.AddRange(new[] {10d, 20d});
      }

      protected override void Because()
      {
         _results = sut.CalculateFor(_populationSimulation);
      }

      [Observation]
      public void should_load_the_results_for_the_population_simulation()
      {
         _lazyLoadTask.LoadResults(_populationSimulation);
      }

      [Observation]
      public void should_have_created_one_pk_analyses_for_each_output_defined_in_the_simulation()
      {
         _results.AllPKParametersFor(_quantityPath1).Count.ShouldBeEqualTo(2);
         _results.AllPKParametersFor(_quantityPath2).Count.ShouldBeEqualTo(2);
      }

      [Observation]
      public void should_calculate_the_pk_analyses_using_a_dose_per_body_weight_for_each_individual_and_each_curve()
      {
         A.CallTo(() => _pkCalculationOptionsFactory.UpdateAppliedDose(_populationSimulation, "Drug", A<PKCalculationOptions>._, A<IReadOnlyList<PKCalculationOptionsFactory.ApplicationParameters>>._)).MustHaveHappened(Repeated.Exactly.Times(4));
      }

      [Observation]
      public void should_reset_the_body_weight_parameter_at_the_end_of_the_calculation()
      {
         A.CallTo(() => _bodyWeight.ResetToDefault()).MustHaveHappened();
      }
   }

   public class When_calculating_the_population_pk_analyses_for_a_population_simulation_with_less_body_weight_values_than_the_number_of_individual : concern_for_PopulationPKAnalysesTask
   {
      private PopulationSimulationPKAnalyses _results;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _populationSimulation.HasResults).Returns(true);
         _allBodyWeights.AddRange(new[] {10d});
         _outputSelections.AddOutput(new QuantitySelection(_quantityPath1, QuantityType.Drug));
      }

      protected override void Because()
      {
         _results = sut.CalculateFor(_populationSimulation);
      }

      [Observation]
      public void should_not_crash()
      {
         _results.AllPKParametersFor(_quantityPath1).Count.ShouldBeEqualTo(2);
      }
   }

   public class When_calculating_the_population_pk_analyses_for_a_population_simulation_that_has_no_results : concern_for_PopulationPKAnalysesTask
   {
      private PopulationSimulationPKAnalyses _results;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _populationSimulation.HasResults).Returns(false);
      }

      protected override void Because()
      {
         _results = sut.CalculateFor(_populationSimulation);
      }

      [Observation]
      public void should_have_created_one_pk_analyses_for_each_output_defined_in_the_simulation()
      {
         _results.IsNull().ShouldBeTrue();
      }
   }
}