using OSPSuite.Core.Domain;

namespace PKSim.Core.Model;

public class ClassifiablePopulation : Classifiable<Population>
{
   public ClassifiablePopulation() : base(ClassificationType.Population)
   {
   }

   public Population Population => Subject;
}
