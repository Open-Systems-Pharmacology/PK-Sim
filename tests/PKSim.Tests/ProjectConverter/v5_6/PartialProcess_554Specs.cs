using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Infrastructure.ProjectConverter;
using PKSim.Infrastructure.ProjectConverter.v5_6;
using PKSim.IntegrationTests;
using OSPSuite.Core.Domain;

namespace PKSim.ProjectConverter.v5_6
{
   public abstract class concern_for_PartialProcess_554 : ContextWithLoadedProject<Converter552To561>
   {
      protected Compound _compound;
      protected Simulation _S2;
      protected Simulation _S1;

      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("PartialProcess_554");
         _compound = First<Compound>();
         _S1 = FindByName<Simulation>("S1");
         _S2 = FindByName<Simulation>("S2");
      }
   }

   public class When_converting_the_partial_process_554_project : concern_for_PartialProcess_554
   {
      [Observation]
      public void should_have_converted_the_partial_process_from_the_compound()
      {
         var firstOrderProcess = _compound.AllProcesses<PartialProcess>().FindByName("CYP-FO");
         firstOrderProcess.ShouldNotBeNull();
         firstOrderProcess.Parameter(ConverterConstants.Parameter.SpecificClearance).Value.ShouldBeEqualTo(7.89268,1e-2);
      }

      [Observation]
      public void should_have_the_previous_inhibition_process_to_non_inhibition_process()
      {
         var process = _compound.AllProcesses<PartialProcess>().FindByName("CYP-Comp");
         process.InternalName.Contains("Inhibition").ShouldBeFalse();
      }

      [Observation]
      public void should_have_removed_the_mapping_to_any_partial_process_using_an_old_inhibition_process()
      {
         _S2.CompoundPropertiesFor(_compound.Name).AnyProcessesDefined.ShouldBeFalse();
      }

      [Observation]
      public void should_have_not_removed_the_mapping_to_any_partial_process_that_was_not_an_old_inhibition()
      {
         _S1.CompoundPropertiesFor(_compound.Name).AnyProcessesDefined.ShouldBeTrue();
      }
   }
}	