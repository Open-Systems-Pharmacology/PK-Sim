using OSPSuite.Presentation.Presenters;
using PKSim.Presentation.Views.Observers;

namespace PKSim.Presentation.Presenters.Observers
{
   public interface IImportObserversPresenter : IObserverItemPresenter
   {
   }

   public class ImportObserversPresenter : AbstractSubPresenter<IImportObserversView, IImportObserversPresenter>, IImportObserversPresenter
   {
      public ImportObserversPresenter(IImportObserversView view) : base(view)
      {
      }
   }
}