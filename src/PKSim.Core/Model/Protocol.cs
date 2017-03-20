using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.UnitSystem;

namespace PKSim.Core.Model
{
   public abstract class Protocol : PKSimBuildingBlock
   {
      protected Protocol() : base(PKSimBuildingBlockType.Protocol)
      {
      }

      /// <summary>
      ///    Time in minute when the protocol will end
      /// </summary>
      public abstract Unit TimeUnit { get; set; }

      public abstract IEnumerable<string> UsedFormulationKeys { get; }
      public abstract ApplicationType ApplicationTypeUsing(string formulationKey);
      public abstract double EndTime { get; }

      public override void UpdatePropertiesFrom(IUpdatable sourceObject, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(sourceObject, cloneManager);
         var sourceProtocol = sourceObject as Protocol;
         if (sourceProtocol == null) return;
         TimeUnit = sourceProtocol.TimeUnit;
      }
   }
}