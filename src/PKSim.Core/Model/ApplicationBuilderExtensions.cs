using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;

namespace PKSim.Core.Model
{
   public static class ApplicationBuilderExtensions
   {
      public static IContainer ProtocolSchemaItemContainer(this ApplicationBuilder applicationBuilder)
      {
         return applicationBuilder.Container(CoreConstants.ContainerName.ProtocolSchemaItem);
      }
   }
}