using System.Linq;
using System.Xml.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Infrastructure.ProjectConverter;
using PKSim.Infrastructure.ProjectConverter.v5_3;

namespace PKSim.ProjectConverter.v5_3
{
   public abstract class concern_for_Converter531To532 : ContextSpecification<Converter531To532>
   {
      private ICalculationMethodsUpdater _calculationMethodsUpdater;

      protected override void Context()
      {
         _calculationMethodsUpdater = A.Fake<ICalculationMethodsUpdater>();
         sut = new Converter531To532(_calculationMethodsUpdater);
      }
   }

   public class When_converting_the_project_element : concern_for_Converter531To532
   {
      private XElement _projectElement;

      protected override void Context()
      {
         base.Context();
         _projectElement = new XElement("Project");
         _projectElement.Add(new XElement("Favorites"));
      }

      protected override void Because()
      {
         sut.ConvertXml(_projectElement, 5);
      }

      [Observation]
      public void should_remove_the_favorite_node()
      {
         _projectElement.Descendants("Favorites").Any().ShouldBeFalse();
      }
   }
}