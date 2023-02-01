using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using OSPSuite.Core.Extensions;

namespace PKSim.Core
{
   public abstract class concern_for_FloatMatrix : ContextSpecification<FloatMatrix>
   {
      protected override void Context()
      {
         sut = new FloatMatrix();
      }
   }

   public class When_creating_a_float_matrix_with_a_given_number_of_colums : concern_for_FloatMatrix
   {
      private float[] _valideRow;
      private float[] _invalideRow;
      private float[] _lessRow;

      protected override void Context()
      {
         base.Context();
         _valideRow = new float[10].InitializeWith(1);
         _lessRow = new float[9].InitializeWith(1);
         _invalideRow = new float[12].InitializeWith(2);
      }

      [Observation]
      public void should_accepts_rows_with_the_appropriate_number_of_columns()
      {
         sut.AddSortedValues(_valideRow);
      }

      [Observation]
      public void should_accepts_rows_with_too_many_columns()
      {
         sut.AddSortedValues(_invalideRow);
      }

      [Observation]
      public void should_accepts_rows_with_not_enough_columns()
      {
         sut.AddSortedValues(_lessRow);
      }
   }

   public class When_adding_rows_to_the_matrix : concern_for_FloatMatrix
   {
      private float[] _row1;
      private float[] _row2;
      private float[] _lessRow;

      protected override void Context()
      {
         base.Context();
         _row1 = new float[10].InitializeWith(1);
         _row2 = new float[10].InitializeWith(1);
         _lessRow = new float[9].InitializeWith(1);
         sut.AddSortedValues(_row1);
         sut.AddSortedValues(_row2);
         sut.AddSortedValues(_lessRow);
      }

      [Observation]
      public void should_be_able_to_retrieve_the_rows_in_the_order_they_were_added()
      {
         sut.SortedValueAt(0).ShouldBeEqualTo(_row1);
         sut.SortedValueAt(1).ShouldBeEqualTo(_row2);
      }
   }

   public class when_adding_an_unsorted_and_unfiltered_row_to_the_matric : concern_for_FloatMatrix
   {
      protected override void Because()
      {
         sut.AddValuesAndSort(new []{1,float.NaN,5,4});
      }

      [Observation]
      public void should_store_the_results_sorted_and_filtered()
      {
         sut.SortedValueAt(0).ShouldOnlyContainInOrder(1f,4f,5f);
      }
   }

   public class When_asking_for_a_slice : concern_for_FloatMatrix
   {
      private float[] _row1;
      private float[] _row2;

      protected override void Context()
      {
         base.Context();
         _row1 = new[] { 0.0f, 1.0f, 2.0f };
         _row2 = new[] { 0.1f, 0.2f, 0.3f };
         sut.AddSortedValues(_row1);
         sut.AddSortedValues(_row2);
      }

      [Observation]
      public void should_retrive_correctly()
      {
         sut.SliceAt(0).ShouldBeEqualTo(new[] { 0.0f, 0.1f });
         sut.SliceAt(1).ShouldBeEqualTo(new[] { 1.0f, 0.2f });
         sut.SliceAt(2).ShouldBeEqualTo(new[] { 2.0f, 0.3f });
      }
   }
}