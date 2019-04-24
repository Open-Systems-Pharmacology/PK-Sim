using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Assets;
using PKSim.Core.Commands;
using PKSim.Core.Model;


namespace PKSim.Core
{
   public abstract class concern_for_ChangeEnzymaticProcessMetaboliteNameCommand : ContextSpecification<ChangeEnzymaticProcessMetaboliteNameCommand>
   {
      protected IExecutionContext _context;
      protected string _newMetabolite;
      protected EnzymaticProcess _process;
      protected string _oldMetabolite;

      protected override void Context()
      {
         _context = A.Fake<IExecutionContext>();
         _newMetabolite = "newMetabolite";
         _oldMetabolite = "oldMetabolite";
         _process = new EnzymaticProcess() {MetaboliteName = _oldMetabolite};
         sut = new ChangeEnzymaticProcessMetaboliteNameCommand(_process, _newMetabolite, _context);

         A.CallTo(() => _context.Get<EnzymaticProcess>(_process.Id)).Returns(_process);
      }
   }

   public class when_reverting_the_change_command_for_an_enzymatic_process : concern_for_ChangeEnzymaticProcessMetaboliteNameCommand
   {
      protected override void Because()
      {
         sut.ExecuteAndInvokeInverse(_context);
      }

      [Observation]
      public void metabolite_change_should_be_reversed()
      {
         _process.MetaboliteName.ShouldBeEqualTo(_oldMetabolite);
      }
   }

   public class when_changing_the_metabolite_for_an_enzymatic_process : concern_for_ChangeEnzymaticProcessMetaboliteNameCommand
   {
      protected override void Because()
      {
         sut.Execute(_context);
      }

      [Observation]
      public void new_metabolite_must_be_part_of_process()
      {
         _process.MetaboliteName.ShouldBeEqualTo(_newMetabolite);
      }

      [Observation]
      public void command_descriptive_properties_are_set()
      {
         A.CallTo(() => _context.UpdateBuildingBlockPropertiesInCommand(sut, A<IPKSimBuildingBlock>._)).MustHaveHappened();
         sut.CommandType.ShouldBeEqualTo(PKSimConstants.Command.CommandTypeEdit);
         sut.Description.ShouldNotBeNull();
         sut.ObjectType.ShouldBeEqualTo(PKSimConstants.ObjectTypes.MetabolizingEnzyme);
      }
   }
}
