using PKSim.Core.Model;

namespace PKSim.Core.Events
{
   public interface ICompoundEvent
   {
      Compound Compound { get; }
   }

   public class AddCompoundParameterGroupAlternativeEvent : AddEntityEvent<ParameterAlternative, ParameterAlternativeGroup>
   {
   }

   public class RemoveCompoundParameterGroupAlternativeEvent : RemoveEntityEvent<ParameterAlternative, ParameterAlternativeGroup>
   {
   }

   public class RemoveCompoundProcessEvent : RemoveEntityEvent<CompoundProcess, Compound>, ICompoundEvent
   {
      public Compound Compound => Container;
   }

   public class AddCompoundProcessEvent : AddEntityEvent<CompoundProcess, Compound>, ICompoundEvent
   {
      public Compound Compound => Container;
   }

   public class MoleculeRenamedInCompound : ICompoundEvent
   {
      public Compound Compound { get; private set; }

      public MoleculeRenamedInCompound(Compound compound)
      {
         Compound = compound;
      }
   }

   public class OverwriteParameterSetChangedEvent : ICompoundEvent
   {
      public Compound Compound { get; }
      public OverwriteParameterSet OverwriteParameterSet { get; }

      public OverwriteParameterSetChangedEvent(Compound compound, OverwriteParameterSet overwriteParameterSet)
      {
         Compound = compound;
         OverwriteParameterSet = overwriteParameterSet;
      }
   }
}