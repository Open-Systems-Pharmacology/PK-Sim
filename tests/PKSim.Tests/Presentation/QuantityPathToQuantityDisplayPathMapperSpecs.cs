using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;
using FakeItEasy;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Presentation.Mappers;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Mappers;

namespace PKSim.Presentation
{
   public abstract class concern_for_QuantityPathToQuantityDisplayPathMapper : ContextSpecification<IQuantityPathToQuantityDisplayPathMapper>
   {
      protected IndividualSimulation _individualSimulation;
      protected IContainer _bone;
      protected IContainer _intracellular;
      private IObjectPathFactory _objectPathFactory;
      protected IList<string> _displayPath;
      protected IObserver _observer;
      protected string _intracellularDisplay;
      protected string _boneDisplay;
      protected IContainer _root;
      protected IContainer _interstitial;
      protected string _interstitialDisplay;
      protected string _observerDisplayName;
      protected string _displayNameDimension;
      private IPathToPathElementsMapper _pathToPathElementsMapper;
      protected IDataColumnToPathElementsMapper _dataColumnToPathElementsMapper;
      protected DataColumn _column;

      protected override void Context()
      {
         _objectPathFactory = new ObjectPathFactoryForSpecs();
         _dataColumnToPathElementsMapper = A.Fake<IDataColumnToPathElementsMapper>();
         _pathToPathElementsMapper = A.Fake<IPathToPathElementsMapper>();
         sut = new PKSimQuantityPathToQuantityDisplayPathMapper(_objectPathFactory, _pathToPathElementsMapper, _dataColumnToPathElementsMapper);

         _root = new Container().WithName("ROOT");
         var baseGrid = new BaseGrid("Time", DomainHelperForSpecs.TimeDimensionForSpecs());
         _column = new DataColumn("Conc", DomainHelperForSpecs.ConcentrationDimensionForSpecs(), baseGrid);
         _individualSimulation = new IndividualSimulation {Model = new Model {Root = _root}};
      }
   }

   public class When_retrieving_the_path_elements_for_a_column_defined_in_an_individual_simulation : concern_for_QuantityPathToQuantityDisplayPathMapper
   {
      private PathElements _pathElements;
      private PathElements _results;

      protected override void Because()
      {
         _results = sut.DisplayPathFor(_individualSimulation, _column);
      }

      protected override void Context()
      {
         base.Context();
         _pathElements = new PathElements();
         A.CallTo(() => _dataColumnToPathElementsMapper.MapFrom(_column, _individualSimulation.Model.Root)).Returns(_pathElements);
      }

      [Observation]
      public void should_leverage_the_data_column_to_path_elements_mapper_to_create_a_path_element_for_the_given_column()
      {
         _results.ShouldBeEqualTo(_pathElements);
      }
   }

   public class When_retrieving_the_display_name_for_a_column_defined_in_an_individual_simulation : concern_for_QuantityPathToQuantityDisplayPathMapper
   {
      private string _display;
      private PathElements _pathElements;

      protected override void Context()
      {
         base.Context();
         _pathElements = new PathElements();
         _pathElements[PathElement.Simulation] = new PathElementDTO {DisplayName = "Sim"};
         _pathElements[PathElement.TopContainer] = new PathElementDTO {DisplayName = "Organism"};
         _pathElements[PathElement.Container] = new PathElementDTO {DisplayName = "Liver"};
         _pathElements[PathElement.Molecule] = new PathElementDTO {DisplayName = "Drug"};
         _pathElements[PathElement.BottomCompartment] = new PathElementDTO {DisplayName = "Plasma"};
         _pathElements[PathElement.Name] = new PathElementDTO {DisplayName = "OBS"};
         A.CallTo(() => _dataColumnToPathElementsMapper.MapFrom(_column, _individualSimulation.Model.Root)).Returns(_pathElements);
      }

      protected override void Because()
      {
         _display = sut.DisplayPathAsStringFor(_individualSimulation, _column, false);
      }

      [Observation]
      public void should_return_a_string_containing_only_the_relevant_path_elements()
      {
         _display.ShouldBeEqualTo(new[] {"Drug", "Liver", "Plasma", "OBS"}.ToString(Constants.DISPLAY_PATH_SEPARATOR));
      }
   }

