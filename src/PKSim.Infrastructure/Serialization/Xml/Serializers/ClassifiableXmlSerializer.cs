using OSPSuite.Core.Domain;
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

   public class ClassifiableCompoundXmlSerializer : ClassifiableXmlSerializer<ClassifiableCompound, Compound>, IPKSimXmlSerializer
   {

   }

   public class ClassifiableFormulationXmlSerializer : ClassifiableXmlSerializer<ClassifiableFormulation, Formulation>, IPKSimXmlSerializer
   {

   }

   public class ClassifiableIndividualXmlSerializer : ClassifiableXmlSerializer<ClassifiableIndividual, Individual>, IPKSimXmlSerializer
   {

   }

   public class ClassifiablePopulationXmlSerializer : ClassifiableXmlSerializer<ClassifiablePopulation, Population>, IPKSimXmlSerializer
   {

   }

   public class ClassifiableProtocolXmlSerializer : ClassifiableXmlSerializer<ClassifiableProtocol, Protocol>, IPKSimXmlSerializer
   {

   }

   public class ClassifiableEventXmlSerializer : ClassifiableXmlSerializer<ClassifiableEvent, PKSimEvent>, IPKSimXmlSerializer
   {

   }

   public class ClassifiableObserverSetXmlSerializer : ClassifiableXmlSerializer<ClassifiableObserverSet, ObserverSet>, IPKSimXmlSerializer
   {

   }

   public class ClassifiableExpressionProfileXmlSerializer : ClassifiableXmlSerializer<ClassifiableExpressionProfile, ExpressionProfile>, IPKSimXmlSerializer
   {

   }
}