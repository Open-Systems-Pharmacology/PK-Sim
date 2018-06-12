using System.Collections.Generic;
using OSPSuite.Utility.Collections;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Infrastructure.ORM.FlatObjects;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public class SchemaItemRepository :StartableRepository<ISchemaItem>, ISchemaItemRepository
   {
      private readonly IFlatSchemaItemRepository _schemaItemRepo;
      private readonly IFlatContainerRepository _flatContainerRepo;
      private readonly IParameterContainerTask _parameterContainerTask;
      private readonly ICache<string, ISchemaItem> _schemaItems;

      public SchemaItemRepository(IFlatSchemaItemRepository schemaItemRepo, 
                                  IFlatContainerRepository flatContainerRepo,
                                  IParameterContainerTask parameterContainerTask)
      {
         _schemaItemRepo = schemaItemRepo;
         _flatContainerRepo = flatContainerRepo;
         _parameterContainerTask = parameterContainerTask;
         _schemaItems = new Cache<string, ISchemaItem>(si => si.ApplicationType.Name);
      }

      public override IEnumerable<ISchemaItem> All()
      {
         Start();
         return _schemaItems;
      }

      protected override void DoStart()
      {
         foreach (var flatSchemaItem in _schemaItemRepo.All())
         {
            _schemaItems.Add(mapFrom(flatSchemaItem));
         }
      }

      private ISchemaItem mapFrom(FlatSchemaItem flatSchemaItem)
      {
         var schemaItem = new SchemaItem
         {
            Name = flatSchemaItem.Name,
            ApplicationType = ApplicationTypes.ByName(flatSchemaItem.ApplicationType)
         };

         // temporarily create parent container hierarchy of the
         //schema item container in order to retrieve parameters from the DB
         FlatContainerId flatContainer = flatSchemaItem;
         IContainer container = schemaItem;

         while(_flatContainerRepo.ParentContainerFrom(flatContainer.Id) != null)
         {
            var flatParentContainer = _flatContainerRepo.ParentContainerFrom(flatContainer.Id);
            container.ParentContainer = new Container {Name = flatParentContainer.Name};

            container = container.ParentContainer;
            flatContainer = flatParentContainer;
         }

         // now parameters can be added
         _parameterContainerTask.AddSchemaItemParametersTo(schemaItem);

         return schemaItem;
      }

      public ISchemaItem SchemaItemBy(ApplicationType applicationType)
      {
         Start();
         return _schemaItems[applicationType.Name];
      }
   }
}
