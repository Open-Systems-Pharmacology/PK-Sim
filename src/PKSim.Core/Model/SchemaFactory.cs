using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public interface ISchemaFactory
   {
      Schema CreateWithDefaultItem(ApplicationType applicationType, IContainer container);
      Schema Create(IContainer container);
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
         var schema = _objectBaseFactory.Create<Schema>();
         var usedNames = new List<string>();

         if (container != null)
            usedNames.AddRange(container.GetChildren<Schema>().Select(x => x.Name));

         schema.Name = _containerTask.CreateUniqueName(usedNames, PKSimConstants.UI.Schema);

         schema.Add(_parameterFactory.CreateFor(Constants.Parameters.START_TIME, 0, Constants.Dimension.TIME, PKSimBuildingBlockType.Protocol));
         schema.Add(_parameterFactory.CreateFor(CoreConstants.Parameter.NUMBER_OF_REPETITIONS, 1, PKSimBuildingBlockType.Protocol));
         schema.Add(_parameterFactory.CreateFor(CoreConstants.Parameter.TIME_BETWEEN_REPETITIONS, 0, Constants.Dimension.TIME, PKSimBuildingBlockType.Protocol));
         return schema;
      }
   }
}