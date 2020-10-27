using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public class MoleculeExpressionContainer : Container
   {
      /// <summary>
      ///    Name of the parent container of the compartment (for organ, name of organ, for segment either lumen or segment name)
      /// </summary>
      public string GroupName { get; set; }

      public override void UpdatePropertiesFrom(IUpdatable sourceObject, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(sourceObject, cloneManager);
         if (!(sourceObject is MoleculeExpressionContainer moleculeExpressionContainer)) return;

         GroupName = moleculeExpressionContainer.GroupName;
      }

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