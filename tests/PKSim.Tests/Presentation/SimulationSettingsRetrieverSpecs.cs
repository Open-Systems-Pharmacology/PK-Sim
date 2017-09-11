using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Services;

namespace PKSim.Presentation
{
   public abstract class concern_for_SimulationSettingsRetriever : ContextSpecification<ISimulationSettingsRetriever>
   {
      protected IPopulationSimulationSettingsPresenter _populationSimulationSettingsPresenter;
      protected IApplicationController _applicationController;
      private IPKSimProjectRetriever _projectRetriever;
      protected PKSimProject _project;
      protected PopulationSimulation _populationSimulation;
      protected IKeyPathMapper _keyPathMapper;
      protected IEntityPathResolver _entityPathResolver;
      protected OutputSelections _originalSettings;
      protected ICoreUserSettings _userSettings;
      protected Compound _compound1;
      protected Individual _individual;
      private Species _human;
      protected Species _rat;
      protected Species _mouse;

      protected override void Context()
      {
         _populationSimulationSettingsPresenter = A.Fake<IPopulationSimulationSettingsPresenter>();
         _applicationController = A.Fake<IApplicationController>();
         _projectRetriever = A.Fake<IPKSimProjectRetriever>();
         _project = A.Fake<PKSimProject>();
         _compound1 = A.Fake<Compound>();
         _individual = A.Fake<Individual>();
         _human = new Species().WithName(CoreConstants.Species.Human);
         _rat = new Species().WithName(CoreConstants.Species.Rat);
         _mouse = new Species().WithName(CoreConstants.Species.Mouse);
         A.CallTo(() => _individual.Species).Returns(_human);
         _populationSimulation = A.Fake<PopulationSimulation>();
         A.CallTo(() => _populationSimulation.Compounds).Returns(new[] {_compound1});
         A.CallTo(() => _populationSimulation.Individual).Returns(_individual);
         _keyPathMapper = A.Fake<IKeyPathMapper>();
         _entityPathResolver = A.Fake<IEntityPathResolver>();
         _userSettings = A.Fake<ICoreUserSettings>();
         _originalSettings = new OutputSelections();
         A.CallTo(() => _populationSimulation.OutputSelections).Returns(_originalSettings);

         _populationSimulation.Model = new Model();
         _populationSimulation.Model.Root = new Container();
         _compound1.Name = "DRUG";
         var organism = new Organism();
         var peripheralVenousBlood = new Container().WithName(CoreConstants.Organ.PeripheralVenousBlood);
         var venousBlood = new Container().WithName(CoreConstants.Organ.VenousBlood);
         var venousBloodPlasma = new Container().WithName(CoreConstants.Compartment.Plasma).WithParentContainer(venousBlood);
         var drugPeripheralBlood = new Container().WithName(_compound1.Name);
         var drugVenousBlood = new Container().WithName(_compound1.Name);
         var periperhalVenousBloodObserver = new Observer {Name = CoreConstants.Observer.PLASMA_PERIPHERAL_VENOUS_BLOOD};
         drugPeripheralBlood.Add(periperhalVenousBloodObserver);
         var venousBloodObserver = new Observer {Name = CoreConstants.Observer.CONCENTRATION};
         drugVenousBlood.Add(venousBloodObserver);
         peripheralVenousBlood.Add(drugPeripheralBlood);
         venousBloodPlasma.Add(drugVenousBlood);
         organism.Add(peripheralVenousBlood);
         organism.Add(venousBlood);

         _populationSimulation.Model.Root.Add(organism);


         A.CallTo(() => _projectRetriever.Current).Returns(_project);
         A.CallTo(() => _applicationController.Start<ISimulationOutputSelectionPresenter<PopulationSimulation>>()).Returns(_populationSimulationSettingsPresenter);
         sut = new SimulationSettingsRetriever(_applicationController, _projectRetriever, _entityPathResolver, _keyPathMapper, _userSettings);

         A.CallTo(() => _entityPathResolver.PathFor(periperhalVenousBloodObserver)).Returns("PERIPHERAL_OBSERVER");
         A.CallTo(() => _entityPathResolver.PathFor(venousBloodObserver)).Returns("VENOUS_BLOOD_OBSERVER");
      }
   }

