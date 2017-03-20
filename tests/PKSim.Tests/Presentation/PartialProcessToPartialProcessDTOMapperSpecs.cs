using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Presentation.DTO.Compounds;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;

using PKSim.Presentation.DTO.Mappers;

namespace PKSim.Presentation
{
   public abstract class concern_for_PartialProcessToPartialProcessDTOMapper : ContextSpecification<IPartialProcessToPartialProcessDTOMapper>
   {
      protected EnzymaticProcess _partialProc;

      protected override void Context()
      {
         _partialProc = new EnzymaticProcess()
            .WithName("Tralala")
            .WithId("Trululu");
         _partialProc.MoleculeName = "E1";
         _partialProc.DataSource = "DataSource";
         sut = new PartialProcessToPartialProcessDTOMapper();
      }
   }

   
   public class When_mapping_an_enzymatic_partial_process_to_an_enzymatic_partial_process_dto : concern_for_PartialProcessToPartialProcessDTOMapper
   {
      protected PartialProcessDTO _partialProcDTO;
      private Compound _compound;

      protected override void Context()
      {
         base.Context();
         _compound = new Compound();
      }

      protected override void Because()
      {
         _partialProcDTO = sut.MapFrom(_partialProc, _compound);
      }

      [Observation]
      public void should_map_dto()
      {
         _partialProcDTO.DataSource.ShouldBeEqualTo(_partialProc.DataSource);
         _partialProcDTO.MoleculeName.ShouldBeEqualTo(_partialProc.MoleculeName);
         _partialProcDTO.Process.ShouldBeEqualTo(_partialProc);
      }
   }
}