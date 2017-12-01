using System.Collections.Generic;
using System.Linq;
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
      /// <paramref name="container"/> if defined
      /// </summary>
      SchemaItem Create(ApplicationType applicationType, IContainer container=null);

      /// <summary>
      /// Returns an exact duplicate of the <paramref name="schemaItemToClone"/> and adjust its name to be unique in the
      /// <paramref name="container"/>
      /// </summary>
      SchemaItem CreateBasedOn(SchemaItem schemaItemToClone, IContainer container);
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

      public SchemaItem Create(ApplicationType applicationType, IContainer container = null)
      {
         var applicationSchemaItem = _objectBaseFactory.Create<SchemaItem>().WithName(PKSimConstants.UI.SchemaItem);
        
         if (container != null)
             applicationSchemaItem.Name = _containerTask.CreateUniqueName(container, PKSimConstants.UI.SchemaItem);

         applicationSchemaItem.ApplicationType = applicationType;
         applicationSchemaItem.FormulationKey = string.Empty;

         foreach (var parameter in _schemaItemParameterRetriever.AllParametersFor(applicationSchemaItem.ApplicationType))
         {
            applicationSchemaItem.Add(parameter);
         }
         return applicationSchemaItem;
      }

      public SchemaItem CreateBasedOn(SchemaItem schemaItemToClone, IContainer container)
      {
         return _cloner.Clone(schemaItemToClone)
            .WithName(_containerTask.CreateUniqueName(container, PKSimConstants.UI.SchemaItem));
      }
   }
}