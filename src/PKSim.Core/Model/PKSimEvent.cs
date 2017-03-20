using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public class PKSimEvent : PKSimBuildingBlock
   {
      /// <summary>
      ///    Name of event builder template used to create the building block
      /// </summary>
      public virtual string TemplateName { get; set; }

      public PKSimEvent() : base(PKSimBuildingBlockType.Event)
      {
      }

      public override void UpdatePropertiesFrom(IUpdatable sourceObject, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(sourceObject, cloneManager);
         var pksimEvent = sourceObject as PKSimEvent;
         if (pksimEvent == null) return;
         TemplateName = pksimEvent.TemplateName;
      }
   }
}