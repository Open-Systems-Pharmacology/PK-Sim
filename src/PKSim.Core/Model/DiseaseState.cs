using System.Collections.Generic;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Model
{
   public class DiseaseState : Container
   {
      public virtual string DisplayName { get; set; }

      public virtual IReadOnlyList<IParameter> Parameters => this.GetAllChildren<IParameter>();

      public bool IsHealthy => this.IsNamed(CoreConstants.DiseaseStates.HEALTHY);
   }
}