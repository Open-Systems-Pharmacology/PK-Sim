using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core;
using PKSim.Core.Repositories;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_ITransporterTemplateRepository : ContextForIntegration<ITransporterTemplateRepository>
   {
   }


   public class When_getting_default_transporter_type : concern_for_ITransporterTemplateRepository
   {
      [Observation]
      public void should_return_correct_type_for_ABCB1()
      {
         sut.TransportTypeOrDefaultFor(CoreConstants.Species.HUMAN, "ABCB1").ShouldBeEqualTo(TransportType.Efflux);
      }

      [Observation]
      public void should_return_correct_type_for_ABCB1_written_in_lower_case()
      {
         sut.TransportTypeOrDefaultFor(CoreConstants.Species.HUMAN, "abcb1").ShouldBeEqualTo(TransportType.Efflux);
      }

      [Observation]
      public void should_return_correct_type_for_ABCB2()
      {
         sut.TransportTypeOrDefaultFor(CoreConstants.Species.HUMAN, "ABCC2").ShouldBeEqualTo(TransportType.Efflux);
      }

      [Observation]
      public void should_return_correct_type_for_SLC10A1()
      {
         sut.TransportTypeOrDefaultFor(CoreConstants.Species.HUMAN, "SLC10A1").ShouldBeEqualTo(TransportType.Influx);
      }

      [Observation]
      public void should_return_the_correcty_type_for_MDR1_defined_as_synonym_of_ABCB1()
      {
         sut.TransportTypeOrDefaultFor(CoreConstants.Species.HUMAN, "MDR1").ShouldBeEqualTo(TransportType.Efflux);
      }

      [Observation]
      public void should_return_the_correcty_type_for_OATP1B1_defined_as_synonym_of_SLCO1B1()
      {
         sut.TransportTypeOrDefaultFor(CoreConstants.Species.HUMAN, "OATP1B1").ShouldBeEqualTo(TransportType.Influx);
      }

      [Observation]
      public void should_return_efflux_for_non_existing_transporter()
      {
         sut.TransportTypeOrDefaultFor(CoreConstants.Species.HUMAN, "DOES NOT EXIST").ShouldBeEqualTo(TransportType.Efflux);
      }
   }
}