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

         quantityPresenter.SetCaption(PathElementId.Simulation, Captions.Simulation);
         quantityPresenter.SetCaption(PathElementId.TopContainer, Captions.Organism);
         quantityPresenter.SetCaption(PathElementId.Container, Captions.Organ);
         quantityPresenter.SetCaption(PathElementId.BottomCompartment, Captions.Compartment);
         quantityPresenter.SetCaption(PathElementId.Molecule, Captions.Molecule);
         quantityPresenter.SetCaption(PathElementId.Name, Captions.Name);
         quantityPresenter.GroupBy(PathElementId.Container);
         quantityPresenter.SortColumn(PathElementId.BottomCompartment);
         quantityPresenter.Hide(PathElementId.Simulation);
         quantityPresenter.Hide(PathElementId.TopContainer);
         quantityPresenter.Hide(QuantityColumn.QuantityType);
         quantityPresenter.Hide(QuantityColumn.Dimension);
      }
   }
}