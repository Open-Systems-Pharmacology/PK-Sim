using OSPSuite.Core.Domain;
using OSPSuite.Utility.Visitor;

namespace PKSim.Core.Services
{
   public interface IRegisterObjectVisitor
   {
      void Register(IWithId objectToRegister);
   }

   public class RegisterObjectVisitor : IRegisterObjectVisitor,
      IVisitor<IObjectBase>
   {
      private readonly IWithIdRepository _withIdRepository;

      public RegisterObjectVisitor(IWithIdRepository withIdRepository)
      {
         _withIdRepository = withIdRepository;
      }

      public void Register(IWithId objectToRegister)
      {
         if (objectToRegister == null)
            return;

         var objectBase = objectToRegister as IObjectBase;
         if (objectBase == null)
            register(objectToRegister);
         else
            objectBase.AcceptVisitor(this);
      }

      private void register<TObject>(TObject objectToRegister) where TObject : IWithId
      {
         _withIdRepository.Register(objectToRegister);
      }

      public void Visit(IObjectBase objToVisit)
      {
         if (objToVisit == null) return;
         if (string.IsNullOrEmpty(objToVisit.Id)) return;
         _withIdRepository.Unregister(objToVisit.Id);
         register(objToVisit);
      }
   }
}