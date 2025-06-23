using System;
using System.Text;
using OSPSuite.Core.Services;
using PKSim.Core.Snapshots;

namespace PKSim.Core.Services;

public interface IModuleSnapshotSerializer
{
   public string Serialize(Project project);
}

public class ModuleSnapshotSerializer : IModuleSnapshotSerializer
{
   private readonly IJsonSerializer _jsonSerializer;

   public ModuleSnapshotSerializer(IJsonSerializer jsonSerializer)
   {
      _jsonSerializer = jsonSerializer;
   }

   public string Serialize(Project projectSnapshot)
   {
      return Convert.ToBase64String(Encoding.UTF8.GetBytes(_jsonSerializer.Serialize(projectSnapshot)));
   }
}