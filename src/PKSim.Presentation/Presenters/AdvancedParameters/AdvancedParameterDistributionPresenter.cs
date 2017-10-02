using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using PKSim.Assets;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

using PKSim.Presentation.Presenters.Populations;
using PKSim.Presentation.Views.AdvancedParameters;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Presentation.Presenters;
using DistributionSettings = PKSim.Core.Chart.DistributionSettings;

namespace PKSim.Presentation.Presenters.AdvancedParameters
{
   public interface IAdvancedParameterDistributionPresenter : ISubPresenter
   {
      /// <summary>
      ///    Returns all bar types available for display
      /// </summary>
      IEnumerable<BarType> AllBarTypes();

      /// <summary>
      ///    Returns all genders available for the given population
      /// </summary>
      IEnumerable<string> AllGenders();

      /// <summary>
      ///    Returns all scaling modes available for display
      /// </summary>
      IEnumerable<AxisCountMode> AllScalingModes();

      /// <summary>
      ///    Returns the string to be displayed for the given genderKey
      /// </summary>
      string GenderDisplayFor(string genderId);

      /// <summary>
      ///    Select the advanced parameter and display its distribution
      /// </summary>
      /// <param name="advancedParameter">Advanced parameter whose distribution should be displayed</param>
      void Select(AdvancedParameter advancedParameter);

      void AddAdvancedParameter(AdvancedParameter advancedParameter);

      void RemoveAdvancedParameter(AdvancedParameter advancedParameter);

      /// <summary>
      ///    Defines if the selected parameter should be displayed in a report
      /// </summary>
      void UseSelectedParameterInReport(bool useInReport);
   }

