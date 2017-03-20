using PKSim.Core.Model;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public abstract class CompoundProcessXmlSerializer<TProcess> : PKSimContainerXmlSerializer<TProcess> where TProcess : CompoundProcess
   {
      public override void PerformMapping()
      {
         base.PerformMapping();
         Map(x => x.Species);
         Map(x => x.InternalName);
         Map(x => x.DataSource);
      }
   }

   public abstract class PartialProcessXmlSerializer<TProcess> : CompoundProcessXmlSerializer<TProcess> where TProcess : PartialProcess
   {
      public override void PerformMapping()
      {
         base.PerformMapping();
         Map(x => x.MoleculeName);
      }
   }

   public abstract class EnzymaticProcessXmlSerializer<TEnzymaticProcess> : PartialProcessXmlSerializer<TEnzymaticProcess> where TEnzymaticProcess : EnzymaticProcess
   {
      public override void PerformMapping()
      {
         base.PerformMapping();
         Map(x => x.MetaboliteName);
      }
   }

   public class EnzymaticProcessXmlSerializer : EnzymaticProcessXmlSerializer<EnzymaticProcess>
   {
   }

   public class EnzymaticProcessWithSpeciesXmlSerializer : EnzymaticProcessXmlSerializer<EnzymaticProcessWithSpecies>
   {
   }

   public abstract class InteractionProcessXmlSerializer<TInteractionProcess> : PartialProcessXmlSerializer<TInteractionProcess> where TInteractionProcess : InteractionProcess
   {
      public override void PerformMapping()
      {
         base.PerformMapping();
         Map(x => x.InteractionType);
      }
   }

   public class InhibitionProcessXmlSerializer : InteractionProcessXmlSerializer<InhibitionProcess>
   {
      
   }

   public class InductionProcessXmlSerializer : InteractionProcessXmlSerializer<InductionProcess>
   {

   }

   public class TransportPartialProcessWithSpeciesXmlSerializer : PartialProcessXmlSerializer<TransportPartialProcessWithSpecies>
   {
   }

   public class TransportPartialProcessXmlSerializer : PartialProcessXmlSerializer<TransportPartialProcess>
   {
   }

   public class SpecificBindingPartialProcessXmlSerializer : PartialProcessXmlSerializer<SpecificBindingPartialProcess>
   {
   }

   public class SystemicProcessXmlSerializer : CompoundProcessXmlSerializer<SystemicProcess>
   {
      public override void PerformMapping()
      {
         base.PerformMapping();
         Map(x => x.SystemicProcessType);
      }
   }
}