   public class When_retrieving_the_display_name_for_an_observed_Data_column_defined_in_an_individual_simulation : concern_for_QuantityPathToQuantityDisplayPathMapper
   {
      private string _display;
      private PathElements _pathElements;

      protected override void Context()
      {
         base.Context();
         _pathElements = new PathElements();
         _pathElements[PathElement.Simulation] = new PathElementDTO {DisplayName = "Sim"};
         _pathElements[PathElement.TopContainer] = new PathElementDTO {DisplayName = "Organism"};
         _pathElements[PathElement.Container] = new PathElementDTO {DisplayName = "Liver"};
         _pathElements[PathElement.Molecule] = new PathElementDTO {DisplayName = "Drug"};
         _pathElements[PathElement.BottomCompartment] = new PathElementDTO {DisplayName = "Plasma"};
         _pathElements[PathElement.Name] = new PathElementDTO {DisplayName = "OBS"};
         _column.DataInfo.Origin = ColumnOrigins.Observation;
         A.CallTo(() => _dataColumnToPathElementsMapper.MapFrom(_column, _individualSimulation.Model.Root)).Returns(_pathElements);
      }

      protected override void Because()
      {
         _display = sut.DisplayPathAsStringFor(_individualSimulation, _column, false);
      }

      [Observation]
      public void should_return_a_string_containing_only_the_relevant_path_elements()
      {
         _display.ShouldBeEqualTo(new[] {"Sim", "Drug", "Liver", "Plasma", "OBS"}.ToString(Constants.DISPLAY_PATH_SEPARATOR));
      }
   }

   public class When_retrieving_the_display_name_for_a_column_defined_in_an_individual_simulation_and_the_simulation_name_should_be_added : concern_for_QuantityPathToQuantityDisplayPathMapper
   {
      private string _display;
      private PathElements _pathElements;

      protected override void Context()
      {
         base.Context();
         _pathElements = new PathElements();
         _pathElements[PathElement.Simulation] = new PathElementDTO {DisplayName = "Sim"};
         _pathElements[PathElement.TopContainer] = new PathElementDTO {DisplayName = "Organism"};
         _pathElements[PathElement.Container] = new PathElementDTO {DisplayName = "Liver"};
         _pathElements[PathElement.Molecule] = new PathElementDTO {DisplayName = "Drug"};
         _pathElements[PathElement.BottomCompartment] = new PathElementDTO {DisplayName = "Plasma"};
         _pathElements[PathElement.Name] = new PathElementDTO {DisplayName = "OBS"};
         A.CallTo(() => _dataColumnToPathElementsMapper.MapFrom(_column, _individualSimulation.Model.Root)).Returns(_pathElements);
      }

      protected override void Because()
      {
         _display = sut.DisplayPathAsStringFor(_individualSimulation, _column, true);
      }

      [Observation]
      public void should_return_a_string_containing_only_the_relevant_path_elements()
      {
         _display.ShouldBeEqualTo(new[] {"Sim", "Drug", "Liver", "Plasma", "OBS"}.ToString(Constants.DISPLAY_PATH_SEPARATOR));
      }
   }

   public class When_retrieving_the_display_name_for_an_entity_defined_in_a_reaction : concern_for_QuantityPathToQuantityDisplayPathMapper
   {
      private string _display;
      private PathElements _pathElements;

      protected override void Context()
      {
         base.Context();
         _pathElements = new PathElements();
         _pathElements[PathElement.Simulation] = new PathElementDTO { DisplayName = "Sim" };
         _pathElements[PathElement.TopContainer] = new PathElementDTO { DisplayName = "Organism" };
         _pathElements[PathElement.Name] = new PathElementDTO { DisplayName = "OBS" };
         A.CallTo(() => _dataColumnToPathElementsMapper.MapFrom(_column, _individualSimulation.Model.Root)).Returns(_pathElements);
      }

      protected override void Because()
      {
         _display = sut.DisplayPathAsStringFor(_individualSimulation, _column, true);
      }

      [Observation]
      public void should_return_a_string_containing_only_the_relevant_path_elements()
      {
         _display.ShouldBeEqualTo(new[] { "Sim", "Organism", "OBS" }.ToString(Constants.DISPLAY_PATH_SEPARATOR));
      }
   }
}