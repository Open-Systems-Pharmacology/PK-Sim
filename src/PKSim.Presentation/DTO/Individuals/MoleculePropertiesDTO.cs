using PKSim.Core.Model;
using PKSim.Presentation.DTO.Parameters;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.DTO.Individuals
{
   public class MoleculePropertiesDTO : ValidatableDTO<IndividualMolecule>
   {
      public IParameterDTO ReferenceConcentrationParameter { get; set; }
      public IParameterDTO HalfLifeLiverParameter { get; set; }
      public IParameterDTO HalfLifeIntestineParameter { get; set; }
      public double ReferenceConcentration { get; set; }
      public double HalfLifeLiver { get; set; }
      public double HalfLifeIntestine { get; set; }
      public string MoleculeType { get; set; }
      public MoleculePropertiesDTO(IndividualMolecule molecule)
         : base(molecule)
      {
      }
   }
}