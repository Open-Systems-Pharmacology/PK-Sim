using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Mappers;

namespace PKSim.Presentation
{
   public abstract class concern_for_EnzymaticProcessToEnzymaticProcessDTOMapper : ContextSpecification<EnzymaticProcessToEnzymaticProcessDTOMapper>
   {

      protected override void Context()
      {
         sut = new EnzymaticProcessToEnzymaticProcessDTOMapper(A.Fake<IRepresentationInfoRepository>());
      }
   }

   public class when_mapping_from_enzymatic_process_to_dto : concern_for_EnzymaticProcessToEnzymaticProcessDTOMapper
   {
      private EnzymaticProcess _process;
      private EnzymaticProcessDTO _result;
      private string _metabolite;

      protected override void Context()
      {
         base.Context();
         _metabolite = "metabolite";
         _process = new EnzymaticProcess {MetaboliteName = _metabolite};
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_process);
      }

      [Observation]
      public void metabolite_should_be_set_correctly()
      {
         _result.Metabolite.ShouldBeEqualTo(_metabolite);
      }
   }
}
