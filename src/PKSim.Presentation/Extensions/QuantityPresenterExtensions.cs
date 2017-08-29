using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Assets;

namespace PKSim.Presentation.Extensions
{
   public static class QuantityPresenterExtensions
   {
      public static void UpdateColumnSettings(this IQuantityPresenter quantityPresenter, IAnalysable analysable)
      {
         //update default captions
         if (!analysable.ComesFromPKSim)
            return;

         quantityPresenter.SetCaption(PathElement.Simulation, Captions.Simulation);
         quantityPresenter.SetCaption(PathElement.TopContainer, Captions.Organism);
         quantityPresenter.SetCaption(PathElement.Container, Captions.Organ);
         quantityPresenter.SetCaption(PathElement.BottomCompartment, Captions.Compartment);
         quantityPresenter.SetCaption(PathElement.Molecule, Captions.Molecule);
         quantityPresenter.SetCaption(PathElement.Name, Captions.Name);
         quantityPresenter.GroupBy(PathElement.Container);
         quantityPresenter.SortColumn(PathElement.BottomCompartment);
         quantityPresenter.Hide(PathElement.Simulation);
         quantityPresenter.Hide(PathElement.TopContainer);
         quantityPresenter.Hide(QuantityColumn.QuantityType);
         quantityPresenter.Hide(QuantityColumn.Dimension);
      }
   }
}