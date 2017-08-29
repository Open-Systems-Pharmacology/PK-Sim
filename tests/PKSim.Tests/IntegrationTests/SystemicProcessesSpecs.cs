using System.Linq;
using System.Threading.Tasks;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using NUnit.Framework;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Infrastructure;
using PKSim.Infrastructure.ProjectConverter;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_SystemicProcesses : ContextForSimulationIntegration<Simulation>
   {
      protected Compound _compound;
      protected ICompoundProcessRepository _compoundProcessRepository;
      protected ICloneManager _cloneManager;
      protected Individual _individual;
      protected Protocol _protocol;
      protected IParameterGroupTask _parameterGroupTask;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _compound = DomainFactoryForSpecs.CreateStandardCompound();
         _individual = DomainFactoryForSpecs.CreateStandardIndividual();
         _protocol = DomainFactoryForSpecs.CreateStandardIVBolusProtocol();
         _compoundProcessRepository = IoC.Resolve<ICompoundProcessRepository>();
         _parameterGroupTask = IoC.Resolve<IParameterGroupTask>();
         _cloneManager = IoC.Resolve<ICloneManager>();
      }
   }

   public class When_creating_a_simulation_with_a_kidney_clearance_defined_in_the_compound : concern_for_SystemicProcesses
   {
      private SystemicProcess _kidneyProcess;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _kidneyProcess = _cloneManager.Clone(_compoundProcessRepository.ProcessByName(CoreConstants.Process.KIDNEY_CLEARANCE).DowncastTo<SystemicProcess>());
         _kidneyProcess.Name = "My Kidney Process";
         _kidneyProcess.Parameter(ConverterConstants.Parameter.PlasmaClearance).Value = 10;
         _compound.AddProcess(_kidneyProcess);
         _simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(_individual, _compound, _protocol).DowncastTo<IndividualSimulation>();
         _simulation.CompoundPropertiesList.First()
            .Processes
            .TransportAndExcretionSelection
            .AddSystemicProcessSelection(new SystemicProcessSelection {ProcessName = _kidneyProcess.Name, ProcessType = _kidneyProcess.SystemicProcessType});

         DomainFactoryForSpecs.AddModelToSimulation(_simulation);
      }

      [Observation]
      public void the_created_simulation_should_have_a_kdiney_clearance_process_created_based_on_the_one_defined_in_the_compound()
      {
         var allProcessParameters = _parameterGroupTask.ParametersInTopGroup(CoreConstants.Groups.COMPOUND_PROCESSES, _simulation.All<IParameter>());
         allProcessParameters.Select(x => x.ParentContainer.Name).Distinct().ShouldOnlyContain(_kidneyProcess.Name);

         allProcessParameters.FindByName(ConverterConstants.Parameter.PlasmaClearance).Value.ShouldBeEqualTo(_kidneyProcess.Parameter(ConverterConstants.Parameter.PlasmaClearance).Value);
      }
   }

   public class When_creating_a_simulation_with_a_GFR_clearance_defined_in_the_compound : concern_for_SystemicProcesses
   {
      private SystemicProcess _processGFR;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _processGFR = _cloneManager.Clone(_compoundProcessRepository.ProcessByName(CoreConstantsForSpecs.Process.GLOMERULAR_FILTRATION).DowncastTo<SystemicProcess>());
         _processGFR.Name = "My GFR Process";
         _processGFR.Parameter(CoreConstants.Parameter.GFR_FRACTION).Value = 0.8;
         _compound.AddProcess(_processGFR);
         _simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(_individual, _compound, _protocol).DowncastTo<IndividualSimulation>();
         _simulation.CompoundPropertiesList.First()
            .Processes
            .TransportAndExcretionSelection
            .AddSystemicProcessSelection(new SystemicProcessSelection { ProcessName = _processGFR.Name, ProcessType = _processGFR.SystemicProcessType });

         DomainFactoryForSpecs.AddModelToSimulation(_simulation);
      }

      [Observation]
      public void the_created_simulation_should_have_a_kdiney_GFR_clearance_process_created_based_on_the_one_defined_in_the_compound()
      {
         var allProcessParameters = _parameterGroupTask.ParametersInTopGroup(CoreConstants.Groups.COMPOUND_PROCESSES, _simulation.All<IParameter>());
         allProcessParameters.Select(x => x.ParentContainer.Name).Distinct().ShouldOnlyContain(_processGFR.Name);

         allProcessParameters.FindByName(CoreConstants.Parameter.GFR_FRACTION).Value.ShouldBeEqualTo(_processGFR.Parameter(CoreConstants.Parameter.GFR_FRACTION).Value);
      }
   }

   public class When_creating_a_simulation_with_a_biliary_clearance_defined_in_the_compound : concern_for_SystemicProcesses
   {
      private SystemicProcess _biliaryClearance;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _biliaryClearance = _cloneManager.Clone(_compoundProcessRepository.ProcessByName(CoreConstantsForSpecs.Process.BILIARY_CLEARANCE).DowncastTo<SystemicProcess>());
         _biliaryClearance.Name = "My Biliary Process";
         _biliaryClearance.Parameter(ConverterConstants.Parameter.PlasmaClearance).Value = 10;
         _compound.AddProcess(_biliaryClearance);
         _simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(_individual, _compound, _protocol).DowncastTo<IndividualSimulation>();
         _simulation.CompoundPropertiesList.First()
            .Processes
            .TransportAndExcretionSelection
            .AddSystemicProcessSelection(new SystemicProcessSelection {ProcessName = _biliaryClearance.Name, ProcessType = _biliaryClearance.SystemicProcessType});

         DomainFactoryForSpecs.AddModelToSimulation(_simulation);
      }

      [Observation]
      public void the_created_simulation_should_have_a_biliary_clearance_process_created_based_on_the_one_defined_in_the_compound()
      {
         var allProcessParameters = _parameterGroupTask.ParametersInTopGroup(CoreConstants.Groups.COMPOUND_PROCESSES, _simulation.All<IParameter>());
         var allProcessContainer = allProcessParameters.Select(x => x.ParentContainer).Distinct();
         var allProcessNames = allProcessContainer.Select(x => x.Name);
         allProcessNames.ShouldOnlyContain(_biliaryClearance.Name);

         allProcessParameters.FindByName(ConverterConstants.Parameter.PlasmaClearance).Value.ShouldBeEqualTo(_biliaryClearance.Parameter(ConverterConstants.Parameter.PlasmaClearance).Value);
      }

      [Observation]
      public void the_biliary_transport_should_have_been_induced_by_an_undefined_transporter_in_liver()
      {
         var individualInSim = _simulation.BuildingBlock<Individual>();
         var molecule = individualInSim.MoleculeByName<IndividualTransporter>(CoreConstants.Molecule.UndefinedLiverTransporter);
         molecule.ShouldNotBeNull();
      }

      [Observation]
      public void should_create_biliary_clearance_to_gallbladder_transport()
      {
         biliaryClearancePeriportalCellToGallBladderTransport().ShouldNotBeNull();
      }

      [Observation]
      public void the_created_transport_should_reference_undefined_transport_concentration_in_periportal_liver()
      {
         var transport = biliaryClearancePeriportalCellToGallBladderTransport();
         var undefinedTransportRef = transport.Formula.ObjectPaths.First(x => x.Alias == "CP");
         undefinedTransportRef.ShouldContain(CoreConstants.Organ.Liver, CoreConstants.Compartment.Periportal);
      }

      private ITransport biliaryClearancePeriportalCellToGallBladderTransport()
      {
         return _simulation.Model.Neighborhoods.EntityAt<ITransport>(CoreConstantsForSpecs.Neigborhood.PERIPORTAL_CELL_GALLBLADDER,
            _compound.Name,_biliaryClearance.Name,  CoreConstants.Process.BILIARY_CLEARANCE_TO_GALL_BLADDER);
      }
   }

   public class When_creating_a_simulation_with_a_liver_clearance_defined_in_the_compound : concern_for_SystemicProcesses
   {
      private SystemicProcess _liverClearance;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _liverClearance = _cloneManager.Clone(_compoundProcessRepository.ProcessByName(CoreConstantsForSpecs.Process.LIVER_CLEARANCE).DowncastTo<SystemicProcess>());
         _liverClearance.Name = "My Liver Process";
         _liverClearance.Parameter(ConverterConstants.Parameter.PlasmaClearance).Value = 10;
         _compound.AddProcess(_liverClearance);
         _simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(_individual, _compound, _protocol).DowncastTo<IndividualSimulation>();
         _simulation.CompoundPropertiesFor(_compound.Name)
            .Processes
            .MetabolizationSelection
            .AddSystemicProcessSelection(new SystemicProcessSelection {ProcessName = _liverClearance.Name, ProcessType = _liverClearance.SystemicProcessType});

         DomainFactoryForSpecs.AddModelToSimulation(_simulation);
      }

      [Observation]
      public void the_created_simulation_should_have_a_biliary_clearance_process_created_based_on_the_one_defined_in_the_compound()
      {
         var allProcessParameters = _parameterGroupTask.ParametersInTopGroup(CoreConstants.Groups.COMPOUND_PROCESSES, _simulation.All<IParameter>()).ToList();
         allProcessParameters.Select(x => x.ParentContainer.Name).Distinct().ShouldOnlyContain(CoreConstants.CompositeNameFor(_compound.Name,_liverClearance.Name));

         allProcessParameters.FindByName(ConverterConstants.Parameter.PlasmaClearance).Value.ShouldBeEqualTo(_liverClearance.Parameter(ConverterConstants.Parameter.PlasmaClearance).Value);
      }

      [Observation]
      public void the_created_process_kinetic_should_reference_an_undefined_enzyme_in_liver_whose_start_concentration_is_set_to_1_by_f_cell()
      {
         var liver_periportal = _simulation.Model.Root.EntityAt<Container>(Constants.ORGANISM, CoreConstants.Organ.Liver, CoreConstants.Compartment.Periportal);
         var startConcentration = liver_periportal.EntityAt<IParameter>(CoreConstants.Compartment.Intracellular, CoreConstants.Molecule.UndefinedLiver, CoreConstants.Parameter.CONCENTRATION);

         var f_cell = liver_periportal.EntityAt<IParameter>(CoreConstants.Parameter.FractionIntracellular);
         startConcentration.Value.ShouldBeEqualTo(1 / f_cell.Value);
      }

      [Observation]
      public void the_created_process_should_have_two_parameters_enzyme_concentration_and_cl_spec_per_enzyme_hidden_set_to_one()
      {
         var processName = CoreConstants.CompositeNameFor(_compound.Name, _liverClearance.Name);
         var processContainer = _simulation.Model.Root.EntityAt<Container>(processName);
         processContainer.Parameter(CoreConstantsForSpecs.Parameter.ENZYME_CONCENTRATION).Value.ShouldBeEqualTo(1);
         processContainer.Parameter(CoreConstantsForSpecs.Parameter.ENZYME_CONCENTRATION).Visible.ShouldBeFalse();

         processContainer.Parameter(CoreConstantsForSpecs.Parameter.CL_SPEC_PER_ENZYME).Value.ShouldBeEqualTo(processContainer.Parameter(CoreConstants.Parameter.SPECIFIC_CLEARANCE).Value);
         processContainer.Parameter(CoreConstantsForSpecs.Parameter.CL_SPEC_PER_ENZYME).Visible.ShouldBeFalse();
      }

      [Observation]
      public void the_clearance_process_should_have_been_induced_by_an_undefined_enzyme_in_liver()
      {
         var individualInSim = _simulation.BuildingBlock<Individual>();
         var molecule = individualInSim.MoleculeByName<IndividualEnzyme>(CoreConstants.Molecule.UndefinedLiver);
         molecule.ShouldNotBeNull();
      }
   }

   public class When_creating_a_simulation_with_two_compounds_using_a_systemic_process_with_the_same_name : concern_for_SystemicProcesses
   {
      private Compound _otherCompound;
      private Protocol _otherProtocol;

      public override void GlobalContext()
      {
         base.GlobalContext();

         _otherCompound = DomainFactoryForSpecs.CreateStandardCompound().WithName("OtherCompound");
         _otherProtocol = DomainFactoryForSpecs.CreateStandardIVBolusProtocol().WithName("OtherProtocol");
        var liverClearance= addLiverPlasmaClearanceTo(_compound);
         addLiverPlasmaClearanceTo(_otherCompound);

         _simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(_individual, new[] { _compound, _otherCompound }, new[] { _protocol, _otherProtocol, })
                             .DowncastTo<IndividualSimulation>();

         _simulation.CompoundPropertiesFor(_compound.Name)
            .Processes
            .MetabolizationSelection
            .AddSystemicProcessSelection(new SystemicProcessSelection {CompoundName  =_compound.Name,  ProcessName = liverClearance.Name, ProcessType = liverClearance.SystemicProcessType });

         _simulation.CompoundPropertiesFor(_otherCompound.Name)
             .Processes
             .MetabolizationSelection
             .AddSystemicProcessSelection(new SystemicProcessSelection { CompoundName = _otherProtocol.Name, ProcessName = liverClearance.Name, ProcessType = liverClearance.SystemicProcessType });


         DomainFactoryForSpecs.AddModelToSimulation(_simulation);
      }

      private SystemicProcess addLiverPlasmaClearanceTo(Compound compound)
      {
         var liverClearance = _cloneManager.Clone(_compoundProcessRepository.ProcessByName(CoreConstantsForSpecs.Process.LIVER_CLEARANCE).DowncastTo<SystemicProcess>());
         liverClearance.Name = "My Liver Process";
         liverClearance.Parameter(ConverterConstants.Parameter.PlasmaClearance).Value = 10;
         compound.AddProcess(liverClearance);
         return liverClearance;
      }

      [Observation]
      public async Task should_be_able_to_create_and_run_the_simulation()
      {
         var simulationEngine = IoC.Resolve<ISimulationEngine<IndividualSimulation>>();
         await simulationEngine.RunAsync(_simulation, new Core.Services.SimulationRunOptions());
         _simulation.HasResults.ShouldBeTrue();
      }

   }

   public abstract class When_creating_a_simulation_with_an_inVitro_process_defined_in_the_compound : concern_for_SystemicProcesses
   {
      private SystemicProcess _process;

      protected abstract string ProcessName { get; }

      public override void GlobalContext()
      {
         base.GlobalContext();
         _process = _cloneManager.Clone(_compoundProcessRepository.ProcessByName(ProcessName).DowncastTo<SystemicProcess>());
         _process.Name = "My Liver Process";
         _process.Parameter(ConverterConstants.Parameter.CLspec).Value = 1;
         _compound.AddProcess(_process);
         _simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(_individual, _compound, _protocol).DowncastTo<IndividualSimulation>();
         _simulation.CompoundPropertiesFor(_compound.Name)
            .Processes
            .MetabolizationSelection
            .AddSystemicProcessSelection(new SystemicProcessSelection { ProcessName = _process.Name, ProcessType = _process.SystemicProcessType });

         DomainFactoryForSpecs.AddModelToSimulation(_simulation);
      }

      protected void CheckProcess()
      {
         var allProcessParameters = _parameterGroupTask.ParametersInTopGroup(CoreConstants.Groups.COMPOUND_PROCESSES, _simulation.All<IParameter>()).ToList();
         allProcessParameters.Select(x => x.ParentContainer.Name).Distinct().ShouldOnlyContain(CoreConstants.CompositeNameFor(_compound.Name, _process.Name));

         allProcessParameters.FindByName(ConverterConstants.Parameter.CLspec).Value.ShouldBeEqualTo(_process.Parameter(ConverterConstants.Parameter.CLspec).Value);         
      }
   }


   public class When_creating_a_simulation_with_an_InVitro_Hepatocytes_tHalf_process_defined_in_the_compound : When_creating_a_simulation_with_an_inVitro_process_defined_in_the_compound
   {
      [Observation]
      public void should_create_a_clearance_process_with_correct_value_of_specific_clearance()
      {
         CheckProcess();
      }

      protected override string ProcessName => CoreConstantsForSpecs.Process.HEPATOCYTESHALFTIME;
   }

   public class When_creating_a_simulation_with_an_InVitro_Hepatocytes_ResidualFraction_process_defined_in_the_compound : When_creating_a_simulation_with_an_inVitro_process_defined_in_the_compound
   {
      [Observation]
      public void should_create_a_clearance_process_with_correct_value_of_specific_clearance()
      {
         CheckProcess();
      }

      protected override string ProcessName => CoreConstantsForSpecs.Process.HEPATOCYTESRES;
   }

   public class When_creating_a_simulation_with_an_InVitro_Microsomes_tHalf_process_defined_in_the_compound : When_creating_a_simulation_with_an_inVitro_process_defined_in_the_compound
   {
      [Observation]
      public void should_create_a_clearance_process_with_correct_value_of_specific_clearance()
      {
         CheckProcess();
      }

      protected override string ProcessName => CoreConstantsForSpecs.Process.LIVERMICROSOMEHALFTIME;
   }

   public class When_creating_a_simulation_with_an_InVitro_Microsomes_ResidualFraction_process_defined_in_the_compound : When_creating_a_simulation_with_an_inVitro_process_defined_in_the_compound
   {
      [Observation]
      public void should_create_a_clearance_process_with_correct_value_of_specific_clearance()
      {
         CheckProcess();
      }

      protected override string ProcessName => CoreConstantsForSpecs.Process.LIVERMICROSOMERES;
   }

}