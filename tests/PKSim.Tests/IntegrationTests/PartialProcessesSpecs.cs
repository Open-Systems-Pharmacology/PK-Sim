using System.Linq;
using System.Threading.Tasks;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Infrastructure;
using PKSim.Infrastructure.ProjectConverter;
using SimulationRunOptions = PKSim.Core.Services.SimulationRunOptions;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_PartialProcesses : ContextForSimulationIntegration<Simulation>
   {
      protected ICompoundProcessRepository _compoundProcessRepository;
      protected ICloneManager _cloneManager;
      protected Individual _individual;
      protected Compound _compound;
      protected Protocol _protocol;
      protected IIndividualEnzymeTask _enzymeTask;
      protected IIndividualTransporterTask _transporterTask;
      protected double _relExpPls = 0.2;
      protected double _relExpBloodCells = 0.5;
      protected double _relExpVascEndo = 0.3;
      protected IndividualEnzyme _enzyme;
      protected double _hct = 0.6;
      protected PartialProcess _metabolizationProcess;
      protected IModelPropertiesTask _modelPropertiesTask;
      protected IModelConfigurationRepository _modelConfigurationRepository;
      protected SimulationRunOptions _simulationRunOptions;
      protected ICache<string, IParameter> _allExpressionParameters;
      protected const double _relExpDuo = 0.2;
      protected const double _relExpBone = 0.3;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _compoundProcessRepository = IoC.Resolve<ICompoundProcessRepository>();
         _cloneManager = IoC.Resolve<ICloneManager>();
         _enzymeTask = IoC.Resolve<IIndividualEnzymeTask>();
         _transporterTask = IoC.Resolve<IIndividualTransporterTask>();
         _modelPropertiesTask = IoC.Resolve<IModelPropertiesTask>();
         _modelConfigurationRepository = IoC.Resolve<IModelConfigurationRepository>();
         _compound = DomainFactoryForSpecs.CreateStandardCompound();
         _individual = DomainFactoryForSpecs.CreateStandardIndividual();
         _protocol = DomainFactoryForSpecs.CreateStandardIVBolusProtocol();
         _enzyme = _enzymeTask.AddMoleculeTo(_individual, "CYP").DowncastTo<IndividualEnzyme>();
         _allExpressionParameters = _individual.AllExpressionParametersFor(_enzyme);
         _allExpressionParameters[CoreConstants.Compartment.Plasma].Value = _relExpPls;
         _allExpressionParameters[CoreConstants.Compartment.BloodCells].Value = _relExpBloodCells;
         _allExpressionParameters[CoreConstants.Compartment.VascularEndothelium].Value = _relExpVascEndo;
         _hct = _individual.Organism.Parameter(CoreConstants.Parameters.HCT).Value;
         _metabolizationProcess = _cloneManager.Clone(_compoundProcessRepository
            .ProcessByName(CoreConstantsForSpecs.Process.METABOLIZATION_SPECIFIC_FIRST_ORDER).DowncastTo<PartialProcess>());
         _metabolizationProcess.Name = "My Partial Process";
         _metabolizationProcess.Parameter(ConverterConstants.Parameters.CLspec).Value = 15;
         _compound.AddProcess(_metabolizationProcess);
         _simulationRunOptions = new SimulationRunOptions {RaiseEvents = false};
      }
   }


   public class
      When_creating_a_simulation_with_an_individual_containing_an_enzyme_localized_on_the_extracellular_membrane_basolateral_and_a_partial_process_in_compound :
         concern_for_PartialProcesses
   {
      public override void GlobalContext()
      {
         base.GlobalContext();

         _allExpressionParameters[CoreConstants.Organ.Bone].Value = _relExpBone;
         _allExpressionParameters[CoreConstants.Compartment.Duodenum].Value = _relExpDuo;

         _simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(_individual, _compound, _protocol)
            .DowncastTo<IndividualSimulation>();
         _simulation.CompoundPropertiesList.First()
            .Processes
            .MetabolizationSelection
            .AddPartialProcessSelection(new EnzymaticProcessSelection {ProcessName = _metabolizationProcess.Name, MoleculeName = _enzyme.Name});

         DomainFactoryForSpecs.AddModelToSimulation(_simulation);
      }

      [Observation]
      public void the_value_of_the_relative_expression_in_organ_should_have_been_set_to_the_value_defined_in_the_enzyme()
      {
         var allRelExp = _simulation.All<IMoleculeAmount>()
            .Where(x => x.Name.Equals(_enzyme.Name))
            .Where(x => x.ParentContainer.Name.Equals(CoreConstants.Compartment.Intracellular))
            .Select(x => x.Parameter(CoreConstants.Parameters.REL_EXP));

         foreach (var parameter in allRelExp)
         {
            var grandparent = parameter.ParentContainer.ParentContainer.ParentContainer;
            if (grandparent.Name.Equals(CoreConstants.Organ.Bone))
               parameter.Value.ShouldBeEqualTo(_relExpBone);

            else if (grandparent.Name.Equals(CoreConstants.Compartment.Duodenum))
               parameter.Value.ShouldBeEqualTo(_relExpDuo);
            else
               parameter.Value.ShouldBeEqualTo(0);
         }
      }

      [Observation]
      public void the_reference_concentration_parameter_should_be_marked_as_can_be_varied_in_the_simulation()
      {
         var refConc = _simulation.Model.Root.Container("CYP").Parameter(CoreConstants.Parameters.REFERENCE_CONCENTRATION);
         refConc.CanBeVaried.ShouldBeTrue();
         refConc.CanBeVariedInPopulation.ShouldBeTrue();
      }
   }

   public class
      When_creating_a_simulation_with_an_individual_containing_an_enzyme_localized_in_intracellular_with_location_in_vasc_endothelium_is_interstial_and_a_partial_process_in_compound :
         concern_for_PartialProcesses
   {
      public override void GlobalContext()
      {
         base.GlobalContext();

         _allExpressionParameters[CoreConstants.Organ.Bone].Value = _relExpBone;
         _allExpressionParameters[CoreConstants.Compartment.Duodenum].Value = _relExpDuo;

         _simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(_individual, _compound, _protocol)
            .DowncastTo<IndividualSimulation>();
         _simulation.CompoundPropertiesList.First()
            .Processes
            .MetabolizationSelection
            .AddPartialProcessSelection(new EnzymaticProcessSelection {ProcessName = _metabolizationProcess.Name, MoleculeName = _enzyme.Name});

         DomainFactoryForSpecs.AddModelToSimulation(_simulation);
      }

      [Observation]
      public void the_value_of_the_relative_expression_in_organ_should_have_been_set_to_the_value_defined_in_the_enzyme()
      {
         var allRelExpNorm = _simulation.All<IMoleculeAmount>()
            .Where(x => x.Name.Equals(_enzyme.Name))
            .Where(x => x.ParentContainer.Name.Equals(CoreConstants.Compartment.Intracellular))
            .Select(x => x.Parameter(CoreConstants.Parameters.REL_EXP));

         foreach (var parameter in allRelExpNorm)
         {
            var grandparent = parameter.ParentContainer.ParentContainer.ParentContainer;
            if (grandparent.Name.Equals(CoreConstants.Organ.Bone))
               parameter.Value.ShouldBeEqualTo(_relExpBone);

            else if (grandparent.Name.Equals(CoreConstants.Compartment.Duodenum))
               parameter.Value.ShouldBeEqualTo(_relExpDuo);
            else
               parameter.Value.ShouldBeEqualTo(0);
         }
      }
   }

   public class When_creating_a_simulation_with_an_individual_containing_an_enzyme_localized_in_interstitial_and_a_partial_process_in_compound :
      concern_for_PartialProcesses
   {
      public override void GlobalContext()
      {
         base.GlobalContext();

         _enzyme.TissueLocation = TissueLocation.Interstitial;
         _allExpressionParameters[CoreConstants.Organ.Bone].Value = _relExpBone;
         _allExpressionParameters[CoreConstants.Compartment.Duodenum].Value = _relExpDuo;

         _simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(_individual, _compound, _protocol)
            .DowncastTo<IndividualSimulation>();

         _simulation.CompoundPropertiesList.First()
            .Processes
            .MetabolizationSelection
            .AddPartialProcessSelection(new EnzymaticProcessSelection {ProcessName = _metabolizationProcess.Name, MoleculeName = _enzyme.Name});

         DomainFactoryForSpecs.AddModelToSimulation(_simulation);
      }

      [Observation]
      public void the_value_of_the_relative_expression_in_organ_should_have_been_set_to_the_value_defined_in_the_enzyme()
      {
         var allRelExp = _simulation.All<IMoleculeAmount>()
            .Where(x => x.Name.Equals(_enzyme.Name))
            .Where(x => x.ParentContainer.Name.Equals(CoreConstants.Compartment.Intracellular))
            .Select(x => x.Parameter(CoreConstants.Parameters.REL_EXP));

         foreach (var parameter in allRelExp)
         {
            var grandparent = parameter.ParentContainer.ParentContainer.ParentContainer;
            if (grandparent.Name.Equals(CoreConstants.Organ.Bone))
               parameter.Value.ShouldBeEqualTo(_relExpBone);

            else if (grandparent.Name.Equals(CoreConstants.Compartment.Duodenum))
               parameter.Value.ShouldBeEqualTo(_relExpDuo); 
            else
               parameter.Value.ShouldBeEqualTo(0);
         }
      }
   }

   public class
      When_creating_a_simulation_with_an_individual_containing_an_enzyme_with_intracellular_location_endosomal_with_the_two_pore_model :
         concern_for_PartialProcesses
   {
      public override void GlobalContext()
      {
         base.GlobalContext();

         var modelConfig = _modelConfigurationRepository.AllFor(_individual.Species).First(x => x.ModelName == CoreConstants.Model.TWO_PORES);
         var twoPoreModelProperties = _modelPropertiesTask.DefaultFor(modelConfig, _individual.OriginData);

         _compound.Parameter(Constants.Parameters.IS_SMALL_MOLECULE).Value = 0;
         _simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(_individual, _compound, _protocol, twoPoreModelProperties)
            .DowncastTo<IndividualSimulation>();

         _simulation.CompoundPropertiesList.First()
            .Processes
            .MetabolizationSelection
            .AddPartialProcessSelection(new EnzymaticProcessSelection {ProcessName = _metabolizationProcess.Name, MoleculeName = _enzyme.Name});

         DomainFactoryForSpecs.AddModelToSimulation(_simulation);
      }

      [Observation]
      public void should_have_created_the_enzyme_in_all_endosome_compartment()
      {
         var allContainerWithEnzyme = _simulation.All<IMoleculeAmount>().Where(x => x.Name.Equals(_enzyme.Name)).Select(x => x.ParentContainer);
         allContainerWithEnzyme.Select(x => x.Name).Distinct().Contains(CoreConstants.Compartment.Endosome).ShouldBeTrue();
      }
   }

   public class When_creating_a_transporter_for_brain_BBB_influx : concern_for_PartialProcesses
   {
      private IndividualTransporter _transporter;
      private ITransporterContainerTemplateRepository _transporterContainerTemplateRepository;
      private PartialProcess _transportProcess;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _transporterContainerTemplateRepository = IoC.Resolve<ITransporterContainerTemplateRepository>();
         var allTransporters = _transporterContainerTemplateRepository.TransportersFor(_individual.Species.Name, CoreConstants.Organ.Brain);

         var influxBBB = allTransporters.Where(x => x.MembraneLocation == MembraneLocation.BloodBrainBarrier)
            .FirstOrDefault(x => x.TransportType == TransportType.Influx);

         _transporter = _transporterTask.CreateFor(_individual, "TRANS", TransportType.Efflux).DowncastTo<IndividualTransporter>();
         //TODO    var transportContainer = _transporter.ExpressionContainer(CoreConstants.Organ.Brain).DowncastTo<TransporterExpressionContainer>();
         TransporterExpressionContainer transportContainer = null;

         transportContainer.UpdatePropertiesFrom(influxBBB);
         _individual.AddMolecule(_transporter);
         _transportProcess = _cloneManager.Clone(_compoundProcessRepository.ProcessByName(CoreConstantsForSpecs.Process.ACTIVE_TRANSPORT_SPECIFIC_MM)
            .DowncastTo<PartialProcess>());
         _transportProcess.Name = "My Transport Process";
         _compound.AddProcess(_transportProcess);

         _simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(_individual, _compound, _protocol)
            .DowncastTo<IndividualSimulation>();

         _simulation.CompoundPropertiesList.First()
            .Processes
            .TransportAndExcretionSelection
            .AddPartialProcessSelection(new EnzymaticProcessSelection
               {CompoundName = _compound.Name, ProcessName = _transportProcess.Name, MoleculeName = _transporter.Name});

         DomainFactoryForSpecs.AddModelToSimulation(_simulation);
      }

      [Observation]
      public void should_have_created_a_simulation_and_a_transporter_in_brain_plasma()
      {
         var allContainerWithTransporter =
            _simulation.All<IMoleculeAmount>().Where(x => x.Name.Equals(_transporter.Name)).Select(x => x.ParentContainer);
         allContainerWithTransporter.Select(x => x.Name).Distinct().ShouldContain(CoreConstants.Compartment.Plasma);
      }
   }

   public class When_simulating_a_simulation_with_a_metabolization_process_in_liver : concern_for_PartialProcesses
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         _allExpressionParameters[CoreConstants.Compartment.Plasma].Value = 0;
         _allExpressionParameters[CoreConstants.Compartment.BloodCells].Value = 0;
         _allExpressionParameters[CoreConstants.Compartment.VascularEndothelium].Value = 0;
         _allExpressionParameters[CoreConstants.Compartment.Periportal].Value = 1;

         _simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(_individual, _compound, _protocol)
            .DowncastTo<IndividualSimulation>();

         _simulation.CompoundPropertiesList.First()
            .Processes
            .MetabolizationSelection
            .AddPartialProcessSelection(new EnzymaticProcessSelection
               {CompoundName = _compound.Name, ProcessName = _metabolizationProcess.Name, MoleculeName = _enzyme.Name});

         DomainFactoryForSpecs.AddModelToSimulation(_simulation);

         var objectPatFactory = new ObjectPathFactoryForSpecs();
         var path = objectPatFactory.CreateObjectPathFrom(Constants.ORGANISM,
            CoreConstants.Organ.Liver,
            CoreConstants.Compartment.Pericentral,
            CoreConstants.Compartment.Intracellular,
            CoreConstants.Molecule.ProcessProductName(_compound.Name, _enzyme.Name, CoreConstants.Molecule.Metabolite),
            CoreConstants.Observer.ObserverNameFrom(CoreConstants.Observer.FRACTION_OF_DOSE, _compound.Name));

         var quantitySelection = new QuantitySelection(path.ToString(), QuantityType.Metabolite);
         _simulation.OutputSelections.AddOutput(quantitySelection);
      }

      [Observation]
      public async Task should_be_able_to_retrieve_the_fraction_metabolized_in_liver_intracellular()
      {
         var simulationEngine = IoC.Resolve<IIndividualSimulationEngine>();
         await simulationEngine.RunAsync(_simulation, _simulationRunOptions);

         _simulation.HasResults.ShouldBeTrue();

         var observerColumn = _simulation.DataRepository.Where(col => col.DataInfo.Origin == ColumnOrigins.Calculation)
            .Where(col => col.QuantityInfo.Type.Is(QuantityType.Metabolite))
            .Where(col => col.QuantityInfo.Path.Contains(CoreConstants.Organ.Liver))
            .FirstOrDefault(col => col.QuantityInfo.Path.Contains(CoreConstants.Compartment.Intracellular));

         observerColumn.ShouldNotBeNull();
      }
   }

   public class When_creating_a_simulation_with_two_compounds_using_a_partial_process_with_the_same_name : concern_for_PartialProcesses
   {
      private Compound _otherCompound;
      private Protocol _otherProtocol;

      public override void GlobalContext()
      {
         base.GlobalContext();

         _otherCompound = DomainFactoryForSpecs.CreateStandardCompound().WithName("OtherCompound");
         _otherProtocol = DomainFactoryForSpecs.CreateStandardIVBolusProtocol().WithName("OtherProtocol");
         _otherCompound.AddProcess(_cloneManager.Clone(_metabolizationProcess));

         _simulation = DomainFactoryForSpecs
            .CreateModelLessSimulationWith(_individual, new[] {_compound, _otherCompound}, new[] {_protocol, _otherProtocol,})
            .DowncastTo<IndividualSimulation>();

         _simulation.CompoundPropertiesFor(_compound.Name)
            .Processes
            .MetabolizationSelection
            .AddPartialProcessSelection(new EnzymaticProcessSelection
               {CompoundName = _compound.Name, ProcessName = _metabolizationProcess.Name, MoleculeName = _enzyme.Name});

         _simulation.CompoundPropertiesFor(_otherCompound.Name)
            .Processes
            .MetabolizationSelection
            .AddPartialProcessSelection(new EnzymaticProcessSelection
               {CompoundName = _otherCompound.Name, ProcessName = _metabolizationProcess.Name, MoleculeName = _enzyme.Name});

         DomainFactoryForSpecs.AddModelToSimulation(_simulation);
      }

      [Observation]
      public async Task should_be_able_to_create_and_run_the_simulation()
      {
         var simulationEngine = IoC.Resolve<IIndividualSimulationEngine>();
         await simulationEngine.RunAsync(_simulation, _simulationRunOptions);
         _simulation.HasResults.ShouldBeTrue();
      }
   }

   public class When_creating_a_simulation_using_an_irreversible_inhibition : concern_for_PartialProcesses
   {
      private InhibitionProcess _irreversibleProcess;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _irreversibleProcess = _cloneManager.Clone(_compoundProcessRepository.ProcessByName(CoreConstantsForSpecs.Process.IRREVERSIBLE_INHIBITION)
            .DowncastTo<InhibitionProcess>());
         _irreversibleProcess.Name = "IrreversibleProcess";
         _irreversibleProcess.Parameter(CoreConstantsForSpecs.Parameter.KINACT).Value = 2;
         _compound.AddProcess(_irreversibleProcess);

         _simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(_individual, new[] {_compound}, new[] {_protocol})
            .DowncastTo<IndividualSimulation>();

         _simulation.CompoundPropertiesFor(_compound.Name)
            .Processes
            .MetabolizationSelection
            .AddPartialProcessSelection(new EnzymaticProcessSelection
               {CompoundName = _compound.Name, ProcessName = _metabolizationProcess.Name, MoleculeName = _enzyme.Name});


         _simulation.InteractionProperties.AddInteraction(new InteractionSelection
            {CompoundName = _compound.Name, MoleculeName = _enzyme.Name, ProcessName = _irreversibleProcess.Name});
         DomainFactoryForSpecs.AddModelToSimulation(_simulation);
      }

      [Observation]
      public async Task should_be_able_to_create_and_run_the_simulation()
      {
         var simulationEngine = IoC.Resolve<IIndividualSimulationEngine>();
         await simulationEngine.RunAsync(_simulation, _simulationRunOptions);
         _simulation.HasResults.ShouldBeTrue();
      }
   }

   public class When_creating_a_simulation_using_an_induction_interaction : concern_for_PartialProcesses
   {
      private InductionProcess _induction;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _induction = _cloneManager.Clone(_compoundProcessRepository.ProcessByName(CoreConstantsForSpecs.Process.INDUCTION)
            .DowncastTo<InductionProcess>());
         _induction.Name = "Induction";
         _induction.Parameter(CoreConstantsForSpecs.Parameter.EC50).Value = 10;
         _compound.AddProcess(_induction);

         _simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(_individual, new[] {_compound}, new[] {_protocol})
            .DowncastTo<IndividualSimulation>();

         _simulation.CompoundPropertiesFor(_compound.Name)
            .Processes
            .MetabolizationSelection
            .AddPartialProcessSelection(new EnzymaticProcessSelection
               {CompoundName = _compound.Name, ProcessName = _metabolizationProcess.Name, MoleculeName = _enzyme.Name});


         _simulation.InteractionProperties.AddInteraction(new InteractionSelection
            {CompoundName = _compound.Name, MoleculeName = _enzyme.Name, ProcessName = _induction.Name});
         DomainFactoryForSpecs.AddModelToSimulation(_simulation);
      }

      [Observation]
      public void should_be_able_to_create_and_run_the_simulation()
      {
         var simulationEngine = IoC.Resolve<IIndividualSimulationEngine>();
         var mobiExportTask = IoC.Resolve<IMoBiExportTask>();
         simulationEngine.RunAsync(_simulation, _simulationRunOptions).Wait();
         _simulation.HasResults.ShouldBeTrue();
      }
   }

   public class When_creating_a_simulation_using_an_induction_and_an_irreversible_interaction : concern_for_PartialProcesses
   {
      private InductionProcess _induction;
      private InhibitionProcess _irreversibleInhibition;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _induction = _cloneManager.Clone(_compoundProcessRepository.ProcessByName(CoreConstantsForSpecs.Process.INDUCTION)
            .DowncastTo<InductionProcess>());
         _induction.Name = "Induction";
         _induction.Parameter(CoreConstantsForSpecs.Parameter.EC50).Value = 10;
         _compound.AddProcess(_induction);

         _irreversibleInhibition = _cloneManager.Clone(_compoundProcessRepository.ProcessByName(CoreConstantsForSpecs.Process.IRREVERSIBLE_INHIBITION)
            .DowncastTo<InhibitionProcess>());
         _irreversibleInhibition.Name = "IrreversibleProcess";
         _irreversibleInhibition.Parameter(CoreConstantsForSpecs.Parameter.KINACT).Value = 10;
         _compound.AddProcess(_irreversibleInhibition);

         _simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(_individual, new[] {_compound}, new[] {_protocol})
            .DowncastTo<IndividualSimulation>();

         _simulation.CompoundPropertiesFor(_compound.Name)
            .Processes
            .MetabolizationSelection
            .AddPartialProcessSelection(new EnzymaticProcessSelection
               {CompoundName = _compound.Name, ProcessName = _metabolizationProcess.Name, MoleculeName = _enzyme.Name});


         _simulation.InteractionProperties.AddInteraction(new InteractionSelection
            {CompoundName = _compound.Name, MoleculeName = _enzyme.Name, ProcessName = _induction.Name});
         _simulation.InteractionProperties.AddInteraction(new InteractionSelection
            {CompoundName = _compound.Name, MoleculeName = _enzyme.Name, ProcessName = _irreversibleInhibition.Name});
         DomainFactoryForSpecs.AddModelToSimulation(_simulation);
      }

      [Observation]
      public void should_only_create_one_turnover_reaction()
      {
         _simulation.Reactions.Count().ShouldBeEqualTo(4); //Induction, turnover, irreversible and metabolization
      }
   }
}