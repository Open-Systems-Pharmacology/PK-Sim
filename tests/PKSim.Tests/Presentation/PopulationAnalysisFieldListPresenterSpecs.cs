using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.PopulationAnalyses;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;

namespace PKSim.Presentation
{
   public abstract class concern_for_PopulationAnalysisFieldListPresenter : ContextSpecification<IPopulationAnalysisFieldListPresenter>
   {
      protected IPopulationAnalysisFieldListView _view;
      protected IPopulationAnalysisFieldToPopulationAnalysisFieldDTOMapper _mapper;
      protected IPopulationAnalysisFieldsDragDropBinder _dragDropBinder;

      protected override void Context()
      {
         _view = A.Fake<IPopulationAnalysisFieldListView>();
         _mapper = A.Fake<IPopulationAnalysisFieldToPopulationAnalysisFieldDTOMapper>();
         _dragDropBinder = A.Fake<IPopulationAnalysisFieldsDragDropBinder>();
         A.CallTo(() => _view.DragDropBinder).Returns(_dragDropBinder);

         sut = new PopulationAnalysisFieldListPresenter(_view, _mapper);
      }
   }

   public class When_intializing_the_population_analysis_field_presenter : concern_for_PopulationAnalysisFieldListPresenter
   {
      [Observation]
      public void should_set_the_default_allowed_type_to_string_field()
      {
         sut.AllowedType.ShouldBeEqualTo(typeof(IStringValueField));
      }
   }

   public class When_asked_to_update_the_description : concern_for_PopulationAnalysisFieldListPresenter
   {
      protected override void Because()
      {
         sut.UpdateDescription("toto");
      }

      [Observation]
      public void should_update_the_description_in_the_view()
      {
         _view.Description.ShouldBeEqualTo("toto");
      }
   }

   public class When_notify_that_fields_were_dropped : concern_for_PopulationAnalysisFieldListPresenter
   {
      private IReadOnlyList<IPopulationAnalysisField> _fields;
      private IPopulationAnalysisField _target;
      private object _presenter;
      private FieldsMovedEventArgs _args;

      protected override void Context()
      {
         base.Context();
         _fields = new List<IPopulationAnalysisField>();
         _target = A.Fake<IPopulationAnalysisField>();
         sut.FieldsMoved += (o, e) =>
         {
            _presenter = o;
            _args = e;
         };
      }

      protected override void Because()
      {
         _dragDropBinder.FieldsDropped += Raise.With(new FieldsMovedEventArgs(_fields, _target, PivotArea.FilterArea));
      }

      [Observation]
      public void should_raise_an_event_notifying_that_fields_were_moved()
      {
         _args.Fields.ShouldBeEqualTo(_fields);
         _args.Target.ShouldBeEqualTo(_target);
         _args.Area.ShouldBeEqualTo(PivotArea.FilterArea);
         _presenter.ShouldBeEqualTo(sut);
      }
   }

   public class When_refreshing_the_analysis : concern_for_PopulationAnalysisFieldListPresenter
   {
      private PopulationPivotAnalysis _populationAnalysis;
      private PopulationAnalysisFieldDTO _field1DTO;
      private PopulationAnalysisFieldDTO _field2DTO;
      private IEnumerable<PopulationAnalysisFieldDTO> _fields;

      protected override void Context()
      {
         base.Context();
         _populationAnalysis = A.Fake<PopulationPivotAnalysis>();
         sut.AllowedType = typeof (PopulationAnalysisDataField);
         sut.Area = PivotArea.RowArea;
         var field1 = A.Fake<IPopulationAnalysisField>();
         var field2 = A.Fake<IPopulationAnalysisField>();
         var allDataFields = new List<IPopulationAnalysisField> { field1, field2 };
         sut.StartAnalysis(A.Fake<IPopulationDataCollector>(), _populationAnalysis);
         _field1DTO= new PopulationAnalysisFieldDTO(field1);
         _field2DTO = new PopulationAnalysisFieldDTO(field2);
         A.CallTo(() => _mapper.MapFrom(field1)).Returns(_field1DTO);
         A.CallTo(() => _mapper.MapFrom(field2)).Returns(_field2DTO);
         A.CallTo(() => _populationAnalysis.AllFieldsOn(sut.Area, sut.AllowedType)).Returns(allDataFields);

         A.CallTo(() => _view.BindTo(A<IEnumerable<PopulationAnalysisFieldDTO>>._))
            .Invokes(x => _fields = x.GetArgument<IEnumerable<PopulationAnalysisFieldDTO>>(0));
      }

      protected override void Because()
      {
         sut.RefreshAnalysis();
      }

      [Observation]
      public void should_retrieve_all_fields_located_on_the_area_matching_the_allowed_type_and_display_them_in_the_UI()
      {
         _fields.ShouldOnlyContainInOrder(_field1DTO, _field2DTO);
      }
   }
}