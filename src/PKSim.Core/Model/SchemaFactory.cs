using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using PKSim.Assets;

namespace PKSim.Core.Model
{
   public interface ISchemaFactory
   {
      Schema CreateWithDefaultItem(ApplicationType applicationType, IContainer container);
      Schema Create(IContainer container = null);
   }

   public class SchemaFactory : ISchemaFactory
   {
      private readonly IObjectBaseFactory _objectBaseFactory;
      private readonly IParameterFactory _parameterFactory;
      private readonly ISchemaItemFactory _schemaItemFactory;
      private readonly IContainerTask _containerTask;

      public SchemaFactory(IObjectBaseFactory objectBaseFactory, IParameterFactory parameterFactory, ISchemaItemFactory schemaItemFactory, IContainerTask containerTask)
      {
         _objectBaseFactory = objectBaseFactory;
         _parameterFactory = parameterFactory;
         _schemaItemFactory = schemaItemFactory;
         _containerTask = containerTask;
      }

      public Schema CreateWithDefaultItem(ApplicationType applicationType, IContainer container)
      {
         var schema = Create(container);
         var schemaItem = _schemaItemFactory.Create(applicationType, schema);
         schema.AddSchemaItem(schemaItem);
         return schema;
      }

      public Schema Create(IContainer container)
      {
         var schema = _objectBaseFactory.Create<Schema>().WithName(PKSimConstants.UI.Schema);

         if (container != null)
            schema.Name = _containerTask.CreateUniqueName(container, PKSimConstants.UI.Schema);

         schema.Add(_parameterFactory.CreateFor(Constants.Parameters.START_TIME, 0, Constants.Dimension.TIME, PKSimBuildingBlockType.Protocol));
         schema.Add(_parameterFactory.CreateFor(CoreConstants.Parameter.NUMBER_OF_REPETITIONS, 1, PKSimBuildingBlockType.Protocol));
         schema.Add(_parameterFactory.CreateFor(CoreConstants.Parameter.TIME_BETWEEN_REPETITIONS, 0, Constants.Dimension.TIME, PKSimBuildingBlockType.Protocol));

         return schema;
      }
   }
}