using PKSim.Core.Model;
using OSPSuite.Core.Serialization.Xml;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class ClassifiableSimulationXmlSerializer : ClassifiableXmlSerializer<ClassifiableSimulation, Simulation>, IPKSimXmlSerializer
   {
      
   }

   public class ClassifiableComparisonXmlSerializer : ClassifiableXmlSerializer<ClassifiableComparison, ISimulationComparison>, IPKSimXmlSerializer
   {
      
   }

   public class ClassifiableQualificationPlanXmlSerializer : ClassifiableXmlSerializer<ClassifiableQualificationPlan, QualificationPlan>, IPKSimXmlSerializer
   {

   }
}