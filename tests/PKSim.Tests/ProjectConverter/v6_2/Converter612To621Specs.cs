using System.Xml.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Container;
using FakeItEasy;
using PKSim.Core;
using PKSim.Infrastructure.ProjectConverter.v6_2;
using OSPSuite.Core.Domain.UnitSystem;

namespace PKSim.ProjectConverter.v6_2
{
   public abstract class concern_for_Converter612To621 : ContextSpecification<Converter612To621>
   {
      private IDimensionFactory _dimensionFactory;
      private IContainer _container;
      private IObservedDataConvertor _observedDataConvertor;

      protected override void Context()
      {
         _dimensionFactory = A.Fake<IDimensionFactory>();
         _container = A.Fake<IContainer>();
         _observedDataConvertor= A.Fake<IObservedDataConvertor>();
         sut = new Converter612To621(_container, _dimensionFactory, _observedDataConvertor);
      }
   }

   public class When_converting_a_simulation_concentration_chart_element : concern_for_Converter612To621
   {
      private XElement _chartElement;

      protected override void Context()
      {
         base.Context();
         _chartElement = new XElement("SimulationConcentrationChart");
      }

      protected override void Because()
      {
         sut.ConvertXml(_chartElement, ProjectVersions.V5_5_1);
      }

      [Observation]
      public void should_rename_it_to_simulation_time_profile_chart()
      {
         _chartElement.Name.ShouldBeEqualTo("SimulationTimeProfileChart");
      }
   }
}