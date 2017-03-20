using PKSim.Core.Model;

using PKSim.Presentation.DTO.Compounds;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface IPartialProcessToPartialProcessDTOMapper
   {
      PartialProcessDTO MapFrom(PKSim.Core.Model.PartialProcess partialProcess, PKSim.Core.Model.Compound compound);
      void UpdateProperties(PKSim.Core.Model.PartialProcess partialProcess, PartialProcessDTO partialProcessDTO);
   }

   public class PartialProcessToPartialProcessDTOMapper : IPartialProcessToPartialProcessDTOMapper
   {

      public PartialProcessDTO MapFrom(PKSim.Core.Model.PartialProcess partialProcess, PKSim.Core.Model.Compound compound)
      {
         return new PartialProcessDTO(partialProcess)
                   {
                      MoleculeName = partialProcess.MoleculeName,
                      DataSource = partialProcess.DataSource,
                      Compound = compound,
                      Species = partialProcess.Species
                   };
      }

      public void UpdateProperties(PKSim.Core.Model.PartialProcess partialProcess, PartialProcessDTO partialProcessDTO)
      {
         partialProcess.Name = partialProcessDTO.Name;
         partialProcess.DataSource = partialProcessDTO.DataSource;
         partialProcess.MoleculeName = partialProcessDTO.MoleculeName;
      }
   }
}