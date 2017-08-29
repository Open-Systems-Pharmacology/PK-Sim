using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;

namespace PKSim.Core
{
   public abstract class concern_for_IndividualTransporter : ContextSpecification<PKSim.Core.Model.IndividualTransporter>
   {
      protected TransporterExpressionContainer _transpoterExpressionLiver;
      protected TransporterExpressionContainer _transpoterExpressionKidney;
      protected TransporterExpressionContainer _transpoterExpressionBrain;

      protected override void Context()
      {
         sut = new IndividualTransporter().WithName("T");
         _transpoterExpressionLiver = new TransporterExpressionContainer().WithName("Liver");
         _transpoterExpressionLiver.AddProcessName("P1");
         _transpoterExpressionKidney = new TransporterExpressionContainer ().WithName("Kidney");
         _transpoterExpressionKidney.AddProcessName("P2");
         _transpoterExpressionBrain = new TransporterExpressionContainer ().WithName("Brain");
         _transpoterExpressionBrain.AddProcessName("P1");
 
         sut.Add(_transpoterExpressionLiver);
         sut.Add(_transpoterExpressionKidney);
         sut.Add(_transpoterExpressionBrain);
      }
   }

   
   public class When_retrieving_the_processes_induced_by_a_given_transporter : concern_for_IndividualTransporter
   {
      private IEnumerable<string> _result;

      protected override void Because()
      {
         _result = sut.AllInducedProcesses();
      }

      [Observation]
      public void should_retun_the_distinct_names_of_all_processes_that_are_defined_for_the_transporter()
      {
         _result.ShouldOnlyContain("P1","P2");
      }
   }



   
   public class When_retrieving_the_list_of_all_organs_where_a_process_does_not_take_place : concern_for_IndividualTransporter
   {

      [Observation]
      public void should_return_the_organ_names_where_the_process_will_not_be_defined()
      {
         sut.AllOrgansWhereProcessDoesNotTakePlace("P1").ShouldOnlyContain("Kidney");
         sut.AllOrgansWhereProcessDoesNotTakePlace("P2").ShouldOnlyContain("Liver","Brain");          
      }
   }


}	