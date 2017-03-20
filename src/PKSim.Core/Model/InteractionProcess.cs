using System;
using PKSim.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   [Flags]
   public enum InteractionType
   {
      CompetitiveInhibition = 1 << 0,
      UncompetitiveInhibition = 1 << 1,
      NonCompetitiveInhibition = 1 << 2,
      MixedInhibition = 1 << 3,
      IrreversibleInhibition = 1 << 4,
      Induction = 1 << 5,
      KineticUpdater = CompetitiveInhibition | UncompetitiveInhibition | NonCompetitiveInhibition | MixedInhibition,
      ReactionInducer = IrreversibleInhibition | Induction
   }

   public static class InteractionTypeExtensions
   {
      public static bool Is(this InteractionType interactionType, InteractionType typeToCompare)
      {
         return (interactionType & typeToCompare) != 0;
      }
   }

   public abstract class InteractionProcess : PartialProcess
   {
      public InteractionType InteractionType { get; set; }

      public override void UpdatePropertiesFrom(IUpdatable sourceObject, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(sourceObject, cloneManager);
         var sourceInteractionProcess = sourceObject as InteractionProcess;
         if (sourceInteractionProcess == null) return;
         InteractionType = sourceInteractionProcess.InteractionType;
      }

      public override string ToString()
      {
         var compound = ParentCompound;
         var defaultToString = base.ToString();
         if (compound == null)
            return defaultToString;

         return $"{compound.Name} {defaultToString}";
      }
   }

   public class InhibitionProcess : InteractionProcess
   {
      public override string GetProcessClass()
      {
         return CoreConstants.ProcessClasses.INHIBITION;
      }
   }

   public class InductionProcess : InteractionProcess
   {
      public override string GetProcessClass()
      {
         return CoreConstants.ProcessClasses.INDUCTION;
      }
   }

   public class NoInteractionProcess : InteractionProcess
   {
      public NoInteractionProcess()
      {
         Name = PKSimConstants.UI.None;
      }

      public override string GetProcessClass()
      {
         return CoreConstants.ProcessClasses.NONE;
      }
   }
}