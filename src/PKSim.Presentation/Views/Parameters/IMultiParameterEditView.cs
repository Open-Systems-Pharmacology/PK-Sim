using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Views;
using PKSim.Presentation.DTO.Parameters;
using PKSim.Presentation.Presenters.Parameters;

namespace PKSim.Presentation.Views.Parameters
{
   public interface IMultiParameterEditView : IView<IMultiParameterEditPresenter>, IResizableView
   {
      void BindTo(IEnumerable<ParameterDTO> parameters);
      bool DistributionVisible { get; set; }
      bool ValueOriginVisible { get; set; }
      bool ParameterNameVisible { get; set; }
      void SetVisibility(PathElement pathElement, bool visible);
      void SetCaption(PathElement pathElement, string caption);
      bool CategoryVisible { get; set; }
      bool ParameterPathVisible { set; }
      bool GroupingVisible { get; set; }
      bool FavoritesVisible { set; }
      bool HeaderVisible { get; set; }
      bool ShowRowIndicator { get; set; }
      bool ScalingVisible { get; set; }

      void SetScaleParameterView(IScaleParametersView view);
      void DeleteBinding();
      void RefreshData();
      bool AllowMultiSelect { set; }

      void GroupByCategory();
      void GroupBy(PathElement pathElement, int groupIndex = 0, bool useCustomSort = true);
      void FixParameterColumnWidth(int parameterWitdh);

      /// <summary>
      ///    Returns all parameters currently being dislayed in the view. This is a subest of all edited parameters (user might
      ///    have filtered out some parameters)
      /// </summary>
      IEnumerable<ParameterDTO> AllVisibleParameters { get; }

      void SaveEditor();

      /// <summary>
      ///    if set to true, parameter will be compared for sort only if sharing the same hiearchy of visible groups
      ///    It is useful for events, default is false
      /// </summary>
      bool UseAdvancedSortingMode { set; }

      /// <summary>
      ///    Set to false, the custom sort using parameter sequence is not taken into consideration. Default is true
      /// </summary>
      bool CustomSortEnabled { set; }

      /// <summary>
      ///    Returns or set parameters selected in the view.
      /// </summary>
      IReadOnlyList<ParameterDTO> SelectedParameters { get; set; }
   }
}