using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Container;
using OSPSuite.Core.Domain.Services;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_InVitroSpecificClearance : ContextForIntegration<EnzymaticProcess>
   {
      private ICompoundProcessRepository _compoundProcessRepository;
      private ICloneManager _cloneManager;
      protected CompoundProcess _liverMicrosomeFirstOrder;
      protected CompoundProcess _liverMicrosomeRes;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _compoundProcessRepository = IoC.Resolve<ICompoundProcessRepository>();
         _cloneManager = IoC.Resolve<ICloneManager>();
         _liverMicrosomeFirstOrder = _cloneManager.Clone(_compoundProcessRepository.ProcessByName(CoreConstantsForSpecs.Process.METABOLIZATION_LIVER_MICROSOME_FIRST_ORDER));
         _liverMicrosomeFirstOrder.Name = "liverMicrosomeFirstOrder";
         _liverMicrosomeRes = _cloneManager.Clone(_compoundProcessRepository.ProcessByName(CoreConstantsForSpecs.Process.LIVER_MICROSOME_RES));
         _liverMicrosomeRes.Name = "liverMicrosomeRes";
      }
   }

   public class When_setting_the_residual_fraction_in_an_invitro_microsome_residual_fraction_process : concern_for_InVitroSpecificClearance
   {
      protected override void Context()
      {
         _liverMicrosomeRes.Parameter("Residual fraction").Value = 0.8;
         _liverMicrosomeRes.Parameter("Measuring time").Value = 20;
         _liverMicrosomeRes.Parameter("Fraction intracellular (liver)").Value = 0.66700;
         _liverMicrosomeRes.Parameter("Fraction unbound (assay)").Value = 0.28476;

         var p = _liverMicrosomeRes.Parameter("Amount protein/incubation");
         var dim = p.Dimension;
         var unit = dim.Unit("mg/ml");
         p.Value = dim.UnitValueToBaseUnitValue(unit, 0.1);
         p = _liverMicrosomeRes.Parameter("Microsomal protein mass/g liver");
         dim = p.Dimension;
         unit = dim.Unit("mg/g");
         p.Value = dim.UnitValueToBaseUnitValue(unit, 40);
      }

      [Observation]
      public void the_calcualted_value_for_cl_spec_per_enzyme_should_be_the_one_expected()
      {
         var specificClearance = _liverMicrosomeRes.Parameter("Specific clearance");
         specificClearance.Value.ShouldBeEqualTo(23.4972, 1e-2);
      }
   }

   public class When_setting_the_value_of_invitro_cl_in_a_liver_mircrosome_first_order_process : concern_for_InVitroSpecificClearance
   {
      protected override void Context()
      {
         var inVitroCLForLiverMicrosomes = _liverMicrosomeFirstOrder.Parameter("In vitro CL for liver microsomes");
         var dim = inVitroCLForLiverMicrosomes.Dimension;
         var unit = dim.Unit("µl/min/mg mic. protein");
         inVitroCLForLiverMicrosomes.Value = dim.UnitValueToBaseUnitValue(unit, 1);

         var contentOfCyp = _liverMicrosomeFirstOrder.Parameter("Content of CYP proteins in liver microsomes");
         dim = contentOfCyp.Dimension;
         unit = dim.Unit("pmol/mg mic. protein");
         contentOfCyp.Value = dim.UnitValueToBaseUnitValue(unit, 108);
      }

      [Observation]
      public void the_calcualted_value_for_cl_spec_per_enzyme_should_be_the_one_expected()
      {
         var clSpecPerEnzyme = _liverMicrosomeFirstOrder.Parameter("CLspec/[Enzyme]");
         clSpecPerEnzyme.DisplayUnit = clSpecPerEnzyme.Dimension.Unit("l/µmol/min");
         clSpecPerEnzyme.ValueInDisplayUnit.ShouldBeEqualTo(0.009259, 1e-2);
      }
   }
}