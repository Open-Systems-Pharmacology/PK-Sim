using System.Collections.Generic;

namespace PKSim.Core.Model
{
   /// <summary>
   ///    Expression of a protein in each container, for one unit of the gene expression database
   /// </summary>
   public class UnitExpression
   {
      public UnitExpression(string unit, IReadOnlyList<ContainerExpression> containerExpressions)
      {
         Unit = unit;
         ContainerExpressions = containerExpressions;
      }

      public string Unit { get; }
      public IReadOnlyList<ContainerExpression> ContainerExpressions { get; }
   }

   public class ContainerExpression
   {
      public ContainerExpression(ExpressionContainerInfo container, double? expressionValue, double? relativeExpression)
      {
         Container = container;
         ExpressionValue = expressionValue;
         RelativeExpression = relativeExpression;
      }

      public ExpressionContainerInfo Container { get; }

      /// <summary>
      ///    Mean normalized expression of the records of the container, or null when the container has no record
      /// </summary>
      public double? ExpressionValue { get; }

      /// <summary>
      ///    <see cref="ExpressionValue" /> relative to the highest expression value of the unit, or null when the container has
      ///    no record
      /// </summary>
      public double? RelativeExpression { get; }
   }
}
