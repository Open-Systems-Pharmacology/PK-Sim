using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Presentation.Presenters.Charts;
using OSPSuite.Presentation.Views;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Formulations;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.Formulations;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Formulations;
using PKSim.Presentation.Views.Parameters;
using IFormulaFactory = PKSim.Core.Model.IFormulaFactory;

namespace PKSim.Presentation
{
   public abstract class concern_for_FormulationSettingsPresenter : ContextSpecification<IFormulationSettingsPresenter>
   {
      protected IFormulationSettingsView _view;
      protected IFormulationToFormulationDTOMapper _formulationDTOMapper;
      protected IMultiParameterEditPresenter _formulaParameterPresenter;
      protected IFormulationRepository _formulationRepository;
      protected IRepresentationInfoRepository _representationInfoRepository;
      protected ICloner _cloner;
      protected ITableFormulationPresenter _tableFormulationPresenter;
      protected IFormulaFactory _formulaFactory;
      protected ISimpleChartPresenter _simpleChartPresenter;
      protected IFormulationValuesRetriever _formulationValuesRetriever;

      protected override void Context()
      {
         _view = A.Fake<IFormulationSettingsView>();
         _formulationDTOMapper = A.Fake<IFormulationToFormulationDTOMapper>();
         _formulationRepository = A.Fake<IFormulationRepository>();
         _formulaParameterPresenter = A.Fake<IMultiParameterEditPresenter>();
         _representationInfoRepository = A.Fake<IRepresentationInfoRepository>();
         _tableFormulationPresenter = A.Fake<ITableFormulationPresenter>();
         _cloner = A.Fake<ICloner>();
         A.CallTo(() => _formulaParameterPresenter.View).Returns(A.Fake<IMultiParameterEditView>());
         A.CallTo(() => _tableFormulationPresenter.BaseView).Returns(A.Fake<IView>());
         _simpleChartPresenter = A.Fake<ISimpleChartPresenter>();
         _formulationValuesRetriever = A.Fake<IFormulationValuesRetriever>();

         sut = new FormulationSettingsPresenter(_view, _formulationDTOMapper, _formulaParameterPresenter, _formulationRepository,
            _representationInfoRepository, _tableFormulationPresenter, _cloner, _simpleChartPresenter, _formulationValuesRetriever);
      }
   }

   public class When_the_formulation_settings_presenter_is_retrieved_all_the_formulation_type : concern_for_FormulationSettingsPresenter
   {
      private Formulation _formulation1;
      private Formulation _formulation2;
      private IList<FormulationTypeDTO> _results;

      protected override void Context()
      {
         base.Context();
         _formulation1 = A.Fake<Formulation>();
         _formulation1.FormulationType = "gel";
         _formulation2 = A.Fake<Formulation>();
         _formulation2.FormulationType = "caps";
         sut.ApplicationRoute = "ORAL";
         A.CallTo(() => _representationInfoRepository.InfoFor(_formulation1)).Returns(new RepresentationInfo());
         A.CallTo(() => _representationInfoRepository.InfoFor(_formulation2)).Returns(new RepresentationInfo());
         A.CallTo(() => _formulationRepository.AllFor(sut.ApplicationRoute)).Returns(new[] {_formulation1, _formulation2});
      }

      protected override void Because()
      {
         _results = sut.AllFormulationTypes().ToList();
      }

      [Observation]
      public void should_return_the_formulation_types_available_in_the_formulation_repository()
      {
         _results.Count.ShouldBeEqualTo(2);
         _results[0].Id.ShouldBeEqualTo(_formulation1.FormulationType);
         _results[1].Id.ShouldBeEqualTo(_formulation2.FormulationType);
      }
   }

   public class When_the_formulation_settings_presenter_is_being_initialized : concern_for_FormulationSettingsPresenter
   {
      private ICommandCollector _commandRegister;

      protected override void Context()
      {
         base.Context();
         _commandRegister = A.Fake<ICommandCollector>();
      }

      protected override void Because()
      {
         sut.InitializeWith(_commandRegister);
      }

      [Observation]
      public void should_set_the_command_register_into_the_formulation_parameters()
      {
         A.CallTo(() => _formulaParameterPresenter.InitializeWith(sut)).MustHaveHappened();
      }
   }

