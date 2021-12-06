using OSPSuite.Core.Domain;
using OSPSuite.Utility.Exceptions;
using static OSPSuite.Core.Domain.Constants.Parameters;
using static PKSim.Core.CoreConstants.Organ;
using static PKSim.Core.CoreConstants.Parameters;

namespace PKSim.Core.Model
{
   public class CKDDiseaseStateImplementationFactory : IDiseaseStateImplementationSpecificationFactory
   {
      public bool IsSatisfiedBy(DiseaseState diseaseState) => diseaseState.IsNamed(CoreConstants.DiseaseStates.CKD);

      public IDiseaseStateImplementation Create()
      {
         return new CKDDiseaseStateImplementation();
      }
   }

   public class CKDDiseaseStateImplementation : IDiseaseStateImplementation
   {
      private static string GFR_TARGET = "GFR_Target";

      public void ApplyTo(Individual individual)
      {
         var GFR_t = individual.OriginData.DiseaseStateParameters.FindByName(GFR_TARGET);
         var organism = individual.Organism;
         var kidney = organism.Organ(KIDNEY);
         var bsa = organism.Parameter(BSA);
         var GFR_spec = kidney.Parameter(GFR_SPEC);
         var kid_vol = kidney.Parameter(VOLUME);

         var GFR_0 = GFR_spec.Value * kid_vol.Value * 1.73 / bsa.Value;
         if (GFR_0 < GFR_t.Value)
            throw new OSPSuiteException("ERROR");


      }
   }
}