using System.Linq;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Compounds;
using OSPSuite.Core.Domain;

namespace PKSim.Infrastructure.ProjectConverter.v5_2
{
   public interface ICompoundConverter
   {
      void Convert(Compound compound);
      void UpdateGainPerChargeInAlternatives(Compound compound, bool updateValues = true);
   }

   public class CompoundConverter : ICompoundConverter
   {
      private readonly ICompoundFactory _compoundFactory;
      private readonly ICloner _cloner;

      public CompoundConverter(ICompoundFactory compoundFactory, ICloner cloner)
      {
         _compoundFactory = compoundFactory;
         _cloner = cloner;
      }

      public void Convert(Compound compound)
      {
         UpdateGainPerChargeInAlternatives(compound);
         updateFractionUnboundName(compound);
         addNewParameters(compound);
         convertSystemicProcesses(compound);
      }

      private void convertSystemicProcesses(Compound compound)
      {
         var allSystemicProcesses = compound.AllProcesses<SystemicProcess>()
            .Where(p => p.ContainsName(ConverterConstants.Parameter.Lipophilicity)).ToList();

         foreach (var systemicProcess in allSystemicProcesses)
         {
            var lipo = systemicProcess.Parameter(ConverterConstants.Parameter.Lipophilicity);
            lipo.Visible = true;
            lipo.Name = CoreConstants.Parameter.LIPOPHILICITY_EXPERIMENT;
            updateLipophilicityReferences(systemicProcess, ConverterConstants.Parameter.BloodPlasmaConcentrationRatio);
            updateLipophilicityReferences(systemicProcess, ConverterConstants.Parameter.PartitionCoefficientWwaterProtein);
         }
      }

      private void updateLipophilicityReferences(SystemicProcess systemicProcess, string parameterName)
      {
         var parameter = systemicProcess.Parameter(parameterName);
         if (parameter == null) return;
         foreach (var objectPath in parameter.Formula.ObjectPaths)
         {
            objectPath.Replace(ConverterConstants.Parameter.Lipophilicity, CoreConstants.Parameter.LIPOPHILICITY_EXPERIMENT);
         }
      }

      private void addNewParameters(Compound compound)
      {
         var defaultCompound = _compoundFactory.Create();

         var plasmaProteinBindingPartner = _cloner.Clone(defaultCompound.Parameter(CoreConstants.Parameter.PLASMA_PROTEIN_BINDING_PARTNER));
         plasmaProteinBindingPartner.Value = (int) PlasmaProteinBindingPartner.Unknown;
         compound.Add(plasmaProteinBindingPartner);
         compound.Add(_cloner.Clone(defaultCompound.Parameter(ConverterConstants.Parameter.Kass_FcRn)));
         compound.Add(_cloner.Clone(defaultCompound.Parameter(ConverterConstants.Parameter.Kd_FcRn_pls_int)));
         compound.Add(_cloner.Clone(defaultCompound.Parameter(ConverterConstants.Parameter.BP_AGP)));
         compound.Add(_cloner.Clone(defaultCompound.Parameter(ConverterConstants.Parameter.BP_ALBUMIN)));
         compound.Add(_cloner.Clone(defaultCompound.Parameter(ConverterConstants.Parameter.BP_UNKNOWN)));
         compound.Add(_cloner.Clone(defaultCompound.Parameter(ConverterConstants.Parameter.CalculatedSpecificIntestinalPermeability)));

         var oldKdFCRn = compound.Parameter("Kd (FcRn)");
         oldKdFCRn.Name = ConverterConstants.Parameter.Kd_FcRn_Endo;
      }

      private void updateFractionUnboundName(Compound compound)
      {
         var fuGroup = compound.ParameterAlternativeGroup(CoreConstants.Groups.COMPOUND_FRACTION_UNBOUND);
         foreach (var alternative in fuGroup.AllAlternatives)
         {
            updateFractionUnboundParameterInContainer(alternative);
         }
      }

      private static void updateFractionUnboundParameterInContainer(IContainer container)
      {
         container.Parameter(ConverterConstants.Parameter.FractionUnboundPlasma).Name = CoreConstants.Parameter.FRACTION_UNBOUND_PLASMA_REFERENCE_VALUE;
      }

      public void UpdateGainPerChargeInAlternatives(Compound compound, bool updateValues = true)
      {
         var gainPerCharge = compound.Parameter(CoreConstants.Parameter.SolubilityGainPerCharge);
         gainPerCharge.GroupName = CoreConstants.Groups.COMPOUND_SOLUBILITY;

         var solGroup = compound.ParameterAlternativeGroup(CoreConstants.Groups.COMPOUND_SOLUBILITY);
         foreach (var alternative in solGroup.AllAlternatives)
         {
            var alternativeParameter = alternative.Parameter(CoreConstants.Parameter.SolubilityGainPerCharge);
            if (alternativeParameter != null)
            {
               if (updateValues)
                  alternativeParameter.Value = gainPerCharge.Value;
            }
            else
               alternative.Add(_cloner.Clone(gainPerCharge));
         }
      }
   }
}