   public class When_the_population_settings_retriever_is_retrieving_the_settings_for_a_simulation_run : concern_for_SimulationSettingsRetriever
   {
      protected override void Context()
      {
         base.Context();
         _project.OutputSelections = null;
      }

      protected override void Because()
      {
         sut.SettingsFor(_populationSimulation);
      }

      [Observation]
      public void should_display_the_available_quantities_that_the_user_can_choose_from()
      {
         A.CallTo(() => _populationSimulationSettingsPresenter.CreateSettings(_populationSimulation)).MustHaveHappened();
      }

      [Observation]
      public void should_dispose_the_presenter()
      {
         A.CallTo(() => _populationSimulationSettingsPresenter.Dispose()).MustHaveHappened();
      }
   }

   public class When_retrieving_the_settings_for_a_simulation_that_does_not_have_any_settings_and_default_settings_were_not_set_for_the_type : concern_for_SimulationSettingsRetriever
   {
      private OutputSelections _settings;

      protected override void Context()
      {
         base.Context();
         _project.OutputSelections = null;
         A.CallTo(() => _populationSimulationSettingsPresenter.CreateSettings(_populationSimulation)).Returns(_originalSettings);
      }

      protected override void Because()
      {
         _settings = sut.SettingsFor(_populationSimulation);
      }

      [Observation]
      public void should_select_the_default_settings()
      {
         _settings.HasSelection.ShouldBeTrue();
      }
   }

   public class When_retrieving_the_settings_for_a_simulation_that_does_not_have_any_settings_and_default_settings_were_set_for_the_type_in_the_project_but_have_no_selection : concern_for_SimulationSettingsRetriever
   {
      private OutputSelections _settings;
      private OutputSelections _defaultSettings;

      protected override void Context()
      {
         base.Context();
         _defaultSettings = new OutputSelections();
         _project.OutputSelections = _defaultSettings;
         A.CallTo(() => _populationSimulationSettingsPresenter.CreateSettings(_populationSimulation)).Returns(_originalSettings);
      }

      protected override void Because()
      {
         _settings = sut.SettingsFor(_populationSimulation);
      }

      [Observation]
      public void should_select_the_default_settings()
      {
         _settings.HasSelection.ShouldBeTrue();
      }
   }

   public class When_retrieving_the_settings_for_a_simulation_that_does_not_have_any_settings_and_default_settings_were_set_for_the_type_in_the_project : concern_for_SimulationSettingsRetriever
   {
      private OutputSelections _settings;
      private OutputSelections _defaultSettings;
      private List<IQuantity> _allQuantities;
      private readonly IQuantity q1 = A.Fake<IQuantity>();
      private readonly IQuantity q2 = A.Fake<IQuantity>();
      private readonly IQuantity q3 = A.Fake<IQuantity>();
      private readonly IQuantity q4 = A.Fake<IQuantity>();

