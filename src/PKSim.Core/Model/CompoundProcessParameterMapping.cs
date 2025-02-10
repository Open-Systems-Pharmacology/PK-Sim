using OSPSuite.Core.Domain;

namespace PKSim.Core.Model
{
   /// <summary>
   ///    Used for mapping of compound process parameters measured in-vitro, in plasma etc.
   ///    Parameters mapped are usually Organ/Compartment volumes, blood flows, etc.
   /// </summary>
   public interface ICompoundProcessParameterMapping
   {
      /// <summary>
      ///    Name of compound process
      /// </summary>
      string ProcessName { get; set; }

      /// <summary>
      ///    Name of process parameter to be mapped
      /// </summary>
      string ParameterName { get; set; }

      /// <summary>
      ///    Path to the mapped parameter (e.g. ORGANISM/Liver/Volume)
      /// </summary>
      ObjectPath MappedParameterPath { get; set; }
   }

   public class CompoundProcessParameterMapping : ICompoundProcessParameterMapping
   {
      public string ProcessName { get; set; }
      public string ParameterName { get; set; }
      public ObjectPath MappedParameterPath { get; set; }
   }
}