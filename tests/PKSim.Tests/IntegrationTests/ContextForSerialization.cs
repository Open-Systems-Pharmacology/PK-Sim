using OSPSuite.Utility.Container;
using PKSim.Core.Services;

namespace PKSim.IntegrationTests
{
   public abstract class ContextForSerialization<T> : ContextForIntegration<T>
   {
      protected ISerializationManager _serializationManager;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _serializationManager = IoC.Resolve<ISerializationManager>();
      }

      protected override void Context()
      {
      }

      public virtual T SerializeAndDeserialize(T source)
      {
         var stream = _serializationManager.Serialize(source);
         return _serializationManager.Deserialize<T>(stream);
      }
   }
}