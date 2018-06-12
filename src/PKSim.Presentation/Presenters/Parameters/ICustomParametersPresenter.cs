using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.Parameters
{
   public interface ICustomParametersPresenter : ICommandCollectorPresenter
   {
      /// <summary>
      ///    Set the string that describes the role of this parameters in the model
      /// </summary>
      string Description { get; set; }

      /// <summary>
      ///    Edit the given parameters
      /// </summary>
      void Edit(IEnumerable<IParameter> parameters);

      /// <summary>
      ///    Returns true if the presenter should be displayed even if not parameters are available otherwise false
      /// </summary>
      bool ForcesDisplay { get; }

      /// <summary>
      /// Returns true if the Edit method should be called to refresh the presenter even if the presenter was already loaded. 
      /// </summary>
      bool AlwaysRefresh { get; }

      IEnumerable<IParameter> EditedParameters { get; }
   }
}