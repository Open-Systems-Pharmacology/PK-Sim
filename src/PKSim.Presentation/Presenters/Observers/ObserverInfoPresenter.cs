using OSPSuite.Core.Domain.Builder;
using OSPSuite.Presentation.Presenters;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Views.Observers;

namespace PKSim.Presentation.Presenters.Observers
{
   public interface IObserverInfoPresenter : IPresenter<IObserverInfoView>
   {
      void Edit(IObserverBuilder observer);
   }

   public class ObserverInfoPresenter : AbstractPresenter<IObserverInfoView, IObserverInfoPresenter>, IObserverInfoPresenter
   {
      private readonly IObservedToObserverDTOMapper _observerMapper;

      public ObserverInfoPresenter(IObserverInfoView view, IObservedToObserverDTOMapper observerMapper) : base(view)
      {
         _observerMapper = observerMapper;
      }

      public void Edit(IObserverBuilder observer)
      {
         if (observer == null)
            _view.Clear();
         else
            _view.BindTo(_observerMapper.MapFrom(observer));
      }
   }
}