using System.Collections.Generic;
using PKSim.Assets;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface ISimulationModelSelectionPresenter : ISimulationModelConfigurationItemPresenter, IPresenter<ISimulationModelSelectionView>
   {
      IEnumerable<ModelConfiguration> AllModels();
      void ModelSelectionChanged();
      string DisplayFor(ModelConfiguration modelConfiguration);
      void EditModelConfiguration(ISimulationSubject selectedSubject);

      /// <summary>
      ///    Returns the selected model configuration as defined in the view
      /// </summary>
      ModelProperties ModelProperties { get; }

      void ModelSelectionChanging(ModelConfiguration newModelConfiguration);
   }

   public class SimulationModelSelectionPresenter : AbstractSubPresenter<ISimulationModelSelectionView, ISimulationModelSelectionPresenter>, ISimulationModelSelectionPresenter
   {
      private readonly IModelConfigurationRepository _modelConfigurationRepository;
      private readonly IModelPropertiesTask _modelPropertiesTask;
      private readonly IModelConfigurationDTOToModelPropertiesMapper _modelPropertiesMapper;
      private ModelConfigurationDTO _modelConfigurationDTO;
      private OriginData _originData;
      private ModelProperties _modelPropertiesToUse;
      private readonly IRepresentationInfoRepository _representationInfoRepository;
      private readonly ICalculationMethodToCategoryCalculationMethodDTOMapper _categoryCalculationMethodDTOMapper;
      public ModelConfiguration ModelConfiguration { get; private set; }

      public SimulationModelSelectionPresenter(ISimulationModelSelectionView view, IModelConfigurationRepository modelConfigurationRepository,
         IModelPropertiesTask modelPropertiesTask, IModelConfigurationDTOToModelPropertiesMapper modelPropertiesMapper,
         IRepresentationInfoRepository representationInfoRepository, ICalculationMethodToCategoryCalculationMethodDTOMapper categoryCalculationMethodDTOMapper)
         : base(view)
      {
         _modelConfigurationRepository = modelConfigurationRepository;
         _modelPropertiesTask = modelPropertiesTask;
         _modelPropertiesMapper = modelPropertiesMapper;
         _representationInfoRepository = representationInfoRepository;
         _categoryCalculationMethodDTOMapper = categoryCalculationMethodDTOMapper;
      }

      public IEnumerable<ModelConfiguration> AllModels()
      {
         return _modelConfigurationRepository.AllFor(_originData.Species);
      }

      public void ModelSelectionChanging(ModelConfiguration newModelConfiguration)
      {
         ModelConfiguration = newModelConfiguration;
         _modelPropertiesToUse = _modelPropertiesTask.DefaultFor(newModelConfiguration, _originData);
      }

      public void ModelSelectionChanged()
      {
         updateModelProperties(_modelPropertiesToUse);
      }

      public string DisplayFor(ModelConfiguration modelConfiguration)
      {
         return _representationInfoRepository.InfoFor(RepresentationObjectType.MODEL, modelConfiguration.ModelName).DisplayName;
      }

      public void EditSimulation(Simulation simulation, CreationMode creationMode)
      {
         _originData = simulation.BuildingBlock<ISimulationSubject>().OriginData;
         editModelProperties(simulation.ModelProperties);
      }

      public void EditModelConfiguration(ISimulationSubject selectedSubject)
      {
         if (selectedSubject == null) return;
         _originData = selectedSubject.OriginData;
         ModelProperties modelProperties;
         if (selectedModelConfiguration != null)
            modelProperties = _modelPropertiesTask.DefaultFor(_originData, selectedModelConfiguration.ModelName);
         else
            modelProperties = _modelPropertiesTask.DefaultFor(_originData);

         editModelProperties(modelProperties);
      }

      private void editModelProperties(ModelProperties newModelProperties)
      {
         _modelConfigurationDTO = new ModelConfigurationDTO {ModelConfiguration = newModelProperties.ModelConfiguration};
         _view.BindTo(_modelConfigurationDTO);
         updateModelProperties(newModelProperties);
      }

      private void updateModelProperties(ModelProperties modelProperties)
      {
         _view.UpdateModelImage(selectedModelConfiguration.ModelName);
         updateCalculationMethods(modelProperties.AllCalculationMethods());
      }

      private void updateCalculationMethods(IEnumerable<CalculationMethod> calculationMethods)
      {
         _modelConfigurationDTO.CalculationMethodDTOs = calculationMethods.MapAllUsing(_categoryCalculationMethodDTOMapper);
      }

      public ModelProperties ModelProperties => _modelPropertiesMapper.MapFrom(_modelConfigurationDTO);

      private ModelConfiguration selectedModelConfiguration => _modelConfigurationDTO?.ModelConfiguration;
   }
}