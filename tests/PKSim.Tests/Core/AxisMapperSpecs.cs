﻿using System.Drawing;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain.UnitSystem;
using PKSim.Core.Repositories;
using PKSim.Core.Snapshots.Mappers;

namespace PKSim.Core
{
   public abstract class concern_for_AxisMapper : ContextSpecificationAsync<AxisMapper>
   {
      protected Axis _axis;
      protected Snapshots.Axis _snapshot;
      protected IDimension _dimension;
      protected IDimensionRepository _dimensionRepository;

      protected override Task Context()
      {
         _dimensionRepository = A.Fake<IDimensionRepository>();
         sut = new AxisMapper(_dimensionRepository);
         _dimension = DomainHelperForSpecs.TimeDimensionForSpecs();

         _axis = new Axis(AxisTypes.Y)
         {
            Caption = "Axis Caption",
            GridLines = true,
            Dimension = _dimension,
            UnitName = _dimension.Unit("h").Name,
            Visible = true,
            Min = 60, //min
            Max = 120,
            DefaultColor = Color.AntiqueWhite,
            DefaultLineStyle = LineStyles.Solid
         };

         A.CallTo(() => _dimensionRepository.DimensionByName(_dimension.Name)).Returns(_dimension);

         return _completed;
      }
   }

   public class When_mapping_an_axis_to_snapshot : concern_for_AxisMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_axis);
      }

      [Observation]
      public void should_save_the_axis_properties_to_snapshot()
      {
         _snapshot.Caption.ShouldBeEqualTo(_axis.Caption);
         _snapshot.GridLines.ShouldBeEqualTo(_axis.GridLines);
         _snapshot.Dimension.ShouldBeEqualTo(_axis.Dimension.Name);
         _snapshot.Unit.ShouldBeEqualTo(_axis.UnitName);
         _snapshot.Visible.ShouldBeEqualTo(_axis.Visible);
         _snapshot.DefaultColor.ShouldBeEqualTo(_axis.DefaultColor);
         _snapshot.DefaultLineStyle.ShouldBeEqualTo(_axis.DefaultLineStyle);
      }

      [Observation]
      public void should_have_saved_the_min_and_max_as_is_as_they_are_already_saved_in_display_unit()
      {
         _snapshot.Min.ShouldBeEqualTo(60);
         _snapshot.Max.ShouldBeEqualTo(120);
      }
   }

   public class When_mapping_an_axis_to_snapshot_that_does_not_have_a_dimension : concern_for_AxisMapper
   {
      protected override async Task Context()
      {
         await base.Context();
         _axis.Dimension = null;
      }

      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_axis);
      }

      [Observation]
      public void should_return_a_snapshot_with_an_empty_dimension()
      {
         _snapshot.Dimension.ShouldBeNull();
      }
   }

   public class When_mapping_an_axis_without_min_or_max_to_snapshot : concern_for_AxisMapper
   {
      protected override async Task Context()
      {
         await base.Context();
         _axis.Max = null;
      }

      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_axis);
      }

      [Observation]
      public void should_not_set_any_min_or_max_into_the_snapshot()
      {
         _snapshot.Min.ShouldBeEqualTo(60);
         _snapshot.Max.ShouldBeNull();
      }
   }

   public class When_mapping_an_axis_snapshot_to_axis : concern_for_AxisMapper
   {
      private Axis _newAxis;
      private IDimension _optimalDimension;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_axis);
         _optimalDimension = DomainHelperForSpecs.TimeDimensionForSpecs();
         A.CallTo(() => _dimensionRepository.OptimalDimensionFor(_dimension)).Returns(_optimalDimension);
      }

      protected override async Task Because()
      {
         _newAxis = await sut.MapToModel(_snapshot);
      }

      [Observation]
      public void should_return_an_axis_with_the_expected_properties()
      {
         _newAxis.Caption.ShouldBeEqualTo(_snapshot.Caption);
         _newAxis.GridLines.ShouldBeEqualTo(_snapshot.GridLines);
         _newAxis.UnitName.ShouldBeEqualTo(_snapshot.Unit);
         _newAxis.Visible.ShouldBeEqualTo(_snapshot.Visible);
         _newAxis.DefaultColor.ShouldBeEqualTo(_snapshot.DefaultColor);
         _newAxis.DefaultLineStyle.ShouldBeEqualTo(_snapshot.DefaultLineStyle);
      }

      [Observation]
      public void should_return_the_optimal_merged_dimension_for_the_original_dimension()
      {
         _newAxis.Dimension.ShouldBeEqualTo(_optimalDimension);
      }

      [Observation]
      public void should_have_loaded_the_min_and_max_from_snapshot_as_is()
      {
         _newAxis.Min.ShouldBeEqualTo(60);
         _newAxis.Max.ShouldBeEqualTo(120);
      }
   }
}