   public abstract class AdvancedParameterDistributionPresenter : AbstractSubPresenter<IAdvancedParameterDistributionView, IAdvancedParameterDistributionPresenter>,
      IAdvancedParameterDistributionPresenter
   {
      private readonly IPopulationParameterGroupsPresenter _parametersPresenter;
      private readonly IRepresentationInfoRepository _representationInfoRepository;
      private readonly IEntityPathResolver _entityPathResolver;
      private readonly IPopulationDistributionPresenter _populationParameterDistributionPresenter;
      private readonly IProjectChangedNotifier _projectChangedNotifier;
      private readonly DistributionSettings _defaultSettings;
      private IList<string> _genderSelection;
      private IVectorialParametersContainer _vectorialParametersContainer;
      private PathCache<IParameter> _allParametersCache;
      private ParameterDistributionSettingsCache _selectedDistributions;

      protected AdvancedParameterDistributionPresenter(IAdvancedParameterDistributionView view, IPopulationParameterGroupsPresenter parametersPresenter,
         IRepresentationInfoRepository representationInfoRepository, IEntityPathResolver entityPathResolver, IPopulationDistributionPresenter populationParameterDistributionPresenter,
         IProjectChangedNotifier projectChangedNotifier)
         : base(view)
      {
         _parametersPresenter = parametersPresenter;
         _representationInfoRepository = representationInfoRepository;
         _entityPathResolver = entityPathResolver;
         _populationParameterDistributionPresenter = populationParameterDistributionPresenter;
         _projectChangedNotifier = projectChangedNotifier;
         _defaultSettings = new DistributionSettings();
         _parametersPresenter.GroupNodeSelected += (o, e) => activateNode(e.Node);
         _parametersPresenter.ParameterNodeSelected += (o, e) => activateNode(e.Node);
         _view.UpdateParameterGroupView(_parametersPresenter.View);
         _allParametersCache = new PathCache<IParameter>(_entityPathResolver);
         _view.AddDistributionView(_populationParameterDistributionPresenter.View);
         AddSubPresenters(_parametersPresenter, _populationParameterDistributionPresenter);
      }

      private void refresh()
      {
         activateNode(_parametersPresenter.SelectedNode);
      }

      public void Select(AdvancedParameter advancedParameter)
      {
         _parametersPresenter.SelectParameter(_allParametersCache[advancedParameter.ParameterPath]);
      }

      public void AddAdvancedParameter(AdvancedParameter advancedParameter)
      {
         var parameter = parameterFor(advancedParameter);
         if (parameter == null) return;
         _parametersPresenter.AddParameter(parameter);
         _parametersPresenter.SelectParameter(parameter);
      }

      public void RemoveAdvancedParameter(AdvancedParameter advancedParameter)
      {
         var parameter = parameterFor(advancedParameter);
         if (parameter == null) return;
         _parametersPresenter.RemoveParameter(parameter);
         refresh();
      }

      private IParameter parameterFor(AdvancedParameter advancedParameter)
      {
         return _allParametersCache[advancedParameter.ParameterPath];
      }

      private string parameterPathFor(IParameter parameter)
      {
         if (parameter == null)
            return string.Empty;

         return _entityPathResolver.PathFor(parameter);
      }

      protected void EditParameterDistributionFor(IVectorialParametersContainer vectorialParametersContainer)
      {
         _vectorialParametersContainer = vectorialParametersContainer;
         _selectedDistributions = _vectorialParametersContainer.SelectedDistributions;
         _allParametersCache = vectorialParametersContainer.AllParameters(_entityPathResolver);
         _parametersPresenter.AddParameters(_vectorialParametersContainer.AllVectorialParameters(_entityPathResolver).Where(p => p.Visible), vectorialParametersContainer.DisplayParameterUsingGroupStructure);
         _genderSelection = genderSelectionsFrom(vectorialParametersContainer).ToList();
         _defaultSettings.SelectedGender = _genderSelection[0];
         updateView();
      }

      private void updateView()
      {
         _view.BarTypeVisible = _genderSelection.Count > 1;
         _view.GenderSelectionVisible = _genderSelection.Count > 1;
         _view.BindToPlotOptions(_defaultSettings);
         _selectedDistributions.Each(d =>
         {
            d.UseInReport = true;
            updateNodeColor(d.ParameterPath);
         });
         refresh();
      }

      public void UseSelectedParameterInReport(bool useInReport)
      {
         var selectedParameter = _parametersPresenter.SelectedParameter;
         if (selectedParameter == null) return;

         var parameterPath = parameterPathFor(selectedParameter);
         if (!useInReport)
            _selectedDistributions.Remove(parameterPath);
         else
            _selectedDistributions.Add(createSettingsFor(parameterPath));

         refresh();
         _defaultSettings.UseInReport = false;
         _projectChangedNotifier.NotifyChangedFor(_vectorialParametersContainer);
      }

      private ParameterDistributionSettings createSettingsFor(string parameterPath)
      {
         return new ParameterDistributionSettings
         {
            ParameterPath = parameterPath,
            Settings = _defaultSettings.Clone(),
            UseInReport = true
         };
      }

      private DistributionSettings settingsFor(IParameter parameter)
      {
         var parameterSettings = parameterDistributionSettingsFor(parameter);
         return parameterSettings != null ? parameterSettings.Settings : _defaultSettings;
      }

      private ParameterDistributionSettings parameterDistributionSettingsFor(IParameter parameter)
      {
         var parameterPath = parameterPathFor(parameter);
         return _selectedDistributions[parameterPath];
      }

      private void activateNode(ITreeNode node)
      {
         var parameterNode = node as ITreeNode<IParameter>;
         if (parameterNode == null)
         {
            _view.BindToPlotOptions(_defaultSettings);
            _populationParameterDistributionPresenter.ResetPlot();
            _view.SettingsEnabled = false;
            return;
         }

         var parameter = parameterNode.Tag;
         var settings = settingsFor(parameter);
         _view.SettingsEnabled = true;
         settings.Reset();
         settings.PlotCaption = parameterNode.Text;
         updateNodeColor(parameterNode, parameter);
         _populationParameterDistributionPresenter.Plot(_vectorialParametersContainer, parameterNode.Tag, settings: settings);
         _view.BindToPlotOptions(settings);
      }

      private void updateNodeColor(string parameterPath)
      {
         var parameter = _allParametersCache[parameterPath];
         var node = _parametersPresenter.NodeFor(parameter);
         updateNodeColor(node, parameter);
      }

      private void updateNodeColor(ITreeNode parameterNode, IParameter parameter)
      {
         if (parameterNode == null || parameter == null)
            return;

         var parameterSettings = parameterDistributionSettingsFor(parameter);
         parameterNode.ForeColor = parameterSettings == null ? Color.Empty : PKSimColors.SelectedDistribution;
      }

      public override void ViewChanged()
      {
         base.ViewChanged();
         refresh();
      }

      public IEnumerable<BarType> AllBarTypes()
      {
         return EnumHelper.AllValuesFor<BarType>();
      }

      public IEnumerable<string> AllGenders()
      {
         return _genderSelection;
      }

      public IEnumerable<AxisCountMode> AllScalingModes()
      {
         return EnumHelper.AllValuesFor<AxisCountMode>();
      }

      public string GenderDisplayFor(string genderId)
      {
         if (string.Equals(genderId, CoreConstants.Population.AllGender))
            return PKSimConstants.UI.AllGender;

         var gender = _vectorialParametersContainer.AllGenders.Distinct().FindByName(genderId);
         return _representationInfoRepository.DisplayNameFor(gender);
      }

      private IEnumerable<string> genderSelectionsFrom(IVectorialParametersContainer vectorialParametersContainer)
      {
         var allGenders = vectorialParametersContainer.AllGenders.Distinct().ToList();
         if (allGenders.Count == 1)
            yield return allGenders[0].Name;
         else
         {
            yield return CoreConstants.Population.AllGender;
            foreach (var gender in allGenders)
               yield return gender.Name;
         }
      }
   }
}