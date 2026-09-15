using OSPSuite.Core.Serialization.Xml;
using IContainer = OSPSuite.Utility.Container.IContainer;

namespace PKSim.Starter;

// All objects handed to MoBi are exchanged as serialized PKML so that they are recreated in the MoBi container
// (MoBi dimensions, MoBiSpatialStructures instead of PKSimSpatialStructures, ...)
internal static class ExchangeSerializer
{
   internal static string Serialize<T>(T itemToSerialize, IContainer container) => container.Resolve<IPKMLPersistor>().Serialize(itemToSerialize);
}
