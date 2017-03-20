using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public interface IMoleculeExpressionContainer : IContainer
   {
      /// <summary>
      ///    Path to organ  where the reaction induced by the protein should take place
      /// </summary>
      IObjectPath OrganPath { get; set; }

      /// <summary>
      ///    Name of the parent container of the compartment (for organ, name of organ, for segemnt either lumen or segment name)
      /// </summary>
      string GroupName { get; set; }

      /// <summary>
      ///    Name of the physical container where the expression will be defined(Organ or segment).
      /// </summary>
      string ContainerName { get; set; }

      /// <summary>
      ///    relative expression value for the container
      /// </summary>
      double RelativeExpression { get; set; }

      /// <summary>
      ///    relative expression value normalized to all other expressions value for the container
      /// </summary>
      double RelativeExpressionNorm { get; set; }

      /// <summary>
      ///    Parameter representing the relative expression value for this container
      /// </summary>
      IParameter RelativeExpressionParameter { get; }

      /// <summary>
      ///    Parameter representing the relative expression normalized value for this container
      /// </summary>
      IParameter RelativeExpressionNormParameter { get; }

      /// <summary>
      ///    Return the path of the compartment where the protein will be defined
      /// </summary>
      IObjectPath CompartmentPath(string compartmentName);

      /// <summary>
      ///    returns true if the container represents a lumen segment otherwise false
      /// </summary>
      bool IsLumen { get; }
   }

   public class MoleculeExpressionContainer : Container, IMoleculeExpressionContainer
   {
      public IObjectPath OrganPath { get; set; }
      public string GroupName { get; set; }
      public string ContainerName { get; set; }

      public override void UpdatePropertiesFrom(IUpdatable sourceObject, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(sourceObject, cloneManager);
         var moleculeExpressionContainer = sourceObject as IMoleculeExpressionContainer;
         if (moleculeExpressionContainer == null) return;
         OrganPath = moleculeExpressionContainer.OrganPath.Clone<IObjectPath>();
         GroupName = moleculeExpressionContainer.GroupName;
         ContainerName = moleculeExpressionContainer.ContainerName;
      }

      public double RelativeExpression
      {
         get { return RelativeExpressionParameter.Value; }
         set { RelativeExpressionParameter.Value = value; }
      }

      public double RelativeExpressionNorm
      {
         get { return RelativeExpressionNormParameter.Value; }
         set { RelativeExpressionNormParameter.Value = value; }
      }

      public IParameter RelativeExpressionParameter
      {
         get { return this.Parameter(CoreConstants.Parameter.RelExp); }
      }

      public IParameter RelativeExpressionNormParameter
      {
         get { return this.Parameter(CoreConstants.Parameter.RelExpNorm); }
      }

      public IObjectPath CompartmentPath(string compartmentName)
      {
         return OrganPath.Clone<IObjectPath>().AndAdd(compartmentName);
      }

      public bool IsLumen
      {
         get { return string.Equals(GroupName, CoreConstants.Groups.GI_LUMEN); }
      }
   }
}