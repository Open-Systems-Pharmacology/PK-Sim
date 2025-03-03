using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.PKAnalyses;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Chart;
using PKSim.Core.Extensions;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using ILazyLoadTask = PKSim.Core.Services.ILazyLoadTask;
using IParameterFactory = PKSim.Core.Model.IParameterFactory;
using IPKCalculationOptionsFactory = PKSim.Core.Services.IPKCalculationOptionsFactory;
using PKAnalysesTask = PKSim.Core.Services.PKAnalysesTask;

namespace PKSim.Core
{
   public abstract class concern_for_PKAnalysesTask : ContextSpecification<PKAnalysesTask>
   {
      protected IPKValuesCalculator _pkCalculator;
      protected IPKValuesToPKAnalysisMapper _pkMapper;
      private IDimensionRepository _dimensionRepository;
      private IPKCalculationOptionsFactory _pkCalculationOptionsFactory;
      private IPKParameterRepository _pkParameterRepository;
      private ILazyLoadTask _lazyLoadTask;
      protected IStatisticalDataCalculator _statisticalDataCalculator;
      protected IRepresentationInfoRepository _representationInfoRepository;
      protected IVSSCalculator _vssCalculator;
      protected IParameterFactory _parameterFactory;
      protected IProtocolToSchemaItemsMapper _protocolMapper;
      protected IProtocolFactory _protocolFactory;
      protected IGlobalPKAnalysisRunner _globalPKAnalysisRunner;
      protected IInteractionTask _interactionTask;
      protected ICloner _cloner;
      private IEntityPathResolver _entityPathResolver;

      protected override void Context()
      {
         _lazyLoadTask = A.Fake<ILazyLoadTask>();
         _pkCalculator = A.Fake<IPKValuesCalculator>();
         _pkMapper = A.Fake<IPKValuesToPKAnalysisMapper>();
         _dimensionRepository = A.Fake<IDimensionRepository>();
         _pkCalculationOptionsFactory = A.Fake<IPKCalculationOptionsFactory>();
         _pkParameterRepository = A.Fake<IPKParameterRepository>();
         _statisticalDataCalculator = new StatisticalDataCalculator();
         _representationInfoRepository = A.Fake<IRepresentationInfoRepository>();
         _entityPathResolver = A.Fake<IEntityPathResolver>();

         _vssCalculator = A.Fake<IVSSCalculator>();
         _parameterFactory = A.Fake<IParameterFactory>();
         _protocolMapper = A.Fake<IProtocolToSchemaItemsMapper>();
         _protocolFactory = A.Fake<IProtocolFactory>();
         _globalPKAnalysisRunner = A.Fake<IGlobalPKAnalysisRunner>();
         _interactionTask = A.Fake<IInteractionTask>();
         _cloner = A.Fake<ICloner>();

         sut = new PKAnalysesTask(_lazyLoadTask, _pkCalculator, _pkParameterRepository, _pkCalculationOptionsFactory, _pkMapper, _dimensionRepository,
            _statisticalDataCalculator, _representationInfoRepository, _parameterFactory, _protocolMapper, _protocolFactory, _globalPKAnalysisRunner,
            _vssCalculator, _interactionTask, _cloner, _entityPathResolver);
      }
   }

   public class for_global_pk_analyses : concern_for_PKAnalysesTask
   {
      protected DataColumn _peripheralVenousBloodPlasma;
      protected DataColumn _venousBloodPlasma;

      protected CompoundProperties _compoundProperties;
      protected Individual _individual;
      protected const string _compoundName = "DRUG";
      protected PKValues _venousBloodPK;
      protected PKValues _peripheralVenousBloodPK;
      protected Species _species;
      protected SimpleProtocol _protocol;
      protected Compound _compound;
      protected EventGroup _eventGroup;
      protected Container _application1;
      protected Container _application2;

      protected override void Context()
      {
         base.Context();
         var baseGrid = new BaseGrid("time", A.Fake<IDimension>());
         baseGrid.Values = new[] {0f, 1f, 2f};

         _peripheralVenousBloodPlasma = CalculationColumnFor(baseGrid, CoreConstants.Organ.PERIPHERAL_VENOUS_BLOOD, CoreConstants.Observer.PLASMA_PERIPHERAL_VENOUS_BLOOD, CoreConstants.Observer.PLASMA_PERIPHERAL_VENOUS_BLOOD, _compoundName);
         _venousBloodPlasma = CalculationColumnFor(baseGrid, CoreConstants.Organ.VENOUS_BLOOD, CoreConstants.Compartment.PLASMA, CoreConstants.Observer.CONCENTRATION_IN_CONTAINER, _compoundName);
         _individual = A.Fake<Individual>();
         _species = new Species();
         A.CallTo(() => _individual.Species).Returns(_species);

         _compound = new Compound().WithName(_compoundName);
         _compoundProperties = new CompoundProperties {Compound = _compound};
         _protocol = new SimpleProtocol();
         _compoundProperties.ProtocolProperties.Protocol = _protocol;


         _eventGroup = new EventGroup();
         _application1 = new Container().WithName("App1").WithContainerType(ContainerType.Application);
         _application1.Add(new MoleculeAmount().WithName(_compoundName));
         _application1.Add(DomainHelperForSpecs.ConstantParameterWithValue(10).WithName(Constants.Parameters.START_TIME));
         _application2 = new Container().WithName("App2").WithContainerType(ContainerType.Application);
         _application2.Add(DomainHelperForSpecs.ConstantParameterWithValue(10).WithName(Constants.Parameters.START_TIME));
         _application2.Add(new MoleculeAmount().WithName(_compoundName));

         _eventGroup.Add(_application1);
         _venousBloodPK = new PKValues();
         _venousBloodPK.AddValue(Constants.PKParameters.Vss, 10);
         _venousBloodPK.AddValue(Constants.PKParameters.Vd, 11);
         _venousBloodPK.AddValue(Constants.PKParameters.CL, 12);

         _venousBloodPK.AddValue(Constants.PKParameters.AUC_inf, 13);
         _venousBloodPK.AddValue(Constants.PKParameters.AUC_inf_tD1_norm, 14);

         _peripheralVenousBloodPK = new PKValues();
         _peripheralVenousBloodPK.AddValue(Constants.PKParameters.Vss, 21);
         _peripheralVenousBloodPK.AddValue(Constants.PKParameters.Vd, 22);
         _peripheralVenousBloodPK.AddValue(Constants.PKParameters.CL, 23);
         _peripheralVenousBloodPK.AddValue(Constants.PKParameters.AUC_inf, 24);
         _peripheralVenousBloodPK.AddValue(Constants.PKParameters.AUC_inf_tD1_norm, 25);
         _peripheralVenousBloodPK.AddValue(Constants.PKParameters.C_max, 26);
         _peripheralVenousBloodPK.AddValue(Constants.PKParameters.C_max_tDLast_tDEnd, 27);


         A.CallTo(() => _pkCalculator.CalculatePK(_venousBloodPlasma, A<PKCalculationOptions>._, null)).Returns(_venousBloodPK);
         A.CallTo(() => _pkCalculator.CalculatePK(_peripheralVenousBloodPlasma, A<PKCalculationOptions>._, null)).Returns(_peripheralVenousBloodPK);

         A.CallTo(() => _parameterFactory.CreateFor(A<string>._, A<double>._, A<string>._, PKSimBuildingBlockType.Simulation))
            .ReturnsLazily(s => new PKSimParameter().WithName((string) s.Arguments[0])
               .WithDimension(A.Fake<IDimension>())
               .WithFormula(new ConstantFormula((double) s.Arguments[1])));
      }

      protected DataColumn CalculationColumnFor(BaseGrid baseGrid, string organ, string compartment, string name, string compoundName)
      {
         var dataColumn = new DataColumn(name, A.Fake<IDimension>(), baseGrid);
         dataColumn.DataInfo.Origin = ColumnOrigins.Calculation;
         dataColumn.QuantityInfo.Type = QuantityType.Drug;
         dataColumn.QuantityInfo.Path = new[] {organ, compartment, compoundName};
         dataColumn.Values = new[] {0f, 1f, 2f};
         return dataColumn;
      }
   }

