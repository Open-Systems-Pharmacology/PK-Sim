using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;

namespace PKSim.Core.Model
{
   /// <summary>
   ///    Represents list of alternatives for a group of compound parameters
   /// </summary>
   public class ParameterAlternativeGroup : Container
   {
      /// <summary>
      ///    All available alternative definitions for current group.
      ///    One alternative contains all group parameters
      /// </summary>
      public virtual IEnumerable<ParameterAlternative> AllAlternatives => GetChildren<ParameterAlternative>();

      public virtual void AddAlternative(ParameterAlternative parameterAlternative)
      {
         Add(parameterAlternative);
      }

      public virtual void RemoveAlternative(ParameterAlternative parameterAlternative)
      {
         RemoveChild(parameterAlternative);
      }

      public virtual ParameterAlternative DefaultAlternative
      {
         get { return AllAlternatives.FirstOrDefault(x => x.IsDefault); }
      }

      /// <summary>
      ///    Template parameters for current group (usually read fro DB)
      /// </summary>
      public virtual IEnumerable<IParameter> TemplateParameters
      {
         get { return ParentContainer.GetChildren<IParameter>(p => string.Equals(p.GroupName, Name)); }
      }

      public virtual ParameterAlternative AlternativeByName(string alternativeName)
      {
         return this.GetSingleChildByName<ParameterAlternative>(alternativeName);
      }

   }
}