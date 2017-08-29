using System.Linq;
using OSPSuite.Utility.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;

namespace PKSim.Core.Model
{
   public class PKSimParameter : Parameter
   {
      public override double Value
      {
         get => base.Value;
         set
         {
            if (Formula == null || !Formula.IsConstant())
            {
               base.Value = value;
               return;
            }

            var constantFormula = Formula.DowncastTo<ConstantFormula>();

            //To avoid event trigger
            if (constantFormula.Value != value)
               constantFormula.Value = value;

            if (IsFixedValue)
               IsFixedValue = false;
         }
      }

      public override bool IsChangedByCreateIndividual
      {
         get
         {
            if (string.IsNullOrEmpty(Name))
               return false;

            if (!this.IsOfType(PKSimBuildingBlockType.Individual))
               return false;

            if (CoreConstants.Parameter.StandardCreateIndividualParameters.Contains(Name))
               return true;

            if (CoreConstants.Parameter.DerivedCreatedIndividualParameters.Contains(Name))
               return true;

            if (CoreConstants.Parameter.OntogenyFactors.Contains(Name))
               return true;

            if (this.NameIsOneOf(CoreConstants.Parameter.ONTOGENY_FACTOR_AGP, CoreConstants.Parameter.ONTOGENY_FACTOR_ALBUMIN))
               return true;

            //only parameter in these 4 organs needs to be treated specially
            if (!isInFatOrMuscleOrLungOrPortailVein())
               return false;

            if (CoreConstants.Parameter.VolumeFractionWaterParameters.Contains(Name) && isInFatOrMuscle())
               return true;

            //formula parameter indirectly changed by create individual 
            if (string.Equals(CoreConstants.Parameter.FractionIntracellular, Name) && isInFatOrMuscle())
               return true;

            if (CoreConstants.Parameter.VolumeFractionLipidsParameters.Contains(Name) && isInFat())
               return true;

            if (CoreConstants.Parameter.VolumeFractionProteinsParameters.Contains(Name) && isInMuscle())
               return true;

            return false;
         }
      }

      private bool isInFatOrMuscleOrLungOrPortailVein()
      {
         return isInFatOrMuscle() || isInLungOrPortalVein();
      }

      private bool isInFatOrMuscle()
      {
         return isInFat() || isInMuscle();
      }

      private bool isInLungOrPortalVein()
      {
         return isInLung() || isInPortalVein();
      }

      private bool isInPortalVein()
      {
         return isIn(CoreConstants.Organ.PortalVein);
      }

      private bool isInLung()
      {
         return isIn(CoreConstants.Organ.Lung);
      }

      private bool isInFat()
      {
         return isIn(CoreConstants.Organ.Fat);
      }

      private bool isInMuscle()
      {
         return isIn(CoreConstants.Organ.Muscle);
      }

      private bool isIn(string organName)
      {
         if (ParentContainer == null)
            return false;

         return string.Equals(ParentContainer.Name, organName);
      }
   }

   public class NullParameter : PKSimParameter
   {
      
   }
}