using OSPSuite.Utility;

namespace PKSim.Core.Model
{
   public enum RepresentationObjectType
   {
      CALCULATION_METHOD = 1,
      CATEGORY,
      CONTAINER,
      SPECIES,
      DIMENSION,
      DISTRIBUTION_TYPE,
      GENDER,
      GROUP,
      KINETIC_TYPE,
      MODEL,
      OBSERVER,
      PARAMETER,
      PARAMETER_VALUE_VERSION,
      PROCESS_TYPE,
      POPULATION,
      MOLECULE,
      PROCESS,
      ONTOGENY,
      EVENT,
      CURVE_SELECTION,
      UNKNOWN,
   }

   public class RepresentationInfo
   {
      public RepresentationObjectType ObjectType { get; set; }
      public string Name { get; set; }
      public string DisplayName { get; set; }
      public string Description { get; set; }
      public string IconName { get; set; }
      public string SubType { get; set; }

      public static RepresentationObjectType ObjectTypeFrom(string objectType)
      {
         return EnumHelper.ParseValue<RepresentationObjectType>(objectType);
      }
   }

   public class EmptyRepresentationInfo : RepresentationInfo
   {
   }
}