using System.Collections.Generic;
using OSPSuite.Core.Domain;
using PKSim.Presentation.DTO.Parameters;
using PKSim.Presentation.Presenters.Parameters;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Parameters
{
   public interface IMultiParameterEditView : IView<IMultiParameterEditPresenter>, IResizableView
   {
      void BindTo(IEnumerable<ParameterDTO> parameters);
      bool DistributionVisible { get; set; }
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
      IEnumerable<ParameterDTO> SelectedParameters { get; }
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
   }
}