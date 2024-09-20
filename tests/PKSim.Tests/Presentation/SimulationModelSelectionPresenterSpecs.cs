using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;

using OSPSuite.Core.Domain;

namespace PKSim.Presentation
{
   public abstract class concern_for_SimulationModelSelectionPresenter : ContextSpecification<ISimulationModelSelectionPresenter>
   {
      private ICalculationMethodToCategoryCalculationMethodDTOMapper _categoryCalculationMethodDTOMapper;
      protected ISimulationModelSelectionView _view;
      protected IModelConfigurationRepository _modelConfigurationRepository;
      protected IModelPropertiesTask _modelPropertiesTask;
      private IModelConfigurationDTOToModelPropertiesMapper _modelPropertiesMapper;
      protected Simulation _simulation;
      protected Species _species;
      protected ModelConfiguration _modelConfiguration;
      protected ModelProperties _modelProperties;
      private Individual _individual;
      protected IRepresentationInfoRepository _representationInfoRepository;
      protected OriginData _originData;

      protected override void Context()
      {
         _view = A.Fake<ISimulationModelSelectionView>();
         _categoryCalculationMethodDTOMapper = A.Fake<ICalculationMethodToCategoryCalculationMethodDTOMapper>();
         _modelPropertiesTask = A.Fake<IModelPropertiesTask>();
         _modelConfigurationRepository = A.Fake<IModelConfigurationRepository>();
         _modelPropertiesMapper = A.Fake<IModelConfigurationDTOToModelPropertiesMapper>();
         _representationInfoRepository = A.Fake<IRepresentationInfoRepository>();

         _simulation = A.Fake<Simulation>();
         _species = A.Fake<Species>();
         _individual = A.Fake<Individual>();
         _modelProperties = A.Fake<ModelProperties>();
         _modelConfiguration = A.Fake<ModelConfiguration>();
         _modelConfiguration.ModelName = "4Comp";
         _originData = new OriginData();
         _modelProperties.ModelConfiguration = _modelConfiguration;
         _simulation.ModelProperties = _modelProperties;
         _simulation.ModelConfiguration = _modelConfiguration;
         A.CallTo(() => _simulation.BuildingBlock<ISimulationSubject>()).Returns(_individual);
         _individual.OriginData = _originData;
         _originData.Species = _species;
         

         sut = new SimulationModelSelectionPresenter(_view, _modelConfigurationRepository, _modelPropertiesTask, _modelPropertiesMapper,  _representationInfoRepository,_categoryCalculationMethodDTOMapper);
      }
   }

   public class When_the_model_configuration_presenter_is_told_to_edit_a_simulation : concern_for_SimulationModelSelectionPresenter
   {
      private ModelConfigurationDTO _modelConfigurationDTO;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _view.BindTo(A<ModelConfigurationDTO>._))
            .Invokes(x => _modelConfigurationDTO = x.GetArgument<ModelConfigurationDTO>(0));
      }

      protected override void Because()
      {
         sut.EditSimulation(_simulation, CreationMode.New);
      }

      [Observation]
      public void should_bind_the_view_to_the_model_properties_for_this_simulaton()
      {
         _modelConfigurationDTO.ModelConfiguration.ShouldBeEqualTo(_simulation.ModelConfiguration);
      }
   }

   public class When_resolving_all_available_models : concern_for_SimulationModelSelectionPresenter
   {
      private IEnumerable<ModelConfiguration> _result;
      private ModelConfiguration _model1;
      private ModelConfiguration _model2;

      protected override void Context()
      {
         base.Context();
         sut.EditSimulation(_simulation, CreationMode.New);
         _model1 = A.Fake<ModelConfiguration>();
         _model2 = A.Fake<ModelConfiguration>();
         A.CallTo(() => _modelConfigurationRepository.AllFor(_species)).Returns(new[] {_model1, _model2});
      }

      protected override void Because()
      {
         _result = sut.AllModels();
      }

      [Observation]
      public void should_return_the_models_available_for_the_species()
      {
         _result.ShouldOnlyContainInOrder(_model1, _model2);
      }
   }

 

   public class When_the_model_properties_presenter_is_being_notified_the_selected_model_has_changed : concern_for_SimulationModelSelectionPresenter
   {
      protected override void Context()
      {
         base.Context();
         sut.EditSimulation(_simulation, CreationMode.New);
         A.CallTo(() => _modelPropertiesTask.DefaultFor(_modelConfiguration, _originData)).Returns(_modelProperties);
      }

      protected override void Because()
      {
         sut.ModelSelectionChanging(_modelConfiguration);
         sut.ModelSelectionChanged();
      }

      [Observation]
      public void should_update_the_image_for_the_active_model()
      {
         A.CallTo(() => _view.UpdateModelImage(_modelConfiguration.ModelName)).MustHaveHappened();
      }
      
   }

   public class When_retrieving_the_display_name_for_a_model_property : concern_for_SimulationModelSelectionPresenter
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _representationInfoRepository.InfoFor(RepresentationObjectType.MODEL, _modelConfiguration.ModelName))
            .Returns(new RepresentationInfo {DisplayName = "DISPLAY"});
      }

      [Observation]
      public void should_return_the_display_name_defined_for_the_model_property()
      {
         sut.DisplayFor(_modelConfiguration).ShouldBeEqualTo("DISPLAY");
      }
   }

 
}