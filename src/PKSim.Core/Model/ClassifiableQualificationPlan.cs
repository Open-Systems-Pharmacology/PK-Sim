using OSPSuite.Core.Domain;

namespace PKSim.Core.Model
{
   public class ClassifiableQualificationPlan : Classifiable<QualificationPlan>
   {
      public ClassifiableQualificationPlan() : base(ClassificationType.QualificationPlan)
      {
      }

      public QualificationPlan QualificationPlan => Subject;
   }
}