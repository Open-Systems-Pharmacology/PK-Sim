using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;
using CalculationMethodCache = OSPSuite.Core.Domain.CalculationMethodCache;

namespace PKSim.Core
{
   public abstract class concern_for_CalculationMethodCacheMapper : ContextSpecificationAsync<CalculationMethodCacheMapper>
   {
      private ICalculationMethodRepository _calculationMethodRepository;
      private ICalculationMethodCategoryRepository _calculationMethodCategoryRepository;
      protected CalculationMethodCache _calculationMethodCache;
      protected CalculationMethod _calculationMethodWithMultipleOptions;
      protected CalculationMethod _calculationMethodWithSingleOption;
      protected Snapshots.CalculationMethodCache _snapshot;
      protected CalculationMethodCategory _singleCategory;
      protected CalculationMethodCategory _multipleCategory;
      protected CalculationMethod _anotherCalculationMethodInMultipleOptions;

      protected override Task Context()
      {
         _calculationMethodRepository = A.Fake<ICalculationMethodRepository>();
         _calculationMethodCategoryRepository = A.Fake<ICalculationMethodCategoryRepository>();
         sut = new CalculationMethodCacheMapper(_calculationMethodRepository, _calculationMethodCategoryRepository);
         _singleCategory = new CalculationMethodCategory {Name = "Multiple"};
         _multipleCategory = new CalculationMethodCategory {Name = "Single"};


         _calculationMethodWithMultipleOptions = new CalculationMethod
         {
            Name = "CM1",
            DisplayName = "CM1",
            Category = _multipleCategory.Name
         };

         _calculationMethodWithSingleOption = new CalculationMethod
         {
            Name = "CM2",
            DisplayName = "CM2",
            Category = _singleCategory.Name
         };

         _anotherCalculationMethodInMultipleOptions = new CalculationMethod
         {
            Name = "Another CM",
            Category = _multipleCategory.Name
         };

         A.CallTo(() => _calculationMethodRepository.All()).Returns(new []{_calculationMethodWithMultipleOptions, _anotherCalculationMethodInMultipleOptions, _calculationMethodWithSingleOption, });

         _singleCategory.Add(_calculationMethodWithSingleOption);
         _multipleCategory.Add(_calculationMethodWithMultipleOptions);
         _multipleCategory.Add(_anotherCalculationMethodInMultipleOptions);

         _calculationMethodCache = new CalculationMethodCache();
         _calculationMethodCache.AddCalculationMethod(_calculationMethodWithMultipleOptions);
         _calculationMethodCache.AddCalculationMethod(_calculationMethodWithSingleOption);

         A.CallTo(() => _calculationMethodCategoryRepository.FindBy(_singleCategory.Name)).Returns(_singleCategory);
         A.CallTo(() => _calculationMethodCategoryRepository.FindBy(_multipleCategory.Name)).Returns(_multipleCategory);

         return Task.FromResult(true);
      }
   }

   public class When_mapping_a_calculation_method_cache_to_snapshot : concern_for_CalculationMethodCacheMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_calculationMethodCache);
      }

      [Observation]
      public void should_return_a_snapshot_with_one_entry_for_each_category_containing_more_than_one_calculation_method()
      {
         _snapshot.ShouldOnlyContain(_calculationMethodWithMultipleOptions.Name);
      }
   }

   public class When_mapping_a_calculation_method_cache_snapshot_to_a_calculation_method_cache : concern_for_CalculationMethodCacheMapper
   {
      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_calculationMethodCache);
         //Switch two calculation methods
         _calculationMethodCache.RemoveCalculationMethod(_calculationMethodWithMultipleOptions);
         _calculationMethodCache.AddCalculationMethod(_anotherCalculationMethodInMultipleOptions);
      }

      protected override Task Because()
      {
         return sut.MapToModel(_snapshot,_calculationMethodCache);
      }

      [Observation]
      public void should_update_the_used_calculation_method_for_the_category()
      {
         _calculationMethodCache.CalculationMethodFor(_multipleCategory.Name).ShouldBeEqualTo(_calculationMethodWithMultipleOptions);
      }

      [Observation]
      public void should_not_change_other_calculation_method()
      {
         _calculationMethodCache.CalculationMethodFor(_singleCategory.Name).ShouldBeEqualTo(_calculationMethodWithSingleOption);
      }
   }

   public class When_mapping_a_calculation_method_cache_snapshot_containing_an_unknow_calculation_method : concern_for_CalculationMethodCacheMapper
   {
      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_calculationMethodCache);
         _snapshot.Add("UNKNOW CM");
      }

      [Observation]
      public void should_throw_an_exception()
      {
         The.Action(() => sut.MapToModel(_snapshot, _calculationMethodCache))
            .ShouldThrowAn<SnapshotOutdatedException>();  
      }
   }
}