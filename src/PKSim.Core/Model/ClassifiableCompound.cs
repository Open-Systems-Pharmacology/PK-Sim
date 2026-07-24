using OSPSuite.Core.Domain;

namespace PKSim.Core.Model;

public class ClassifiableCompound : Classifiable<Compound>
{
   public ClassifiableCompound() : base(ClassificationType.Compound)
   {
   }

   public Compound Compound => Subject;
}
