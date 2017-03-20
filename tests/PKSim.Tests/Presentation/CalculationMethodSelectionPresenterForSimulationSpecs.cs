using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation.DTO;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Core.Domain;

namespace PKSim.Presentation
{
   public abstract class concern_for_CalculationMethodSelectionPresenterForSimulation : ContextSpecification<CalculationMethodSelectionPresenterForSimulation>
   {
      private ICompoundCalculationMethodCategoryRepository _repository;
      private ICalculationMethodSelectionViewForSimulation _view;
      private ICalculationMethodToCategoryCalculationMethodDTOMapper _mapper;
      protected List<CalculationMethodCategory> _allCalculationMethodCategories;
      protected List<CategoryCalculationMethodDTO> _allDTOs;

      protected override void Context()
      {
         _repository = A.Fake<ICompoundCalculationMethodCategoryRepository>();
         _view = A.Fake<ICalculationMethodSelectionViewForSimulation>();
         _mapper = A.Fake<ICalculationMethodToCategoryCalculationMethodDTOMapper>();

         sut = new CalculationMethodSelectionPresenterForSimulation(_view, _mapper, _repository);
         _allCalculationMethodCategories = new List<CalculationMethodCategory>();
         A.CallTo(() => _repository.All()).Returns(_allCalculationMethodCategories);

         A.CallTo(() => _view.BindTo(A<IEnumerable<CategoryCalculationMethodDTO>>._))
            .Invokes(x => _allDTOs = x.GetArgument<IEnumerable<CategoryCalculationMethodDTO>>(0).ToList());
      }
   }

   public class when_editing_categories_and_none_are_editable : concern_for_CalculationMethodSelectionPresenterForSimulation
   {
      private string _categoryName;
      private CalculationMethodCategory _calculationMethodCategory1;
      private CalculationMethod _cm1;
      private CalculationMethod _cm2;
      private CalculationMethodCategory _calculationMethodCategory2;
      private bool _result;
      private CalculationMethod _cm3;

      protected override void Context()
      {
         base.Context();
         _categoryName = "categoryName";
         _calculationMethodCategory1 = new CalculationMethodCategory { Name = _categoryName + "1" };
         _calculationMethodCategory2 = new CalculationMethodCategory { Name = _categoryName + "2" };

         _cm1 = new CalculationMethod { Category = _categoryName + "1", Name = "name1"};
         _cm2 = new CalculationMethod { Category = _categoryName + "2", Name = "name2"};
         _cm3 = new CalculationMethod {Category = _categoryName + "2", Name = "name3"};
         _calculationMethodCategory1.Add(_cm1);
         _calculationMethodCategory2.Add(_cm2);
         _calculationMethodCategory2.Add(_cm3);

         _allCalculationMethodCategories.Add(_calculationMethodCategory1);
         _allCalculationMethodCategories.Add(_calculationMethodCategory2);

         var compound = new Compound();
         compound.CalculationMethodCache.AddCalculationMethod(_cm1);
         compound.CalculationMethodCache.AddCalculationMethod(_cm2);
         compound.CalculationMethodCache.AddCalculationMethod(_cm3);

         sut.Edit(compound, x => !string.Equals(x.Name, "categoryName2"));
      }

      protected override void Because()
      {
         _result = sut.AnyCategories();
      }

      [Observation]
      public void should_not_have_any_categories_visible()
      {
         _result.ShouldBeFalse();
      }
   }

   public class When_asked_for_the_list_of_all_calculation_methods_available_for_a_category : concern_for_CalculationMethodSelectionPresenterForSimulation
   {
      private string _categoryName;
      private IEnumerable<CalculationMethod> _result;
      private CalculationMethodCategory _calculationMethodCategory;
      private CalculationMethod _cm1;
      private CalculationMethod _cm2;

      protected override void Context()
      {
         base.Context();
         _categoryName = "tralal";
         _calculationMethodCategory = new CalculationMethodCategory {Name = _categoryName};
         _cm1 = new CalculationMethod();
         _cm2 = new CalculationMethod();
         _calculationMethodCategory.Add(_cm1);
         _calculationMethodCategory.Add(_cm2);

         _allCalculationMethodCategories.Add(_calculationMethodCategory);
      }

      protected override void Because()
      {
         _result = sut.AllCalculationMethodsFor(_categoryName);
      }

      [Observation]
      public void should_return_the_calculation_methods_available_for_the_selected_model()
      {
         _result.ShouldOnlyContainInOrder(_cm1, _cm2);
      }
   }

   public class When_the_calculation_methods_presenter_is_asked_if_a_category_should_be_displayed : concern_for_CalculationMethodSelectionPresenterForSimulation
   {
      private CalculationMethodCategory _catOnlyOne;
      private CalculationMethodCategory _catTwo;

      protected override void Context()
      {
         base.Context();

         _catOnlyOne = new CalculationMethodCategory {Name = "Model"};
         _catOnlyOne.Add(new CalculationMethod {Name = "tralala"});
         _catTwo = new CalculationMethodCategory {Name = "Compound"};
         _catTwo.Add(new CalculationMethod {Name = "tralala"});
         _catTwo.Add(new CalculationMethod {Name = "tutu"});

         _allCalculationMethodCategories.Add(_catOnlyOne);
         _allCalculationMethodCategories.Add(_catTwo);
      }

      [Observation]
      public void should_return_false_if_the_category_has_only_one_calculation_method()
      {
         sut.ShouldDisplayCategory(_catOnlyOne.Name).ShouldBeFalse();
      }

      [Observation]
      public void should_return_true_if_the_category_has_two_or_more_calculation_method_and_is_not_an_individual_category()
      {
         sut.ShouldDisplayCategory(_catTwo.Name).ShouldBeTrue();
      }
   }

   public class When_saving_the_calculation_method_selection : concern_for_CalculationMethodSelectionPresenterForSimulation
   {
      private IWithCalculationMethods _withCalculationMethods;
      private CalculationMethod _cm2;
      private CalculationMethodCategory _category;

      protected override void Context()
      {
         base.Context();
         _category = new CalculationMethodCategory {Name = "Compound"};
         var cm1 = new CalculationMethod {Name = "tralala", Category = _category.Name};
         _cm2 = new CalculationMethod {Name = "tutu", Category = _category.Name};
         _category.Add(cm1);
         _category.Add(_cm2);

         _withCalculationMethods = new CompoundProperties();
         _withCalculationMethods.AddCalculationMethod((cm1));
         _allCalculationMethodCategories.Add(_category);

         sut.Edit(_withCalculationMethods);

         _allDTOs[0].CalculationMethod = _cm2;
      }

      protected override void Because()
      {
         sut.SaveConfiguration();
      }

      [Observation]
      public void should_have_save_the_selected_calculation_method_used_in_the_underlying_object()
      {
         _withCalculationMethods.ContainsCalculationMethod(_cm2.Name).ShouldBeTrue();
      }
   }
}