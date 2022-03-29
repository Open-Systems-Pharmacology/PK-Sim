using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views
{
   public interface IConfigurableContainerLayoutView : IView
   {

      /// <summary>
      /// Adds finishing touches to the view once it's been configured
      /// </summary>
      void StartAddingViews();

      /// <summary>
      /// Adds a view dynamically to the list of views shown
      /// </summary>
      /// <param name="view"></param>
      void AddView(IView view);

      /// <summary>
      /// Adds finishing touches to the view once it's been configured
      /// </summary>
      void FinishedAddingViews();
   }

   public interface IAccordionLayoutView : IConfigurableContainerLayoutView
   {
      
   }

   public interface ITabbedLayoutView : IConfigurableContainerLayoutView
   {
      
   }
}
