namespace PKSim.Core.Model
{
   /// <summary>
   /// Defines the mapping of a <see cref="CompoundProcess"/> in a <see cref="Compound"/> and an <see cref="IndividualMolecule"/> in an <see cref="Individual"/>
   /// </summary>
   public interface IProcessMapping
   {
      /// <summary>
      ///    Process name as defined in <see cref="Compound"/>
      /// </summary>
      string ProcessName { get; set; }

      /// <summary>
      ///    Name of <see cref="IndividualMolecule"/> in charge of triggering the reaction (defined in <see cref="Individual"/>)
      /// </summary>
      string MoleculeName { get; set; }

      /// <summary>
      /// Name of the <see cref="Compound"/> where the process is defined. This is required as the same name may be used for a process in different <see cref="Compound"/>.
      /// </summary>
      string CompoundName { get; set; }
   }

   /// <summary>
   /// Defines a reaction mapping resulting in the creation of a product
   /// </summary>
   public interface IReactionMapping : IProcessMapping
   {
      /// <summary>
      ///    Returns the name of the product resulting of the reaction induced by the process and the drug
      /// </summary>
      string ProductName(string productNameTemplate);
   }
}