﻿using System;
using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.UnitSystem;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Services;
using IParameterFactory = PKSim.Core.Model.IParameterFactory;
using IPKAnalysesTask = PKSim.Core.Services.IPKAnalysesTask;
using IPKCalculationOptionsFactory = PKSim.Core.Services.IPKCalculationOptionsFactory;

namespace PKSim.Core
{
   public abstract class concern_for_GlobalPKAnalysisTask : ContextSpecification<IGlobalPKAnalysisTask>
   {
      private IParameterFactory _parameterFactory;
      protected IProtocolToSchemaItemsMapper _protocolMapper;
      protected IProtocolFactory _protocolFactory;
      protected IGlobalPKAnalysisRunner _globalPKAnalysisRunner;
      private IPKCalculationOptionsFactory _pkCalculationOptionsFactory;
      protected IPKAnalysesTask _pkAnalysisTask;
      private IVSSCalculator _vssCalculator;
      protected DataColumn _peripheralVenousBloodPlasma;
      protected DataColumn _venousBloodPlasma;
      protected IndividualSimulation _simulation;
      protected CompoundProperties _compoundProperties;
      protected Individual _individual;
      protected const string _compoundName = "DRUG";
      protected PKValues _venousBloodPK;
      protected PKValues _peripheralVenousBloodPK;
      protected Species _species;
      protected SimpleProtocol _protocol;
      private IInteractionTask _interactionTask;
      protected Compound _compound;
      private ICloner _cloner;
      protected EventGroup _eventGroup;
      protected Container _application1;
      protected Container _application2;

      protected override void Context()
      {
         _vssCalculator = A.Fake<IVSSCalculator>();
         _parameterFactory = A.Fake<IParameterFactory>();
         _protocolMapper = A.Fake<IProtocolToSchemaItemsMapper>();
         _protocolFactory = A.Fake<IProtocolFactory>();
         _globalPKAnalysisRunner = A.Fake<IGlobalPKAnalysisRunner>();
         _pkCalculationOptionsFactory = A.Fake<IPKCalculationOptionsFactory>();
         _pkAnalysisTask = A.Fake<IPKAnalysesTask>();
         _interactionTask = A.Fake<IInteractionTask>();
         _cloner = A.Fake<ICloner>();
         sut = new GlobalPKAnalysisTask(_parameterFactory, _protocolMapper, _protocolFactory, _globalPKAnalysisRunner,
            _pkAnalysisTask, _pkCalculationOptionsFactory, _vssCalculator, _interactionTask, _cloner);

         var baseGrid = new BaseGrid("time", A.Fake<IDimension>());
         _peripheralVenousBloodPlasma = CalculationColumnFor(baseGrid, CoreConstants.Organ.PERIPHERAL_VENOUS_BLOOD, CoreConstants.Observer.PLASMA_PERIPHERAL_VENOUS_BLOOD, CoreConstants.Observer.PLASMA_PERIPHERAL_VENOUS_BLOOD, _compoundName);
         _venousBloodPlasma = CalculationColumnFor(baseGrid, CoreConstants.Organ.VENOUS_BLOOD, CoreConstants.Compartment.PLASMA, CoreConstants.Observer.CONCENTRATION_IN_CONTAINER, _compoundName);

         _individual = A.Fake<Individual>();
         _species = new Species();
         A.CallTo(() => _individual.Species).Returns(_species);

         _compound = new Compound().WithName(_compoundName);
         _compoundProperties = new CompoundProperties { Compound = _compound };
         _protocol = new SimpleProtocol();
         _compoundProperties.ProtocolProperties.Protocol = _protocol;

         _simulation = new IndividualSimulation { Properties = new SimulationProperties() };

         _simulation.Properties.AddCompoundProperties(_compoundProperties);
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock("CompId", PKSimBuildingBlockType.Compound) { BuildingBlock = _compound });
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock("IndividualId", PKSimBuildingBlockType.Individual) { BuildingBlock = _individual });
         _simulation.DataRepository = new DataRepository { _venousBloodPlasma, _peripheralVenousBloodPlasma };
         _simulation.SimulationSettings = new SimulationSettings();
         _simulation.OutputSchema = new OutputSchema();
         _simulation.OutputSchema.AddInterval(new OutputInterval { DomainHelperForSpecs.ConstantParameterWithValue(100).WithName(Constants.Parameters.END_TIME) });
         _simulation.Model = new OSPSuite.Core.Domain.Model { Root = new Container() };
         _eventGroup = new EventGroup();
         _application1 = new Container().WithName("App1").WithContainerType(ContainerType.Application);
         _application1.Add(new MoleculeAmount().WithName(_compoundName));
         _application1.Add(DomainHelperForSpecs.ConstantParameterWithValue(10).WithName(Constants.Parameters.START_TIME));
         _application2 = new Container().WithName("App2").WithContainerType(ContainerType.Application);
         _application2.Add(DomainHelperForSpecs.ConstantParameterWithValue(10).WithName(Constants.Parameters.START_TIME));
         _application2.Add(new MoleculeAmount().WithName(_compoundName));
         _simulation.Model.Root.Add(_eventGroup);
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


