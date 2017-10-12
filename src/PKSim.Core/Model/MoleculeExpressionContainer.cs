using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public class MoleculeExpressionContainer : Container
   {
      /// <summary>
      ///    Path to organ  where the reaction induced by the protein should take place
      /// </summary>
      public IObjectPath OrganPath { get; set; }

      /// <summary>
      ///    Name of the parent container of the compartment (for organ, name of organ, for segemnt either lumen or segment name)
      /// </summary>
      public string GroupName { get; set; }

      /// <summary>
      ///    Name of the physical container where the expression will be defined(Organ or segment).
      /// </summary>
      public string ContainerName { get; set; }

      public override void UpdatePropertiesFrom(IUpdatable sourceObject, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(sourceObject, cloneManager);
         if (!(sourceObject is MoleculeExpressionContainer moleculeExpressionContainer)) return;

         OrganPath = moleculeExpressionContainer.OrganPath.Clone<IObjectPath>();
         GroupName = moleculeExpressionContainer.GroupName;
         ContainerName = moleculeExpressionContainer.ContainerName;
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
      ///    relative expression value normalized to all other expressions value for the container
      /// </summary>
      public double RelativeExpressionNorm
      {
         get => RelativeExpressionNormParameter.Value;
         set => RelativeExpressionNormParameter.Value = value;
      }

      /// <summary>
      ///    Parameter representing the relative expression value for this container
      /// </summary>
      public IParameter RelativeExpressionParameter => this.Parameter(CoreConstants.Parameter.REL_EXP);

      /// <summary>
      ///    Parameter representing the relative expression normalized value for this container
      /// </summary>
      public IParameter RelativeExpressionNormParameter => this.Parameter(CoreConstants.Parameter.REL_EXP_NORM);

      /// <summary>
      ///    Return the path of the compartment where the protein will be defined
      /// </summary>
      public IObjectPath CompartmentPath(string compartmentName)
      {
         return OrganPath.Clone<IObjectPath>().AndAdd(compartmentName);
      }

      /// <summary>
      ///    returns true if the container represents a lumen segment otherwise false
      /// </summary>
      public bool IsLumen => string.Equals(GroupName, CoreConstants.Groups.GI_LUMEN);
   }
}