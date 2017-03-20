using PKSim.Presentation.Regions;
using OSPSuite.Core.Journal;
using OSPSuite.Presentation.Mappers;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Presentation.Presenters.Journal;
using OSPSuite.Presentation.Regions;
using OSPSuite.Presentation.Views.Journal;

namespace PKSim.Presentation.Presenters.Main
{
   public class JournalPresenter : OSPSuite.Presentation.Presenters.Journal.JournalPresenter
   {
      public JournalPresenter(IJournalView view, IRegionResolver regionResolver, IJournalPageToJournalPageDTOMapper mapper,
         IJournalTask journalTask, IViewItemContextMenuFactory viewItemContextMenuFactory,
         IJournalRetriever journalRetriever, IJournalPagePreviewPresenter previewPresenter,
         IJournalSearchPresenter searchPresenter) :
            base(view, regionResolver, mapper, journalTask, viewItemContextMenuFactory, journalRetriever,
               previewPresenter, searchPresenter, RegionNames.Journal)
      {
      }
   }
}