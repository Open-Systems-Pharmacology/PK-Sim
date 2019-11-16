using System.Linq;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.ComponentActivator;
using Castle.MicroKernel.Context;
using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.ModelBuilder;
using PKSim.Core;
using PKSim.Core.Services;
using PKSim.Infrastructure.Services;
using OSPSuite.Utility.Extensions;

namespace PKSim.Infrastructure
{
   public class SerializationFacility : AbstractFacility
   {
      protected override void Init()
      {
         Kernel.ComponentModelBuilder.AddContributor(new SerializationComponentModelConstruction());
      }

      public class SerializationComponentModelConstruction : IContributeComponentModelConstruction
      {
         public void ProcessModel(IKernel kernel, ComponentModel model)
         {
            //we should defined here all component for which the Compressed Serializer Manager 
            //should be use instead of the default serialization manager
            if (!(
               model.Implementation.IsAnImplementationOf<TemplateTaskQuery>()
            ))
               return;

            model.CustomComponentActivator = typeof(SerializationCustomComponentActivator);
         }
      }

      public class SerializationCustomComponentActivator : DefaultComponentActivator
      {
         public SerializationCustomComponentActivator(ComponentModel model, IKernelInternal kernel, ComponentInstanceDelegate onCreation, ComponentInstanceDelegate onDestruction)
            : base(model, kernel, onCreation, onDestruction)
         {
         }

         protected override object CreateInstance(CreationContext context, ConstructorCandidate constructor, object[] arguments)
         {
            var index = constructor.Dependencies.Select(x=>x.TargetType).ToList().FindIndex(type => type.IsAnImplementationOf<IStringSerializer>());
            if (index >= 0)
               arguments[index] = Kernel.Resolve<IStringSerializer>(CoreConstants.Serialization.Compressed);

            return base.CreateInstance(context, constructor, arguments);
         }
      }
   }
}