using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility.Events;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Populations;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Presenters.Populations;
using PKSim.Presentation.Views.AdvancedParameters;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.AdvancedParameters
{
   public class DistributionTypeChangedEventArgs : EventArgs
   {
      public AdvancedParameter AdvancedParameter { get; }
      public DistributionType DistributionType { get; }

      public DistributionTypeChangedEventArgs(AdvancedParameter advancedParameter, DistributionType distributionType)
      {
         AdvancedParameter = advancedParameter;
         DistributionType = distributionType;
      }
   }

   public interface IAdvancedParameterPresenter : IPresenter<IAdvancedParameterView>, ICommandCollectorPresenter
   {
      /// <summary>
      ///    display the settings for the advanced parameter presenter
      /// </summary>
      /// <param name="advancedParameter">advanced parameter to edit</param>
      void Edit(AdvancedParameter advancedParameter);

      /// <summary>
      ///    Triggered when the user decides to change the distribution type for the edited advanced parameter
      /// </summary>
      void DistributionTypeChanged();

      /// <summary>
      ///    Event is raised whenever the user changed the AdvancedParameterType for the edited parameter.
      ///    The Type of the advanced parameter has not been updated yet. This should be accomplished
      ///    by the main presenter listening to the event
      /// </summary>
      event EventHandler<DistributionTypeChangedEventArgs> OnDistributionTypeChanged;

      /// <summary>
      ///    returns all available distributions supported by the application for advanced parameters
      /// </summary>
      IEnumerable<DistributionType> AllDistributions(AdvancedParameterDTO advancedParameterDTO);

      /// <summary>
      ///    Remove the selected information displayed for the advanced parameter if any was selected
      /// </summary>
      void RemoveSelection();

      /// <summary>
      ///    Removes the selection for the advanced paramter if the parameter was being edited
      /// </summary>
      /// <param name="advancedParameter"></param>
      void RemoveSelectionFor(AdvancedParameter advancedParameter);
   }

   public class AdvancedParameterPresenter : AbstractCommandCollectorPresenter<IAdvancedParameterView, IAdvancedParameterPresenter>, IAdvancedParameterPresenter
   {
      private readonly IAdvancedParameterToAdvancedParameterDTOMapper _advancedParameterDTOMapper;
      private readonly IMultiParameterEditPresenter _distributionParameterPresenter;
      private AdvancedParameter _advancedParameter;
      private AdvancedParameterDTO _advancedParameterDTO;
      public event EventHandler<DistributionTypeChangedEventArgs> OnDistributionTypeChanged = delegate { };

      public AdvancedParameterPresenter(IAdvancedParameterView view, IAdvancedParameterToAdvancedParameterDTOMapper advancedParameterDTOMapper,
         IEditDistributionParametersPresenter distributionParameterPresenter)
         : base(view)
      {
         _advancedParameterDTOMapper = advancedParameterDTOMapper;
         _distributionParameterPresenter = distributionParameterPresenter;
         _view.AddParameterView(_distributionParameterPresenter.View);
         RemoveSelection();
      }

      public override void InitializeWith(ICommandCollector commandCollector)
      {
         base.InitializeWith(commandCollector);
         _distributionParameterPresenter.InitializeWith(commandCollector);
      }

      public override void ReleaseFrom(IEventPublisher eventPublisher)
      {
         base.ReleaseFrom(eventPublisher);
         _distributionParameterPresenter.ReleaseFrom(eventPublisher);
      }

      public void Edit(AdvancedParameter advancedParameter)
      {
         _advancedParameter = advancedParameter;
         _advancedParameterDTO = _advancedParameterDTOMapper.MapFrom(_advancedParameter);
         _distributionParameterPresenter.Edit(_advancedParameterDTO.Parameters);
         _view.BindTo(_advancedParameterDTO);
      }

      public void DistributionTypeChanged()
      {
         OnDistributionTypeChanged(this, new DistributionTypeChangedEventArgs(_advancedParameter, _advancedParameterDTO.DistributionType));
      }

      public IEnumerable<DistributionType> AllDistributions(AdvancedParameterDTO advancedParameterDTO)
      {
         var allDistributions = DistributionTypes.All().ToList();
         if (advancedParameterDTO.DistributionType != DistributionTypes.Unknown)
            allDistributions.Remove(DistributionTypes.Unknown);

         return allDistributions;
      }

      public void RemoveSelection()
      {
         _view.DeleteBinding();
         _distributionParameterPresenter.Clear();
      }

      public void RemoveSelectionFor(AdvancedParameter advancedParameter)
      {
         if (Equals(advancedParameter, _advancedParameter))
            RemoveSelection();
      }
   }
}