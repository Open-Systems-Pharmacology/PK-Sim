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
   }
}