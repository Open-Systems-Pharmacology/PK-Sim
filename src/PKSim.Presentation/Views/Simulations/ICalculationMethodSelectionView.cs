using System.Collections.Generic;
using PKSim.Presentation.DTO;
using PKSim.Presentation.Presenters;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Simulations
{
   public interface ICalculationMethodSelectionView<TPresenter> : IResizableView, IView<TPresenter> where TPresenter : IPresenter
   {
      void BindTo(IEnumerable<CategoryCalculationMethodDTO> allCalculationMethodDTOs);
   }

   public interface ICalculationMethodSelectionViewForSimulation : ICalculationMethodSelectionView<ICalculationMethodSelectionPresenterForSimulation>
   {
      void SetReadOnly(bool readOnly);
   }

   public interface ICalculationMethodSelectionViewForCompound : ICalculationMethodSelectionView<ICalculationMethodSelectionPresenterForCompound>
   {
   }
}