using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core
{
   public abstract class concern_for_EnzymaticProcessSelection : ContextSpecification<EnzymaticProcessSelection>
   {
      protected override void Context()
      {
         sut = new EnzymaticProcessSelection();
         sut.ProcessName = "Proc";
         sut.MetaboliteName = "Meta";
         sut.CompoundName = "Comp";
         sut.MoleculeName = "Enzyme";
      }
   }

   public class When_cloning_an_enzymatic_process_selection : concern_for_EnzymaticProcessSelection
   {
      private EnzymaticProcessSelection _result;

      protected override void Because()
      {
         _result = sut.Clone(A.Fake<ICloneManager>()) as EnzymaticProcessSelection;
      }

      [Observation]
      public void should_return_an_enzymatic_process_selection_with_all_properties_from_the_original_object()
      {
         _result.ShouldNotBeNull();
         _result.ProcessName.ShouldBeEqualTo(sut.ProcessName);
         _result.MetaboliteName.ShouldBeEqualTo(sut.MetaboliteName);
         _result.CompoundName.ShouldBeEqualTo(sut.CompoundName);
         _result.MoleculeName.ShouldBeEqualTo(sut.MoleculeName);
      }
   }

   public class When_checking_if_an_enzymatic_process_is_a_sink : concern_for_EnzymaticProcessSelection
   {
      [Observation]
      public void should_return_false_if_metabolite_name_is_set()
      {
         sut.IsSink.ShouldBeFalse();
      }

      [Observation]
      public void should_return_true_if_the_metabolite_name_is_not_set()
      {
         sut.MetaboliteName = string.Empty;
         sut.IsSink.ShouldBeTrue();
      }
   }

   public class When_retrieving_the_product_name_of_an_enzymatic_process_reaction : concern_for_EnzymaticProcessSelection
   {
      [Observation]
      public void should_return_the_metabolite_name_if_the_metabolite_name_is_set()
      {
         sut.ProductName().ShouldBeEqualTo(sut.MetaboliteName);
      }

      [Observation]
      public void should_return_the_generic_product_name_for_metabolite_otherwise()
      {
         sut.MetaboliteName = string.Empty;
         sut.ProductName().ShouldBeEqualTo(sut.ProductName(CoreConstants.Molecule.Metabolite));
      }
   }
}