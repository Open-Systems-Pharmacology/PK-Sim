using System.Xml.Linq;
using OSPSuite.Serializer.Xml.Extensions;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Repositories;
using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Core.Serialization.Xml.Extensions;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class PivotPositionXmlSerializer : BaseXmlSerializer<PivotPosition>
   {
      public override void PerformMapping()
      {
         Map(x => x.Area);
         Map(x => x.Index);
      }
   }

   public abstract class PopulationPivotAnalysisXmlSerializer<TAnalysis> : PopulationAnalysisXmlSerializer<TAnalysis> where TAnalysis : PopulationPivotAnalysis
   {
      protected override void TypedDeserialize(TAnalysis populationPivotAnalysis, XElement element, SerializationContext serializationContext)
      {
         base.TypedDeserialize(populationPivotAnalysis, element, serializationContext);
         var fieldPositionSerializer = SerializerRepository.SerializerFor<PivotPosition>();

         foreach (var positionElement in element.Descendants(CoreConstants.Serialization.PivotPositionList).Descendants())
         {
            var position = fieldPositionSerializer.Deserialize<PivotPosition>(positionElement, serializationContext);
            var fieldName = positionElement.GetAttribute(CoreConstants.Serialization.Attribute.Name);
            populationPivotAnalysis.SetPosition(fieldName, position);
         }
      }

      protected override XElement TypedSerialize(TAnalysis populationPivotAnalysis, SerializationContext serializationContext)
      {
         var element = base.TypedSerialize(populationPivotAnalysis, serializationContext);
         var fieldPositionSerializer = SerializerRepository.SerializerFor<PivotPosition>();
         var positionListElement = SerializerRepository.CreateElement(CoreConstants.Serialization.PivotPositionList);

         element.Add(positionListElement);
         foreach (var fieldWithDefinedPosition in populationPivotAnalysis.AllFieldPositions.KeyValues)
         {
            var positionElement = fieldPositionSerializer.Serialize(fieldWithDefinedPosition.Value, serializationContext);
            positionElement.AddAttribute(CoreConstants.Serialization.Attribute.Name, fieldWithDefinedPosition.Key.Name);
            positionListElement.Add(positionElement);
         }

         return element;
      }
   }

   public class PopulationPivotAnalysisXmlSerializer : PopulationPivotAnalysisXmlSerializer<PopulationPivotAnalysis>
   {
   }

   public class PopulationStatisticalAnalysisXmlSerializer : PopulationPivotAnalysisXmlSerializer<PopulationStatisticalAnalysis>
   {
      public override void PerformMapping()
      {
         base.PerformMapping();
         MapEnumerable(x => x.Statistics, x => x.AddStatistic);
      }

      protected override void TypedDeserialize(PopulationStatisticalAnalysis populationStatisticalAnalysis, XElement element, SerializationContext serializationContext)
      {
         var dimensionRepository = serializationContext.Resolve<IDimensionRepository>();
         base.TypedDeserialize(populationStatisticalAnalysis, element, serializationContext);
         populationStatisticalAnalysis.TimeUnit = element.GetDisplayUnit(dimensionRepository.Time);
      }

      protected override XElement TypedSerialize(PopulationStatisticalAnalysis populationStatisticalAnalysis, SerializationContext serializationContext)
      {
         var element = base.TypedSerialize(populationStatisticalAnalysis, serializationContext);
         element.AddDisplayUnit(populationStatisticalAnalysis.TimeUnit);
         return element;
      }
   }

   public class PopulationBoxWhiskerAnalysisXmlSerializer : PopulationPivotAnalysisXmlSerializer<PopulationBoxWhiskerAnalysis>
   {
      public override void PerformMapping()
      {
         base.PerformMapping();
         Map(x => x.ShowOutliers);
      }
   }
}