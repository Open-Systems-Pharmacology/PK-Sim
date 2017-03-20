using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;

namespace PKSim.IntegrationTests
{
   public class When_serializing_a_jacobi_matrix : ContextForSerialization<JacobianMatrix>
   {
      private JacobianMatrix _jacobianMatrix;
      private JacobianMatrix _deserializedMatrix;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _jacobianMatrix =new JacobianMatrix(new [] {"ParameterPath1", "ParameterPath2" });

         _jacobianMatrix.AddRow(new JacobianRow("O1", 1, new [] {10d,11d}));
         _jacobianMatrix.AddRow(new JacobianRow("O2", 2, new [] {20d,21d}));
         _jacobianMatrix.AddRow(new JacobianRow("O3", 3, new [] {30d,31d}));

         _deserializedMatrix = SerializeAndDeserialize(_jacobianMatrix);
      }

      [Observation]
      public void should_be_able_to_deserialize_the_matrix()
      {
         _deserializedMatrix.ShouldNotBeNull();
      }

      [Observation]
      public void should_have_the_same_number_of_rows_as_the_original()
      {
         _jacobianMatrix.RowCount.ShouldBeEqualTo(_deserializedMatrix.RowCount);
      }

      [Observation]
      public void should_have_the_same_number_of_columns_as_the_original()
      {
         _jacobianMatrix.ColumnCount.ShouldBeEqualTo(_deserializedMatrix.ColumnCount);
      }

      [Observation]
      public void should_have_the_same_values_as_the_original()
      {
         _deserializedMatrix[0]["ParameterPath1"].ShouldBeEqualTo(10d);
         _deserializedMatrix[0]["ParameterPath2"].ShouldBeEqualTo(11d);

         _deserializedMatrix[1].FullOutputPath.ShouldBeEqualTo("O2");
         _deserializedMatrix[1].Time.ShouldBeEqualTo(2);
         _deserializedMatrix[1].Values.ShouldBeEqualTo(new [] { 20d, 21d });
      }
   }
}