using PKSim.Core.Model;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public abstract class CompoundProcessMappingXmlSerializer<T> : BaseXmlSerializer<T> where T : IProcessMapping
   {
      public override void PerformMapping()
      {
         Map(x => x.ProcessName);
         Map(x => x.CompoundName);
         Map(x => x.MoleculeName);
      }
   }

   public class ProcessSelectionXmlSerializer : CompoundProcessMappingXmlSerializer<ProcessSelection>
   {
   }

   public class EnzymaticProcessSelectionXmlSerializer : CompoundProcessMappingXmlSerializer<EnzymaticProcessSelection>
   {
      public override void PerformMapping()
      {
         base.PerformMapping();
         Map(x => x.MetaboliteName);
      }
   }
}