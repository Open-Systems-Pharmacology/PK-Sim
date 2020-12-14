using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_TransporterContainerTemplateRepository : ContextForIntegration<ITransporterContainerTemplateRepository>
   {
   }

   public class When_getting_default_transporter_type : concern_for_TransporterContainerTemplateRepository
   {
      [Observation]
      public void should_return_correct_type_for_ABCB1()
      {
         sut.TransportTypeFor(CoreConstants.Species.HUMAN, "ABCB1").ShouldBeEqualTo(TransportType.Efflux);
      }

      [Observation]
      public void should_return_correct_type_for_ABCB1_written_in_lower_case()
      {
         sut.TransportTypeFor(CoreConstants.Species.HUMAN, "abcb1").ShouldBeEqualTo(TransportType.Efflux);
      }

      [Observation]
      public void should_return_correct_type_for_ABCB2()
      {
         sut.TransportTypeFor(CoreConstants.Species.HUMAN, "ABCC2").ShouldBeEqualTo(TransportType.Efflux);
      }

      [Observation]
      public void should_return_correct_type_for_SLC10A1()
      {
         sut.TransportTypeFor(CoreConstants.Species.HUMAN, "SLC10A1").ShouldBeEqualTo(TransportType.Influx);
      }

      [Observation]
      public void should_return_the_correcty_type_for_MDR1_defined_as_synonym_of_ABCB1()
      {
         sut.TransportTypeFor(CoreConstants.Species.HUMAN, "MDR1").ShouldBeEqualTo(TransportType.Efflux);
      }

      [Observation]
      public void should_return_the_correcty_type_for_OATP1B1_defined_as_synonym_of_SLCO1B1()
      {
         sut.TransportTypeFor(CoreConstants.Species.HUMAN, "OATP1B1").ShouldBeEqualTo(TransportType.Influx);
      }

      [Observation]
      public void should_return_efflux_for_non_existing_transporter()
      {
         sut.TransportTypeFor(CoreConstants.Species.HUMAN, "DOES NOT EXIST").ShouldBeEqualTo(TransportType.Efflux);
      }
   }


   public class When_retrieving_all_predefined_transport_processes_for_pericentral_and_periportal_from_the_database : concern_for_TransporterContainerTemplateRepository
   {
      private List<TransporterContainerTemplate> _pericentrals;
      private List<TransporterContainerTemplate> _periportals;

      protected override void Because()
      {
         _pericentrals = sut.TransportersFor(CoreConstants.Species.HUMAN, CoreConstants.Compartment.PERICENTRAL).ToList();
         _periportals = sut.TransportersFor(CoreConstants.Species.HUMAN, CoreConstants.Compartment.PERIPORTAL).ToList();
      }

      [Observation]
      public void should_have_created_the_same_membrane_types_for_both_zones()
      {
        _pericentrals.Count.ShouldBeEqualTo(_pericentrals.Count);

         foreach (var pericentral in _pericentrals)
         {
            var periportal = _periportals.Find(t=>areSame(t, pericentral));
            periportal.ShouldNotBeNull();
         }
      }

      private bool areSame(TransporterContainerTemplate transporter1, TransporterContainerTemplate transporter2   )
      {
         return
            transporter1.Gene == transporter2.Gene &&
            transporter1.TransportType == transporter2.TransportType;

      }
   }
}