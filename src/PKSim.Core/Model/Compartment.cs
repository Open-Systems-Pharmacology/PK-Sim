using OSPSuite.Utility.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public class Compartment : Container
   {
      public Compartment()
      {
         ContainerType = ContainerType.Compartment;
         Mode = ContainerMode.Physical;
      }

      public virtual bool Visible { get; set; }

      public virtual string OrganName
      {
         get { return ParentContainer.Name; }
      }

      public virtual Organism Organism
      {
         get
         {
            var parentContainer = ParentContainer;
            while (parentContainer != null && !parentContainer.IsAnImplementationOf<Organism>())
            {
               parentContainer = parentContainer.ParentContainer;
            }
            return parentContainer.DowncastTo<Organism>();
         }
      }

      public override void UpdatePropertiesFrom(IUpdatable sourceObject, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(sourceObject, cloneManager);
         var sourceCompartment = sourceObject as Compartment;
         if (sourceCompartment == null) return;
         Visible = sourceCompartment.Visible;
      }
   }
}