using OSPSuite.Core.Domain;

namespace PKSim.Core.Model
{
   public class SupportedProtein : IWithName
   {
      public string Name { get; set; }
      public string ParameterName { get; }
      public string TableParameterName { get; }

      public SupportedProtein(string name, string parameterName, string tableParameterName)
      {
         Name = name;
         ParameterName = parameterName;
         TableParameterName = tableParameterName;
      }
   }
}