      protected override void Context()
      {
         base.Context();
         _defaultSettings = new OutputSelections();
         _allQuantities = new List<IQuantity>();
         var moleculeSelection1 = new QuantitySelection("PATH1", QuantityType.Drug);
         _defaultSettings.AddOutput(moleculeSelection1);
         var moleculeSelection2 = new QuantitySelection("PATH2", QuantityType.Drug);
         _defaultSettings.AddOutput(moleculeSelection2);
         var moleculeSelection3 = new QuantitySelection("PATH3", QuantityType.Drug);
         _defaultSettings.AddOutput(moleculeSelection3);

         _allQuantities.AddRange(new[] {q1, q2, q3, q4});
         q1.QuantityType = QuantityType.Drug;
         q2.QuantityType = QuantityType.Drug;
         q3.QuantityType = QuantityType.Drug;
         q4.QuantityType = QuantityType.Drug;
         A.CallTo(() => _keyPathMapper.MapFrom(q1)).Returns("KEY1");
         A.CallTo(() => _keyPathMapper.MapFrom(q2)).Returns("KEY2");
         A.CallTo(() => _keyPathMapper.MapFrom(q3)).Returns("KEY3");
         A.CallTo(() => _keyPathMapper.MapFrom(q4)).Returns("KEY2");

         A.CallTo(() => _keyPathMapper.MapFrom(moleculeSelection1)).Returns("KEY1");
         A.CallTo(() => _keyPathMapper.MapFrom(moleculeSelection2)).Returns("KEY2");
         A.CallTo(() => _keyPathMapper.MapFrom(moleculeSelection3)).Returns("DOES NOT EXIST");

         A.CallTo(() => _entityPathResolver.PathFor(q1)).Returns("Q1");
         A.CallTo(() => _entityPathResolver.PathFor(q2)).Returns("Q2");
         A.CallTo(() => _entityPathResolver.PathFor(q3)).Returns("Q3");
         A.CallTo(() => _entityPathResolver.PathFor(q4)).Returns("Q4");

         _project.OutputSelections = _defaultSettings;
         _userSettings.OutputSelections = null;
         A.CallTo(() => _populationSimulationSettingsPresenter.CreateSettings(_populationSimulation)).Returns(_originalSettings);

         A.CallTo(() => _populationSimulation.All<IQuantity>()).Returns(_allQuantities);
      }

      protected override void Because()
      {
         _settings = sut.SettingsFor(_populationSimulation);
      }

      [Observation]
      public void should_select_the_project_settings()
      {
         _settings.AllOutputs.Count().ShouldBeEqualTo(3);
      }
   }

   public class When_retrieving_the_settings_for_a_simulation_that_does_not_have_any_settings_and_default_settings_were_set_in_the_user_settings : concern_for_SimulationSettingsRetriever
   {
      private OutputSelections _settings;
      private OutputSelections _defaultSettings;
      private List<IQuantity> _allQuantities;
      private readonly IQuantity _q1 = A.Fake<IQuantity>();
      private readonly IQuantity _q2 = A.Fake<IQuantity>();

      protected override void Context()
      {
         base.Context();
         _defaultSettings = new OutputSelections();
         _allQuantities = new List<IQuantity>();
         var moleculeSelection1 = new QuantitySelection("PATH1", QuantityType.Drug);
         _defaultSettings.AddOutput(moleculeSelection1);

         _allQuantities.AddRange(new[] {_q1, _q2});
         _q1.QuantityType = QuantityType.Drug;
         _q2.QuantityType = QuantityType.Transporter;
         A.CallTo(() => _keyPathMapper.MapFrom(_q1)).Returns("KEY1");
         A.CallTo(() => _keyPathMapper.MapFrom(_q2)).Returns("KEY1");
         A.CallTo(() => _keyPathMapper.MapFrom(moleculeSelection1)).Returns("KEY1");

         A.CallTo(() => _entityPathResolver.PathFor(_q1)).Returns("Q1");
         _userSettings.OutputSelections = _defaultSettings;
         A.CallTo(() => _populationSimulationSettingsPresenter.CreateSettings(_populationSimulation)).Returns(_originalSettings);

         A.CallTo(() => _populationSimulation.All<IQuantity>()).Returns(_allQuantities);
      }

      protected override void Because()
      {
         _settings = sut.SettingsFor(_populationSimulation);
      }

      [Observation]
      public void should_use_the_default_user_settings_and_only_add_quantities_with_the_matching_quantity_type()
      {
         _settings.AllOutputs.Count().ShouldBeEqualTo(1);
         _settings.AllOutputs.ElementAt(0).Path.ShouldBeEqualTo("Q1");
      }
   }