   public class When_the_formulation_setting_is_asked_to_edit_a_formulation_that_is_not_a_table_formula : concern_for_FormulationSettingsPresenter
   {
      private FormulationDTO _formulationDTO;
      private Formulation _formulation;

      protected override void Context()
      {
         base.Context();
         _formulation = A.Fake<Formulation>();
         A.CallTo(() => _formulation.IsTable).Returns(false);
         _formulationDTO = new FormulationDTO(new List<IParameter>()) {Type = new FormulationTypeDTO {Id = "oral"}};
         A.CallTo(() => _formulationDTOMapper.MapFrom(_formulation)).Returns(_formulationDTO);
      }

      protected override void Because()
      {
         sut.EditFormulation(_formulation);
      }

      [Observation]
      public void should_retrieve_the_formulation_dto_for_the_given_formulation()
      {
         A.CallTo(() => _formulationDTOMapper.MapFrom(_formulation)).MustHaveHappened();
      }

      [Observation]
      public void should_display_the_formulation_settings_to_be_edited_in_the_view()
      {
         A.CallTo(() => _view.BindTo(_formulationDTO)).MustHaveHappened();
      }

      [Observation]
      public void should_display_the_parameters_for_the_formulation()
      {
         A.CallTo(() => _formulaParameterPresenter.Edit(_formulationDTO.Parameters)).MustHaveHappened();
      }

      [Observation]
      public void should_add_the_parameter_presenter_view_to_its_view()
      {
         A.CallTo(() => _view.AddParameterView(_formulaParameterPresenter.View)).MustHaveHappened();
      }
   }

   public class When_the_formulation_setting_is_asked_to_edit_a_table_formulation : concern_for_FormulationSettingsPresenter
   {
      private FormulationDTO _formulationDTO;
      private Formulation _formulation;
      private IParameter _tableParameter;

      protected override void Context()
      {
         base.Context();
         _tableParameter = new PKSimParameter();
         _formulation = new Formulation {FormulationType = CoreConstants.Formulation.Table};
         _formulation.Add(_tableParameter);
         _formulationDTO = new FormulationDTO(new[] {_tableParameter}) {Type = new FormulationTypeDTO {Id = "oral"}};
         A.CallTo(() => _formulationDTOMapper.MapFrom(_formulation)).Returns(_formulationDTO);
         A.CallTo(() => _tableFormulationPresenter.EditedFormula).Returns(new TableFormula());
      }

      protected override void Because()
      {
         sut.EditFormulation(_formulation);
      }

      [Observation]
      public void should_retrieve_the_formulation_dto_for_the_given_formulation()
      {
         A.CallTo(() => _formulationDTOMapper.MapFrom(_formulation)).MustHaveHappened();
      }

      [Observation]
      public void should_display_the_formulation_settings_to_be_edited_in_the_view()
      {
         A.CallTo(() => _view.BindTo(_formulationDTO)).MustHaveHappened();
      }

      [Observation]
      public void should_edit_the_table_formulation()
      {
         A.CallTo(() => _tableFormulationPresenter.Edit(_formulation)).MustHaveHappened();
      }

      [Observation]
      public void should_add_the_table_parameter_presenter_view_to_its_view()
      {
         A.CallTo(() => _view.AddParameterView(_tableFormulationPresenter.BaseView)).MustHaveHappened();
      }

      [Observation]
      public void should_set_the_edited_table_as_source_for_the_data_chart_being_displayed()
      {
         A.CallTo(() => _simpleChartPresenter.Plot(_tableFormulationPresenter.EditedFormula)).MustHaveHappened();
      }

      [Observation]
      public void should_show_the_plot()
      {
         _view.ChartVisible.ShouldBeTrue();
      }
   }

   public class When_the_formulation_setting_presenter_is_notifed_that_the_formulation_type_was_changed : concern_for_FormulationSettingsPresenter
   {
      private bool _eventRaised;
      private FormulationTypeDTO _formulationTypeDTO;
      private string _valueRaised;
      private Formulation _formulation;
      private FormulationDTO _formulationDTO;

