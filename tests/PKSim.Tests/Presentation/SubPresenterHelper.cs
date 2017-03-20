using System.Collections.Generic;
using OSPSuite.Utility.Events;
using FakeItEasy;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation
{
   public static class SubPresenterHelper
   {
      public static ISubPresenterItemManager<T> Create<T>() where T : ISubPresenter
      {
         var manager = A.Fake<ISubPresenterItemManager<T>>();
         A.CallTo(() => manager.EventPublisher).Returns(A.Fake<IEventPublisher>());
         A.CallTo(() => manager.AllSubPresenters).Returns(new List<T>());
         return manager;
      }

      public static T CreateFake<T, U>(this ISubPresenterItemManager<U> subPresenterManager, ISubPresenterItem<T> subPresenterItem)
         where U : ISubPresenter
         where T : class, U
      {
         var subPresenter = A.Fake<T>();
         A.CallTo(() => subPresenter.BaseView).Returns(A.Fake<IView>());
         A.CallTo(() => subPresenterManager.PresenterAt(subPresenterItem)).Returns(subPresenter);
         A.CallTo(() => subPresenterManager.PresenterAt((ISubPresenterItem<U>)subPresenterItem)).Returns(subPresenter);
         var list = (List<U>) subPresenterManager.AllSubPresenters;
         list.Add(subPresenter);
         return subPresenter;
      }
   }
}