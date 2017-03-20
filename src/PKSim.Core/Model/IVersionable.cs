namespace PKSim.Core.Model
{
   public interface IVersionable
   {
      /// <summary>
      ///    Version of the building block  when the simulation was created or building block version itself
      /// </summary>
      int Version { get; set; }

      /// <summary>
      ///    Structure version of the building block when the simulation was created or building block structure version itself
      /// </summary>
      int StructureVersion { get; set; }
   }
}