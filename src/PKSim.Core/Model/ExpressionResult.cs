namespace PKSim.Core.Model
{
 
   public class ExpressionResult
   {
      /// <summary>
      ///    Name of PK-Sim container for which an expression was found
      /// </summary>
      public string ContainerName { get; set; }


      /// <summary>
      ///    Relative expression for that container.
      /// </summary>
      public double RelativeExpression { get; set; }

      public override string ToString()
      {
         return ContainerName;
      }
   }
}