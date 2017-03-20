using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PKSim.Infrastructure.ORM.FlatObjects
{
   public class FlatCalculationMethodRateDescriptorConditions : FlatDescriptorConditionBase
   {
      public string CalculationMethod { get; set; }
      public string Rate { get; set; }
   }
}
