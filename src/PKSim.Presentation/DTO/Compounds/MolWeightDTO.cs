using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.DTO.Compounds
{
   public class MolWeightDTO : DxValidatableDTO
   {
      public IParameterDTO MolWeightParameter { get; set; }
      public IParameterDTO MolWeightEffParameter { get; set; }
      public IParameterDTO HasHalogensParameter { get; set; }
   }
}