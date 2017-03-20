using PKSim.Presentation.Core;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views
{
    public static class ContainerViewExtensions
    {
        public static void HideControl<TSubPresenterItem>(this IContainerView view, TSubPresenterItem itemToHide) where TSubPresenterItem : ISubPresenterItem
        {
            view.SetControlVisible(itemToHide, false);
        }

        public static void ShowControl<TSubPresenterItem>(this IContainerView view, TSubPresenterItem itemToShow) where TSubPresenterItem : ISubPresenterItem
        {
            view.SetControlVisible(itemToShow, true);
        }

        public static void EnableControl<TSubPresenterItem>(this IContainerView view, TSubPresenterItem itemToEnable) where TSubPresenterItem : ISubPresenterItem
        {
            view.SetControlEnabled(itemToEnable, true);
        }

        public static void DisableControl<TSubPresenterItem>(this IContainerView view, TSubPresenterItem itemToDisable) where TSubPresenterItem : ISubPresenterItem
        {
            view.SetControlEnabled(itemToDisable, false);
        }
    }
}