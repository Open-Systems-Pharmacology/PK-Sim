using OSPSuite.Core.Domain;

namespace PKSim.Core.Model;

public class ClassifiableIndividual : Classifiable<Individual>
{
   public ClassifiableIndividual() : base(ClassificationType.Individual)
   {
   }

   public Individual Individual => Subject;
}
