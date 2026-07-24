using OSPSuite.Core.Domain;

namespace PKSim.Core.Model;

public class ClassifiableEvent : Classifiable<PKSimEvent>
{
   public ClassifiableEvent() : base(ClassificationType.Event)
   {
   }

   public PKSimEvent Event => Subject;
}
