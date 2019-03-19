using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Views.Individuals;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters.Charts;


namespace PKSim.Presentation
{
   public abstract class concern_for_ShowOntogenyDataPresenter : ContextSpecification<IShowOntogenyDataPresenter>
   {
      protected IShowOntogenyDataView _view;
      protected IOntogenyRepository _ontogenyRepository;
      protected ISimpleChartPresenter _simpleChartPresenter;
      private IDimensionRepository _dimensionRepository;
      protected Ontogeny _ontogeny;
      protected List<OntogenyMetaData> _ontoData;
      protected List<Ontogeny> _allOntongies;
      protected Ontogeny _anotherOntogeny;
      protected List<OntogenyMetaData> _allMetaData;
      protected TableFormula _tableFormula;
      private IGroupRepository _groupRepository;
      protected IGroup _groupLiver;
      protected IGroup _groupDuodenum;
      protected ShowOntogenyDataDTO ShowOntogenyDataDTO { get; private set; }
      protected IDisplayUnitRetriever _displayUnitRetriever;

      protected override void Context()
      {
         _view = A.Fake<IShowOntogenyDataView>();
         _ontogenyRepository = A.Fake<IOntogenyRepository>();
         _simpleChartPresenter = A.Fake<ISimpleChartPresenter>();
         _dimensionRepository = A.Fake<IDimensionRepository>();
         _groupRepository = A.Fake<IGroupRepository>();
         _displayUnitRetriever = A.Fake<IDisplayUnitRetriever>();

         _groupLiver = new Group {Name = "Liver"};
         _groupDuodenum = new Group {Name = "Duodenum"};
         _tableFormula = new TableFormula();
         _ontogeny = new DatabaseOntogeny {SpeciesName = "toto"};
         _anotherOntogeny = new DatabaseOntogeny {SpeciesName = "toto"};
         _allOntongies = new List<Ontogeny> {_ontogeny, _anotherOntogeny};
         _ontoData = new List<OntogenyMetaData>();
         _ontoData.Add(new OntogenyMetaData {GroupName = "Liver"});
         _ontoData.Add(new OntogenyMetaData {GroupName = "Duodenum"});
         _allMetaData = new List<OntogenyMetaData>();

         A.CallTo(() => _dimensionRepository.AgeInYears).Returns(A.Fake<IDimension>());
         A.CallTo(() => _dimensionRepository.Fraction).Returns(DomainHelperForSpecs.FractionDimensionForSpecs());
         A.CallTo(() => _simpleChartPresenter.Plot(A<TableFormula>._)).Returns(new CurveChart().WithAxes());
         A.CallTo(() => _simpleChartPresenter.Plot(A<DataRepository>._, Scalings.Linear)).Returns(new CurveChart().WithAxes());
         A.CallTo(() => _groupRepository.GroupByName("Liver")).Returns(_groupLiver);
         A.CallTo(() => _groupRepository.GroupByName("Duodenum")).Returns(_groupDuodenum);
         A.CallTo(() => _ontogenyRepository.AllValuesFor(_ontogeny)).Returns(_ontoData);
         A.CallTo(() => _ontogenyRepository.AllFor(_ontogeny.SpeciesName)).Returns(_allOntongies);
         A.CallTo(() => _ontogenyRepository.AllValuesFor(_ontogeny, _groupLiver.Name)).Returns(_allMetaData);

         sut = new ShowOntogenyDataPresenter(_view, _ontogenyRepository, _simpleChartPresenter, _dimensionRepository, _groupRepository, _displayUnitRetriever);

         A.CallTo(() => _view.BindTo(A<ShowOntogenyDataDTO>._))
            .Invokes(x => ShowOntogenyDataDTO = x.GetArgument<ShowOntogenyDataDTO>(0));
      }
   }

   public class When_the_show_ontogeny_data_presenter_is_asked_to_display_the_ontogeny_data_for_a_given_ontogeny : concern_for_ShowOntogenyDataPresenter
   {
      protected override void Because()
      {
         sut.Show(_ontogeny);
      }

      [Observation]
      public void should_retrieve_all_the_meta_data_available_in_the_pksim_database_for_the_given_ontogeny()
      {
         A.CallTo(() => _ontogenyRepository.AllValuesFor(_ontogeny)).MustHaveHappened();
      }

      [Observation]
      public void should_plot_the_data_for_the_ontogeny()
      {
         A.CallTo(() => _simpleChartPresenter.Plot(A<DataRepository>._, A<Scalings>._)).MustHaveHappened();
      }

      [Observation]
      public void should_display_them_in_the_view()
      {
         var dto = ShowOntogenyDataDTO;
         dto.SelectedOntogeny.ShouldBeEqualTo(_ontogeny);
         dto.SelectedContainer.ShouldBeEqualTo(_groupLiver);
      }
   }

   public class When_the_selected_target_container_is_being_changed_for_a_given_enzyme : concern_for_ShowOntogenyDataPresenter
   {
      protected override void Context()
      {
         base.Context();
         sut.Show(_ontogeny);
         var dto = ShowOntogenyDataDTO;
         dto.SelectedContainer = _groupDuodenum;
         A.CallTo(() => _ontogenyRepository.AllValuesFor(_ontogeny, _groupDuodenum.Name)).Returns(_allMetaData);
      }

      protected override void Because()
      {
         sut.ContainerChanged();
      }

      [Observation]
      public void should_retrieve_the_data_for_duodenum()
      {
         A.CallTo(() => _ontogenyRepository.AllValuesFor(_ontogeny, _groupDuodenum.Name)).MustHaveHappened();
      }

      [Observation]
      public void should_plot_the_data_for_the_new_enzyme()
      {
         //twice: first time in context and second time with container changed
         A.CallTo(() => _simpleChartPresenter.Plot(A<DataRepository>._, A<Scalings>._)).MustHaveHappenedTwiceExactly();
      }
   }
}