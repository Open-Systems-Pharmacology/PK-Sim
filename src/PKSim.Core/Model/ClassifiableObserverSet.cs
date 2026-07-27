using OSPSuite.Core.Domain;

namespace PKSim.Core.Model;

public class ClassifiableObserverSet : Classifiable<ObserverSet>
{
   public ClassifiableObserverSet() : base(ClassificationType.ObserverSet)
   {
   }

   public ObserverSet ObserverSet => Subject;
}
