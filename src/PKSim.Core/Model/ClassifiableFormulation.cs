using OSPSuite.Core.Domain;

namespace PKSim.Core.Model;

public class ClassifiableFormulation : Classifiable<Formulation>
{
   public ClassifiableFormulation() : base(ClassificationType.Formulation)
   {
   }

   public Formulation Formulation => Subject;
}
