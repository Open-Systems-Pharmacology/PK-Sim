using System.Collections.Generic;

namespace PKSim.Core.Batch
{
   internal abstract class CompoundProcess
   {
      public string InternalName { get; set; }
      public string DataSource { get; set; }
      public string Species { get; set; }
      public Dictionary<string, double> ParameterValues { get; set; }

      protected CompoundProcess()
      {
         ParameterValues = new Dictionary<string, double>();
      }
   }

   internal class SystemicProcess : CompoundProcess
   {
      public string SystemicProcessType { get; set; }
   }

   internal class PartialProcess : CompoundProcess
   {
      public string MoleculeName { get; set; }
   }
}