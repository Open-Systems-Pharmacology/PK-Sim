using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain.Builder;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_ModelPassiveTransportMoleculeNameRepository : ContextForIntegration<IModelPassiveTransportMoleculeNameRepository>
   {
   }


   public class when_getting_model_passivetransport_molecule_names_from_repository : concern_for_ModelPassiveTransportMoleculeNameRepository
   {
      private IEnumerable<MoleculeList> _transportMoleculeNames;

      protected override void Because()
      {
         _transportMoleculeNames = sut.All();
         _transportMoleculeNames = sut.All();
      }

      [Observation]
      public void should_return_at_least_one_value()
      {
         _transportMoleculeNames.Count().ShouldBeGreaterThan(0);
      }

      [Observation]
      public void should_retrieve_values_for_2pore_transport_link()
      {
         var compoundName = "Aspirin";

         var moleculeNames = sut.MoleculeNamesFor(CoreConstants.Model.TWO_PORES, "TwoPoresTransportLink", new[]{compoundName});
         moleculeNames.ShouldNotBeNull();

         moleculeNames.MoleculeNamesToExclude.ShouldContain(CoreConstants.Molecule.FcRn, 
                                                            CoreConstants.Molecule.DrugFcRnComplexName(compoundName),
                                                            CoreConstants.Molecule.LigandEndo,
                                                            CoreConstants.Molecule.LigandEndoComplex);
      }
   }

}
