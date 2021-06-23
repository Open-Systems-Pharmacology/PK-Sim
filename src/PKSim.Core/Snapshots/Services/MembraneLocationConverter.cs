using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using static PKSim.Core.CoreConstants.Compartment;
using static PKSim.Core.CoreConstants.Parameters;

namespace PKSim.Core.Snapshots.Services
{
   public enum MembraneLocation
   {
      Apical,
      Basolateral,
      BloodBrainBarrier, //(apical)
      Tissue,            //(basolateral)
   }


   public static class MembraneLocationConverter
   {
      public static void ConvertMembraneLocationToParameterFraction(IReadOnlyList<TransporterExpressionContainer> transporterExpressionContainers, MembraneLocation membraneLocation)
      {
         var firstContainer = transporterExpressionContainers[0];
         if (firstContainer.LogicalContainer.IsBrain())
         {
            //Is there by construction
            var plasma = transporterExpressionContainers.First(x => x.CompartmentName == PLASMA);
            var fractionBBB = plasma.Parameter(FRACTION_EXPRESSED_AT_BLOOD_BRAIN_BARRIER);
            switch (membraneLocation)
            {
               //we need to set the value of f_bbb to zero
               case MembraneLocation.Tissue:
                  fractionBBB.Value = 0;
                  return;
               //we need to set the value of f_bbb to 1
               case MembraneLocation.BloodBrainBarrier:
                  fractionBBB.Value = 1;
                  return;
            }
         }

         if (firstContainer.LogicalContainer.IsOrganWithLumen())
         { 
            //Is there by construction
            var intracellular = transporterExpressionContainers.First(x => x.CompartmentName == INTRACELLULAR);
            var fractionExpressedApical = intracellular.Parameter(FRACTION_EXPRESSED_APICAL);
            switch (membraneLocation)
            {
               //we need to set the value of f_expressed_apical to zero
               case MembraneLocation.Basolateral:
                  fractionExpressedApical.Value = 0;
                  return;
               //we need to set the value of f_bbb to 1
               case MembraneLocation.Apical:
                  fractionExpressedApical.Value = 1;
                  return;
            }

         }


         //this should never happen
      }
   }
}