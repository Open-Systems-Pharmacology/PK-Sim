using PKSim.Assets;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public interface ISchemaItemFactory
   {
      /// <summary>
      /// Returns a new <see cref="ISchemaItem"/> with an application type set to <paramref name="applicationType"/>. Its name will be unique in the
      /// <paramref name="container"/>
      /// </summary>
      ISchemaItem Create(ApplicationType applicationType, IContainer container);

      /// <summary>
      /// Returns an exact duplicate of the <paramref name="schemaItemToClone"/> and adjust its name to be unique in the
      /// <paramref name="container"/>
      /// </summary>
      ISchemaItem CreateBasedOn(ISchemaItem schemaItemToClone, IContainer container);
   }

   public class SchemaItemFactory : ISchemaItemFactory
   {
      private readonly IObjectBaseFactory _objectBaseFactory;
      private readonly ISchemaItemParameterRetriever _schemaItemParameterRetriever;
      private readonly IContainerTask _containerTask;
      private readonly ICloner _cloner;

      public SchemaItemFactory(IObjectBaseFactory objectBaseFactory, ISchemaItemParameterRetriever schemaItemParameterRetriever,
         IContainerTask containerTask, ICloner cloner)
      {
         _objectBaseFactory = objectBaseFactory;
         _schemaItemParameterRetriever = schemaItemParameterRetriever;
         _containerTask = containerTask;
         _cloner = cloner;
      }

      public ISchemaItem Create(ApplicationType applicationType, IContainer container)
      {
         var applicationSchemaItem = _objectBaseFactory.Create<ISchemaItem>();
         applicationSchemaItem.Name = _containerTask.CreateUniqueName(container, PKSimConstants.UI.SchemaItem);
         applicationSchemaItem.ApplicationType = applicationType;
         applicationSchemaItem.FormulationKey = string.Empty;

         foreach (var parameter in _schemaItemParameterRetriever.AllParametersFor(applicationSchemaItem.ApplicationType))
         {
            applicationSchemaItem.Add(parameter);
         }
         return applicationSchemaItem;
      }

      public ISchemaItem CreateBasedOn(ISchemaItem schemaItemToClone, IContainer container)
      {
         return _cloner.Clone(schemaItemToClone)
            .WithName(_containerTask.CreateUniqueName(container, PKSimConstants.UI.SchemaItem));
      }
   }
}