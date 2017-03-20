using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using NUnit.Framework;
using PKSim.Core.Model;

namespace PKSim.Core
{
   public abstract class concern_for_TransporterContainerTemplate : ContextSpecification<TransporterContainerTemplate>
   {
      protected override void Context()
      {
         sut = new TransporterContainerTemplate();
      }
   }

   public class When_testing_if_a_template_container_is_template_for_a_given_transporter_name : concern_for_TransporterContainerTemplate
   {
      protected override void Context()
      {
         base.Context();
         sut.Gene = "TRANS";
         sut.AddSynonym("SYN1");
         sut.AddSynonym("SYN2");
      }

      [Observation]
      [TestCase("TRANS")]
      [TestCase("TranS")]
      [TestCase("trans")]
      public void should_return_true_if_the_gene_name_is_the_transporter_name(string transporterName)
      {
         sut.IsTemplateFor(transporterName).ShouldBeTrue();
      }

      [Observation]
      [TestCase("SYN1")]
      [TestCase("syn1")]
      [TestCase("SYN2")]
      public void should_return_true_the_transporter_name_matches_a_synonym_of_the_template(string transporterName)
      {
         sut.IsTemplateFor(transporterName).ShouldBeTrue();
      }

      [Observation]
      [TestCase("AA")]
      [TestCase("SYN")]
      public void should_return_false_otherwise(string transporterName)
      {
         sut.IsTemplateFor(transporterName).ShouldBeFalse();
      }
   }
}	