      protected override void Context()
      {
         base.Context();
         _formulationTypeDTO = new FormulationTypeDTO {Id = "trala"};
         _formulation = A.Fake<Formulation>();
         _formulationDTO = new FormulationDTO(new List<IParameter>()) {Type = _formulationTypeDTO};

         A.CallTo(() => _formulationDTOMapper.MapFrom(_formulation)).Returns(_formulationDTO);
         sut.EditFormulation(_formulation);
         sut.FormulationTypeChanged += e =>
         {
            _eventRaised = true;
            _valueRaised = e;
         };
         var templateFormulation = A.Fake<Formulation>();
         A.CallTo(() => _cloner.Clone(templateFormulation)).Returns(_formulation);
         A.CallTo(() => _formulationRepository.FormulationBy(_formulationTypeDTO.Id)).Returns(templateFormulation);
      }

      protected override void Because()
      {
         sut.OnFormulationTypeChanged();
      }

      [Observation]
      public void should_spread_the_notification_further()
      {
         _eventRaised.ShouldBeTrue();
         _valueRaised.ShouldBeEqualTo(_formulationTypeDTO.Id);
      }

      [Observation]
      public void should_edit_a_clone_of_the_template_defined_for_the_given_type()
      {
         A.CallTo(() => _view.BindTo(_formulationDTO)).MustHaveHappened();
      }
   }

   public class When_saving_the_formulation_explicitely : concern_for_FormulationSettingsPresenter
   {
      private bool _eventRaised;

      protected override void Context()
      {
         base.Context();
         var formulation = A.Fake<Formulation>();
         sut.EditFormulation(formulation);

         sut.StatusChanged += (o, e) => { _eventRaised = true; };
      }

      protected override void Because()
      {
         sut.SaveFormulation();
      }

      [Observation]
      public void should_not_raised_the_status_changed_event()
      {
         _eventRaised.ShouldBeFalse();
      }
   }

   public class When_switching_the_particle_dissolution_mode_from_polydisperse_to_monodisperse : concern_for_FormulationSettingsPresenter
   {
      private Formulation _formulation;
      private FormulationDTO _formulationDTO;
      private IParameter _particleDisperseSystem;

      protected override void Context()
      {
         base.Context();
         _particleDisperseSystem = DomainHelperForSpecs.ConstantParameterWithValue(CoreConstants.Parameters.MONODISPERSE).WithName(CoreConstants.Parameters.PARTICLE_DISPERSE_SYSTEM);
         _formulationDTO = new FormulationDTO(new List<IParameter>());

         _formulation = A.Fake<Formulation>();
         A.CallTo(() => _formulation.IsParticleDissolution).Returns(true);
         A.CallTo(() => _formulationDTOMapper.MapFrom(_formulation)).Returns(_formulationDTO);


         sut.EditFormulation(_formulation);
      }

      protected override void Because()
      {
         _formulaParameterPresenter.ParameterChanged += Raise.FreeForm.With(_particleDisperseSystem);
      }

      [Observation]
      public void should_set_the_visibility_of_monodisperse_parameter_to_false_and_the_default_state_to_true()
      {
         A.CallTo(() => _formulation.UpdateParticleParametersVisibility()).MustHaveHappened();
      }
   }

   public class When_switching_the_particle_dissolution_mode_from_polydispertse_normal_to_polydisperse_lognormal : concern_for_FormulationSettingsPresenter
   {
      private Formulation _formulation;
      private FormulationDTO _formulationDTO;
      private IParameter _particleSizeDistribution;

      protected override void Context()
      {
         base.Context();
         _particleSizeDistribution = DomainHelperForSpecs.ConstantParameterWithValue(CoreConstants.Parameters.PARTICLE_SIZE_DISTRIBUTION_LOG_NORMAL).WithName(CoreConstants.Parameters.PARTICLE_SIZE_DISTRIBUTION);
     
         _formulationDTO = new FormulationDTO(new List<IParameter>());

         _formulation = A.Fake<Formulation>();
         A.CallTo(() => _formulationDTOMapper.MapFrom(_formulation)).Returns(_formulationDTO);


         sut.EditFormulation(_formulation);
      }

      protected override void Because()
      {
         _formulaParameterPresenter.ParameterChanged += Raise.FreeForm.With(_particleSizeDistribution);
      }

      [Observation]
      public void should_set_the_visibility_of_monodisperse_parameter_to_false_and_the_default_state_to_true()
      {
         A.CallTo(() => _formulation.UpdateParticleParametersVisibility()).MustHaveHappened();
      }
   }
}