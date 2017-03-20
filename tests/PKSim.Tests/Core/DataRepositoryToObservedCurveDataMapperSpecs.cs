using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Chart;
using PKSim.Core.Mappers;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Repositories;
using PKSim.Presentation.Mappers;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Presentation.Mappers;

namespace PKSim.Core
{
   public abstract class concern_for_DataRepositoryToObservedCurveDataMapper : ContextSpecification<IDataRepositoryToObservedCurveDataMapper>
   {
      private IQuantityPathToQuantityDisplayPathMapper _displayPathMapper;
      private ObservedDataCollection _observedDataCollection;
      protected DataRepository _observedData;
      private BaseGrid _baseGrid;
      protected DataColumn _data;
      protected DataColumn _errorArithmetic;
      protected DataColumn _errorGeometric;
      protected IReadOnlyList<ObservedCurveData> _result;
      protected CurveOptions _curveOptions;
      private IDimensionRepository _dimensionRepository;
      private IDimension _yAxisDimension;

      protected override void Context()
      {
         _yAxisDimension = DomainHelperForSpecs.ConcentrationDimensionForSpecs();
         _observedDataCollection = new ObservedDataCollection();
         _displayPathMapper = A.Fake<IQuantityPathToQuantityDisplayPathMapper>();
         _dimensionRepository = A.Fake<IDimensionRepository>();
         sut = new DataRepositoryToObservedCurveDataMapper(_displayPathMapper, _dimensionRepository);
         _observedData = new DataRepository();
         _baseGrid = new BaseGrid("Time", DomainHelperForSpecs.TimeDimensionForSpecs());
         _baseGrid.Values = new[] {1.0f, 2.0f, 3.0f};

         _data = new DataColumn("Col", DomainHelperForSpecs.ConcentrationDimensionForSpecs(), _baseGrid);
         _data.Values = new[] {10f, 20f, 30f};
         _data.DataInfo.Origin = ColumnOrigins.Observation;

         _observedData.Add(_data);

         _errorArithmetic = new DataColumn("ErrorArithmetic", _data.Dimension, _baseGrid);
         _errorArithmetic.Values = new[] {1.0f, 2.2f, 3.3f};
         _errorArithmetic.DataInfo.AuxiliaryType = AuxiliaryType.ArithmeticStdDev;

         _errorGeometric = new DataColumn("ErrorGeometric", DomainHelperForSpecs.NoDimension(), _baseGrid);
         _errorGeometric.Values = new[] {1.0f, 1.2f, 1.3f};
         _errorGeometric.DataInfo.AuxiliaryType = AuxiliaryType.GeometricStdDev;

         _observedDataCollection.AddObservedData(_observedData);

         _curveOptions = _observedDataCollection.ObservedDataCurveOptionsFor(_data).CurveOptions;
         _curveOptions.Color = Color.Aqua;
         _curveOptions.LineStyle = LineStyles.Dot;
         _curveOptions.Symbol = Symbols.Diamond;
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_observedData, _observedDataCollection, _yAxisDimension);
      }
   }

   public class When_mapping_a_data_repository_to_selected_observed_curve_data : concern_for_DataRepositoryToObservedCurveDataMapper
   {
      [Observation]
      public void should_return_one_curve_for_each_observed_data_column_defined_in_the_repository()
      {
         _result.Count.ShouldBeEqualTo(1);
      }

      [Observation]
      public void should_have_set_the_time_and_concentration_value_according_to_the_value_defined_in_the_data_repository()
      {
         var observedCurveData = _result.ElementAt(0);
         observedCurveData.XValues.Select(x => x.X).ShouldOnlyContainInOrder(1.0f, 2.0f, 3.0f);
         observedCurveData.YValues.Select(x => x.Y).ShouldOnlyContainInOrder(10f, 20f, 30f);
         observedCurveData.YValues.Select(x => x.Error).ShouldOnlyContainInOrder(float.NaN, float.NaN, float.NaN);
      }

      [Observation]
      public void should_have_set_the_error_type_to_undefined()
      {
         var observedCurveData = _result.ElementAt(0);
         observedCurveData.ErrorType.ShouldBeEqualTo(AuxiliaryType.Undefined);
      }

      [Observation]
      public void should_have_updated_the_curve_settings()
      {
         var observedCurveData = _result.ElementAt(0);
         observedCurveData.Color.ShouldBeEqualTo(_curveOptions.Color);
         observedCurveData.LineStyle.ShouldBeEqualTo(_curveOptions.LineStyle);
         observedCurveData.Symbol.ShouldBeEqualTo(_curveOptions.Symbol);
      }
   }

   public class When_mapping_a_data_repository_with_arithmetical_error_to_selected_observed_curve_data : concern_for_DataRepositoryToObservedCurveDataMapper
   {
      protected override void Context()
      {
         base.Context();
         _data.AddRelatedColumn(_errorArithmetic);
         _observedData.Add(_errorArithmetic);
      }

      [Observation]
      public void should_return_one_curve_for_each_observed_data_column_defined_in_the_repository()
      {
         _result.Count.ShouldBeEqualTo(1);
      }

      [Observation]
      public void should_have_set_the_time_and_concentration_value_according_to_the_value_defined_in_the_data_repository()
      {
         var observedCurveData = _result.ElementAt(0);
         observedCurveData.XValues.Select(x => x.X).ShouldOnlyContainInOrder(1.0f, 2.0f, 3.0f);
         observedCurveData.YValues.Select(x => x.Y).ShouldOnlyContainInOrder(10f, 20f, 30f);
         observedCurveData.YValues.Select(x => x.Error).ShouldOnlyContainInOrder(1.0f, 2.2f, 3.3f);
      }

      [Observation]
      public void should_have_set_the_error_type_to_arithmetic()
      {
         var observedCurveData = _result.ElementAt(0);
         observedCurveData.ErrorType.ShouldBeEqualTo(AuxiliaryType.ArithmeticStdDev);
      }
   }

   public class When_mapping_a_data_repository_with_geometric_error_to_selected_observed_curve_data : concern_for_DataRepositoryToObservedCurveDataMapper
   {
      protected override void Context()
      {
         base.Context();
         _data.AddRelatedColumn(_errorGeometric);
         _observedData.Add(_errorGeometric);
      }

      [Observation]
      public void should_return_one_curve_for_each_observed_data_column_defined_in_the_repository()
      {
         _result.Count.ShouldBeEqualTo(1);
      }

      [Observation]
      public void should_have_set_the_time_and_concentration_value_according_to_the_value_defined_in_the_data_repository()
      {
         var observedCurveData = _result.ElementAt(0);
         observedCurveData.XValues.Select(x => x.X).ShouldOnlyContainInOrder(1.0f, 2.0f, 3.0f);
         observedCurveData.YValues.Select(x => x.Y).ShouldOnlyContainInOrder(10f, 20f, 30f);
         observedCurveData.YValues.Select(x => x.Error).ShouldOnlyContainInOrder(1.0f, 1.2f, 1.3f);
      }

      [Observation]
      public void should_have_set_the_error_type_to_geometric()
      {
         var observedCurveData = _result.ElementAt(0);
         observedCurveData.ErrorType.ShouldBeEqualTo(AuxiliaryType.GeometricStdDev);
      }
   }

   public class When_mapping_a_data_repository_to_selected_observed_curve_data_that_are_hidden : concern_for_DataRepositoryToObservedCurveDataMapper
   {
      protected override void Context()
      {
         base.Context();
         _curveOptions.Visible = false;
      }

      [Observation]
      public void should_hide_the_observed_curve_data()
      {
         var observedCurveData = _result.ElementAt(0);
         observedCurveData.Visible.ShouldBeFalse();
      }
   }

   public class When_mapping_a_data_repository_to_selected_observed_curve_data_that_are_visible : concern_for_DataRepositoryToObservedCurveDataMapper
   {
      protected override void Context()
      {
         base.Context();
         _curveOptions.Visible = true;
      }

      [Observation]
      public void should_show_the_observed_curve_data()
      {
         var observedCurveData = _result.ElementAt(0);
         observedCurveData.Visible.ShouldBeTrue();
      }
   }
}