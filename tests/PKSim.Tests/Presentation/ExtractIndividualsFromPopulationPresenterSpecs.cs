using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Populations;
using PKSim.Presentation.Presenters.Populations;
using PKSim.Presentation.Views.Populations;

namespace PKSim.Presentation
{
   public abstract class concern_for_ExtractIndividualsFromPopulationPresenter : ContextSpecification<IExtractIndividualsFromPopulationPresenter>
   {
      protected IExtractIndividualsFromPopulationView _view;
      protected IIndividualExtractor _individualExtractor;
      protected ExtractIndividualsDTO _dto;
      protected List<int> _originalIndividualIds = new List<int>();
      protected Population _population;
      protected ILazyLoadTask _lazyLoadTask;

      protected override void Context()
      {
         _view = A.Fake<IExtractIndividualsFromPopulationView>();
         _individualExtractor = A.Fake<IIndividualExtractor>();
         _lazyLoadTask = A.Fake<ILazyLoadTask>();

         sut = new ExtractIndividualsFromPopulationPresenter(_view, _individualExtractor, _lazyLoadTask);

         A.CallTo(() => _view.BindTo(A<ExtractIndividualsDTO>._))
            .Invokes(x => _dto = x.GetArgument<ExtractIndividualsDTO>(0))
            .Invokes(x => ConfigureDTO());

         _population = A.Fake<Population>().WithName("MY POP");
         A.CallTo(() => _population.NumberOfItems).Returns(10);
      }

      protected virtual void ConfigureDTO()
      {
      }
   }

   public class When_starting_the_individual_extraction_workflow_for_a_given_population : concern_for_ExtractIndividualsFromPopulationPresenter
   {
      protected override void Context()
      {
         base.Context();
         _originalIndividualIds.Add(4);
         _originalIndividualIds.Add(6);
      }

      protected override void Because()
      {
         sut.ExctractIndividuals(_population, _originalIndividualIds);
      }

      [Observation]
      public void should_load_the_population_from_which_individuals_should_be_extracted()
      {
         A.CallTo(() => _lazyLoadTask.Load(_population)).MustHaveHappened();
      }

      [Observation]
      public void should_bind_the_view_to_the_predefined_individual_ids_passed_as_parameter()
      {
         _dto.ShouldNotBeNull();
         _dto.IndividualIds.ShouldOnlyContain(_originalIndividualIds);
         _dto.NamingPattern.ShouldBeEqualTo(IndividualExtractionOptions.DEFAULT_NAMING_PATTERN);
      }

      [Observation]
      public void should_display_a_view_allowing_the_user_to_specify_which_individuals_should_be_extracted()
      {
         A.CallTo(() => _view.Display()).MustHaveHappened();
      }
   }

   public class When_the_user_cancels_the_extraction_of_individuals_from_a_population : concern_for_ExtractIndividualsFromPopulationPresenter
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _view.Canceled).Returns(true);
         sut.ExctractIndividuals(_population, _originalIndividualIds);
      }

      [Observation]
      public void should_not_extract_any_individuals()
      {
         A.CallTo(() => _individualExtractor.ExtractIndividualsFrom(_population, A<IndividualExtractionOptions>._)).MustNotHaveHappened();
      }
   }

   public class When_the_user_confirms_the_extraction_of_individuals_from_a_population : concern_for_ExtractIndividualsFromPopulationPresenter
   {
      private IndividualExtractionOptions _options;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _view.Canceled).Returns(false);

         A.CallTo(() => _individualExtractor.ExtractIndividualsFrom(_population, A<IndividualExtractionOptions>._))
            .Invokes(x => _options = x.GetArgument<IndividualExtractionOptions>(1));

         sut.ExctractIndividuals(_population, _originalIndividualIds);
      }

      protected override void ConfigureDTO()
      {
         _dto.NamingPattern = "AAA";
         _dto.IndividualIds = new[] {1, 5, 6};
      }

      [Observation]
      public void should_extract_the_individuals_selected_by_the_user()
      {
         _options.NamingPattern.ShouldBeEqualTo(_dto.NamingPattern);
         _options.IndividualIds.ShouldOnlyContain(_dto.IndividualIds);
      }
   }

   public class When_the_extract_individuals_from_population_presenter_is_updating_the_output_based_on_user_inputs : concern_for_ExtractIndividualsFromPopulationPresenter
   {
      private IReadOnlyList<string> _names;

      protected override void Context()
      {
         base.Context();
         sut.ExctractIndividuals(_population, _originalIndividualIds);
         A.CallTo(() => _view.UpdateGeneratedOutputDescription(A<int>._, A<IReadOnlyList<string>>._, _population.Name))
            .Invokes(x => _names = x.GetArgument<IReadOnlyList<string>>(1));
      }

      protected override void Because()
      {
         sut.UpdateGeneratedOutput(IndividualExtractionOptions.DEFAULT_NAMING_PATTERN, "1,2,3");
      }

      [Observation]
      public void should_calculate_the_number_of_individual_that_will_be_created_as_well_as_the_name_that_will_be_generated_and_update_the_view_accordingly()
      {
         A.CallTo(() => _view.UpdateGeneratedOutputDescription(3, A<IReadOnlyList<string>>._, _population.Name)).MustHaveHappened();
         var options = new IndividualExtractionOptions();
         _names.ShouldContain(options.GenerateIndividualName(_population.Name,1));
         _names.ShouldContain(options.GenerateIndividualName(_population.Name,2));
         _names.ShouldContain(options.GenerateIndividualName(_population.Name,3));
      }
   }

   public class When_the_extract_individuals_from_population_presenter_is_updating_the_output_based_on_invalid_user_inputs : concern_for_ExtractIndividualsFromPopulationPresenter
   {
      private IReadOnlyList<string> _names;

      protected override void Context()
      {
         base.Context();
         sut.ExctractIndividuals(_population, _originalIndividualIds);
         A.CallTo(() => _view.UpdateGeneratedOutputDescription(A<int>._, A<IReadOnlyList<string>>._, _population.Name))
            .Invokes(x => _names = x.GetArgument<IReadOnlyList<string>>(1));
      }

      protected override void Because()
      {
         sut.UpdateGeneratedOutput(null, null);
      }

      [Observation]
      public void should_update_the_view_with_the_expected_description()
      {
         A.CallTo(() => _view.UpdateGeneratedOutputDescription(0, A<IReadOnlyList<string>>._, _population.Name)).MustHaveHappened();
         _names.ShouldBeEmpty();
      }
   }
}