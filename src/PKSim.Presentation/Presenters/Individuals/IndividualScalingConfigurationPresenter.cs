using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Parameters;
using PKSim.Presentation.Views.Individuals;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.Individuals
{
   public interface IIndividualScalingConfigurationPresenter : IIndividualItemPresenter
   {
      void ConfigureScaling(Individual sourceIndividual, Individual targetIndividual);
      IEnumerable<ScalingMethod> AllScalingMethodsFor(ParameterScalingDTO parameterScalingDTO);
      void ScalingMethodChanged(ParameterScalingDTO parameterScalingDTO, ScalingMethod newScalingMethod);
      void PerformScaling();
      bool ScalingPerformed { get; }
      IParameterDTO Weight { get; }
   }

   public class IndividualScalingConfigurationPresenter : AbstractSubPresenter<IIndividualScalingConfigurationView, IIndividualScalingConfigurationPresenter>, IIndividualScalingConfigurationPresenter
   {
      private readonly IIndividualScalingTask _individualScalingTask;
      private readonly IParameterScalingToParameterScalingDTOMapper _mapper;
      private readonly IScalingMethodTask _scalingMethodTask;
      private readonly IParameterToParameterDTOMapper _parameterDTOMapper;
      private IList<ParameterScaling> _parameterScalingList;
      private bool _needWeight;
      public bool ScalingPerformed { get; private set; }
      public IParameterDTO Weight { get; private set; }

      public IndividualScalingConfigurationPresenter(IIndividualScalingConfigurationView view, IIndividualScalingTask individualScalingTask,
         IParameterScalingToParameterScalingDTOMapper mapper, IScalingMethodTask scalingMethodTask, IParameterToParameterDTOMapper parameterDTOMapper) : base(view)
      {
         _individualScalingTask = individualScalingTask;
         _mapper = mapper;
         _scalingMethodTask = scalingMethodTask;
         _parameterDTOMapper = parameterDTOMapper;
         Weight = new NullParameterDTO();
      }

      public virtual void EditIndividual(Individual individualToEdit)
      {
         /*nothing to do here*/
      }

      public void ConfigureScaling(Individual sourceIndividual, Individual targetIndividual)
      {
         ScalingPerformed = false;
         Weight = _parameterDTOMapper.MapFrom(targetIndividual.WeightParameter);
         _parameterScalingList = _individualScalingTask.AllParameterScalingsFrom(sourceIndividual, targetIndividual).ToList();
         _needWeight = weightShouldBeDisplayed();
         refreshView();
      }

      private void refreshView()
      {
         _view.BindTo(_parameterScalingList.MapAllUsing(_mapper));
         updateWeight();
      }

      private void updateWeight()
      {
         _view.WeightVisible = _needWeight;
         if (_needWeight)
            _view.BindToWeight();
      }

      private bool weightShouldBeDisplayed()
      {
         var query = from parameterScaling in _parameterScalingList
            where parameterScaling.SourceParameter.IsOrganVolume()
            select parameterScaling;


         return query.Any();
      }

      public IEnumerable<ScalingMethod> AllScalingMethodsFor(ParameterScalingDTO parameterScalingDTO)
      {
         return _scalingMethodTask.AllMethodsFor(parameterScalingFrom(parameterScalingDTO));
      }

      public void ScalingMethodChanged(ParameterScalingDTO parameterScalingDTO, ScalingMethod newScalingMethod)
      {
         parameterScalingDTO.ScalingMethod = newScalingMethod;
         ScalingPerformed = false;
      }

      public void PerformScaling()
      {
         var allCommands = _individualScalingTask.PerformScaling(_parameterScalingList);
         allCommands.Each(AddCommand);
         ScalingPerformed = true;
      }

      private ParameterScaling parameterScalingFrom(ParameterScalingDTO parameterScalingDTO)
      {
         return parameterScalingDTO.ParameterScaling;
      }
   }
}