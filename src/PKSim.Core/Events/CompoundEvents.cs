using PKSim.Core.Model;

namespace PKSim.Core.Events
{
   public interface ICompoundEvent
   {
      Compound Compound { get; }
   }

   public class AddCompoundParameterGroupAlternativeEvent : AddEntityEvent<PKSim.Core.Model.ParameterAlternative, PKSim.Core.Model.ParameterAlternativeGroup>
   {
   }

   public class RemoveCompoundParameterGroupAlternativeEvent : RemoveEntityEvent<PKSim.Core.Model.ParameterAlternative, PKSim.Core.Model.ParameterAlternativeGroup>
   {
   }

   public class RemoveCompoundProcessEvent : RemoveEntityEvent<PKSim.Core.Model.CompoundProcess, Compound>, ICompoundEvent
   {
      public Compound Compound
      {
         get { return Container; }
      }
   }

   public class AddCompoundProcessEvent : AddEntityEvent<PKSim.Core.Model.CompoundProcess, Compound>, ICompoundEvent
   {
      public Compound Compound
      {
         get { return Container; }
      }
   }

   public class MoleculeRenamedInCompound : ICompoundEvent
   {
      public Compound Compound { get; private set; }

      public MoleculeRenamedInCompound(Compound compound)
      {
         Compound = compound;
      }
   }
}