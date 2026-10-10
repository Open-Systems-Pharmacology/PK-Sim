namespace PKSim.Core.Model
{
   /// <summary>
   ///    An expression data record assigned to one of the containers it is mapped to
   /// </summary>
   public class ContainerExpressionDataRecord
   {
      public ContainerExpressionDataRecord(ExpressionDataRecord record, ExpressionContainerInfo container)
      {
         Record = record;
         Container = container;
      }

      public ExpressionDataRecord Record { get; }
      public ExpressionContainerInfo Container { get; }
   }
}
