using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Infrastructure.ProjectConverter.v6_2;
using PKSim.IntegrationTests;

namespace PKSim.ProjectConverter.v6_2
{
   public class When_converting_the_Theophylline_612_project : ContextWithLoadedProject<Converter612To621>
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("Theophylline_612");
      }

      [Observation]
      public void should_have_converted_the_osberved_data()
      {
         _project.AllObservedData.Count.ShouldBeEqualTo(47);
      }

      [Observation]
      public void should_have_set_the_mol_weight_to_not_variable()
      {
         All<Compound>().Each(x => x.Parameter(Constants.Parameters.MOL_WEIGHT).CanBeVaried.ShouldBeFalse());

         All<Simulation>().SelectMany(x => x.Compounds).Each(x => x.Parameter(Constants.Parameters.MOL_WEIGHT).CanBeVaried.ShouldBeFalse());
      }
   }
}