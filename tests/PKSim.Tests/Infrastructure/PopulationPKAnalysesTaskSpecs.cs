using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.PKAnalyses;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using ILazyLoadTask = PKSim.Core.Services.ILazyLoadTask;
using IParameterFactory = PKSim.Core.Model.IParameterFactory;
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
      protected IPKValuesToPKAnalysisMapper _pkMapper;
      protected IDimensionRepository _dimensionRepository;
      protected IStatisticalDataCalculator _statisticalDataCalculator;
      protected IRepresentationInfoRepository _representationInfoRepository;
      protected const string _percentileId = "Percentile";
      protected PercentileStatisticalAggregation _percentileStatisticalAggregation;
      protected PopulationStatisticalAnalysis _populationStatisticalAnalysis;
      private PredefinedStatisticalAggregation _rangeAggregation;
      private IParameterFactory _parameterFactory;
      private IProtocolToSchemaItemsMapper _protocolToSchemaItemsMapper;
      private IProtocolFactory _protocolFactory;
      private IGlobalPKAnalysisRunner _globalPKAnalysisRunner;
      private IVSSCalculator _vssCalculator;
      private IInteractionTask _interactionTask;
      private ICloner _cloner;
      private IEntityPathResolver _entityPathResolver;

      protected override void Context()
      {
         _entityPathResolver = A.Fake<IEntityPathResolver>();
         _parameterFactory = A.Fake<IParameterFactory>();
         _protocolToSchemaItemsMapper = A.Fake<IProtocolToSchemaItemsMapper>();
         _protocolFactory = A.Fake<IProtocolFactory>();
         _globalPKAnalysisRunner = A.Fake<IGlobalPKAnalysisRunner>();
         _vssCalculator = A.Fake<IVSSCalculator>();
         _interactionTask = A.Fake<IInteractionTask>();
         _cloner = A.Fake<ICloner>();
         _lazyLoadTask = A.Fake<ILazyLoadTask>();
         _pkValuesCalculator = A.Fake<IPKValuesCalculator>();
         _pkParameterRepository = A.Fake<IPKParameterRepository>();
         _pkCalculationOptionsFactory = A.Fake<IPKCalculationOptionsFactory>();
         _pkMapper = A.Fake<IPKValuesToPKAnalysisMapper>();
         _dimensionRepository = A.Fake<IDimensionRepository>();
         _statisticalDataCalculator = new StatisticalDataCalculator();
         _representationInfoRepository = A.Fake<IRepresentationInfoRepository>();
         _percentileStatisticalAggregation = new PercentileStatisticalAggregation { Selected = true, Percentile = 50 };
         _rangeAggregation = new PredefinedStatisticalAggregation { Selected = true, Method = StatisticalAggregationType.Range90 };
         _populationStatisticalAnalysis = new PopulationStatisticalAnalysis();
         _populationStatisticalAnalysis.AddStatistic(_percentileStatisticalAggregation);
         _populationStatisticalAnalysis.AddStatistic(_rangeAggregation);
         A.CallTo(() => _representationInfoRepository.DisplayNameFor(_percentileStatisticalAggregation)).Returns(_percentileId);
         A.CallTo(() => _representationInfoRepository.DisplayNameFor(_rangeAggregation)).Returns("Range 5% to 95%");
         sut = new PKAnalysesTask(_lazyLoadTask, _pkValuesCalculator, _pkParameterRepository, _pkCalculationOptionsFactory, _pkMapper, _dimensionRepository, 
            _statisticalDataCalculator, _representationInfoRepository, _parameterFactory, _protocolToSchemaItemsMapper, _protocolFactory, _globalPKAnalysisRunner, 
            _vssCalculator, _interactionTask, _cloner, _entityPathResolver);

         _populationSimulation = A.Fake<PopulationSimulation>();
         _outputSelections = new OutputSelections();
         A.CallTo(() => _populationSimulation.OutputSelections).Returns(_outputSelections);
         _allBodyWeights = new List<double>();
         _bodyWeight = A.Fake<IParameter>();
         var bodyWeightPath = "PATH";
         A.CallTo(() => _populationSimulation.BodyWeight).Returns(_bodyWeight);
         A.CallTo(() => _populationSimulation.AllValuesFor(bodyWeightPath)).Returns(_allBodyWeights);
         A.CallTo(() => _populationSimulation.NumberOfItems).Returns(2);
         _individualResult0 = new IndividualResults { IndividualId = 0, Time = new QuantityValues { Values = new[] { 1f, 2f } } };
         _individualResult0.Add(new QuantityValues { QuantityPath = _quantityPath1, Values = new[] { 10f, 20f } });
         _individualResult0.Add(new QuantityValues { QuantityPath = _quantityPath2, Values = new[] { 11f, 21f } });
         _individualResult1 = new IndividualResults { IndividualId = 1, Time = new QuantityValues { Values = new[] { 3f, 4f } } };
         _individualResult1.Add(new QuantityValues { QuantityPath = _quantityPath1, Values = new[] { 30f, 40f } });
         _individualResult1.Add(new QuantityValues { QuantityPath = _quantityPath2, Values = new[] { 31f, 41f } });
         _simulationResults = new SimulationResults { _individualResult0, _individualResult1 };
         _populationSimulation.Results = _simulationResults;



         _pkParameter1 = new PKParameter { Mode = PKParameterMode.Always, Name = "Cmax" };
         _pkParameter2 = new PKParameter { Mode = PKParameterMode.Always, Name = "tMax" };

         A.CallTo(() => _pkParameterRepository.All()).Returns(new[] { _pkParameter1, _pkParameter2 });
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
         _allBodyWeights.AddRange(new[] { 10d, 20d });
         A.CallTo(() => _populationSimulation.Compounds).Returns(new[] { A.Fake<Compound>().WithName("Drug") });
         var baseGrid = new BaseGrid("basegrid", "basegrid", DomainHelperForSpecs.TimeDimensionForSpecs());
         baseGrid.Values = new[] { 0f, 1f };
         var dataColumn = new DataColumn("fabs", "fabs", DomainHelperForSpecs.NoDimension(), baseGrid );
         dataColumn.Values = new[] { 1f, 4f };
         A.CallTo(() => _populationSimulation.FabsOral("Drug")).Returns(dataColumn);
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
         _results.AllPKParametersFor(_quantityPath1).Length.ShouldBeEqualTo(2);
         _results.AllPKParametersFor(_quantityPath2).Length.ShouldBeEqualTo(2);
      }

      [Observation]
      public void should_calculate_the_pk_analyses_using_a_dose_per_body_weight_for_each_individual_and_each_curve()
      {
         // This scaling happens 1*NumberOfResults*NumberOfOutputs + 1*NumberOfResults*NumberOfMolecules
         A.CallTo(() => _pkCalculationOptionsFactory.UpdateTotalDrugMassPerBodyWeight(_populationSimulation, "Drug", A<PKCalculationOptions>._, A<IReadOnlyList<ApplicationParameters>>._)).MustHaveHappened(6, Times.Exactly);
      }
   }

   public class When_calculating_the_population_pk_analyses_for_a_population_simulation_with_less_body_weight_values_than_the_number_of_individual : concern_for_PopulationPKAnalysesTask
   {
      private PopulationSimulationPKAnalyses _results;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _populationSimulation.HasResults).Returns(true);
         _allBodyWeights.AddRange(new[] { 10d });
         _outputSelections.AddOutput(new QuantitySelection(_quantityPath1, QuantityType.Drug));
         A.CallTo(() => _populationSimulation.Compounds).Returns(new[] { A.Fake<Compound>().WithName("Drug") });
      }

      protected override void Because()
      {
         _results = sut.CalculateFor(_populationSimulation);
      }

      [Observation]
      public void should_not_crash()
      {
         _results.AllPKParametersFor(_quantityPath1).Length.ShouldBeEqualTo(2);
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

   public class When_aggregating_pk_parameters_from_individuals : concern_for_PopulationPKAnalysesTask
   {
      private IEnumerable<PopulationPKAnalysis> _pkAnalyses;

      class TestQuantityPKParameter : QuantityPKParameter
      {
         public float[] Values { get; set; }

         public override float[] ValuesAsArray => Values;
      }

      protected override void Context()
      {
         base.Context();

         var model = A.Fake<IModel>();
         A.CallTo(() => model.MoleculeNameFor("Organism|PeripheralVenousBlood|Esomeprazole|Plasma (Peripheral Venous Blood)")).Returns("Esomeprazole");
         A.CallTo(() => model.MoleculeNameFor("Organism|PeripheralVenousBlood|Esomeprazole-2|Plasma (Peripheral Venous Blood)")).Returns("Esomeprazole-2");
         A.CallTo(() => _populationSimulation.Model).Returns(model);
         var esomeprazole = A.Fake<Compound>().WithName("Esomeprazole");
         A.CallTo(() => esomeprazole.MolWeight).Returns(200);
         A.CallTo(() => _populationSimulation.Compounds).Returns(new[] { esomeprazole, A.Fake<Compound>().WithName("Esomeprazole-2") });

         _populationSimulation.AddCompoundPK(new CompoundPK() { CompoundName = "Esomeprazol-2" });
         var analysis = new TimeProfileAnalysisChart();
         analysis.PopulationAnalysis = _populationStatisticalAnalysis;
      }

      protected override void Because()
      {
         var pkParameters = new[]
         {
            new TestQuantityPKParameter() { Name = "Name 1", QuantityPath = "Organism|PeripheralVenousBlood|Esomeprazole|Plasma (Peripheral Venous Blood)",   Values = new[] { 0.000f, 0.050f, 0.025f, 0.075f, 1.000f } },
            new TestQuantityPKParameter() { Name = "Name 2", QuantityPath = "Organism|PeripheralVenousBlood|Esomeprazole|Plasma (Peripheral Venous Blood)",   Values = new[] { 0.00f,  0.25f,  0.75f,  0.50f,  1.00f  } },
            new TestQuantityPKParameter() { Name = "Name 3", QuantityPath = "Organism|PeripheralVenousBlood|Esomeprazole|Plasma (Peripheral Venous Blood)",   Values = new[] { 0.0f,   2.5f,   5.0f,   7.5f,   10.0f  } },
            new TestQuantityPKParameter() { Name = "Name 1", QuantityPath = "Organism|PeripheralVenousBlood|Esomeprazole-2|Plasma (Peripheral Venous Blood)", Values = new[] { 0f,     0f,     0f,     0f,     0f } },
            new TestQuantityPKParameter() { Name = "Name 2", QuantityPath = "Organism|PeripheralVenousBlood|Esomeprazole-2|Plasma (Peripheral Venous Blood)", Values = new[] { 0f,     0f,     0f,     0f,     0f } },
            new TestQuantityPKParameter() { Name = "Name 3", QuantityPath = "Organism|PeripheralVenousBlood|Esomeprazole-2|Plasma (Peripheral Venous Blood)", Values = new[] { 0f,     0f,     0f,     0f,     0f } }
         };

         _pkAnalyses = sut.AggregatePKAnalysis(_populationSimulation, pkParameters, _populationStatisticalAnalysis.SelectedStatistics, "Esomeprazole");
      }

      [Observation]
      public void should_aggregate_correctly()
      {
         _pkAnalyses.ShouldNotBeEmpty();
         A.CallTo(() => _pkMapper.MapFrom(
            200,
            A<PKValues>.That.Matches(p => p.Values.ContainsItem(0.05f) && p.Values.ContainsItem(0.5f) && p.Values.ContainsItem(5f)),
            A<PKParameterMode>.Ignored,
            "Esomeprazole"
         )).MustHaveHappened();
         _pkAnalyses.Count().ShouldBeEqualTo(3);
         var curveData = _pkAnalyses.First().CurveData;
         curveData.QuantityPath.ShouldBeEqualTo("Organism|PeripheralVenousBlood|Esomeprazole|Plasma (Peripheral Venous Blood)");

         _pkAnalyses.Count(x => x.CurveData.Caption.Equals("Esomeprazole-5%")).ShouldBeEqualTo(1);
         _pkAnalyses.Count(x => x.CurveData.Caption.Equals("Esomeprazole-95%")).ShouldBeEqualTo(1);
         _pkAnalyses.Count(x => x.CurveData.Caption.Equals("Esomeprazole-Percentile")).ShouldBeEqualTo(1);
      }
   }
}