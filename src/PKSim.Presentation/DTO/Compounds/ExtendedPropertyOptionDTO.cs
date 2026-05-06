namespace PKSim.Presentation.DTO.Compounds;

public class ExtendedPropertyOptionDTO
{
   public ExtendedPropertyOptionDTO(string name, string displayName = null, string icon = null)
   {
      Name = name;
      DisplayName = displayName ?? name;
      Icon = icon;
   }

   public string Name { get; }
   public string DisplayName { get; }
   public string Icon { get; }
}
