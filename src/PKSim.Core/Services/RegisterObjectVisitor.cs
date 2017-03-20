using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Visitor;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Services
{
   public interface IRegisterObjectVisitor
   {
      void Register(IWithId objectToRegister);
   }

   public class RegisterObjectVisitor : IRegisterObjectVisitor,
      IVisitor<IObjectBase>,
      IVisitor<Simulation>
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

      public void Visit(Simulation simulation)
      {
         Visit(simulation as IObjectBase);
         if (!simulation.IsLoaded) return;
         simulation.UsedBuildingBlocks.Each(bb => Register(bb.BuildingBlock));
      }
   }
}