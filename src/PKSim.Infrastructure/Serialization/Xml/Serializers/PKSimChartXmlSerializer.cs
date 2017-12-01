using System.Linq;
using System.Xml.Linq;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Chart;
using PKSim.Core.Services;
using PKSim.Infrastructure.Serialization.Xml.Extensions;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Repositories;
using OSPSuite.Core.Serialization.Xml;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public abstract class PKSimChartXmlSerializer<TChart> : BaseXmlSerializer<TChart> where TChart : CurveChart
   {
      public override void PerformMapping()
      {
         //nothing to do here as the serialization action will be performed in the overload
      }

      protected override XElement TypedSerialize(TChart chart, SerializationContext serializationContext)
      {
         var chartSerializer = SerializerRepository.SerializerFor<CurveChart>();
         var chartElement = base.TypedSerialize(chart, serializationContext);
         chartElement.Add(chartSerializer.Serialize(chart, serializationContext));
         return chartElement;
      }

      protected override void TypedDeserialize(TChart chart, XElement chartElement, SerializationContext serializationContext)
      {
         var chartSerializer = SerializerRepository.SerializerFor<CurveChart>();
         base.TypedDeserialize(chart, chartElement, serializationContext);
         var dataChartElement = chartElement.Element(chartSerializer.ElementName);
         chartSerializer.Deserialize(chart, dataChartElement, serializationContext);
      }
   }

   public class SimulationChartXmlSerializer : PKSimChartXmlSerializer<SimulationTimeProfileChart>
   {
   }

   public class IndividualSimulationComparisonXmlSerializer : PKSimChartXmlSerializer<IndividualSimulationComparison>
   {
      protected override XElement TypedSerialize(IndividualSimulationComparison chart, SerializationContext serializationContext)
      {
         var element = base.TypedSerialize(chart, serializationContext);
         element.Add(SerializerRepository.CreateSimulationReferenceListElement(chart));
         return element;
      }

      protected override void TypedDeserialize(IndividualSimulationComparison chart, XElement chartElement, SerializationContext serializationContext)
      {
         //first load the simulation and then deserialize the chart as the results are needed 
         var lazyLoadTask = IoC.Resolve<ILazyLoadTask>();
         var withIdRepository = IoC.Resolve<IWithIdRepository>();
         var observedDataRepository = IoC.Resolve<IObservedDataRepository>();

         chartElement.AddReferencedSimulations(chart, withIdRepository, lazyLoadTask);

         //necessary to add data repository that where loaded while laoding the simulations
         chart.AllSimulations.Select(x => x.DataRepository).Each(serializationContext.AddRepository);
         observedDataRepository.All().Each(serializationContext.AddRepository);

         base.TypedDeserialize(chart, chartElement, serializationContext);
      }
   }
}