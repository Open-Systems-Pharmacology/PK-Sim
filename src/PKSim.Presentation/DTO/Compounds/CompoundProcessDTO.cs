using PKSim.Core.Model;

namespace PKSim.Presentation.DTO.Compounds
{
   public class CompoundProcessDTO : ProcessDTO<CompoundProcess>
   {
      public CompoundProcessDTO(CompoundProcess underlyingObject) : base(underlyingObject)
      {
      }
   }
}