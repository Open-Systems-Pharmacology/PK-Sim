using System.Collections;
using System.Drawing;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.PKAnalyses;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core
{
   public abstract class concern_for_PopulationAnalysisFieldFactory : ContextSpecification<IPopulationAnalysisFieldFactory>
   {
      protected IEntityPathResolver _entityPathResolver;
      protected IFullPathDisplayResolver _fullPathDisplayResolver;
      protected IParameter _parameter;
      private IGenderRepository _genderRepository;
      protected Gender _male;
      protected Gender _female;
      private IColorGenerator _colorGenerator;
      protected IPKParameterRepository _pkParameterRepository;

      protected override void Context()
      {
         _entityPathResolver = A.Fake<IEntityPathResolver>();
         _fullPathDisplayResolver = A.Fake<IFullPathDisplayResolver>();
         _parameter = A.Fake<IParameter>();
         _genderRepository = A.Fake<IGenderRepository>();
         _male = new Gender {DisplayName = "Male"};
         _female = new Gender {DisplayName = "Female"};
         A.CallTo(() => _genderRepository.Male).Returns(_male);
         A.CallTo(() => _genderRepository.Female).Returns(_female);
         _colorGenerator = A.Fake<IColorGenerator>();
         _pkParameterRepository = A.Fake<IPKParameterRepository>();
         sut = new PopulationAnalysisFieldFactory(_entityPathResolver, _fullPathDisplayResolver, _genderRepository, _colorGenerator, _pkParameterRepository);
      }
   }

   public class When_creating_a_population_analysis_field_from_a_parameter : concern_for_PopulationAnalysisFieldFactory
   {
      private PopulationAnalysisParameterField _result;
      private const string _parameterPath = "ParameterPath";

      protected override void Context()
      {
         base.Context();
         A.CallTo(_fullPathDisplayResolver).WithReturnType<string>().Returns("Display");
         A.CallTo(() => _entityPathResolver.PathFor(_parameter)).Returns(_parameterPath);
      }

      protected override void Because()
      {
         _result = sut.CreateFor(_parameter);
      }

      [Observation]
      public void should_return_a_parameter_field_whose_parameter_path_was_set_to_the_parameter_path()
      {
         _result.ParameterPath.ShouldBeEqualTo(_parameterPath);
      }

      [Observation]
      public void the_resulting_parameter_field_name_should_have_been_set_to_the_full_display_path_of_the_parameter()
      {
         _result.Name.ShouldBeEqualTo("Display");
      }
   }

   public class When_creating_a_population_analysis_field_from_a_pk_parameter : concern_for_PopulationAnalysisFieldFactory
   {
      private QuantityPKParameter _pkParameter;
      private PopulationAnalysisPKParameterField _result;

      protected override void Context()
      {
         base.Context();
         _pkParameter = new QuantityPKParameter {QuantityPath = "Path", Name = "toto"};
         A.CallTo(() => _pkParameterRepository.DisplayNameFor(_pkParameter.Name)).Returns("totoDisplay");
      }

      protected override void Because()
      {
         _result = sut.CreateFor(_pkParameter, QuantityType.Metabolite, "quantityPath");
      }

      [Observation]
      public void should_return_a_parameter_field_whose_name_was_set_to_the_parameter_name()
      {
         _result.PKParameter.ShouldBeEqualTo(_pkParameter.Name);
      }

      [Observation]
      public void should_return_a_parameter_field_whose_quantity_path_was_set_to_the_parameter_quantity_path()
      {
         _result.QuantityPath.ShouldBeEqualTo(_pkParameter.QuantityPath);
      }

      [Observation]
      public void should_return_a_parameter_field_whose_quantity_type_was_set_to_the_given_quantity_type()
      {
         _result.QuantityType.ShouldBeEqualTo(QuantityType.Metabolite);
      }

      [Observation]
      public void the_resulting_parameter_field_name_should_have_been_set_to_the_default_name()
      {
         _result.Name.ShouldBeEqualTo("quantityPath|totoDisplay");
      }
   }

   public class When_creating_a_covariate_field : concern_for_PopulationAnalysisFieldFactory
   {
      private IPopulationDataCollector _populationDataCollector;
      private PopulationAnalysisCovariateField _covariate;
      private const string _covariateName = "Covariate";

      protected override void Context()
      {
         base.Context();
         _populationDataCollector = A.Fake<IPopulationDataCollector>();
         A.CallTo(() => _populationDataCollector.AllCovariateValuesFor(_covariateName)).Returns(new[] {"Value1", "Value2"});
      }

      protected override void Because()
      {
         _covariate = sut.CreateFor(_covariateName, _populationDataCollector);
      }

      [Observation]
      public void should_return_a_covariate_field()
      {
         _covariate.ShouldNotBeNull();
      }

      [Observation]
      public void should_have_defined_one_grouping_item_for_each_distinct_value_defined_for_the_covariate()
      {
         _covariate.GroupingItems.Count.ShouldBeEqualTo(2);
         _covariate.GroupingByName("Value1").ShouldNotBeNull();
         _covariate.GroupingByName("Value2").ShouldNotBeNull();
      }
   }

   public class When_creating_a_covariate_field_with_reference_grouoping_item : concern_for_PopulationAnalysisFieldFactory
   {
      private IPopulationDataCollector _populationDataCollector;
      private PopulationAnalysisCovariateField _covariate;
      private const string _covariateName = "Covariate";
      private const string _referenceLabel = "Reference";

      protected override void Context()
      {
         base.Context();
         _populationDataCollector = A.Fake<IPopulationDataCollector>();
         A.CallTo(() => _populationDataCollector.AllCovariateValuesFor(_covariateName)).Returns(new[] {"Value1", "Value2"});
      }

      protected override void Because()
      {
         _covariate = sut.CreateFor(_covariateName, _populationDataCollector);
         _covariate.ReferenceGroupingItem = new GroupingItem() {Color = Color.Black, Label = _referenceLabel, Symbol = Symbols.Circle};
      }

      [Observation]
      public void should_return_a_covariate_field()
      {
         _covariate.ShouldNotBeNull();
      }

      [Observation]
      public void should_have_defined_one_grouping_item_for_each_distinct_value_defined_for_the_covariate()
      {
         _covariate.GroupingItems.Count.ShouldBeEqualTo(3);
         _covariate.GroupingByName("Value1").ShouldNotBeNull();
         _covariate.GroupingByName("Value2").ShouldNotBeNull();
         _covariate.GroupingByName(_referenceLabel).ShouldNotBeNull();
      }

      [Observation]
      public void should_sort_values_by_labels_and_reference_is_last()
      {
         var labels = new[] {"Value1", "Value2"};
         _covariate.Compare(labels[0], labels[0]).ShouldBeEqualTo(Comparer.Default.Compare(labels[0], labels[0]));
         _covariate.Compare(labels[0], labels[1]).ShouldBeEqualTo(Comparer.Default.Compare(labels[0], labels[1]));
         _covariate.Compare(labels[1], labels[0]).ShouldBeEqualTo(Comparer.Default.Compare(labels[1], labels[0]));
         _covariate.Compare(labels[1], labels[1]).ShouldBeEqualTo(Comparer.Default.Compare(labels[1], labels[1]));

         //with reference
         _covariate.Compare(labels[0], _referenceLabel).ShouldBeEqualTo(-1);
         _covariate.Compare(labels[1], _referenceLabel).ShouldBeEqualTo(-1);
         _covariate.Compare(_referenceLabel, _referenceLabel).ShouldBeEqualTo(0);
         _covariate.Compare(_referenceLabel, labels[0]).ShouldBeEqualTo(1);
         _covariate.Compare(_referenceLabel, labels[1]).ShouldBeEqualTo(1);
      }
   }

   public class When_creating_a_covariate_field_for_gender : concern_for_PopulationAnalysisFieldFactory
   {
      private IPopulationDataCollector _populationDataCollector;
      private PopulationAnalysisCovariateField _covariate;
      private readonly string _covariateName = CoreConstants.Covariates.GENDER;

      protected override void Context()
      {
         base.Context();
         _populationDataCollector = A.Fake<IPopulationDataCollector>();
         A.CallTo(() => _populationDataCollector.AllCovariateValuesFor(_covariateName)).Returns(new[] {_male.DisplayName, _female.DisplayName});
      }

      protected override void Because()
      {
         _covariate = sut.CreateFor(_covariateName, _populationDataCollector);
      }

      [Observation]
      public void should_return_a_covariate_field()
      {
         _covariate.ShouldNotBeNull();
      }

      [Observation]
      public void should_have_used_the_default_color_and_symbol_settings_for_male_and_female()
      {
         _covariate.GroupingByName("Male").Color.ShouldBeEqualTo(PKSimColors.Male);
         _covariate.GroupingByName("Male").Symbol.ShouldBeEqualTo(Symbols.Circle);
         _covariate.GroupingByName("Female").Color.ShouldBeEqualTo(PKSimColors.Female);
         _covariate.GroupingByName("Female").Symbol.ShouldBeEqualTo(Symbols.Diamond);
      }
   }
}