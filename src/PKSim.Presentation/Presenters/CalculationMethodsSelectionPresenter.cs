using System;
using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation.DTO;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters
{
   public interface ICalculationMethodSelectionPresenter : ISubPresenter
   {
      /// <summary>
      ///    Edit this object with calculation methods and use the optional filter on categories to determine which calculation
      ///    methods to show
      /// </summary>
      /// <param name="objectWithCalculationMethods">The object being edited</param>
      /// <param name="predicate">
      ///    The predicate being used to select calculation methods for editing. Null
      ///    selects all calculation methods
      /// </param>
      void Edit(IWithCalculationMethods objectWithCalculationMethods, Func<CalculationMethodCategory, bool> predicate = null);

      bool ShouldDisplayCategory(string categoryName);

      IEnumerable<CalculationMethod> AllCalculationMethodsFor(string category);

      /// <summary>
      ///    Indicates whether any calculation method categories are actually editable for this presenter
      /// </summary>
      /// <returns>True if there are any categories to be edited, otherwise false</returns>
      bool AnyCategories();
   }

   public abstract class CalculationMethodSelectionPresenter<TView, TPresenter> : AbstractSubPresenter<TView, TPresenter>, ICalculationMethodSelectionPresenter
      where TPresenter : IPresenter
      where TView : ICalculationMethodSelectionView<TPresenter>
   {
      private readonly ICalculationMethodToCategoryCalculationMethodDTOMapper _mapper;
      private readonly ICompoundCalculationMethodCategoryRepository _compoundCalculationMethodCategoryRepository;
      private Func<CalculationMethodCategory, bool> _predicate;
      protected IWithCalculationMethods _objectWithCalculationMethods;
      protected IReadOnlyList<CategoryCalculationMethodDTO> _allCalculationMethodDTOs;

      protected CalculationMethodSelectionPresenter(
         TView view,
         ICalculationMethodToCategoryCalculationMethodDTOMapper mapper,
         ICompoundCalculationMethodCategoryRepository compoundCalculationMethodCategoryRepository)
         : base(view)
      {
         _mapper = mapper;
         _compoundCalculationMethodCategoryRepository = compoundCalculationMethodCategoryRepository;
         _view.Caption = PKSimConstants.UI.CalculationMethods;
      }

      protected CalculationMethodCategory CalculationMethodCategory(string category)
      {
         return _compoundCalculationMethodCategoryRepository.FindByName(category);
      }

      public bool ShouldDisplayCategory(string categoryName)
      {
         var category = CalculationMethodCategory(categoryName);
         return category.AllItems().Count() > 1;
      }

      private bool passesFilters(CalculationMethodCategory category)
      {
         return _predicate(category);
      }

      public IEnumerable<CalculationMethod> AllCalculationMethodsFor(string category)
      {
         return CalculationMethodCategory(category).AllItems();
      }

      private void setPredicate(Func<CalculationMethodCategory, bool> predicate)
      {
         _predicate = predicate ?? (x => true);
      }

      public bool AnyCategories()
      {
         return _objectWithCalculationMethods.AllCalculationMethods().Any(x => passesFilters(CalculationMethodCategory(x.Category)) && ShouldDisplayCategory(x.Category));
      }

      public void Edit(IWithCalculationMethods objectWithCalculationMethods, Func<CalculationMethodCategory, bool> predicate = null)
      {
         setPredicate(predicate);
         _objectWithCalculationMethods = objectWithCalculationMethods;
         updateCalculationMethods(_objectWithCalculationMethods.AllCalculationMethods().Where(x => passesFilters(CalculationMethodCategory(x.Category))));         
      }

      private void updateCalculationMethods(IEnumerable<CalculationMethod> calculationMethods)
      {
         _allCalculationMethodDTOs = calculationMethods.MapAllUsing(_mapper);
         if (_allCalculationMethodDTOs.Any())
            _view.BindTo(_allCalculationMethodDTOs);
      }
   }
}