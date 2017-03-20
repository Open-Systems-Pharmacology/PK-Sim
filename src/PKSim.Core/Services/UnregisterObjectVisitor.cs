using OSPSuite.Utility.Visitor;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Services
{
   public interface IUnregisterObjectVisitor
   {
      void Unregister(IWithId objectToUnregister);
   }

   public class UnregisterObjectVisitor : IUnregisterObjectVisitor,
                                          IVisitor<IObjectBase>
   {
      private readonly IWithIdRepository _withIdRepository;

      public UnregisterObjectVisitor(IWithIdRepository withIdRepository)
      {
         _withIdRepository = withIdRepository;
      }

      public void Unregister(IWithId objectToUnregister)
      {
         if (objectToUnregister == null)
            return;

         var objectBase = objectToUnregister as IObjectBase;
         if (objectBase == null)
            unregister(objectToUnregister);
         else
            objectBase.AcceptVisitor(this);
      }

      private void unregister(IWithId objectToUnregister)
      {
         _withIdRepository.Unregister(objectToUnregister.Id);
      }

      public void Visit(IObjectBase objToVisit)
      {
         if (objToVisit == null) return;
         unregister(objToVisit);
      }
   }
}