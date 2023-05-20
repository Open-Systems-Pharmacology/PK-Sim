using System.Drawing;
using DevExpress.XtraEditors.ColorPickEditControl;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.UI.Controls;

namespace PKSim.UI
{
   // this test is located here to take advantage of the dedicated UI Test project which does not exist in SBSuite.Core
   public abstract class concern_for_CustomColorArrayManager : ContextSpecification<CustomColorArrayManager>
   {

      protected override void Context()
      {
         sut = new CustomColorArrayManager();
      }
   }

   public abstract class With_full_custom_color_matrix : concern_for_CustomColorArrayManager
   {
      protected Matrix _customColors;

      protected override void Context()
      {
         base.Context();
         _customColors = new Matrix(10, 1);
         sut.PushRecentColor(_customColors, Color.Yellow);
         sut.PushRecentColor(_customColors, Color.Brown);
         sut.PushRecentColor(_customColors, Color.Orange);
         sut.PushRecentColor(_customColors, Color.Black);
         sut.PushRecentColor(_customColors, Color.Blue);
         sut.PushRecentColor(_customColors, Color.Red);
         sut.PushRecentColor(_customColors, Color.Green);
         sut.PushRecentColor(_customColors, Color.Purple);
         sut.PushRecentColor(_customColors, Color.White);
         sut.PushRecentColor(_customColors, Color.Salmon);
      }

      protected Color[] AllColors()
      {
         var colors = new Color[_customColors.ColumnCount * _customColors.RowCount];
         var i = 0;
         _customColors.ForEachItem(color =>
            {

               colors[i] = color.Value;
               i++;
            });
         return colors;
      }
   }

   public abstract class With_full_custom_color_array : concern_for_CustomColorArrayManager
   {
      protected Color[] _customColors;
      protected override void Context()
      {
         base.Context();
         _customColors = new Color[8];
         sut.PushRecentColor(_customColors, Color.Yellow);
         sut.PushRecentColor(_customColors, Color.Brown);
         sut.PushRecentColor(_customColors, Color.Orange);
         sut.PushRecentColor(_customColors, Color.Black);
         sut.PushRecentColor(_customColors, Color.Blue);
         sut.PushRecentColor(_customColors, Color.Red);
         sut.PushRecentColor(_customColors, Color.Green);
         sut.PushRecentColor(_customColors, Color.Purple);
      }
   }

   public class When_pushing_new_color_into_full_matrix : With_full_custom_color_matrix
   {
      protected override void Because()
      {
         sut.PushRecentColor(_customColors, Color.Gray);
      }

      [Observation]
      public void colors_must_shuffle_and_last_item_be_removed_from_array()
      {
         AllColors().ShouldOnlyContainInOrder(Color.Gray, Color.Salmon, Color.White, Color.Purple, Color.Green, Color.Red, Color.Blue, Color.Black, Color.Orange, Color.Brown);
      }
   }

   public class When_pushing_new_color_into_full_array : With_full_custom_color_array
   {
      protected override void Because()
      {
         sut.PushRecentColor(_customColors, Color.Gray);
      }

      [Observation]
      public void colors_must_shuffle_and_last_item_be_removed_from_array()
      {
         _customColors.ShouldOnlyContainInOrder(Color.Gray, Color.Purple, Color.Green, Color.Red, Color.Blue, Color.Black, Color.Orange, Color.Brown);
      }
   }

   public class When_pushing_existing_colors_into_matrix : With_full_custom_color_matrix
   {
      protected override void Because()
      {
         sut.PushRecentColor(_customColors, Color.Red);
      }

      [Observation]
      public void should_move_recent_color_to_front_and_remove_from_other_places_and_shuffle_only_earlier_colors()
      {
         AllColors()[0].ShouldBeEqualTo(Color.Red);
         AllColors()[1].ShouldBeEqualTo(Color.Salmon);
         AllColors()[2].ShouldBeEqualTo(Color.White);
         AllColors()[3].ShouldBeEqualTo(Color.Purple);
         AllColors()[4].ShouldBeEqualTo(Color.Green);
         AllColors()[5].ShouldBeEqualTo(Color.Blue);
         AllColors()[6].ShouldBeEqualTo(Color.Black);
         AllColors()[7].ShouldBeEqualTo(Color.Orange);
         AllColors()[8].ShouldBeEqualTo(Color.Brown);
         AllColors()[9].ShouldBeEqualTo(Color.Yellow);
      }
   }

   public class When_pushing_existing_colors_into_array : With_full_custom_color_array
   {
      protected override void Because()
      {
         sut.PushRecentColor(_customColors, Color.Red);
      }

      [Observation]
      public void should_move_recent_color_to_front_and_remove_from_other_places_and_shuffle_only_earlier_colors()
      {
         _customColors[0].ShouldBeEqualTo(Color.Red);
         _customColors[1].ShouldBeEqualTo(Color.Purple);
         _customColors[2].ShouldBeEqualTo(Color.Green);
         _customColors[3].ShouldBeEqualTo(Color.Blue);
         _customColors[4].ShouldBeEqualTo(Color.Black);
         _customColors[5].ShouldBeEqualTo(Color.Orange);
         _customColors[6].ShouldBeEqualTo(Color.Brown);
         _customColors[7].ShouldBeEqualTo(Color.Yellow);
      }
   }

   public class When_pushing_new_colors_into_array : With_full_custom_color_array
   {
      protected override void Context()
      {
         base.Context();
         sut.PushRecentColor(_customColors, Color.Black);
         sut.PushRecentColor(_customColors, Color.Blue);
      }

      protected override void Because()
      {
         sut.PushRecentColor(_customColors, Color.Red);
      }

      [Observation]
      public void should_preserve_existing_colors_and_add_new_color_at_the_front()
      {
         _customColors[0].ShouldBeEqualTo(Color.Red);
         _customColors[1].ShouldBeEqualTo(Color.Blue);
         _customColors[2].ShouldBeEqualTo(Color.Black);
      }
   }
}
