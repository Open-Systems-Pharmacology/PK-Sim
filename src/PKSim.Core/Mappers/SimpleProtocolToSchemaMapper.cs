using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Core.Mappers
{
   public interface ISimpleProtocolToSchemaMapper : IMapper<SimpleProtocol, IEnumerable<Schema>>
   {
   }

   public class SimpleProtocolToSchemaMapper : ISimpleProtocolToSchemaMapper
   {
      private readonly ISchemaFactory _schemaFactory;
      private readonly ISchemaItemFactory _schemaItemFactory;
      private readonly ISchemaItemParameterRetriever _schemaItemParameterRetriever;

      public SimpleProtocolToSchemaMapper(ISchemaFactory schemaFactory, ISchemaItemFactory schemaItemFactory,
                                          ISchemaItemParameterRetriever schemaItemParameterRetriever)
      {
         _schemaFactory = schemaFactory;
         _schemaItemFactory = schemaItemFactory;
         _schemaItemParameterRetriever = schemaItemParameterRetriever;
      }

      public IEnumerable<Schema> MapFrom(SimpleProtocol simpleProtocol)
      {
         var container = new Container();
         var schema = _schemaFactory.Create(container);
         container.Add(schema);

         if (simpleProtocol.DosingInterval == DosingIntervals.Single)
         {
            schema.NumberOfRepetitions.Value = 1;
            schema.AddSchemaItem(createSchemaItem(simpleProtocol.StartTime.Value, simpleProtocol, schema));
            return container.GetChildren<Schema>();
         }

         var protocolDuration = simpleProtocol.EndTimeParameter.Value;
         int numberOfRepetition = (int) Math.Floor(protocolDuration / simpleProtocol.DosingInterval.IntervalLength);
         schema.NumberOfRepetitions.Value = numberOfRepetition;

         addDosingIntevalSchemaItemTo(schema, simpleProtocol);

         if (schema.Duration == protocolDuration)
            return container.GetChildren<Schema>();

         //now we might need to add another schema if the time specified could not fit an entire repetition, or just add 
         //another repetition if the created schema has the same length as the default schema

         var schemaSingular = _schemaFactory.Create(container);
         schemaSingular.NumberOfRepetitions.Value = 1;
         schemaSingular.StartTime.Value = schema.Duration;
         addSingularSchemaItemTo(schemaSingular, simpleProtocol);
         if (schemaSingular.SchemaItems.Any())
         {
            if (schema.SchemaItems.Count() != schemaSingular.SchemaItems.Count())
               container.Add(schemaSingular);
            else
               schema.NumberOfRepetitions.Value++;
         }
         return container.GetChildren<Schema>();
      }

      private void addSingularSchemaItemTo(Schema schema, SimpleProtocol simpleProtocol)
      {
         var availableTime = simpleProtocol.EndTimeParameter.Value - schema.StartTime.Value;
         if (availableTime <= 0) return;

         schema.AddSchemaItem(createSchemaItem(0, simpleProtocol, schema));
         if (simpleProtocol.DosingInterval.Id != DosingIntervalId.DI_6_6_12) return;

         if (availableTime <= 360) return;
         schema.AddSchemaItem(createSchemaItem(360, simpleProtocol, schema));

         if (availableTime <= 720) return;
         schema.AddSchemaItem(createSchemaItem(720, simpleProtocol, schema));
      }

      private void addDosingIntevalSchemaItemTo(Schema schema, SimpleProtocol simpleProtocol)
      {
         //always add a first interval
         schema.AddSchemaItem(createSchemaItem(0, simpleProtocol, schema));
         schema.TimeBetweenRepetitions.Value = simpleProtocol.DosingInterval.IntervalLength;
         switch (simpleProtocol.DosingInterval.Id)
         {
            case DosingIntervalId.DI_6_6_12:
               schema.AddSchemaItem(createSchemaItem(360, simpleProtocol, schema));
               schema.AddSchemaItem(createSchemaItem(720, simpleProtocol, schema));
               break;
            case DosingIntervalId.DI_6_6_6_6:
            case DosingIntervalId.DI_8_8_8:
            case DosingIntervalId.DI_12_12:
            case DosingIntervalId.DI_24:
               break;
            default:
               throw new ArgumentOutOfRangeException();
         }
      }

      private SchemaItem createSchemaItem(double startTime, SimpleProtocol simpleProtocol, Schema schema)
      {
         var schemaItem = _schemaItemFactory.Create(simpleProtocol.ApplicationType, schema);
         schemaItem.StartTime.Value = startTime;
         schemaItem.StartTime.DisplayUnit = simpleProtocol.TimeUnit;
         schemaItem.Dose.Value = simpleProtocol.Dose.Value;
         schemaItem.Dose.DisplayUnit = simpleProtocol.DoseUnit;
         schemaItem.TargetCompartment = simpleProtocol.TargetCompartment;
         schemaItem.TargetOrgan = simpleProtocol.TargetOrgan;
         foreach (var parameter in _schemaItemParameterRetriever.AllDynamicParametersFor(simpleProtocol))
         {
            schemaItem.Parameter(parameter.Name).Value = parameter.Value;
         }

         schemaItem.FormulationKey = simpleProtocol.FormulationKey;
         return schemaItem;
      }
   }
}