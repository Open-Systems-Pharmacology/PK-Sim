namespace PKSim.Core.Model
{
   /// <summary>
   ///    Assigns the expression measured in a tissue of the gene expression database to a PK-Sim container
   /// </summary>
   public class TissueContainerMapping
   {
      public TissueContainerMapping(string tissue, string container)
      {
         Tissue = tissue;
         Container = container;
      }

      public string Tissue { get; }

      /// <summary>
      ///    Name of the PK-Sim container, or null when the tissue is not mapped to any container
      /// </summary>
      public string Container { get; }
   }
}
