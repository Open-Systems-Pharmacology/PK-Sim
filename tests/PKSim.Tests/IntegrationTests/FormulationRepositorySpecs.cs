using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.Repositories;
using OSPSuite.Core.Domain;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_FormulationRepository : ContextForIntegration<IFormulationRepository>
   {
   }

   
   public class When_retrieving_the_defined_formulation : concern_for_FormulationRepository
   {
      protected IEnumerable< PKSim.Core.Model.Formulation> _allFormulations;

      protected override void Because()
      {
         _allFormulations = sut.All();
         _allFormulations = sut.All();
      }

      [Observation]
      public void should_return_the_available_formulation_container_from_the_database()
      {
         _allFormulations.Count().ShouldBeGreaterThan(0);
      }

      [Observation]
      public void every_formulation_should_contain_only_parameter_as_children()
      {
         _allFormulations.Each(f => f.GetChildren<IParameter>().Count()
                                      .ShouldBeEqualTo(f.Children.Count()));
      }


   }
}