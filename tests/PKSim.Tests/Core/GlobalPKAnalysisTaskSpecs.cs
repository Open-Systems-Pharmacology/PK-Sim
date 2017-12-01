using System;
using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.UnitSystem;
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
      private Individual _individual;
      protected const string _compoundName = "DRUG";
      protected PKValues _venousBloodPK;
      protected PKValues _peripheralVenousBloodPK;
      protected Species _species;
      protected SimpleProtocol _protocol;
      protected List<SchemaItem> _simulationSchemaItems;
      private IInteractionTask _interactionTask;
      protected Compound _compound;
      private ICloner _cloner;

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
         _peripheralVenousBloodPlasma = CalculationColumnFor(baseGrid, CoreConstants.Organ.PeripheralVenousBlood, CoreConstants.Observer.PLASMA_PERIPHERAL_VENOUS_BLOOD, CoreConstants.Observer.PLASMA_PERIPHERAL_VENOUS_BLOOD, _compoundName);
         _venousBloodPlasma = CalculationColumnFor(baseGrid, CoreConstants.Organ.VenousBlood, CoreConstants.Compartment.Plasma, CoreConstants.Observer.CONCENTRATION, _compoundName);

         _individual = A.Fake<Individual>();
         _species = new Species();
         A.CallTo(() => _individual.Species).Returns(_species);

         _compound = new Compound().WithName(_compoundName);
         _compoundProperties = new CompoundProperties {Compound = _compound};
         _simulationSchemaItems = new List<SchemaItem>();
         _protocol = new SimpleProtocol();
         _compoundProperties.ProtocolProperties.Protocol = _protocol;
         A.CallTo(() => _protocolMapper.MapFrom(_protocol)).Returns(_simulationSchemaItems);

         _simulation = new IndividualSimulation {Properties = new SimulationProperties()};

         _simulation.Properties.AddCompoundProperties(_compoundProperties);
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock("CompId", PKSimBuildingBlockType.Compound) {BuildingBlock = _compound});
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock("IndividualId", PKSimBuildingBlockType.Individual) {BuildingBlock = _individual});
         _simulation.DataRepository = new DataRepository {_venousBloodPlasma, _peripheralVenousBloodPlasma};

         _venousBloodPK = new PKValues();
         _venousBloodPK.AddValue(Constants.PKParameters.Vss, 10);
         _venousBloodPK.AddValue(Constants.PKParameters.Vd, 11);
         _venousBloodPK.AddValue(Constants.PKParameters.CL, 12);

         _venousBloodPK.AddValue(Constants.PKParameters.AUC_inf, 13);
         _venousBloodPK.AddValue(Constants.PKParameters.AUC_inf_t1_norm, 14);

         _peripheralVenousBloodPK = new PKValues();
         _peripheralVenousBloodPK.AddValue(Constants.PKParameters.Vss, 21);
         _peripheralVenousBloodPK.AddValue(Constants.PKParameters.Vd, 22);
         _peripheralVenousBloodPK.AddValue(Constants.PKParameters.CL, 23);
         _peripheralVenousBloodPK.AddValue(Constants.PKParameters.AUC_inf, 24);
         _peripheralVenousBloodPK.AddValue(Constants.PKParameters.AUC_inf_t1_norm, 25);
         _peripheralVenousBloodPK.AddValue(Constants.PKParameters.C_max, 26);
         _peripheralVenousBloodPK.AddValue(Constants.PKParameters.C_max_tLast_tEnd, 27);


         A.CallTo(() => _pkAnalysisTask.CalculatePK(_venousBloodPlasma, A<PKCalculationOptions>._)).Returns(_venousBloodPK);
         A.CallTo(() => _pkAnalysisTask.CalculatePK(_peripheralVenousBloodPlasma, A<PKCalculationOptions>._)).Returns(_peripheralVenousBloodPK);
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
         return dataColumn;
      }
   }

   public class When_calculating_the_global_pk_analyes_parameter_such_as_VSS_VD_and_Plasma_CL_for_the_human_species : concern_for_GlobalPKAnalysisTask
   {
      private GlobalPKAnalysis _results;

      protected override void Context()
      {
         base.Context();
         var protocol = A.Fake<Protocol>();
         _compoundProperties.ProtocolProperties.Protocol = protocol;
         _species.Name = CoreConstants.Species.Human;


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
         _species.Name = CoreConstants.Species.Human;


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

   public class When_calculating_the_global_pk_analyes_parameter_such_as_VSS_VD_and_Plasma_CL_for_a_single_extra_vascular_with_previous_calculation_of_bioavailability : concern_for_GlobalPKAnalysisTask
   {
      private GlobalPKAnalysis _results;
      private double _bioaValue;

      protected override void Context()
      {
         base.Context();
         var protocol = A.Fake<Protocol>();
         _compoundProperties.ProtocolProperties.Protocol = protocol;
         _species.Name = CoreConstants.Species.Human;
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
         _species.Name = CoreConstants.Species.Mouse;
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
         _protocol.AddParameter(DomainHelperForSpecs.ConstantParameterWithValue(1).WithName(CoreConstants.Parameter.INPUT_DOSE));
         _protocol.AddParameter(DomainHelperForSpecs.ConstantParameterWithValue(1).WithName(Constants.Parameters.START_TIME));

         //single dosing
         var schemaItem = A.Fake<SchemaItem>();
         var inputDose = DomainHelperForSpecs.ConstantParameterWithValue(10);
         var startTime = DomainHelperForSpecs.ConstantParameterWithValue(3);
         A.CallTo(() => schemaItem.Dose).Returns(inputDose);
         A.CallTo(() => schemaItem.StartTime).Returns(startTime);
         _simulationSchemaItems.Add(schemaItem);
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
         _protocol.Parameter(CoreConstants.Parameter.INPUT_DOSE).Value.ShouldBeEqualTo(10);
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
         //single dosing
         _simulationSchemaItems.Add(new SchemaItem());
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
            DomainHelperForSpecs.ConstantParameterWithValue(1).WithName(Constants.PKParameters.C_max_tLast_tEnd),
            DomainHelperForSpecs.ConstantParameterWithValue(2).WithName(Constants.PKParameters.AUC_inf_tLast)
         };

         A.CallTo(() => _globalPKAnalysisRunner.RunForDDIRatio(_simulation)).Returns(_ddiRatioSimulation);
         A.CallTo(() => _pkAnalysisTask.CalculateFor(_ddiRatioSimulation, _peripheralVenousBloodPlasma)).Returns(new IndividualPKAnalysis(_peripheralVenousBloodPlasma, _pkAnalysis));
         //multiple dosing
         _simulationSchemaItems.AddRange(new[] {new SchemaItem(), new SchemaItem()});
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