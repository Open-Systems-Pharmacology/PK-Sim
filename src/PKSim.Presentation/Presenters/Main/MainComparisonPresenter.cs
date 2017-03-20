using OSPSuite.Utility.Events;
using PKSim.Core;
using PKSim.Core.Events;
using PKSim.Presentation.Regions;
using OSPSuite.Core.Services;
using OSPSuite.Presentation;
using OSPSuite.Presentation.Presenters.Comparisons;
using OSPSuite.Presentation.Regions;
using OSPSuite.Presentation.Views.Comparisons;

namespace PKSim.Presentation.Presenters.Main
{
   public class MainComparisonPresenter : OSPSuite.Presentation.Presenters.Comparisons.MainComparisonPresenter,
      IListener<BuildingBlockRemovedEvent>

   {
      public MainComparisonPresenter(IMainComparisonView view, IRegionResolver regionResolver, IComparisonPresenter comparisonPresenter, IComparerSettingsPresenter comparerSettingsPresenter,
         IPresentationUserSettings presentationUserSettings, IDialogCreator dialogCreator, IExportDataTableToExcelTask exportToExcelTask, IExecutionContext executionContext)
         : base(view, regionResolver, comparisonPresenter, comparerSettingsPresenter, presentationUserSettings, dialogCreator, exportToExcelTask, executionContext, RegionNames.Comparison)
      {
      }

      public void Handle(BuildingBlockRemovedEvent eventToHandle)
      {
         ClearComparisonIfComparing(eventToHandle.BuildingBlock);
      }
   }
}