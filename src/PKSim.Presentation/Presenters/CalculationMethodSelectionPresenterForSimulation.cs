using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;

namespace PKSim.Presentation.Presenters
{
   public interface ICalculationMethodSelectionPresenterForSimulation : IConfigurationPresenter, ICalculationMethodSelectionPresenter
   {
      bool ReadOnly { set; }
   }

   public class CalculationMethodSelectionPresenterForSimulation : CalculationMethodSelectionPresenter<ICalculationMethodSelectionViewForSimulation, ICalculationMethodSelectionPresenterForSimulation>, ICalculationMethodSelectionPresenterForSimulation
   {
      public CalculationMethodSelectionPresenterForSimulation(
         ICalculationMethodSelectionViewForSimulation view,
         ICalculationMethodToCategoryCalculationMethodDTOMapper mapper,
         ICompoundCalculationMethodCategoryRepository compoundCalculationMethodCategoryRepository)
         : base(view, mapper, compoundCalculationMethodCategoryRepository)
      {
      }

      public void SaveConfiguration()
      {
         _objectWithCalculationMethods.ClearCalculationMethods();
         _allCalculationMethodDTOs.Each(dto => _objectWithCalculationMethods.AddCalculationMethod(dto.CalculationMethod));
      }

      public bool ReadOnly
      {
         set => _view.SetReadOnly(readOnly: value);
      }
   }
}