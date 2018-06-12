using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using FakeItEasy;

namespace PKSim.Core
{
   public abstract class concern_for_ModelPropertiesTask : ContextSpecification<IModelPropertiesTask>
   {
      protected IModelConfigurationRepository _modelConfigurationRepository;
      protected OriginData _originData;

      protected override void Context()
      {
         _modelConfigurationRepository = A.Fake<IModelConfigurationRepository>();
         _originData = new OriginData {Species = A.Fake<Species>().WithName(CoreConstants.Species.HUMAN)};
         sut = new ModelPropertiesTask(_modelConfigurationRepository);
      }
   }

   
   public class When_resolving_the_default_model_properties_for_a_given_origin_data : concern_for_ModelPropertiesTask
   {
      private CalculationMethod _cm1;
      private CalculationMethod _cm2;
      private ModelProperties _result;
      private ModelConfiguration _modelConfiguration;

      protected override void Context()
      {
         base.Context();
         _modelConfiguration = A.Fake<ModelConfiguration>();
         var cmc1 = A.Fake<CalculationMethodCategory>();
         var cmc2 = A.Fake<CalculationMethodCategory>();
         _cm1 = new CalculationMethod().WithName("toto");
         _cm2 = new CalculationMethod().WithName("tata");
         A.CallTo(() => cmc1.DefaultItemForSpecies(_originData.Species)).Returns(_cm1);
         A.CallTo(() => cmc2.DefaultItemForSpecies(_originData.Species)).Returns(_cm2);
         A.CallTo(() => _modelConfigurationRepository.DefaultFor(_originData.Species)).Returns(_modelConfiguration);
         A.CallTo(() => _modelConfiguration.CalculationMethodCategories).Returns(new[] {cmc1, cmc2});
      }

      protected override void Because()
      {
         _result = sut.DefaultFor(_originData);
      }

      [Observation]
      public void should_return_a_model_properties_containing_the_default_calculation_method_for_the_species()
      {
         _result.AllCalculationMethods().ShouldOnlyContainInOrder(_cm1, _cm2);
      }
   }

   
   public class When_updating_a_model_properties_from_another_model_properties_for_the_same_species_and_model   : concern_for_ModelPropertiesTask
   {
      private ModelProperties _oldModelProperties;
      private ModelProperties _newModelProperties;
      private ModelProperties _result;
      private CalculationMethod _c1;
      private CalculationMethod _c2;

      protected override void Context()
      {
         base.Context();
         _c1 = new CalculationMethod { Name = "C1", Category = "C" };
         _c1.AddSpecies(_originData.Species.Name);
         _c2 = new CalculationMethod { Name = "C2", Category = "C" };
         _c2.AddSpecies(_originData.Species.Name);
         var modelConfiguration = new ModelConfiguration();
         modelConfiguration.ModelName = "Model";
         modelConfiguration.SpeciesName = "Species";
         var category = new CalculationMethodCategory{Name = "C"};
         category.Add(_c1);
         category.Add(_c2);
         modelConfiguration.AddCalculationMethodCategory(category);
         _oldModelProperties = new ModelProperties{ModelConfiguration = modelConfiguration};
         _newModelProperties = new ModelProperties { ModelConfiguration = modelConfiguration };
         _oldModelProperties.AddCalculationMethod(_c1);
         _newModelProperties.AddCalculationMethod(_c2);
      }

      protected override void Because()
      {
         _result = sut.Update(_oldModelProperties, _newModelProperties,_originData);
      }

      [Observation]
      public void should_return_the_former_model_propertie_without_changing_any_selected_calculation_method()
      {
         _result.ShouldBeEqualTo(_oldModelProperties);
         _result.AllCalculationMethods().ShouldContain(_c1);
      }
   }

   
   public class When_updating_a_model_properties_from_another_model_properties_with_a_different_species_or_model : concern_for_ModelPropertiesTask
   {
      private ModelProperties _oldModelProperties;
      private ModelProperties _newModelProperties;
      private ModelProperties _result;
      private CalculationMethod _c1;
      private CalculationMethod _c2;
      private CalculationMethod _c3;

      protected override void Context()
      {
         base.Context();
         _c1 = new CalculationMethod { Name = "C1", Category = "C" };
         _c2 = new CalculationMethod { Name = "C2", Category = "C" };
         _c3 = new CalculationMethod { Name = "C3", Category = "AnotherCategory" };
         var modelConfiguration1 = new ModelConfiguration();
         modelConfiguration1.ModelName = "Model";
         modelConfiguration1.SpeciesName = "Species";
         var category = new CalculationMethodCategory { Name = "C" };
         category.Add(_c1);
         category.Add(_c2);
         var anotherCategory = new CalculationMethodCategory { Name = "AnotherCategory" };
         anotherCategory.Add(_c3);
         var modelConfiguration2 = new ModelConfiguration();
         modelConfiguration2.ModelName = "Model2";
         modelConfiguration2.SpeciesName = "Species2";
         
         modelConfiguration1.AddCalculationMethodCategory(category);
         modelConfiguration2.AddCalculationMethodCategory(category);
         modelConfiguration2.AddCalculationMethodCategory(anotherCategory);
         _oldModelProperties = new ModelProperties { ModelConfiguration = modelConfiguration1 };
         _newModelProperties = new ModelProperties { ModelConfiguration = modelConfiguration2 };

         _oldModelProperties.AddCalculationMethod(_c1);
         _newModelProperties.AddCalculationMethod(_c2);
         _newModelProperties.AddCalculationMethod(_c3);
      }

      protected override void Because()
      {
         _result = sut.Update(_oldModelProperties, _newModelProperties,_originData);
      }

      [Observation]
      public void should_return_the_later_model_propertie_and_try_to_update_the_common_properties()
      {
         _result.ShouldBeEqualTo(_newModelProperties);
         _result.AllCalculationMethods().ShouldContain(_c1);
         _result.AllCalculationMethods().ShouldContain(_c3);
      }
   }
}