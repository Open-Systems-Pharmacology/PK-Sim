using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;

namespace PKSim.Core
{
   public abstract class concern_for_PKSimSpatialStructure : ContextSpecification<PKSimSpatialStructure>
   {
      protected Organism _organism;

      protected override void Context()
      {
         sut = new PKSimSpatialStructure();
         _organism = new Organism();
         sut.AddTopContainer(_organism);
      }
   }

   public class When_resolving_the_organism_in_a_spatial_structure : concern_for_PKSimSpatialStructure
   {
      [Observation]
      public void should_return_the_first_top_container()
      {
         sut.Organism.ShouldBeEqualTo(_organism);
      }
   }
}