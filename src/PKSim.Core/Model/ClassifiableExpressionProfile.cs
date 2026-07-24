using OSPSuite.Core.Domain;

namespace PKSim.Core.Model;

public class ClassifiableExpressionProfile : Classifiable<ExpressionProfile>
{
   public ClassifiableExpressionProfile() : base(ClassificationType.ExpressionProfile)
   {
   }

   public ExpressionProfile ExpressionProfile => Subject;
}
