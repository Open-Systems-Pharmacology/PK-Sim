using OSPSuite.Core.Domain;

namespace PKSim.Presentation.DTO.Compounds;

public class ExtendedPropertyOptionDTO : IWithName
{
   public ExtendedPropertyOptionDTO(string name, string displayName = null, string icon = null)
   {
      Name = name;
      DisplayName = displayName ?? name;
      Icon = icon;
   }

   public string Name { get; set; }
   public string DisplayName { get; }
   public string Icon { get; }
}
