using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core;
using PKSim.Core.Services;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_ForbiddenCompoundNamesRetriever : ContextForIntegration<IForbiddenCompoundNamesRetriever>
   {
   }

   public class When_retrieving_the_forbidden_compound_names_from_the_database : concern_for_ForbiddenCompoundNamesRetriever
   {
      private IReadOnlyList<string> _result;

      protected override void Because()
      {
         _result = sut.All();
      }

      [Observation]
      public void should_contain_the_names_of_the_model_containers()
      {
         _result.ShouldContain("Liver", "Brain");
      }

      [Observation]
      public void should_contain_the_names_of_the_model_parameters()
      {
         _result.ShouldContain("Volume");
      }

      [Observation]
      public void should_contain_the_names_of_the_model_molecules()
      {
         _result.ShouldContain("FcRn", "LigandEndo");
      }

      [Observation]
      public void should_contain_the_names_of_the_model_observers()
      {
         _result.ShouldContain("Whole Blood", "Fraction of dose");
      }

      [Observation]
      public void should_contain_the_names_of_the_model_processes()
      {
         _result.ShouldContain("ActiveEffluxSpecificBloodCellsToPlasma");
      }

      [Observation]
      public void should_contain_the_names_of_the_undefined_liver_enzyme_and_transporter_created_at_runtime()
      {
         _result.ShouldContain(CoreConstants.Molecule.UndefinedLiver, CoreConstants.Molecule.UndefinedLiverTransporter);
      }

      [Observation]
      public void should_not_contain_the_names_of_other_object_types()
      {
         _result.Contains("Human").ShouldBeFalse();
      }
   }
}