   public abstract class IndividualBased : for_global_pk_analyses
   {
      protected IndividualSimulation _simulation;

      protected override void Context()
      {
         base.Context();
         _simulation = new IndividualSimulation {Properties = new SimulationProperties()};

         _simulation.Properties.AddCompoundProperties(_compoundProperties);
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock("CompId", PKSimBuildingBlockType.Compound) {BuildingBlock = _compound});
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock("IndividualId", PKSimBuildingBlockType.Individual) {BuildingBlock = _individual});
         _simulation.DataRepository = new DataRepository {_venousBloodPlasma, _peripheralVenousBloodPlasma};
         _simulation.Settings = new SimulationSettings();
         _simulation.OutputSchema = new OutputSchema();
         _simulation.OutputSchema.AddInterval(new OutputInterval {DomainHelperForSpecs.ConstantParameterWithValue(100).WithName(Constants.Parameters.END_TIME)});
         _simulation.Model = new OSPSuite.Core.Domain.Model {Root = new Container()};
         _simulation.Results = new SimulationResults {new IndividualResults {new QuantityValues()}};
         A.CallTo(() => _interactionTask.HasInteractionInvolving(_compound, _simulation)).Returns(true);

         _simulation.Model.Root.Add(_eventGroup);
      }
   }

   public class When_calculating_the_global_pk_analyses_parameter_such_as_VSS_VD_and_Plasma_CL_for_the_human_species : IndividualBased
   {
      private GlobalPKAnalysis _results;

      protected override void Context()
      {
         base.Context();
         var protocol = A.Fake<Protocol>();
         _compoundProperties.ProtocolProperties.Protocol = protocol;
         _species.Name = CoreConstants.Species.HUMAN;


         var schemaItem = new SchemaItem {ApplicationType = ApplicationTypes.Intravenous};
         schemaItem.Add(new PKSimParameter {Name = Constants.Parameters.INFUSION_TIME});
         A.CallTo(() => _protocolMapper.MapFrom(protocol)).Returns(new[] {schemaItem});
      }

      protected override void Because()
      {
         _results = sut.CalculateGlobalPKAnalysisFor(new[] {_simulation});
      }

      [Observation]
      public void should_have_used_the_peripheral_venous_blood()
      {
         var vss = _results.PKParameter(_compoundName, CoreConstants.PKAnalysis.VssPlasma);
         vss.Value.ShouldBeEqualTo(_peripheralVenousBloodPK["Vss"].Value);

         var vdPlasma = _results.PKParameter(_compoundName, CoreConstants.PKAnalysis.VdPlasma);
         vdPlasma.Value.ShouldBeEqualTo(_peripheralVenousBloodPK["Vd"].Value);

         var totalCL = _results.PKParameter(_compoundName, CoreConstants.PKAnalysis.TotalPlasmaCL);
         totalCL.Value.ShouldBeEqualTo(_peripheralVenousBloodPK["CL"].Value);
      }
   }

   public abstract class PopulationBased : for_global_pk_analyses
   {
      protected PopulationSimulation _populationSimulation;
      protected RandomPopulation _population;
      private OutputSelections _outputSelections;
      protected string _quantityPath1 = "QuantityPath1|DRUG|Concentration";

      protected override void Context()
      {
         base.Context();

         _population = new RandomPopulation {Name = "POP", Id = "PopTemplateId"};

         _populationSimulation = CreatePopulationSimulation();
         A.CallTo(() => _interactionTask.HasInteractionInvolving(_compound, _populationSimulation)).Returns(true);
         A.CallTo(() => _pkCalculator.CalculatePK(A<DataColumn>.That.Matches(x => x.BaseGrid.Values.Contains(1.0f)), A<PKCalculationOptions>._, null)).Returns(_venousBloodPK);
         A.CallTo(() => _pkCalculator.CalculatePK(A<DataColumn>.That.Matches(x => x.BaseGrid.Values.Contains(0.0f)), A<PKCalculationOptions>._, null)).Returns(_peripheralVenousBloodPK);
      }

      protected PopulationSimulation CreatePopulationSimulation()
      {
         var populationSimulation = new PopulationSimulation {Properties = new SimulationProperties()};
         populationSimulation.Properties.AddCompoundProperties(_compoundProperties);
         AddUsedBuildingBlocks(populationSimulation);
         populationSimulation.Results = new SimulationResults();
         populationSimulation.Results.AddRange(new[]
         {
            new IndividualResults()
            {
               new QuantityValues()
               {
                  QuantityPath = "Organism|PeripheralVenousBlood|DRUG|Plasma (Peripheral Venous Blood)",
                  Time = new QuantityValues() {Values = new[] {0.0f}},
                  Values = new[] {0.0f}
               },
               new QuantityValues()
               {
                  QuantityPath = "Organism|VenousBlood|Plasma|DRUG|Concentration in container",
                  Time = new QuantityValues() {Values = new[] {1.0f}},
                  Values = new[] {1.0f}
               }
            },
            new IndividualResults()
            {
               new QuantityValues()
               {
                  QuantityPath = "Organism|PeripheralVenousBlood|DRUG|Plasma (Peripheral Venous Blood)",
                  Time = new QuantityValues() {Values = new[] {0.0f}},
                  Values = new[] {0.0f}
               },
               new QuantityValues()
               {
                  QuantityPath = "Organism|VenousBlood|Plasma|DRUG|Concentration in container",
                  Time = new QuantityValues() {Values = new[] {1.0f}},
                  Values = new[] {1.0f}
               }
            }
         });

         populationSimulation.Results.Each(x => x.IndividualId = populationSimulation.Results.AllIndividualResults.IndexOf(x));

         populationSimulation.Settings = new SimulationSettings();
         populationSimulation.OutputSchema = new OutputSchema();
         populationSimulation.OutputSchema.AddInterval(new OutputInterval {DomainHelperForSpecs.ConstantParameterWithValue(100).WithName(Constants.Parameters.END_TIME)});
         populationSimulation.Model = new OSPSuite.Core.Domain.Model {Root = new Container()};

         populationSimulation.Model.Root.Add(_eventGroup);

         _outputSelections = new OutputSelections();
         _outputSelections.AddOutput(new QuantitySelection(_quantityPath1, QuantityType.Drug));

         populationSimulation.OutputSelections = _outputSelections;

         return populationSimulation;
      }

      protected virtual void AddUsedBuildingBlocks(PopulationSimulation populationSimulation)
      {
         populationSimulation.AddUsedBuildingBlock(new UsedBuildingBlock("CompId", PKSimBuildingBlockType.Compound) {BuildingBlock = _compound});
         populationSimulation.AddUsedBuildingBlock(new UsedBuildingBlock("IndividualId", PKSimBuildingBlockType.Individual) {BuildingBlock = _individual});
         populationSimulation.AddUsedBuildingBlock(new UsedBuildingBlock("PopulationId", PKSimBuildingBlockType.Population) {BuildingBlock = _population});
      }
   }

   public class When_calculating_global_pk_analyses_with_denominator_context_for_population : PopulationBased
   {
      private CompoundPKContext _compoundPKContext;
      private PopulationSimulationPKAnalyses _results;
      private CompoundPK _compoundPK;

      protected override void Context()
      {
         base.Context();
         var quantityPKParameter = new QuantityPKParameter {Dimension = DomainHelperForSpecs.NoDimension(), Name = CoreConstants.PKAnalysis.Bioavailability, QuantityPath = "DRUG"};
         quantityPKParameter.SetValue(0, 3);
         quantityPKParameter.SetValue(1, 2);

         _compoundPKContext = new CompoundPKContext();
         _compoundPK = new CompoundPK
         {
            CompoundName = _compoundName,
            AllBioAvailabilityAucInf =
            {
               [0] = 3,
               [1] = 2
            },
            AllDDIAucInf =
            {
               [0] = 5,
               [1] = 4
            },
            AllDDICMax =
            {
               [0] = 7,
               [1] = 6
            }
         };
         _compoundPKContext.AddCompoundPK(_compoundPK);
      }

      protected override void Because()
      {
         _results = sut.CalculateFor(_populationSimulation, _compoundPKContext);
      }

      [Observation]
      public void the_bioavailability_should_be_calculated_from_the_context_values()
      {
         var quantityPKParameter = _results.PKParameterFor(_compoundName, CoreConstants.PKAnalysis.Bioavailability);
         quantityPKParameter.ValueFor(0).ShouldBeEqualTo(_venousBloodPK.ValueOrDefaultFor(Constants.PKParameters.AUC_inf) / 3f);
         quantityPKParameter.ValueFor(1).ShouldBeEqualTo(_venousBloodPK.ValueOrDefaultFor(Constants.PKParameters.AUC_inf) / 2f);
      }

      [Observation]
      public void the_auc_ratio_should_be_calculated_from_the_context_values()
      {
         var quantityPKParameter = _results.PKParameterFor(_compoundName, CoreConstants.PKAnalysis.AUCRatio);
         quantityPKParameter.ValueFor(0).ShouldBeEqualTo(_peripheralVenousBloodPK.ValueOrDefaultFor(Constants.PKParameters.AUC_inf) / 5f);
         quantityPKParameter.ValueFor(1).ShouldBeEqualTo(_peripheralVenousBloodPK.ValueOrDefaultFor(Constants.PKParameters.AUC_inf) / 4f);
      }

      [Observation]
      public void the_cmax_ratio_should_be_calculated_from_the_context_values()
      {
         var quantityPKParameter = _results.PKParameterFor(_compoundName, CoreConstants.PKAnalysis.C_maxRatio);
         quantityPKParameter.ValueFor(0).ShouldBeEqualTo(_peripheralVenousBloodPK.ValueOrDefaultFor(Constants.PKParameters.C_max) / 7f);
         quantityPKParameter.ValueFor(1).ShouldBeEqualTo(_peripheralVenousBloodPK.ValueOrDefaultFor(Constants.PKParameters.C_max) / 6f);
      }
   }

   public class When_calculating_global_pk_analyses_without_context_for_population : PopulationBased
   {
      private CompoundPKContext _compoundPKContext;
      private PopulationSimulationPKAnalyses _results;

      protected override void Context()
      {
         base.Context();
         _compoundPKContext = new CompoundPKContext();
      }

      protected override void Because()
      {
         _results = sut.CalculateFor(_populationSimulation, _compoundPKContext);
      }

      [Observation]
      public void the_context_should_contain_initialized_context()
      {
         var compoundPK = _compoundPKContext.CompoundPKFor(_compoundName);

         compoundPK.AllBioAvailabilityAucInf[0].ShouldBeEqualTo(13);
         compoundPK.AllBioAvailabilityAucInf[1].ShouldBeEqualTo(13);

         compoundPK.AllDDIAucInf[0].ShouldBeEqualTo(24);
         compoundPK.AllDDIAucInf[1].ShouldBeEqualTo(24);

         compoundPK.AllDDICMax[0].ShouldBeEqualTo(26);
         compoundPK.AllDDICMax[1].ShouldBeEqualTo(26);
      }

      [Observation]
      public void the_ratio_parameters_should_use_the_context_values()
      {
         var quantityPKParameter = _results.PKParameterFor(_compoundName, CoreConstants.PKAnalysis.Bioavailability);
         quantityPKParameter.ValueFor(0).ShouldBeEqualTo(float.NaN);
         quantityPKParameter.ValueFor(1).ShouldBeEqualTo(float.NaN);

         quantityPKParameter = _results.PKParameterFor(_compoundName, CoreConstants.PKAnalysis.AUCRatio);
         quantityPKParameter.ValueFor(0).ShouldBeEqualTo(float.NaN);
         quantityPKParameter.ValueFor(1).ShouldBeEqualTo(float.NaN);


         quantityPKParameter = _results.PKParameterFor(_compoundName, CoreConstants.PKAnalysis.C_maxRatio);
         quantityPKParameter.ValueFor(0).ShouldBeEqualTo(float.NaN);
         quantityPKParameter.ValueFor(1).ShouldBeEqualTo(float.NaN);
      }
   }

   public class When_calculating_global_pk_analyses_with_quantity_pk_context_for_population : PopulationBased
   {
      private CompoundPKContext _compoundPKContext;
      private PopulationSimulation _contextPopulationSimulation;
      private PopulationSimulationPKAnalyses _results;

      protected override void Context()
      {
         base.Context();

         _contextPopulationSimulation = CreatePopulationSimulation();
         var quantityPKParameter = new QuantityPKParameter {Dimension = DomainHelperForSpecs.NoDimension(), Name = CoreConstants.PKAnalysis.Bioavailability, QuantityPath = _compoundName};
         quantityPKParameter.SetValue(0, 3);
         quantityPKParameter.SetValue(1, 2);
         _contextPopulationSimulation.PKAnalyses.AddPKAnalysis(quantityPKParameter);

         quantityPKParameter = new QuantityPKParameter {Dimension = DomainHelperForSpecs.NoDimension(), Name = CoreConstants.PKAnalysis.AUCRatio, QuantityPath = _compoundName};
         quantityPKParameter.SetValue(0, 5);
         quantityPKParameter.SetValue(1, 4);
         _contextPopulationSimulation.PKAnalyses.AddPKAnalysis(quantityPKParameter);

         quantityPKParameter = new QuantityPKParameter {Dimension = DomainHelperForSpecs.NoDimension(), Name = CoreConstants.PKAnalysis.C_maxRatio, QuantityPath = _compoundName};
         quantityPKParameter.SetValue(0, 7);
         quantityPKParameter.SetValue(1, 6);
         _contextPopulationSimulation.PKAnalyses.AddPKAnalysis(quantityPKParameter);

         _compoundPKContext = new CompoundPKContext();
         _compoundPKContext.InitializeQuantityPKParametersFrom(_contextPopulationSimulation);
      }

      protected override void Because()
      {
         _results = sut.CalculateFor(_populationSimulation, _compoundPKContext);
      }

      [Observation]
      public void the_ratio_parameters_should_use_the_context_values()
      {
         var quantityPKParameter = _results.PKParameterFor(_compoundName, CoreConstants.PKAnalysis.Bioavailability);
         quantityPKParameter.ValueFor(0).ShouldBeEqualTo(3);
         quantityPKParameter.ValueFor(1).ShouldBeEqualTo(2);

         quantityPKParameter = _results.PKParameterFor(_compoundName, CoreConstants.PKAnalysis.AUCRatio);
         quantityPKParameter.ValueFor(0).ShouldBeEqualTo(5);
         quantityPKParameter.ValueFor(1).ShouldBeEqualTo(4);


         quantityPKParameter = _results.PKParameterFor(_compoundName, CoreConstants.PKAnalysis.C_maxRatio);
         quantityPKParameter.ValueFor(0).ShouldBeEqualTo(7);
         quantityPKParameter.ValueFor(1).ShouldBeEqualTo(6);
      }
   }

   public class When_calculating_global_pk_analyses_without_context_for_individual : IndividualBased
   {
      private GlobalPKAnalysis _results;

      protected override void Context()
      {
         base.Context();
         _simulation.ClearPKCache();
      }

      protected override void Because()
      {
         _results = sut.CalculateGlobalPKAnalysisFor(new[] {_simulation});
      }

      [Observation]
      public void the_ratio_parameters_should_use_the_context_values()
      {
         var pkParameter = _results.PKParameter(_compoundName, CoreConstants.PKAnalysis.Bioavailability);
         pkParameter.Value.ShouldBeEqualTo(double.NaN);

         pkParameter = _results.PKParameter(_compoundName, CoreConstants.PKAnalysis.AUCRatio);
         pkParameter.Value.ShouldBeEqualTo(double.NaN);

         pkParameter = _results.PKParameter(_compoundName, CoreConstants.PKAnalysis.C_maxRatio);
         pkParameter.Value.ShouldBeEqualTo(double.NaN);
      }
   }

   public class When_calculating_global_pk_analyses_with_denominator_context_for_individual : IndividualBased
   {
      private GlobalPKAnalysis _results;

      protected override void Context()
      {
         base.Context();
         _simulation.AucIV[_compoundName] = 3;
         _simulation.AucDDI[_compoundName] = 4;
         _simulation.CMaxDDI[_compoundName] = 5;
      }

      protected override void Because()
      {
         _results = sut.CalculateGlobalPKAnalysisFor(new[] {_simulation});
      }

      [Observation]
      public void the_ratio_parameters_should_use_the_context_values()
      {
         var pkParameter = _results.PKParameter(_compoundName, CoreConstants.PKAnalysis.Bioavailability);
         pkParameter.Value.ShouldBeEqualTo(_venousBloodPK.ValueOrDefaultFor(Constants.PKParameters.AUC_inf) / 3d);

         pkParameter = _results.PKParameter(_compoundName, CoreConstants.PKAnalysis.AUCRatio);
         pkParameter.Value.ShouldBeEqualTo(_peripheralVenousBloodPK.ValueOrDefaultFor(Constants.PKParameters.AUC_inf) / 4d);

         pkParameter = _results.PKParameter(_compoundName, CoreConstants.PKAnalysis.C_maxRatio);
         pkParameter.Value.ShouldBeEqualTo(_peripheralVenousBloodPK.ValueOrDefaultFor(Constants.PKParameters.C_max) / 5d);
      }
   }

   public class When_calculating_the_global_pk_analyses_parameter_such_as_VSS_VD_and_Plasma_CL_for_the_human_species_for_population : PopulationBased
   {
      private GlobalPKAnalysis _results;

      protected override void Context()
      {
         base.Context();
         var protocol = A.Fake<Protocol>();
         _compoundProperties.ProtocolProperties.Protocol = protocol;
         _species.Name = CoreConstants.Species.HUMAN;


         var schemaItem = new SchemaItem {ApplicationType = ApplicationTypes.Intravenous};
         schemaItem.Add(new PKSimParameter {Name = Constants.Parameters.INFUSION_TIME});
         A.CallTo(() => _protocolMapper.MapFrom(protocol)).Returns(new[] {schemaItem});
         _populationSimulation.PKAnalyses = sut.CalculateFor(_populationSimulation);
      }

      protected override void Because()
      {
         _results = sut.CalculateGlobalPKAnalysisFor(new[] {_populationSimulation});
      }

      [Observation]
      public void should_have_used_the_peripheral_venous_blood()
      {
         var vss = _results.PKParameter(_compoundName, CoreConstants.PKAnalysis.VssPlasma);
         vss.Value.ShouldBeEqualTo(_peripheralVenousBloodPK["Vss"].Value);

         var vdplasma = _results.PKParameter(_compoundName, CoreConstants.PKAnalysis.VdPlasma);
         vdplasma.Value.ShouldBeEqualTo(_peripheralVenousBloodPK["Vd"].Value);

         var totalCL = _results.PKParameter(_compoundName, CoreConstants.PKAnalysis.TotalPlasmaCL);
         totalCL.Value.ShouldBeEqualTo(_peripheralVenousBloodPK["CL"].Value);
      }
   }

   public class When_calculating_the_global_pk_analyes_parameter_such_as_VSS_VD_and_Plasma_CL_for_a_single_extra_vascular_without_previous_calculation_of_bioavailability : IndividualBased
   {
      private GlobalPKAnalysis _results;

      protected override void Context()
      {
         base.Context();
         var protocol = A.Fake<Protocol>();
         _compoundProperties.ProtocolProperties.Protocol = protocol;
         _species.Name = CoreConstants.Species.HUMAN;


         var schemaItem = new SchemaItem {ApplicationType = ApplicationTypes.Oral};
         A.CallTo(() => _protocolMapper.MapFrom(protocol)).Returns(new[] {schemaItem});
      }

      protected override void Because()
      {
         _results = sut.CalculateGlobalPKAnalysisFor(new[] {_simulation});
      }

      [Observation]
      public void should_return_over_F_values_for_parameters()
      {
         var vss = _results.PKParameter(_compoundName, CoreConstants.PKAnalysis.VssPlasmaOverF);
         vss.Value.ShouldBeEqualTo(_peripheralVenousBloodPK["Vss"].Value);

         var vdplasma = _results.PKParameter(_compoundName, CoreConstants.PKAnalysis.VdPlasmaOverF);
         vdplasma.Value.ShouldBeEqualTo(_peripheralVenousBloodPK["Vd"].Value);

         var totalCL = _results.PKParameter(_compoundName, CoreConstants.PKAnalysis.TotalPlasmaCLOverF);
         totalCL.Value.ShouldBeEqualTo(_peripheralVenousBloodPK["CL"].Value);
      }
   }

   public class When_calculating_the_global_pk_analyes_parameter_such_as_VSS_VD_and_Plasma_CL_for_a_single_extra_vascular_without_previous_calculation_of_bioavailability_for_population : PopulationBased
   {
      private GlobalPKAnalysis _results;

      protected override void Context()
      {
         base.Context();
         var protocol = A.Fake<Protocol>();
         _compoundProperties.ProtocolProperties.Protocol = protocol;
         _species.Name = CoreConstants.Species.HUMAN;


         var schemaItem = new SchemaItem {ApplicationType = ApplicationTypes.Oral};
         A.CallTo(() => _protocolMapper.MapFrom(protocol)).Returns(new[] {schemaItem});
         _populationSimulation.PKAnalyses = sut.CalculateFor(_populationSimulation);
      }

      protected override void Because()
      {
         _results = sut.CalculateGlobalPKAnalysisFor(new[] {_populationSimulation});
      }

      [Observation]
      public void should_return_over_F_values_for_parameters()
      {
         var vss = _results.PKParameter(_compoundName, CoreConstants.PKAnalysis.VssPlasmaOverF);
         vss.Value.ShouldBeEqualTo(_peripheralVenousBloodPK["Vss"].Value);

         var vdplasma = _results.PKParameter(_compoundName, CoreConstants.PKAnalysis.VdPlasmaOverF);
         vdplasma.Value.ShouldBeEqualTo(_peripheralVenousBloodPK["Vd"].Value);

         var totalCL = _results.PKParameter(_compoundName, CoreConstants.PKAnalysis.TotalPlasmaCLOverF);
         totalCL.Value.ShouldBeEqualTo(_peripheralVenousBloodPK["CL"].Value);
      }
   }

   public class When_calculating_the_global_pk_analyes_parameter_such_as_VSS_VD_and_Plasma_CL_for_a_single_extra_vascular_with_previous_calculation_of_bioavailability : IndividualBased
   {
      private GlobalPKAnalysis _results;
      private double _bioaValue;

      protected override void Context()
      {
         base.Context();
         var protocol = A.Fake<Protocol>();
         _compoundProperties.ProtocolProperties.Protocol = protocol;
         _species.Name = CoreConstants.Species.HUMAN;
         _simulation.AucIV[_compoundName] = 5;
         _bioaValue = _venousBloodPK[Constants.PKParameters.AUC_inf].Value / _simulation.AucIV[_compoundName].Value;

         var schemaItem = new SchemaItem {ApplicationType = ApplicationTypes.Oral};
         A.CallTo(() => _protocolMapper.MapFrom(protocol)).Returns(new[] {schemaItem});
      }

      protected override void Because()
      {
         _results = sut.CalculateGlobalPKAnalysisFor(new[] {_simulation});
      }

      [Observation]
      public void should_return_IV_values_for_parameters()
      {
         var vss = _results.PKParameter(_compoundName, CoreConstants.PKAnalysis.VssPlasma);
         vss.Value.ShouldBeEqualTo(_peripheralVenousBloodPK["Vss"].Value * _bioaValue);

         var vdplasma = _results.PKParameter(_compoundName, CoreConstants.PKAnalysis.VdPlasma);
         vdplasma.Value.ShouldBeEqualTo(_peripheralVenousBloodPK["Vd"].Value * _bioaValue);

         var totalCL = _results.PKParameter(_compoundName, CoreConstants.PKAnalysis.TotalPlasmaCL);
         totalCL.Value.ShouldBeEqualTo(_peripheralVenousBloodPK["CL"].Value * _bioaValue);
      }
   }

   public class When_calculating_the_global_pk_analyes_parameter_such_as_VSS_VD_and_Plasma_CL_for_the_mouse_species : IndividualBased
   {
      private GlobalPKAnalysis _results;

      protected override void Context()
      {
         base.Context();
         _species.Name = CoreConstants.Species.MOUSE;
         var protocol = A.Fake<Protocol>();
         _compoundProperties.ProtocolProperties.Protocol = protocol;

         var schemaItem = new SchemaItem {ApplicationType = ApplicationTypes.Intravenous};
         schemaItem.Add(new PKSimParameter {Name = Constants.Parameters.INFUSION_TIME});
         A.CallTo(() => _protocolMapper.MapFrom(protocol)).Returns(new[] {schemaItem});
      }

      protected override void Because()
      {
         _results = sut.CalculateGlobalPKAnalysisFor(new[] {_simulation});
      }

      [Observation]
      public void should_have_used_the_venous_blood()
      {
         var vdplasma = _results.PKParameter(_compoundName, CoreConstants.PKAnalysis.VdPlasma);
         vdplasma.ShouldNotBeNull();
         vdplasma.Value.ShouldBeEqualTo(_venousBloodPK["Vd"].Value);
      }
   }

   public class When_calculating_the_global_pk_analyses_for_a_compound_that_was_not_applied : IndividualBased
   {
      private GlobalPKAnalysis _result;

      protected override void Context()
      {
         base.Context();
         _compoundProperties.ProtocolProperties.Protocol = null;
         A.CallTo(() => _protocolMapper.MapFrom(null)).Returns(Array.Empty<SchemaItem>());
      }

      protected override void Because()
      {
         _result = sut.CalculateGlobalPKAnalysisFor(new[] {_simulation});
      }

      [Observation]
      public void should_not_crash()
      {
         _result.ShouldNotBeNull();
      }

      [Observation]
      public void should_not_contain_value_for_fraction_absorbed()
      {
         _result.PKParameter(_compoundName, CoreConstants.PKAnalysis.FractionAbsorbed).ShouldBeNull();
      }
   }

   public class When_calculating_the_global_pk_analyses_for_a_simulation_without_compound_building_blocks : PopulationBased
   {
      private PopulationSimulationPKAnalyses _result;

      protected override void Context()
      {
         base.Context();
         _compoundProperties.ProtocolProperties.Protocol = null;
         A.CallTo(() => _protocolMapper.MapFrom(null)).Throws<NullReferenceException>();
      }

      protected override void AddUsedBuildingBlocks(PopulationSimulation populationSimulation)
      {
         populationSimulation.AddUsedBuildingBlock(new UsedBuildingBlock("IndividualId", PKSimBuildingBlockType.Individual) {BuildingBlock = _individual});
         populationSimulation.AddUsedBuildingBlock(new UsedBuildingBlock("PopulationId", PKSimBuildingBlockType.Population) {BuildingBlock = _population});
      }

      protected override void Because()
      {
         _result = sut.CalculateFor(_populationSimulation);
      }

      [Observation]
      public void should_not_produce_any_pk_parameters()
      {
         _result.AllQuantityPaths.ShouldBeEmpty();
         _result.AllPKParameterNames.ShouldBeEmpty();
      }
   }

   public class When_calculating_the_global_pk_analyses_for_a_compound_that_was_not_applied_for_population : PopulationBased
   {
      private GlobalPKAnalysis _result;

      protected override void Context()
      {
         base.Context();
         _compoundProperties.ProtocolProperties.Protocol = null;
         A.CallTo(() => _protocolMapper.MapFrom(null)).Throws<NullReferenceException>();
      }

      protected override void Because()
      {
         _result = sut.CalculateGlobalPKAnalysisFor(new[] {_populationSimulation});
      }

      [Observation]
      public void should_not_crash()
      {
         _result.ShouldNotBeNull();
      }

      [Observation]
      public void should_not_contain_value_for_fraction_absorbed()
      {
         _result.PKParameter(_compoundName, CoreConstants.PKAnalysis.FractionAbsorbed).ShouldBeNull();
      }
   }

   public class When_calculating_the_auc_iv_for_a_given_individual_simulation : IndividualBased
   {
      private IndividualSimulation _aucIVSimulation;
      private PKAnalysis _pkAnalysis;

      protected override void Context()
      {
         base.Context();

         //Simulate calculated results by setting the DataRepository
         _aucIVSimulation = new IndividualSimulation {DataRepository = _simulation.DataRepository};

         _pkAnalysis = new PKAnalysis();
         _pkAnalysis.Add(DomainHelperForSpecs.ConstantParameterWithValue(2).WithName(Constants.PKParameters.AUC_inf));

         A.CallTo(() => _globalPKAnalysisRunner.RunForBioavailability(_protocol, _simulation, _compound)).Returns(_aucIVSimulation);
         A.CallTo(() => _pkMapper.MapFrom(_venousBloodPlasma, A<PKValues>._, A<PKParameterMode>._, A<string>._)).Returns(_pkAnalysis);

         A.CallTo(() => _protocolFactory.Create(ProtocolMode.Simple, ApplicationTypes.Intravenous)).Returns(_protocol);
         _protocol.AddParameter(DomainHelperForSpecs.ConstantParameterWithValue(1).WithName(Constants.Parameters.INFUSION_TIME));
         _protocol.AddParameter(DomainHelperForSpecs.ConstantParameterWithValue(1).WithName(CoreConstants.Parameters.INPUT_DOSE));
         _protocol.AddParameter(DomainHelperForSpecs.ConstantParameterWithValue(1).WithName(Constants.Parameters.START_TIME));

         //single dosing
         var schemaItem = A.Fake<SchemaItem>();
         var inputDose = DomainHelperForSpecs.ConstantParameterWithValue(10);
         var startTime = DomainHelperForSpecs.ConstantParameterWithValue(3);
         A.CallTo(() => schemaItem.Dose).Returns(inputDose);
         A.CallTo(() => schemaItem.StartTime).Returns(startTime);

         A.CallTo(() => _protocolMapper.MapFrom(_protocol)).Returns(new[] {schemaItem,});
         sut.CalculateGlobalPKAnalysisFor(new[] {_simulation});
      }

      protected override void Because()
      {
         sut.CalculateBioavailabilityFor(_simulation, _compoundName);
      }

      [Observation]
      public void should_use_an_iv_infusion_protocol_with_an_infusion_time_of_15_minutes()
      {
         _protocol.Parameter(Constants.Parameters.INFUSION_TIME).Value.ShouldBeEqualTo(15);
      }

      [Observation]
      public void should_use_the_same_input_dose_as_the_original_simulation()
      {
         _protocol.Parameter(CoreConstants.Parameters.INPUT_DOSE).Value.ShouldBeEqualTo(10);
      }

      [Observation]
      public void should_use_the_same_input_start_time_as_the_original_simulation()
      {
         _protocol.Parameter(Constants.Parameters.START_TIME).Value.ShouldBeEqualTo(3);
      }

      [Observation]
      public void the_simulation_should_be_updated_with_denominator_context()
      {
         _simulation.AucIV[_compoundName].ShouldBeEqualTo(_pkAnalysis.PKParameters(Constants.PKParameters.AUC_inf).First().Value);
      }
   }

   public class When_calculating_the_DDI_ratio_for_a_given_single_dosing_simulation : IndividualBased
   {
      private IndividualSimulation _ddiRatioSimulation;
      private PKAnalysis _pkAnalysis;

      protected override void Context()
      {
         base.Context();

         _ddiRatioSimulation = new IndividualSimulation();
         //Simulate calculated results by setting the DataRepository
         _ddiRatioSimulation.DataRepository = _simulation.DataRepository;

         _pkAnalysis = new PKAnalysis();
         _pkAnalysis.Add(DomainHelperForSpecs.ConstantParameterWithValue(1).WithName(Constants.PKParameters.C_max));
         _pkAnalysis.Add(DomainHelperForSpecs.ConstantParameterWithValue(2).WithName(Constants.PKParameters.AUC_inf));

         A.CallTo(() => _globalPKAnalysisRunner.RunForDDIRatio(_simulation)).Returns(_ddiRatioSimulation);
         A.CallTo(() => _pkMapper.MapFrom(_peripheralVenousBloodPlasma, A<PKValues>._, A<PKParameterMode>._, A<string>._)).Returns(_pkAnalysis);
         sut.CalculateGlobalPKAnalysisFor(new[] {_simulation});
      }

      protected override void Because()
      {
         sut.CalculateDDIRatioFor(_simulation);
      }

      [Observation]
      public void the_simulation_should_be_updated_with_denominator_context()
      {
         _simulation.AucDDI[_compoundName].ShouldBeEqualTo(_pkAnalysis.PKParameters(Constants.PKParameters.AUC_inf).First().Value);
         _simulation.CMaxDDI[_compoundName].ShouldBeEqualTo(_pkAnalysis.PKParameters(Constants.PKParameters.C_max).First().Value);
      }
   }

   public class When_calculating_the_DDI_ratio_for_a_compound_that_is_not_appled_simulation : IndividualBased
   {
      private IndividualSimulation _ddiRatioSimulation;
      private PKAnalysis _pkAnalysis;

      protected override void Context()
      {
         base.Context();

         //Simulate calculated results by setting the DataRepository
         _ddiRatioSimulation = new IndividualSimulation {DataRepository = _simulation.DataRepository};

         _pkAnalysis = new PKAnalysis
         {
            DomainHelperForSpecs.ConstantParameterWithValue(1).WithName(Constants.PKParameters.C_max),
            DomainHelperForSpecs.ConstantParameterWithValue(2).WithName(Constants.PKParameters.AUC_inf)
         };

         A.CallTo(() => _globalPKAnalysisRunner.RunForDDIRatio(_simulation)).Returns(_ddiRatioSimulation);
         A.CallTo(() => _pkMapper.MapFrom(_peripheralVenousBloodPlasma, A<PKValues>._, A<PKParameterMode>._, A<string>._)).Returns(_pkAnalysis);
      }

      protected override void Because()
      {
         sut.CalculateDDIRatioFor(_simulation);
      }

      [Observation]
      public void should_set_the_values_for_cmax_ddi_using_cmax_in_simulation()
      {
         _simulation.CMaxDDI[_compoundName].ShouldBeEqualTo(1);
      }

      [Observation]
      public void should_set_the_values_for_auc_ddi_using_auc_inf_in_simulation()
      {
         _simulation.AucDDI[_compoundName].ShouldBeEqualTo(2);
      }
   }

   public class When_calculating_the_DDI_ratio_for_a_given_multiple_dosing_simulation : IndividualBased
   {
      private IndividualSimulation _ddiRatioSimulation;
      private PKAnalysis _pkAnalysis;

      protected override void Context()
      {
         base.Context();

         //Simulate calculated results by setting the DataRepository
         _ddiRatioSimulation = new IndividualSimulation {DataRepository = _simulation.DataRepository};

         _pkAnalysis = new PKAnalysis
         {
            DomainHelperForSpecs.ConstantParameterWithValue(1).WithName(Constants.PKParameters.C_max_tDLast_tDEnd),
            DomainHelperForSpecs.ConstantParameterWithValue(2).WithName(Constants.PKParameters.AUC_inf_tDLast)
         };

         A.CallTo(() => _globalPKAnalysisRunner.RunForDDIRatio(_simulation)).Returns(_ddiRatioSimulation);
         A.CallTo(() => _pkMapper.MapFrom(_peripheralVenousBloodPlasma, A<PKValues>._, A<PKParameterMode>._, A<string>._)).Returns(_pkAnalysis);

         //multiple dosing
         _eventGroup.Add(_application2);
      }

      protected override void Because()
      {
         sut.CalculateDDIRatioFor(_simulation);
      }

      [Observation]
      public void should_set_the_values_for_cmax_ddi_using_cmax_t_last_t_end_in_simulation()
      {
         _simulation.CMaxDDI[_compoundName].ShouldBeEqualTo(1);
      }

      [Observation]
      public void should_set_the_values_for_auc_ddi_using_auc_inf_t_last_in_simulation()
      {
         _simulation.AucDDI[_compoundName].ShouldBeEqualTo(2);
      }
   }

   public class When_creating_the_pk_analyses_for_a_given_population_simulation : concern_for_PKAnalysesTask
   {
      private ChartData<TimeProfileXValue, TimeProfileYValue> _chartData;
      private IPopulationDataCollector _populationDataCollector;
      private List<PopulationPKAnalysis> _result;
      private List<DataColumn> _dataColumn;

      protected override void Context()
      {
         base.Context();
         var dim = DomainHelperForSpecs.ConcentrationDimensionForSpecs();
         _populationDataCollector = A.Fake<IPopulationDataCollector>();
         _chartData = new ChartData<TimeProfileXValue, TimeProfileYValue>(new AxisData(dim, dim.DefaultUnit, Scalings.Linear), null);
         var pane = new PaneData<TimeProfileXValue, TimeProfileYValue>(new AxisData(dim, dim.DefaultUnit, Scalings.Linear));
         _chartData.AddPane(pane);
         var curve = new CurveData<TimeProfileXValue, TimeProfileYValue>()
         {
            Pane = pane,
            QuantityPath = "PATH",
         };
         curve.Add(new TimeProfileXValue(1), new TimeProfileYValue {Y = 10});
         curve.Add(new TimeProfileXValue(2), new TimeProfileYValue {Y = 20});
         pane.AddCurve(curve);

         var rangeCurve = new CurveData<TimeProfileXValue, TimeProfileYValue>()
         {
            Caption = "Compound-Peripheral Venous Blood-Plasma-Concentration-Range 2.5% to 97.5%",
            Pane = pane,
            QuantityPath = "RANGE_PATH",
         };

         rangeCurve.Add(new TimeProfileXValue(1), new TimeProfileYValue {Y = 10, LowerValue = 1, UpperValue = 2});
         pane.AddCurve(rangeCurve);

         A.CallTo(() => _populationDataCollector.MolWeightFor("PATH")).Returns(100);

         A.CallTo(() => _pkMapper.MapFrom(A<DataColumn>._, A<PKValues>._, A<PKParameterMode>._, A<string>._))
            .Invokes(x => _dataColumn.Add(x.GetArgument<DataColumn>(0)));

         _dataColumn = new List<DataColumn>();
      }

      protected override void Because()
      {
         _result = sut.CalculateFor(_populationDataCollector, _chartData).ToList();
      }

      [Observation]
      public void should_return_curve_data_with_the_mol_weight_set()
      {
         _dataColumn.First(x => x.DataInfo.MolWeight != null).DataInfo.MolWeight.ShouldBeEqualTo(100);
      }

      [Observation]
      public void should_not_generate_two_curves_for_each_range_plot()
      {
         _result.Count.ShouldBeEqualTo(3);
      }

      [Observation]
      public void should_name_the_generated_curves_by_the_correct_range_names()
      {
         _result.Select(x => x.ExtraDescription).ShouldContain("Compound-Peripheral Venous Blood-Plasma-Concentration-2.5%");
         _result.Select(x => x.ExtraDescription).ShouldContain("Compound-Peripheral Venous Blood-Plasma-Concentration-97.5%");
      }

      [Observation]
      public void should_have_calculated_the_pk_analysis_with_the_expected_value()
      {
         _dataColumn.First(x => x.Values.Count == 2).Values.ShouldOnlyContainInOrder(10f, 20f);
      }
   }

   public class When_creating_the_pk_analyses_for_a_given_population_simulation_with_axis_not_in_concentration_unit : concern_for_PKAnalysesTask
   {
      private ChartData<TimeProfileXValue, TimeProfileYValue> _chartData;
      private IPopulationDataCollector _populationDataCollector;
      private List<PopulationPKAnalysis> _result;
      private DataColumn _dataColumn;

      protected override void Context()
      {
         base.Context();
         var dim = DomainHelperForSpecs.LengthDimensionForSpecs();
         _populationDataCollector = A.Fake<IPopulationDataCollector>();
         _chartData = new ChartData<TimeProfileXValue, TimeProfileYValue>(new AxisData(dim, dim.DefaultUnit, Scalings.Linear), null);
         var pane = new PaneData<TimeProfileXValue, TimeProfileYValue>(new AxisData(dim, dim.DefaultUnit, Scalings.Linear));
         _chartData.AddPane(pane);
         var curve = new CurveData<TimeProfileXValue, TimeProfileYValue>()
         {
            Pane = pane,
            QuantityPath = "PATH",
         };
         pane.AddCurve(curve);

         A.CallTo(() => _pkMapper.MapFrom(A<DataColumn>._, A<PKValues>._, A<PKParameterMode>._, A<string>._))
            .Invokes(x => _dataColumn = x.GetArgument<DataColumn>(0));
      }

      protected override void Because()
      {
         _result = sut.CalculateFor(_populationDataCollector, _chartData).ToList();
      }

      [Observation]
      public void should_not_calculate_the_pk_analysis()
      {
         _dataColumn.ShouldBeNull();
      }
   }

   public class When_calculating_the_pk_analyses_for_a_set_of_selected_data_and_simulations : concern_for_PKAnalysesTask
   {
      private IEnumerable<IndividualPKAnalysis> _results;
      private IndividualSimulation _simulation;
      private DataColumn _dataColumn1, _dataColumn2;
      private GlobalPKAnalysis _globalPKAnalysis;
      private BaseGrid _baseGrid;
      private PKAnalysis _pkC1;
      private PKAnalysis _pkC2;
      private int _defaultNumberOfRules;

      protected override void Context()
      {
         base.Context();
         _simulation = new IndividualSimulation {DataRepository = new DataRepository()};
         _baseGrid = new BaseGrid("Time", DomainHelperForSpecs.TimeDimensionForSpecs()) {Values = new[] {10f, 20f, 30f}};
         _dataColumn1 = new DataColumn("C1", DomainHelperForSpecs.ConcentrationDimensionForSpecs(), _baseGrid)
         {
            DataInfo = {Origin = ColumnOrigins.Calculation},
            QuantityInfo = {Path = new[] {"C1", "Concentration"}},
            Values = new[] {11f, 21f, 31f}
         };

         _dataColumn2 = new DataColumn("C2", DomainHelperForSpecs.ConcentrationDimensionForSpecs(), _baseGrid)
         {
            DataInfo = {Origin = ColumnOrigins.Calculation},
            QuantityInfo = {Path = new[] {"C2", "Concentration"}},
            Values = new[] {12f, 22f, 32f}
         };

         _simulation.DataRepository.Add(_dataColumn1);
         _simulation.DataRepository.Add(_dataColumn2);

         //Setup global PK analysis
         _globalPKAnalysis = new GlobalPKAnalysis();
         var c1Container = new Container().WithName("C1");
         c1Container.Add(DomainHelperForSpecs.ConstantParameterWithValue(0.5).WithName(CoreConstants.PKAnalysis.FractionAbsorbed));
         var c2Container = new Container().WithName("C2");
         c2Container.Add(DomainHelperForSpecs.ConstantParameterWithValue(1).WithName(CoreConstants.PKAnalysis.FractionAbsorbed));
         _globalPKAnalysis.Add(c1Container);
         _globalPKAnalysis.Add(c2Container);

         _pkC1 = new PKAnalysis
         {
            DomainHelperForSpecs.ConstantParameterWithValue(1).WithName(Constants.PKParameters.MRT),
            DomainHelperForSpecs.ConstantParameterWithValue(2).WithName(Constants.PKParameters.Tmax)
         };

         _pkC2 = new PKAnalysis
         {
            DomainHelperForSpecs.ConstantParameterWithValue(3).WithName(Constants.PKParameters.MRT),
            DomainHelperForSpecs.ConstantParameterWithValue(4).WithName(Constants.PKParameters.Tmax)
         };

         A.CallTo(() => _pkMapper.MapFrom(_dataColumn1, A<PKValues>._, A<PKParameterMode>._, "C1")).Returns(_pkC1);
         A.CallTo(() => _pkMapper.MapFrom(_dataColumn2, A<PKValues>._, A<PKParameterMode>._, "C2")).Returns(_pkC2);

         _defaultNumberOfRules = DomainHelperForSpecs.ConstantParameterWithValue(3).Rules.Count;
      }

      protected override void Because()
      {
         _results = sut.CalculateFor(new[] {_simulation}, new[] {_dataColumn1, _dataColumn2}, _globalPKAnalysis);
      }

      [Observation]
      public void should_have_calculated_the_pk_analysis_with_the_expected_value()
      {
         _results.Count().ShouldBeEqualTo(2);
         _results.ElementAt(0).PKAnalysis.ShouldBeEqualTo(_pkC1);
         _results.ElementAt(1).PKAnalysis.ShouldBeEqualTo(_pkC2);
      }

      [Observation]
      public void should_have_set_the_warnings_for_the_pk_parameters_belonging_to_a_compound_for_which_fraction_absorbed_is_smaller_than_1()
      {
         _pkC1.Parameter(Constants.PKParameters.MRT).Rules.Count.ShouldBeEqualTo(_defaultNumberOfRules + 1);
         _pkC1.Parameter(Constants.PKParameters.Tmax).Rules.Count.ShouldBeEqualTo(_defaultNumberOfRules);
      }

      [Observation]
      public void should_have_not_set_the_warnings_for_the_pk_parameters_belonging_to_a_compound_for_which_fraction_absorbed_is_equal_to_1()
      {
         _pkC2.Parameter(Constants.PKParameters.MRT).Rules.Count.ShouldBeEqualTo(_defaultNumberOfRules);
         _pkC2.Parameter(Constants.PKParameters.Tmax).Rules.Count.ShouldBeEqualTo(_defaultNumberOfRules);
      }
   }

   public abstract class concern_for_CanCalculateGlobalPKInProtocol : concern_for_PKAnalysesTask
   {
      protected bool _result;
      private AdvancedProtocol _protocol;

      protected override void Context()
      {
         base.Context();
         _protocol = new AdvancedProtocol();

         A.CallTo(() => _protocolMapper.MapFrom(_protocol)).Returns(GetSchemaItems());
      }

      protected abstract IReadOnlyList<SchemaItem> GetSchemaItems();

      protected override void Because()
      {
         _result = sut.CanCalculateGlobalPKFor(_protocol);
      }
   }

   public class When_calculating_if_a_global_pk_analysis_is_possible_for_single_iv : concern_for_CanCalculateGlobalPKInProtocol
   {
      protected override IReadOnlyList<SchemaItem> GetSchemaItems()
      {
         return new[]
         {
            new SchemaItem {ApplicationType = ApplicationTypes.Intravenous}
         };
      }

      [Observation]
      public void should_return_true()
      {
         _result.ShouldBeTrue();
      }
   }

   public class When_calculating_if_a_global_pk_analysis_is_possible_for_multiple_oral : concern_for_CanCalculateGlobalPKInProtocol
   {
      protected override IReadOnlyList<SchemaItem> GetSchemaItems()
      {
         return new[]
         {
            new SchemaItem {ApplicationType = ApplicationTypes.Oral},
            new SchemaItem {ApplicationType = ApplicationTypes.Oral}
         };
      }

      [Observation]
      public void should_return_true()
      {
         _result.ShouldBeTrue();
      }
   }

   public class When_calculating_if_a_global_pk_analysis_is_possible_for_mixed_protocol_oral : concern_for_CanCalculateGlobalPKInProtocol
   {
      protected override IReadOnlyList<SchemaItem> GetSchemaItems()
      {
         return new[]
         {
            new SchemaItem {ApplicationType = ApplicationTypes.Intravenous},
            new SchemaItem {ApplicationType = ApplicationTypes.Oral}
         };
      }

      [Observation]
      public void should_return_false()
      {
         _result.ShouldBeFalse();
      }
   }

   public class When_calculating_if_a_global_pk_analysis_is_possible_for_user_defined : concern_for_CanCalculateGlobalPKInProtocol
   {
      protected override IReadOnlyList<SchemaItem> GetSchemaItems()
      {
         return new[]
         {
            new SchemaItem {ApplicationType = ApplicationTypes.UserDefined},
         };
      }

      [Observation]
      public void should_return_false()
      {
         _result.ShouldBeFalse();
      }
   }

   public class When_calculating_if_a_global_pk_analysis_is_possible_for_an_empty_protocol : concern_for_CanCalculateGlobalPKInProtocol
   {
      protected override IReadOnlyList<SchemaItem> GetSchemaItems()
      {
         return Array.Empty<SchemaItem>();
      }

      [Observation]
      public void should_return_false()
      {
         _result.ShouldBeFalse();
      }
   }

   public class When_calculating_if_a_global_pk_analysis_is_possible_for_an_undefined_protocol : concern_for_CanCalculateGlobalPKInProtocol
   {
      protected override IReadOnlyList<SchemaItem> GetSchemaItems()
      {
         return null;
      }

      [Observation]
      public void should_return_false()
      {
         _result.ShouldBeFalse();
      }
   }

   public abstract class concern_for_CanCalculateGlobalPKAnalysisInSimulation : concern_for_PKAnalysesTask
   {
      protected bool _result;
      protected CompoundProperties _secondCompoundProperties;
      protected CompoundProperties _firstCompoundProperties;
      protected IndividualSimulation _individualSimulation;

      protected override void Context()
      {
         base.Context();
         var firstCompound = new Compound().WithName("Compound1");
         var firstProtocol = new AdvancedProtocol();
         _firstCompoundProperties = new CompoundProperties
         {
            Compound = firstCompound,
            ProtocolProperties = new ProtocolProperties
            {
               Protocol = firstProtocol
            }
         };

         var secondCompound = new Compound().WithName("Compound2");
         var secondProtocol = new AdvancedProtocol();
         _secondCompoundProperties = new CompoundProperties
         {
            Compound = secondCompound,
            ProtocolProperties = new ProtocolProperties
            {
               Protocol = secondProtocol
            }
         };

         var simulationProperties = new SimulationProperties();
         simulationProperties.AddCompoundProperties(_firstCompoundProperties);
         simulationProperties.AddCompoundProperties(_secondCompoundProperties);

         var firstUsedBuildingBlock = new UsedBuildingBlock("t1", PKSimBuildingBlockType.Compound)
         {
            BuildingBlock = firstCompound
         };
         var secondUsedBuildingBlock = new UsedBuildingBlock("t2", PKSimBuildingBlockType.Compound)
         {
            BuildingBlock = secondCompound
         };

         _individualSimulation = new IndividualSimulation
         {
            Properties = simulationProperties
         };
         _individualSimulation.AddUsedBuildingBlock(firstUsedBuildingBlock);
         _individualSimulation.AddUsedBuildingBlock(secondUsedBuildingBlock);

         A.CallTo(() => _protocolMapper.MapFrom(firstProtocol)).Returns(GetSchemaItemsFirstProtocol());
         A.CallTo(() => _protocolMapper.MapFrom(secondProtocol)).Returns(GetSchemaItemsSecondProtocol());
      }

      protected abstract IReadOnlyList<SchemaItem> GetSchemaItemsFirstProtocol();
      protected abstract IReadOnlyList<SchemaItem> GetSchemaItemsSecondProtocol();

      protected override void Because()
      {
         _result = sut.CanCalculateGlobalPKFor(_individualSimulation);
      }
   }

   public class When_calculating_if_a_global_pk_analysis_is_possible_for_multiple : concern_for_CanCalculateGlobalPKAnalysisInSimulation
   {
      [Observation]
      public void the_calculation_is_possible()
      {
         _result.ShouldBeTrue();
      }

      protected override IReadOnlyList<SchemaItem> GetSchemaItemsFirstProtocol()
      {
         return new[]
         {
            new SchemaItem {ApplicationType = ApplicationTypes.Intravenous}
         };
      }

      protected override IReadOnlyList<SchemaItem> GetSchemaItemsSecondProtocol()
      {
         return new[]
         {
            new SchemaItem {ApplicationType = ApplicationTypes.Intravenous}
         };
      }
   }

   public class When_calculating_if_a_global_pk_analysis_is_possible_for_an_simulation_using_a_metabolite : concern_for_CanCalculateGlobalPKAnalysisInSimulation
   {
      protected override void Context()
      {
         base.Context();
         //no protocol for the first compound
         _firstCompoundProperties.ProtocolProperties.Protocol = null;
      }

      protected override IReadOnlyList<SchemaItem> GetSchemaItemsFirstProtocol()
      {
         return Array.Empty<SchemaItem>();
      }

      protected override IReadOnlyList<SchemaItem> GetSchemaItemsSecondProtocol()
      {
         return new[]
         {
            new SchemaItem {ApplicationType = ApplicationTypes.Intravenous},
         };
      }

      [Observation]
      public void the_calculation_is_possible()
      {
         _result.ShouldBeTrue();
      }
   }

   public class When_calculating_if_a_global_pk_analysis_is_possible_for_multiple_with_at_least_one_possible : concern_for_CanCalculateGlobalPKAnalysisInSimulation
   {
      protected override IReadOnlyList<SchemaItem> GetSchemaItemsFirstProtocol()
      {
         return new[]
         {
            new SchemaItem {ApplicationType = ApplicationTypes.Oral}
         };
      }

      protected override IReadOnlyList<SchemaItem> GetSchemaItemsSecondProtocol()
      {
         return new[]
         {
            new SchemaItem {ApplicationType = ApplicationTypes.Intravenous},
            new SchemaItem {ApplicationType = ApplicationTypes.Intravenous},
         };
      }

      [Observation]
      public void the_calculation_is_possible()
      {
         _result.ShouldBeTrue();
      }
   }

   public class When_calculating_if_a_global_pk_analysis_is_possible_for_multiple_with_all_not_possible : concern_for_CanCalculateGlobalPKAnalysisInSimulation
   {
      protected override IReadOnlyList<SchemaItem> GetSchemaItemsFirstProtocol()
      {
         return new[]
         {
            new SchemaItem {ApplicationType = ApplicationTypes.UserDefined}
         };
      }

      protected override IReadOnlyList<SchemaItem> GetSchemaItemsSecondProtocol()
      {
         return new[]
         {
            new SchemaItem {ApplicationType = ApplicationTypes.Intravenous},
            new SchemaItem {ApplicationType = ApplicationTypes.Intravenous},
         };
      }
      [Observation]
      public void the_calculation_is_not_possible()
      {
         _result.ShouldBeFalse();
      }
   }

   public class AdvancedProtocolEqualityComparer : GenericEqualityComparer<AdvancedProtocol>
   {

   }
}