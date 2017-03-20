using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Presentation.DTO.Compounds;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

using PKSim.Presentation.DTO.Mappers;
using FakeItEasy;

namespace PKSim.Presentation
{
   public abstract class concern_for_CompoundActiveProcessToProcessTypeDTOMapper : ContextSpecification<ICompoundProcessToCompoundProcessDTOMapper>
   {
      private IRepresentationInfoRepository _representationInfoRepository;

      protected override void Context()
      {
         _representationInfoRepository = A.Fake<IRepresentationInfoRepository>();
         A.CallTo(_representationInfoRepository).WithReturnType<string>().Returns("XXX");
         sut = new CompoundProcessToCompoundProcessDTOMapper(_representationInfoRepository);
      }
   }

   
   public class When_mapping_a_compound_active_process_to_a_process_type_dto : concern_for_CompoundActiveProcessToProcessTypeDTOMapper
   {
      protected PKSim.Core.Model.PartialProcess _link;
      private CompoundProcessDTO _dto;

      protected override void Context()
      {
         base.Context();
         _link = new EnzymaticProcess().WithName("Tralala");
      }

      protected override void Because()
      {
         _dto = sut.MapFrom(_link);
      }

      [Observation]
      public void should_map_link_properties()
      {
         _dto.Process.ShouldBeEqualTo(_link);
         _dto.Name.ShouldBeEqualTo(_link.Name);
         _dto.ProcessTypeDisplayName.ShouldBeEqualTo("XXX");
      }
   }
}