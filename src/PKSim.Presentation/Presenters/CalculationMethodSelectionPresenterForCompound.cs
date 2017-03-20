using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Core.Domain;

namespace PKSim.Presentation.Presenters
{
   public interface ICalculationMethodSelectionPresenterForCompound : ICalculationMethodSelectionPresenter
   {
      void SetCalculationMethodForCompound(string category, CalculationMethod newCalculationMethod, CalculationMethod oldCalculationMethod);
   }

   public class CalculationMethodSelectionPresenterForCompound : CalculationMethodSelectionPresenter<ICalculationMethodSelectionViewForCompound, ICalculationMethodSelectionPresenterForCompound>, ICalculationMethodSelectionPresenterForCompound
   {
      private readonly IPKSimCalculationMethodsTask _calculationMethodsTask;

      public CalculationMethodSelectionPresenterForCompound(ICalculationMethodSelectionViewForCompound view, ICalculationMethodToCategoryCalculationMethodDTOMapper mapper, ICompoundCalculationMethodCategoryRepository compoundCalculationMethodCategoryRepository, IPKSimCalculationMethodsTask calculationMethodsTask)
         : base(view, mapper, compoundCalculationMethodCategoryRepository)
      {
         _calculationMethodsTask = calculationMethodsTask;
      }

      public void SetCalculationMethodForCompound(string category, CalculationMethod newCalculationMethod, CalculationMethod oldCalculationMethod)
      {
         AddCommand(_calculationMethodsTask.SetCalculationMethod(_objectWithCalculationMethods.DowncastTo<Compound>(), category, newCalculationMethod, oldCalculationMethod));
      }

   }
}