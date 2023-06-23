using System;
using System.Linq;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Presentation.DTO.Applications;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface IContainerToApplicationDTOMapper : IMapper<IContainer,ApplicationDTO>
   {
   }

   public class ContainerToApplicationDTOMapper : IContainerToApplicationDTOMapper
   {
      private readonly IParameterToParameterDTOMapper _parameterDTOMapper;

      public ContainerToApplicationDTOMapper(IParameterToParameterDTOMapper parameterDTOMapper)
      {
         _parameterDTOMapper = parameterDTOMapper;
      }

      public ApplicationDTO MapFrom(IContainer schemaItemContainer)
      {
         //schemaItemContainer are saved within en event group container 
         var eventGroup = schemaItemContainer.ParentContainer as EventGroup;
         string applicationIcon = string.Empty;
         if (eventGroup != null && !string.IsNullOrEmpty(eventGroup.EventGroupType))
            applicationIcon = ApplicationTypes.ByName(eventGroup.EventGroupType).IconName;

         var applicationDTO = new ApplicationDTO { Name = schemaItemContainer.ParentContainer.Name, Icon = applicationIcon };
         schemaItemContainer.AllParameters().Where(p=>p.Visible).Each(p => applicationDTO.AddParameter(_parameterDTOMapper.MapFrom(p)));
         return applicationDTO;
      }
   }
}