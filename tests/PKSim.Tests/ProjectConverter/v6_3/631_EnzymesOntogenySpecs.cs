using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Infrastructure.ProjectConverter.v6_3;
using PKSim.IntegrationTests;
using OSPSuite.Core.Domain;

namespace PKSim.ProjectConverter.v6_3
{

   public class When_converting_the_631_EnzymesOntogenyProject : ContextWithLoadedProject<Converter631To632>
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("631_EnzymesOntogeny");
      }

      [Observation]
      public void should_set_protein_ontogeny_factors_to_variable()
      {
         foreach (var simulation in All<Simulation>())
         {
            _lazyLoadTask.Load(simulation);

            var ontogenyFactorsGI = simulation.Model.Root.GetAllChildren<IParameter>(p=>p.Name.Equals(CoreConstants.Parameters.ONTOGENY_FACTOR_GI));
            ontogenyFactorsGI.Count.ShouldBeGreaterThan(0);

            foreach (var ontogenyFactorGI in ontogenyFactorsGI)
            {
               ontogenyFactorGI.CanBeVaried.ShouldBeTrue();
               ontogenyFactorGI.ParentContainer.Parameter(CoreConstants.Parameters.ONTOGENY_FACTOR).CanBeVaried.ShouldBeTrue();
            }
         }

      }
   }
}	