   public class When_retrieving_the_settings_for_a_simulation_that_does_not_have_any_settings_and_default_settings_cannot_match_the_current_simulation : concern_for_SimulationSettingsRetriever
   {
      private OutputSelections _settings;
      private OutputSelections _defaultSettings;
      private List<IQuantity> _allQuantities;
      private readonly IQuantity q1 = A.Fake<IQuantity>();

      protected override void Context()
      {
         base.Context();
         _defaultSettings = new OutputSelections();
         _allQuantities = new List<IQuantity>();
         var moleculeSelection1 = new QuantitySelection("PATH1", QuantityType.Drug);
         _defaultSettings.AddOutput(moleculeSelection1);

         _allQuantities.Add(q1);
         q1.QuantityType = QuantityType.Drug;
         A.CallTo(() => _keyPathMapper.MapFrom(q1)).Returns("KEY1");

         A.CallTo(() => _keyPathMapper.MapFrom(moleculeSelection1)).Returns("KEY2");

         _project.OutputSelections = _defaultSettings;
         A.CallTo(() => _populationSimulationSettingsPresenter.CreateSettings(_populationSimulation)).Returns(_originalSettings);

         A.CallTo(() => _populationSimulation.All<IQuantity>()).Returns(_allQuantities);
      }

      protected override void Because()
      {
         _settings = sut.SettingsFor(_populationSimulation);
      }

      [Observation]
      public void should_select_the_project_settings()
      {
         _settings.AllOutputs.Count().ShouldBeEqualTo(1);
      }
   }

   public class When_retrieving_the_settings_for_a_simulation_using_the_species_rat : concern_for_SimulationSettingsRetriever
   {
      private OutputSelections _outputSelection;

      protected override void Context()
      {
         base.Context();
         _outputSelection = new OutputSelections();
         A.CallTo(() => _populationSimulation.OutputSelections).Returns(_outputSelection);
         A.CallTo(() => _individual.Species).Returns(_rat);
      }

      protected override void Because()
      {
         sut.CreatePKSimDefaults(_populationSimulation);
      }

      [Observation]
      public void should_select_venous_blood_plasma()
      {
         _outputSelection.AllOutputs.Count().ShouldBeEqualTo(1);
         var output = _outputSelection.AllOutputs.ElementAt(0);
         output.Path.ShouldBeEqualTo("VENOUS_BLOOD_OBSERVER");
      }
   }

   public class When_retrieving_the_settings_for_a_simulation_using_the_species_mouse : concern_for_SimulationSettingsRetriever
   {
      private OutputSelections _outputSelection;

      protected override void Context()
      {
         base.Context();
         _outputSelection = new OutputSelections();
         A.CallTo(() => _populationSimulation.OutputSelections).Returns(_outputSelection);
         A.CallTo(() => _individual.Species).Returns(_mouse);
      }

      protected override void Because()
      {
         sut.CreatePKSimDefaults(_populationSimulation);
      }

      [Observation]
      public void should_select_venous_blood_plasma()
      {
         _outputSelection.AllOutputs.Count().ShouldBeEqualTo(1);
         var output = _outputSelection.AllOutputs.ElementAt(0);
         output.Path.ShouldBeEqualTo("VENOUS_BLOOD_OBSERVER");
      }
   }

   public class When_retrieving_the_settings_for_a_simulation_using_the_species_human : concern_for_SimulationSettingsRetriever
   {
      private OutputSelections _outputSelection;

      protected override void Context()
      {
         base.Context();
         _outputSelection = new OutputSelections();
         A.CallTo(() => _populationSimulation.OutputSelections).Returns(_outputSelection);
      }

      protected override void Because()
      {
         sut.CreatePKSimDefaults(_populationSimulation);
      }

      [Observation]
      public void should_select_peripheral_venous_blood_plasma()
      {
         _outputSelection.AllOutputs.Count().ShouldBeEqualTo(1);
         var output = _outputSelection.AllOutputs.ElementAt(0);
         output.Path.ShouldBeEqualTo("PERIPHERAL_OBSERVER");
      }
   }
}