using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_TransporterContainerTemplateRepository : ContextForIntegration<ITransporterContainerTemplateRepository>
   {
   }

   public class When_retrieving_all_predefined_transport_processes_for_pericentral_and_periportal_from_the_database : concern_for_TransporterContainerTemplateRepository
   {
      private List<TransporterContainerTemplate> _pericentrals;
      private List<TransporterContainerTemplate> _periportals;

      protected override void Because()
      {
         _pericentrals = sut.All().Where(x => x.Species == CoreConstants.Species.HUMAN && x.ContainerName == CoreConstants.Compartment.PERICENTRAL).ToList();
         _periportals = sut.All().Where(x => x.Species == CoreConstants.Species.HUMAN && x.ContainerName == CoreConstants.Compartment.PERIPORTAL).ToList();
      }

      [Observation]
      public void should_have_created_the_same_membrane_types_for_both_zones()
      {
         _pericentrals.Count.ShouldBeEqualTo(_pericentrals.Count);

         foreach (var pericentral in _pericentrals)
         {
            var periportal = _periportals.Find(t => areSame(t, pericentral));
            periportal.ShouldNotBeNull();
         }
      }

      private bool areSame(TransporterContainerTemplate transporter1, TransporterContainerTemplate transporter2)
      {
         return
            transporter1.Gene == transporter2.Gene &&
            transporter1.TransportType == transporter2.TransportType;
      }
   }
}