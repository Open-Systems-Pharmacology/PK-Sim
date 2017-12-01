using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public class Organ : Container
   {
      public Organ()
      {
         ContainerType = ContainerType.Organ;
      }

      public virtual OrganType OrganType { get; set; }

      public virtual IEnumerable<Compartment> Compartments => GetChildren<Compartment>();

       public virtual Compartment Compartment(string compartmentName)
      {
         return this.GetSingleChildByName<Compartment>(compartmentName);
      }

      public override void UpdatePropertiesFrom(IUpdatable sourceObject, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(sourceObject, cloneManager);
         var sourceOrgan = sourceObject as Organ;
         if (sourceOrgan == null) return;
         OrganType = sourceOrgan.OrganType;
      }
   }
}