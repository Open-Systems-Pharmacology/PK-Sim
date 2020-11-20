using OSPSuite.Core.Domain;

namespace PKSim.Core.Model
{
   public class MoleculeExpressionContainer : Container
   {
      /// <summary>
      ///    relative expression value for the container
      /// </summary>
      public double RelativeExpression
      {
         get => RelativeExpressionParameter.Value;
         set => RelativeExpressionParameter.Value = value;
      }

      /// <summary>
      ///    Parameter representing the relative expression value for this container
      /// </summary>
      public IParameter RelativeExpressionParameter => this.Parameter(CoreConstants.Parameters.REL_EXP);


      /// <summary>
      ///    Returns the logical container where the expression container is defined.
      ///    The parent is typically a compartment so we go one level up (parent.parent) 
      /// </summary>
      public IContainer LogicalContainer => ParentContainer?.ParentContainer;

      /// <summary>
      ///    Returns the name of the logical container where the expression container is defined.
      ///    The parent is typically a compartment so we go one level up (parent.parent) and get its name.
      /// </summary>
      public string ContainerName => LogicalContainer?.Name ?? string.Empty;
   }
}