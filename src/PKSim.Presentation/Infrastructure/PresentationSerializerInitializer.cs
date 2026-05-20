using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Presentation.Serialization.Extensions;
using OSPSuite.Utility.Container;

namespace PKSim.Presentation.Infrastructure
{
   /// <summary>
   ///    Bridges OSPSuite.Presentation.Serialization (Windows-only) into the cross-platform
   ///    PKSim.Infrastructure serialization pipeline. Desktop bootstraps call this between
   ///    <see cref="PKSim.Infrastructure.InfrastructureRegister.RegisterSerializationDependencies"/>
   ///    and <see cref="PKSim.Infrastructure.InfrastructureRegister.LoadDefaultEntities"/> so that
   ///    Presentation serializers (e.g. ChartEditorAndDisplaySettingsXmlSerializer) are present on
   ///    the base IOSPSuiteXmlSerializerRepository singleton before PerformMapping runs against it.
   ///    PKSim.R and headless callers skip this step.
   /// </summary>
   public static class PresentationSerializerInitializer
   {
      public static void AddPresentationSerializers(IContainer container)
      {
         container.Resolve<IOSPSuiteXmlSerializerRepository>().AddPresentationSerializers();
      }
   }
}
