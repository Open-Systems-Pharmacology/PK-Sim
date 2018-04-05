using PKSim.Core.Model;

namespace PKSim.Presentation.DTO.Compounds
{
   public class EnzymaticProcessDTO : ProcessDTO<EnzymaticProcess>
   {
      public EnzymaticProcessDTO(EnzymaticProcess process) : base(process)
      {
      }

      private string _metabolite;

      public string Metabolite
      {
         get => _metabolite;
         set => SetProperty(ref _metabolite, value);
      }
   }
}