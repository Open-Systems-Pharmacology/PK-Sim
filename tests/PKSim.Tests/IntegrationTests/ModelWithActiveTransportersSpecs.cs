using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Infrastructure;

namespace PKSim.IntegrationTests
{
   public abstract class When_creating_a_model_with_active_transporter : ContextForSimulationIntegration<ISimulationModelCreator>
   {
      protected const string _drugName = "Drug";
      protected const string _transportProcessName = "ActProc1";

      protected abstract string CompoundTransportName { get; }

      protected TransportType _transportType = TransportType.Passive;
      private IExecutionContext _context;
      private ICoreWorkspace _workspace;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _context = IoC.Resolve<IExecutionContext>();
         _workspace = IoC.Resolve<ICoreWorkspace>();
         _workspace.Project = new PKSimProject();
         const string transporterName = "Tr1";

         var individual = DomainFactoryForSpecs.CreateStandardIndividual();
         _workspace.Project.AddBuildingBlock(individual);

         var transportExpression = DomainFactoryForSpecs.CreateExpressionProfileAndAddToIndividual<IndividualTransporter>(individual, transporterName);
         var compound = DomainFactoryForSpecs.CreateStandardCompound().WithName(_drugName);

         var cloneManager = IoC.Resolve<ICloneManager>();
         var compoundProcessRepository = IoC.Resolve<ICompoundProcessRepository>();
         var transportProcess = cloneManager.Clone(compoundProcessRepository.ProcessByName(CompoundTransportName))
            .WithName(_transportProcessName);
         compound.AddProcess(transportProcess);

         var protocol = DomainFactoryForSpecs.CreateStandardIVBolusProtocol();
         
         new SetTransportTypeCommand(transportExpression.Molecule.DowncastTo<IndividualTransporter>(), _transportType, _context).Run(_context);
         
         _simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(individual, compound, protocol).DowncastTo<IndividualSimulation>();

         _simulation.CompoundPropertiesList.First()
            .Processes
            .TransportAndExcretionSelection
            .AddPartialProcessSelection(new ProcessSelection {ProcessName = transportProcess.Name, MoleculeName = transportExpression.MoleculeName });

         DomainFactoryForSpecs.AddModelToSimulation(_simulation);
         
      }

      protected ExplicitFormula GetSumFormulaFor(string neighborhoodName, string parameterName)
      {
         var nbhood = _simulation.Model.Neighborhoods.Container(neighborhoodName);
         var drugContainer = nbhood.Container(_drugName);
         var sumParameter = drugContainer.Parameter(parameterName);

         return sumParameter.Formula.DowncastTo<ExplicitFormula>();
      }

      protected void NeighborhoodShouldContainTransport(string neighborhoodName, string transportName)
      {
         var neighborhood = _simulation.Model.Neighborhoods.Container(neighborhoodName);
         var transportProcessContainer = neighborhood.Container(_drugName).Container(_transportProcessName);
         transportProcessContainer.ShouldNotBeNull();
         var activeTransports = transportProcessContainer.GetAllChildren<Transport>();

         activeTransports.ExistsByName(transportName).ShouldBeTrue();
      }

