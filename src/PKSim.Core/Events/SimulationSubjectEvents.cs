using PKSim.Core.Model;

namespace PKSim.Core.Events
{
   public interface ISimulationSubjectEvent
   {
      ISimulationSubject SimulationSubject { get; }
   }

   public class RemoveMoleculeFromSimulationSubjectEvent<TSimulationSubject> : RemoveEntityEvent<IndividualMolecule, TSimulationSubject>,
      ISimulationSubjectEvent
      where TSimulationSubject : ISimulationSubject
   {
      public ISimulationSubject SimulationSubject => Container;
   }

   public class AddMoleculeToSimulationSubjectEvent<TSimulationSubject> : AddEntityEvent<IndividualMolecule, TSimulationSubject>,
      ISimulationSubjectEvent
      where TSimulationSubject : ISimulationSubject
   {
      public ISimulationSubject SimulationSubject => Container;
   }

   public class RefreshMoleculeInSimulationSubjectEvent<TSimulationSubject> : EntityContainerEvent<IndividualMolecule, TSimulationSubject>,
      ISimulationSubjectEvent
      where TSimulationSubject : ISimulationSubject
   {
      public ISimulationSubject SimulationSubject => Container;
   }
}