using System.Linq;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Compounds;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface ISystemicProcessToSystemicProcessDTOMapper
   {
      SystemicProcessDTO MapFrom(SystemicProcess systemicProcess, Compound compound);
      void UpdateProperties(SystemicProcess systemicProcess, SystemicProcessDTO systemicProcessDTO);
   }

   public class SystemicProcessToSystemicProcessDTOMapper : ISystemicProcessToSystemicProcessDTOMapper
   {
      private readonly IObjectTypeResolver _objectTypeResolver;

      public SystemicProcessToSystemicProcessDTOMapper(IObjectTypeResolver objectTypeResolver)
      {
         _objectTypeResolver = objectTypeResolver;
      }

      public SystemicProcessDTO MapFrom(SystemicProcess systemicProcess, Compound compound)
      {
         var systemicProcessDTO = new SystemicProcessDTO(systemicProcess)
         {
            ContainerType = _objectTypeResolver.TypeFor(compound),
            Compound = compound,
            SystemicProcessType = systemicProcess.SystemicProcessType.DisplayName,
            DataSource = systemicProcess.DataSource,
            Species = systemicProcess.Species,
            Description = systemicProcess.Description
         };

         systemicProcessDTO.AddUsedNames(compound.AllProcesses<CompoundProcess>().Select(x => x.Name));
         return systemicProcessDTO;
      }

      public void UpdateProperties(SystemicProcess systemicProcess, SystemicProcessDTO systemicProcessDTO)
      {
         systemicProcess.Name = systemicProcessDTO.Name;
         systemicProcess.DataSource = systemicProcessDTO.DataSource;
         systemicProcess.Species = systemicProcessDTO.Species;
         systemicProcess.Description = systemicProcessDTO.Description;
      }
   }
}