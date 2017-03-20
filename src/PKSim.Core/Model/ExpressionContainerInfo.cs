namespace PKSim.Core.Model
{
   public class ExpressionContainerInfo
   {
      public ExpressionContainerInfo(string containerName, string containerDiplayName, double relativeExpression)
      {
         ContainerName = containerName;
         ContainerDiplayName = containerDiplayName;
         RelativeExpression = relativeExpression;
      }

      /// <summary>
      ///    Name of PK-Sim container for which an expression is required
      /// </summary>
      public string ContainerName { get; private set; }

      /// <summary>
      ///    Display name for the container
      /// </summary>
      public string ContainerDiplayName { get; private set; }

      /// <summary>
      ///    Former value for the container if available
      /// </summary>
      public double RelativeExpression { get; private set; }

      public override string ToString()
      {
         return ContainerDiplayName;
      }
   }
}