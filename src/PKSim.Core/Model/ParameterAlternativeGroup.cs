using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;

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

      public virtual void AddAlternative(ParameterAlternative parameterAlternative) => Add(parameterAlternative);

      public virtual void RemoveAlternative(ParameterAlternative parameterAlternative) => RemoveChild(parameterAlternative);

      /// <summary>
      ///    Returns the default alternative if defined otherwise the first alternative, if any is available
      /// </summary>
      public virtual ParameterAlternative DefaultAlternative => AllAlternatives.FirstOrDefault(x => x.IsDefault) ?? AllAlternatives.FirstOrDefault();

      /// <summary>
      ///    Template parameters for current group (usually read from Db)
      /// </summary>
      public virtual IEnumerable<IParameter> TemplateParameters
      {
         get { return ParentContainer.GetChildren<IParameter>(p => string.Equals(p.GroupName, Name)); }
      }

      public virtual ParameterAlternative AlternativeByName(string alternativeName) => this.GetSingleChildByName<ParameterAlternative>(alternativeName);
   }
}