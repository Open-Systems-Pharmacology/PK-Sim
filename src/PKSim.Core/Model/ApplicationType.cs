using System.Collections.Generic;
using PKSim.Assets;
using OSPSuite.Utility.Collections;

namespace PKSim.Core.Model
{
   public static class ApplicationTypes
   {
      private static readonly ICache<string, ApplicationType> _allApplicationTypes = new Cache<string, ApplicationType>(app => app.Name);

      public static ApplicationType IntravenousBolus = create(CoreConstants.Application.Name.IntravenousBolus, CoreConstants.Application.Route.Intravenous, PKSimConstants.UI.IntravenousBolus);
      public static ApplicationType Intravenous = create(CoreConstants.Application.Name.Intravenous, CoreConstants.Application.Route.Intravenous, PKSimConstants.UI.Intravenous);
      public static ApplicationType Oral = create(CoreConstants.Application.Name.Oral, CoreConstants.Application.Route.Oral, PKSimConstants.UI.Oral, needsFormulation:true);
      public static ApplicationType UserDefined = create(CoreConstants.Application.Name.UserDefined, CoreConstants.Application.Route.UserDefined, PKSimConstants.UI.UserDefined, needsFormulation: true, userDefined:true);

      public static ApplicationType ByName(string applicationTypeName)
      {
         return _allApplicationTypes[applicationTypeName];
      }

      private static ApplicationType create(string name, string route, string displayName, bool needsFormulation=false, bool userDefined=false)
      {
         var applicationType = new ApplicationType(name, route, displayName, needsFormulation, userDefined);
         _allApplicationTypes.Add(applicationType);
         return applicationType;
      }

      public static IEnumerable<ApplicationType> All()
      {
         return _allApplicationTypes;
      }
   }

   public class ApplicationType
   {
      /// <summary>
      ///    Application name as defined in the PKSim Database
      /// </summary>
      public string Name { get; }

      /// <summary>
      ///    Which route is used by the application type.
      /// </summary>
      public string Route { get; }

      /// <summary>
      ///    Does a formulation need to be defined for this type?
      /// </summary>
      public bool NeedsFormulation { get; }

      /// <summary>
      ///    is this application type user defined
      /// </summary>
      public bool UserDefined { get; }

      /// <summary>
      ///    Name of icon used to represent the application
      /// </summary>
      public string IconName { get; }

      public string DisplayName { get; }

      public ApplicationType(string name, string route, string displayName, bool needsFormulation, bool userDefined)
      {
         Route = route;
         Name = name;
         DisplayName = displayName;
         IconName = name;
         NeedsFormulation = needsFormulation;
         UserDefined = userDefined;
      }

      public override string ToString()
      {
         return DisplayName;
      }
   }
}