         A.CallTo(() => _pkAnalysisTask.CalculatePK(_venousBloodPlasma, A<PKCalculationOptions>._)).Returns(_venousBloodPK);
         A.CallTo(() => _pkAnalysisTask.CalculatePK(_peripheralVenousBloodPlasma, A<PKCalculationOptions>._)).Returns(_peripheralVenousBloodPK);
         A.CallTo(() => _parameterFactory.CreateFor(A<string>._, A<double>._, A<string>._, PKSimBuildingBlockType.Simulation))
            .ReturnsLazily(s => new PKSimParameter().WithName((string)s.Arguments[0])
               .WithDimension(A.Fake<IDimension>())
               .WithFormula(new ConstantFormula((double)s.Arguments[1])));
      }

      protected DataColumn CalculationColumnFor(BaseGrid baseGrid, string organ, string compartment, string name, string compoundName)
      {
         var dataColumn = new DataColumn(name, A.Fake<IDimension>(), baseGrid);
         dataColumn.DataInfo.Origin = ColumnOrigins.Calculation;
         dataColumn.QuantityInfo.Type = QuantityType.Drug;
         dataColumn.QuantityInfo.Path = new[] { organ, compartment, compoundName };
         return dataColumn;
      }
   }

   public class When_calculating_the_global_pk_analyses_parameter_such_as_VSS_VD_and_Plasma_CL_for_the_human_species : concern_for_GlobalPKAnalysisTask
   {
      private GlobalPKAnalysis _results;

      protected override void Context()
      {
         base.Context();
         var protocol = A.Fake<Protocol>();
         _compoundProperties.ProtocolProperties.Protocol = protocol;
         _species.Name = CoreConstants.Species.HUMAN;


         var schemaItem = new SchemaItem { ApplicationType = ApplicationTypes.Intravenous };
         schemaItem.Add(new PKSimParameter { Name = Constants.Parameters.INFUSION_TIME });
         A.CallTo(() => _protocolMapper.MapFrom(protocol)).Returns(new[] { schemaItem });
      }

      protected override void Because()
      {
         _results = sut.CalculateGlobalPKAnalysisFor(new[] { _simulation });
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

   public abstract class Population_based : concern_for_GlobalPKAnalysisTask
   {
      protected PopulationSimulation _populationSimulation;
      protected RandomPopulation _population;

      protected override void Context()
      {
         base.Context();

         _population = new RandomPopulation() { Name = "POP", Id = "PopTemplateId" };

         _populationSimulation = new PopulationSimulation { Properties = new SimulationProperties() };
         _populationSimulation.Properties.AddCompoundProperties(_compoundProperties);
         _populationSimulation.AddUsedBuildingBlock(new UsedBuildingBlock("CompId", PKSimBuildingBlockType.Compound) { BuildingBlock = _compound });
         _populationSimulation.AddUsedBuildingBlock(new UsedBuildingBlock("IndividualId", PKSimBuildingBlockType.Individual) { BuildingBlock = _individual });
         _populationSimulation.AddUsedBuildingBlock(new UsedBuildingBlock("PopulationId", PKSimBuildingBlockType.Population) { BuildingBlock = _population });
         _populationSimulation.Results = new SimulationResults();
         _populationSimulation.Results.AddRange(new[]
         {
            new IndividualResults()
            {
               new QuantityValues() {
                  QuantityPath = "Organism|PeripheralVenousBlood|R-omeprazole|Plasma (Peripheral Venous Blood)",
                  Time = new QuantityValues() {Values = new [] { 0.0f } },
                  Values = new [] { 0.0f }
               },
               new QuantityValues()
               {
                  QuantityPath = "Organism|VenousBlood|Plasma|Esomeprazole|Concentration in container",
                  Time = new QuantityValues() {Values = new [] { 1.0f } },
                  Values = new [] { 1.0f }
               }
            },
            new IndividualResults()
            {
               new QuantityValues() {
                  QuantityPath = "Organism|PeripheralVenousBlood|R-omeprazole|Plasma (Peripheral Venous Blood)",
                  Time = new QuantityValues() {Values = new [] { 0.0f } },
                  Values = new [] { 0.0f }
               },
               new QuantityValues()
               {
                  QuantityPath = "Organism|VenousBlood|Plasma|Esomeprazole|Concentration in container",
                  Time = new QuantityValues() {Values = new [] { 1.0f } },
                  Values = new [] { 1.0f }
               }
            }
         });
         _populationSimulation.SimulationSettings = new SimulationSettings();
         _populationSimulation.OutputSchema = new OutputSchema();
         _populationSimulation.OutputSchema.AddInterval(new OutputInterval { DomainHelperForSpecs.ConstantParameterWithValue(100).WithName(Constants.Parameters.END_TIME) });
         _populationSimulation.Model = new OSPSuite.Core.Domain.Model { Root = new Container() };
         _application1 = new Container().WithName("App1").WithContainerType(ContainerType.Application);
         _application1.Add(new MoleculeAmount().WithName(_compoundName));
         _application1.Add(DomainHelperForSpecs.ConstantParameterWithValue(10).WithName(Constants.Parameters.START_TIME));
         _application2 = new Container().WithName("App2").WithContainerType(ContainerType.Application);
         _application2.Add(DomainHelperForSpecs.ConstantParameterWithValue(10).WithName(Constants.Parameters.START_TIME));
         _application2.Add(new MoleculeAmount().WithName(_compoundName));
         _populationSimulation.Model.Root.Add(_eventGroup);
         
         A.CallTo(() => _pkAnalysisTask.CalculatePK(A<DataColumn>.That.Matches(x => x.BaseGrid.Values.Contains(1.0f)), A<PKCalculationOptions>._)).Returns(_venousBloodPK);
         A.CallTo(() => _pkAnalysisTask.CalculatePK(A<DataColumn>.That.Matches(x => x.BaseGrid.Values.Contains(0.0f)), A<PKCalculationOptions>._)).Returns(_peripheralVenousBloodPK);
      }
   }

   public class When_calculating_the_global_pk_analyses_parameter_such_as_VSS_VD_and_Plasma_CL_for_the_human_species_for_population : Population_based
   {
      private GlobalPKAnalysis _results;

      protected override void Context()
      {
         base.Context();
         var protocol = A.Fake<Protocol>();
         _compoundProperties.ProtocolProperties.Protocol = protocol;
         _species.Name = CoreConstants.Species.HUMAN;


         var schemaItem = new SchemaItem { ApplicationType = ApplicationTypes.Intravenous };
         schemaItem.Add(new PKSimParameter { Name = Constants.Parameters.INFUSION_TIME });
         A.CallTo(() => _protocolMapper.MapFrom(protocol)).Returns(new[] { schemaItem });
      }

      protected override void Because()
      {
         _results = sut.CalculateGlobalPKAnalysisFor(new[] { _populationSimulation });
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

   public class When_calculating_the_global_pk_analyes_parameter_such_as_VSS_VD_and_Plasma_CL_for_a_single_extra_vascular_without_previous_calculation_of_bioavailability : concern_for_GlobalPKAnalysisTask
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

   public class When_calculating_the_global_pk_analyes_parameter_such_as_VSS_VD_and_Plasma_CL_for_a_single_extra_vascular_without_previous_calculation_of_bioavailability_for_population : Population_based
   {
      private GlobalPKAnalysis _results;

      protected override void Context()
      {
         base.Context();
         var protocol = A.Fake<Protocol>();
         _compoundProperties.ProtocolProperties.Protocol = protocol;
         _species.Name = CoreConstants.Species.HUMAN;


         var schemaItem = new SchemaItem { ApplicationType = ApplicationTypes.Oral };
         A.CallTo(() => _protocolMapper.MapFrom(protocol)).Returns(new[] { schemaItem });
      }

      protected override void Because()
      {
         _results = sut.CalculateGlobalPKAnalysisFor(new[] { _populationSimulation });
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

   public class When_calculating_the_global_pk_analyes_parameter_such_as_VSS_VD_and_Plasma_CL_for_a_single_extra_vascular_with_previous_calculation_of_bioavailability : concern_for_GlobalPKAnalysisTask
   {
      private GlobalPKAnalysis _results;
      private double _bioaValue;

      protected override void Context()
      {
         base.Context();
         var protocol = A.Fake<Protocol>();
         _compoundProperties.ProtocolProperties.Protocol = protocol;
         _species.Name = CoreConstants.Species.HUMAN;
         _simulation.CompoundPKFor(_compoundName).AucIV = 5;
         _bioaValue = _venousBloodPK[Constants.PKParameters.AUC_inf].Value / _simulation.AucIVFor(_compoundName).Value;

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

   public class When_calculating_the_global_pk_analyes_parameter_such_as_VSS_VD_and_Plasma_CL_for_the_mouse_species : concern_for_GlobalPKAnalysisTask
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

   public class When_calculating_the_global_pk_analyses_for_a_compound_that_was_not_applied : concern_for_GlobalPKAnalysisTask
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

   public class When_calculating_the_global_pk_analyses_for_a_compound_that_was_not_applied_for_population : Population_based
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
         _result = sut.CalculateGlobalPKAnalysisFor(new[] { _populationSimulation });
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

   public class When_calculating_the_auc_iv_for_a_given_individual_simulation : concern_for_GlobalPKAnalysisTask
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
         A.CallTo(() => _pkAnalysisTask.CalculateFor(_aucIVSimulation, _venousBloodPlasma)).Returns(new IndividualPKAnalysis(_venousBloodPlasma, _pkAnalysis));

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
      }

      protected override void Because()
      {
         sut.CalculateBioavailabilityFor(_simulation, _compoundName);
      }

      [Observation]
      public void should_use_venous_blood_plasma_to_calculate_the_AUC_inf()
      {
         _simulation.AucIVFor(_compoundName).ShouldBeEqualTo(2);
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
   }

   public class When_calculating_the_DDI_ratio_for_a_given_single_dosing_simulation : concern_for_GlobalPKAnalysisTask
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
         A.CallTo(() => _pkAnalysisTask.CalculateFor(_ddiRatioSimulation, _peripheralVenousBloodPlasma)).Returns(new IndividualPKAnalysis(_peripheralVenousBloodPlasma, _pkAnalysis));
      }

      protected override void Because()
      {
         sut.CalculateDDIRatioFor(_simulation, _compoundName);
      }

      [Observation]
      public void should_set_the_values_for_cmax_ddi_using_cmax_in_simulation()
      {
         _simulation.CmaxDDIFor(_compoundName).ShouldBeEqualTo(1);
      }

      [Observation]
      public void should_set_the_values_for_auc_ddi_using_auc_inf_in_simulation()
      {
         _simulation.AucDDIFor(_compoundName).ShouldBeEqualTo(2);
      }
   }

   public class When_calculating_the_DDI_ratio_for_a_compound_that_is_not_appled_simulation : concern_for_GlobalPKAnalysisTask
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
         A.CallTo(() => _pkAnalysisTask.CalculateFor(_ddiRatioSimulation, _peripheralVenousBloodPlasma)).Returns(new IndividualPKAnalysis(_peripheralVenousBloodPlasma, _pkAnalysis));
      }

      protected override void Because()
      {
         sut.CalculateDDIRatioFor(_simulation, _compoundName);
      }

      [Observation]
      public void should_set_the_values_for_cmax_ddi_using_cmax_in_simulation()
      {
         _simulation.CmaxDDIFor(_compoundName).ShouldBeEqualTo(1);
      }

      [Observation]
      public void should_set_the_values_for_auc_ddi_using_auc_inf_in_simulation()
      {
         _simulation.AucDDIFor(_compoundName).ShouldBeEqualTo(2);
      }
   }

   public class When_calculating_the_DDI_ratio_for_a_given_multiple_dosing_simulation : concern_for_GlobalPKAnalysisTask
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
         A.CallTo(() => _pkAnalysisTask.CalculateFor(_ddiRatioSimulation, _peripheralVenousBloodPlasma)).Returns(new IndividualPKAnalysis(_peripheralVenousBloodPlasma, _pkAnalysis));
         //multiple dosing
         _eventGroup.Add(_application2);
      }

      protected override void Because()
      {
         sut.CalculateDDIRatioFor(_simulation, _compoundName);
      }

      [Observation]
      public void should_set_the_values_for_cmax_ddi_using_cmax_t_last_t_end_in_simulation()
      {
         _simulation.CmaxDDIFor(_compoundName).ShouldBeEqualTo(1);
      }

      [Observation]
      public void should_set_the_values_for_auc_ddi_using_auc_inf_t_last_in_simulation()
      {
         _simulation.AucDDIFor(_compoundName).ShouldBeEqualTo(2);
      }
   }
}