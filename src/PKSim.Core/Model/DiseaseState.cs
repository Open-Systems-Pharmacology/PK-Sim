using System.Collections.Generic;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Model
{
   public class DiseaseState : Container
   {
      public virtual string DisplayName { get; set; }
      public virtual string Implementation { get; set; }

      public virtual IReadOnlyList<IParameter> Parameters => this.GetAllChildren<IParameter>();
   }
}