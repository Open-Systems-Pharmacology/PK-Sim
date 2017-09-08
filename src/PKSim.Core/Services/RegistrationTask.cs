using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IRegistrationTask
   {
      void Register(IWithId objectToRegister);
      void Unregister(IWithId objectToUnregister);
      void RegisterProject(PKSimProject project);
      void UnregisterProject(PKSimProject project);
   }

   public class RegistrationTask : IRegistrationTask
   {
      private readonly IRegisterObjectVisitor _registerObjectVisitor;
      private readonly IUnregisterObjectVisitor _unregisterObjectVisitor;

      public RegistrationTask(IRegisterObjectVisitor registerObjectVisitor, IUnregisterObjectVisitor unregisterObjectVisitor)
      {
         _registerObjectVisitor = registerObjectVisitor;
         _unregisterObjectVisitor = unregisterObjectVisitor;
      }

      public void Register(IWithId objectToRegister)
      {
         _registerObjectVisitor.Register(objectToRegister);
      }

      public void Unregister(IWithId objectToUnregister)
      {
         _unregisterObjectVisitor.Unregister(objectToUnregister);
      }

      public void RegisterProject(PKSimProject project)
      {
         if (project == null) return;
         Register(project);
         project.AllObservedData.Each(Register);
      }

      public void UnregisterProject(PKSimProject project)
      {
         if (project == null) return;
         Unregister(project);
         project.AllObservedData.Each(Unregister);
      }
   }
}