      protected void MassAbsorbedSegmentShouldHaveCorrectFormula()
      {
         var expectedAliases = new[]
         {
            "PassiveRates_para", "PassiveRates_trans",
            "ActiveRates_para_IN", "ActiveRates_trans_IN",
            "ActiveRates_para_OUT", "ActiveRates_trans_OUT"
         };

         var lumen = _simulation.Model.Root.Container("Organism").Container(CoreConstants.Organ.LUMEN);
         foreach (var segment in Constants.Compartment.LumenSegmentsDuodenumToRectum)
         {
            var massAbsorbedSegmentParameter = lumen.Container(segment).Container(_drugName).Parameter("Oral mass absorbed segment");
            massAbsorbedSegmentParameter.RHSFormula.ShouldNotBeNull();
            var formula = massAbsorbedSegmentParameter.RHSFormula.DowncastTo<ExplicitFormula>();
            formula.ShouldNotBeNull();
            expectedAliases.Each(alias => formula.FormulaString.Contains(alias).ShouldBeTrue());
         }
      }
   }

   public class When_creating_a_model_with_active_efflux_specific_MM : When_creating_a_model_with_active_transporter
   {
      protected override string CompoundTransportName => CoreConstantsForSpecs.Process.ACTIVE_TRANSPORT_SPECIFIC_MM;

      public override void GlobalContext()
      {
         _transportType = TransportType.Efflux;
         base.GlobalContext();
      }

      [Observation]
      public void Sum_of_passive_processes_to_duodenum_cell_should_have_one_reference()
      {
         var formula = GetSumFormulaFor("Lumen_duo_Duodenum_cell", "Sum of passive process rates");
         formula.ObjectReferences.Count.ShouldBeEqualTo(1);
      }

      [Observation]
      public void Sum_of_active_processes_to_duodenum_cell_should_have_one_reference()
      {
         var formula = GetSumFormulaFor("Lumen_duo_Duodenum_cell", "Sum of active process rates mucosa to lumen");
         formula.ObjectReferences.Count.ShouldBeEqualTo(1);
      }

      [Observation]
      public void Sum_of_active_processes_from_lumen_cell_should_have_zero_reference()
      {
         var formula = GetSumFormulaFor("Lumen_duo_Duodenum_cell", "Sum of active process rates lumen to mucosa");
         formula.ObjectReferences.Count.ShouldBeEqualTo(0);
      }

      [Observation]
      public void Sum_of_passive_processes_to_duodenum_plasma_should_have_one_reference()
      {
         var formula = GetSumFormulaFor("Lumen_duo_Duodenum_pls", "Sum of passive process rates");
         formula.ObjectReferences.Count.ShouldBeEqualTo(1);
      }

      [Observation]
      public void Sum_of_active_processes_to_duodenum_plasma_should_have_no_references()
      {
         var formula = GetSumFormulaFor("Lumen_duo_Duodenum_pls", "Sum of active process rates mucosa to lumen");
         formula.ObjectReferences.Count.ShouldBeEqualTo(0);
      }

      [Observation]
      public void Sum_of_active_processes_from_duodenum_plasma_should_have_no_references()
      {
         var formula = GetSumFormulaFor("Lumen_duo_Duodenum_pls", "Sum of active process rates lumen to mucosa");
         formula.ObjectReferences.Count.ShouldBeEqualTo(0);
      }

      [Observation]
      public void Should_create_active_efflux_specific_transport_in_bone()
      {
         NeighborhoodShouldContainTransport("Bone_int_Bone_cell", CoreConstantsForSpecs.ActiveTransport.ActiveEffluxSpecificIntracellularToInterstitial_MM);
      }

      [Observation]
      public void MassAbsorbed_for_each_GI_segment_should_have_correct_formula()
      {
         MassAbsorbedSegmentShouldHaveCorrectFormula();
      }
   }

   public class When_creating_a_model_with_active_influx_specific_MM : When_creating_a_model_with_active_transporter
   {
      protected override string CompoundTransportName => CoreConstantsForSpecs.Process.ACTIVE_TRANSPORT_SPECIFIC_MM;

      public override void GlobalContext()
      {
         _transportType = TransportType.Influx;
         base.GlobalContext();
      }

      [Observation]
      public void Sum_of_passive_processes_to_duodenum_cell_should_have_one_reference()
      {
         var formula = GetSumFormulaFor("Lumen_duo_Duodenum_cell", "Sum of passive process rates");
         formula.ObjectReferences.Count.ShouldBeEqualTo(1);
      }

      [Observation]
      public void Sum_of_active_processes_to_duodenum_cell_should_have_zero_reference()
      {
         var formula = GetSumFormulaFor("Lumen_duo_Duodenum_cell", "Sum of active process rates mucosa to lumen");
         formula.ObjectReferences.Count.ShouldBeEqualTo(0);
      }

      [Observation]
      public void Sum_of_active_processes_from_lumen_cell_should_have_one_reference()
      {
         var formula = GetSumFormulaFor("Lumen_duo_Duodenum_cell", "Sum of active process rates lumen to mucosa");
         formula.ObjectReferences.Count.ShouldBeEqualTo(1);
      }

      [Observation]
      public void Sum_of_passive_processes_to_duodenum_plasma_should_have_one_reference()
      {
         var formula = GetSumFormulaFor("Lumen_duo_Duodenum_pls", "Sum of passive process rates");
         formula.ObjectReferences.Count.ShouldBeEqualTo(1);
      }

      [Observation]
      public void Sum_of_active_processes_to_duodenum_plasma_should_have_no_references()
      {
         var formula = GetSumFormulaFor("Lumen_duo_Duodenum_pls", "Sum of active process rates mucosa to lumen");
         formula.ObjectReferences.Count.ShouldBeEqualTo(0);
      }

      [Observation]
      public void Sum_of_active_processes_from_duodenum_plasma_should_have_no_references()
      {
         var formula = GetSumFormulaFor("Lumen_duo_Duodenum_pls", "Sum of active process rates lumen to mucosa");
         formula.ObjectReferences.Count.ShouldBeEqualTo(0);
      }

      [Observation]
      public void Should_create_active_influx_specific_transport_in_bone()
      {
         NeighborhoodShouldContainTransport("Bone_int_Bone_cell", CoreConstantsForSpecs.ActiveTransport.ActiveInfluxSpecificInterstitialToIntracellular_MM);
      }

      [Observation]
      public void MassAbsorbed_for_each_GI_segment_should_have_correct_formula()
      {
         MassAbsorbedSegmentShouldHaveCorrectFormula();
      }
   }

   public class When_creating_model_with_active_transporter_Hill : When_creating_a_model_with_active_transporter
   {
      protected override string CompoundTransportName => CoreConstantsForSpecs.Process.ACTIVE_TRANSPORT_HILL;

      [Observation]
      public void Should_create_active_efflux_specific_with_competitive_inhibition_transport_in_bone()
      {
         NeighborhoodShouldContainTransport("Bone_int_Bone_cell", CoreConstantsForSpecs.ActiveTransport.ActiveEffluxSpecificIntracellularToInterstitial_Hill);
      }

      [Observation]
      public void MassAbsorbed_for_each_GI_segment_should_have_correct_formula()
      {
         MassAbsorbedSegmentShouldHaveCorrectFormula();
      }
   }
}