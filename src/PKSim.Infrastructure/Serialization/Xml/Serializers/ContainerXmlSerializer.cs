using OSPSuite.Core.Domain;
using OSPSuite.Core.Serialization.Xml;
using PKSim.Core.Model;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public abstract class PKSimContainerXmlSerializer<TContainer> : ContainerXmlSerializer<TContainer>, IPKSimXmlSerializer where TContainer : class, IContainer
   {
   }

   public class RootContainerXmlSerializer : PKSimContainerXmlSerializer<RootContainer>
   {
   }

   public class SchemaXmlSerializer : PKSimContainerXmlSerializer<Schema>
   {
   }

   public class OrganismXmlSerializer : PKSimContainerXmlSerializer<Organism>
   {
   }

  
   public class CompartmentXmlSerializer : PKSimContainerXmlSerializer<Compartment>
   {
      public override void PerformMapping()
      {
         base.PerformMapping();
         Map(x => x.Visible);
      }
   }

   public class ParameterAlternativeGroupXmlSerializer : PKSimContainerXmlSerializer<ParameterAlternativeGroup>
   {
   }

   public class AdvancedParameterCollectionXmlSerializer : PKSimContainerXmlSerializer<AdvancedParameterCollection>
   {
   }
}