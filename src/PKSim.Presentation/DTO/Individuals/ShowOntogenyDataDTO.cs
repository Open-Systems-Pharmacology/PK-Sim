using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.Presentation.DTO.Individuals
{
   public class ShowOntogenyDataDTO
   {
      public Ontogeny SelectedOntogeny { get; set; }
      public IGroup SelectedContainer { get; set; }
   }
}