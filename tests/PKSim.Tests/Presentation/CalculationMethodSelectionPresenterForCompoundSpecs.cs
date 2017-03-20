using OSPSuite.BDDHelper;
using OSPSuite.Core.Commands.Core;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Core.Domain;

namespace PKSim.Presentation
{
   public abstract class concern_for_CalculationMethodSelectionPresenterForCompound : ContextSpecification<CalculationMethodSelectionPresenterForCompound>
   {
      private ICalculationMethodSelectionViewForCompound _view;
      private ICalculationMethodToCategoryCalculationMethodDTOMapper _mapper;
      private ICompoundCalculationMethodCategoryRepository _repository;
      protected IPKSimCalculationMethodsTask _calculationMethodsTask;

      protected override void Context()
      {
         _view = A.Fake<ICalculationMethodSelectionViewForCompound>();
         _mapper = A.Fake<ICalculationMethodToCategoryCalculationMethodDTOMapper>();
         _repository = A.Fake<ICompoundCalculationMethodCategoryRepository>();
         _calculationMethodsTask = A.Fake<IPKSimCalculationMethodsTask>();
         
         sut = new CalculationMethodSelectionPresenterForCompound(_view, _mapper, _repository, _calculationMethodsTask);
         sut.InitializeWith(A.Fake<ICommandCollector>());
      }
   }

   public class when_setting_a_new_calculation_method_for_a_compound : concern_for_CalculationMethodSelectionPresenterForCompound
   {
      private CalculationMethod _oldCalculationMethod;
      private CalculationMethod _newCalculationMethod;
      private string _category;
      private Compound _objectWithCalculationMethods;

      protected override void Context()
      {
         base.Context();
         _objectWithCalculationMethods = A.Fake<Compound>();
         sut.Edit(_objectWithCalculationMethods);
      }

      protected override void Because()
      {
         _category = "category";
         _oldCalculationMethod = A.Fake<CalculationMethod>();
         _newCalculationMethod = A.Fake<CalculationMethod>();
         sut.SetCalculationMethodForCompound(_category, _newCalculationMethod, _oldCalculationMethod);
      }

      [Observation]
      public void a_call_to_the_task_updating_the_calculation_method_must_have_happened()
      {
         A.CallTo(() => _calculationMethodsTask.SetCalculationMethod(_objectWithCalculationMethods, _category, _newCalculationMethod, _oldCalculationMethod)).MustHaveHappened();
      }
   }
}
