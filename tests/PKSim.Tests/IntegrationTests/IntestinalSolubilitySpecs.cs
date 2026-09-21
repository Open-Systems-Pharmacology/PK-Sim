using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Infrastructure;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using Compound = PKSim.Core.Model.Compound;
using IContainer = OSPSuite.Core.Domain.IContainer;
using static PKSim.CoreConstantsForSpecs;

namespace PKSim.IntegrationTests
{
   /// <summary>
   ///    Verifies the solubility chain of the intestinal lumen in a fully built simulation:
   ///    <para />
   ///    Solubility           = Saq + SolubilityTable + (UseBileSaltMicellization = 1 ? Max(BS_C - CMC; 0)*(S0/CW*10^K_n + Si/CW*10^K_i) : 0)
   ///    <para />
   ///    Solubility (aqueous) = S0_ref * Solubility_pKa_REFpH_Factor / Solubility_pKa_pH_Factor
   ///    <para />
   ///    Solubility increase from ionization = Max(Saq - S0; 0)
   ///    <para />
   ///    An ionizable compound is used so that the pKa pH factors actually differ between the lumen segments.
   ///    A neutral compound would make all factors 1 and hide any defect in the chain.
   /// </summary>
   //https://github.com/Open-Systems-Pharmacology/PK-Sim/issues/3721
   //https://github.com/Open-Systems-Pharmacology/PK-Sim/issues/3730
   public abstract class concern_for_intestinal_solubility : ContextForIntegration<ISimulationConstructor>
   {
      //di-base, verified against the MATLAB export of the PBBM test model
      private const double _pKaBase0 = 8;
      private const double _pKaBase1 = 1;
      private const double _solubilityGainPerCharge = 2500;
      private const double _referencePH = 6.5;
      private const double _solubilityAtReferencePHValue = 7e-06; //[kg/l]

      protected Simulation _simulation;
      protected Compound _compound;
      protected string[] _lumenSegments = Constants.Compartment.AllLumenSegments.ToArray();

      public override void GlobalContext()
      {
         base.GlobalContext();
         var individual = DomainFactoryForSpecs.CreateStandardIndividual();
         _compound = DomainFactoryForSpecs.CreateStandardCompound();

         setCompoundType(0, CompoundType.Base, _pKaBase0);
         setCompoundType(1, CompoundType.Base, _pKaBase1);

         //the solubility parameters are defined in the default alternative of the solubility group and not
         //as direct children of the compound. Setting them on the compound itself has no effect on the simulation.
         var solubilityAlternative = _compound.ParameterAlternativeGroup(CoreConstants.Groups.COMPOUND_SOLUBILITY).DefaultAlternative;
         solubilityAlternative.Parameter(CoreConstants.Parameters.SOLUBILITY_GAIN_PER_CHARGE).Value = _solubilityGainPerCharge;
         solubilityAlternative.Parameter(CoreConstants.Parameters.REFERENCE_PH).Value = _referencePH;
         solubilityAlternative.Parameter(CoreConstants.Parameters.SOLUBILITY_AT_REFERENCE_PH).Value = _solubilityAtReferencePHValue;

         ConfigureSolubilityAlternatives(_compound.ParameterAlternativeGroup(CoreConstants.Groups.COMPOUND_SOLUBILITY));

         //an oral protocol requires a formulation. The solubility chain of the lumen is independent of the
         //release kinetics, so the simplest formulation is used.
         var protocol = DomainFactoryForSpecs.CreateStandardOralProtocol();
         var formulation = IoC.Resolve<IFormulationRepository>().FormulationBy(CoreConstants.Formulation.DISSOLVED);

         _simulation = DomainFactoryForSpecs.CreateSimulationWith(individual, _compound, protocol, formulation);
      }

      /// <summary>
      ///    Hook for derived contexts which need to change the solubility alternatives (e.g. to use a table solubility)
      ///    before the simulation is built. It is called after the parameters of the default alternative have been set.
      /// </summary>
      protected virtual void ConfigureSolubilityAlternatives(ParameterAlternativeGroup solubilityAlternativeGroup)
      {
         //nothing to do by default
      }

      private void setCompoundType(int index, CompoundType compoundType, double pKa)
      {
         _compound.Parameter(Constants.Parameters.ParameterCompoundType(index)).Value = (int) compoundType;
         _compound.Parameter(CoreConstants.Parameters.ParameterPKa(index)).Value = pKa;
      }

      protected IContainer Lumen => _simulation.Model.Root.Container(Constants.ORGANISM).Container(CoreConstants.Organ.LUMEN);

      protected IContainer CompoundInSimulation => _simulation.Model.Root.EntityAt<IContainer>(_compound.Name);

      protected IContainer DrugIn(string segment) => Lumen.Container(segment).Container(_compound.Name);

      protected IParameter TotalSolubility(string segment) => DrugIn(segment).Parameter(CoreConstants.Parameters.SOLUBILITY);

      protected IParameter AqueousSolubility(string segment) => DrugIn(segment).Parameter(SOLUBILITY_AQUEOUS);

      protected IParameter SolubilityIncreaseFromIonization(string segment) => DrugIn(segment).Parameter(SOLUBILITY_INCREASE_FROM_IONIZATION);

      protected IParameter SolubilityTable(string segment) => DrugIn(segment).Parameter(CoreConstants.Parameters.SOLUBILITY_TABLE);

      protected IParameter FractionUnboundLumen(string segment) => DrugIn(segment).Parameter(FRACTION_UNBOUND_DRUG_LUMEN);

      protected IParameter SegmentPH(string segment) => Lumen.Container(segment).Parameter(CoreConstants.Parameters.PH);

      protected IParameter BileSaltConcentration(string segment) => Lumen.Container(segment).Parameter(BILE_SALT_CONCENTRATION);

      protected IParameter IntrinsicSolubility => CompoundInSimulation.Parameter(Parameters.SOLUBILITY_INTRINSIC);

      protected IParameter CriticalMicellarConcentration => CompoundInSimulation.Parameter(CoreConstants.Parameters.CRITICAL_MICELLAR_CONCENTRATION);

      protected IParameter UseBileSaltMicellization => CompoundInSimulation.Parameter(USE_BILE_SALT_MICELLIZATION);

      /// <summary>
      ///    Segments in which the bile salt concentration exceeds the critical micellar concentration and the
      ///    micellization term therefore contributes to the total solubility.
      /// </summary>
      protected IEnumerable<string> SegmentsWithMicellization =>
         _lumenSegments.Where(segment => BileSaltConcentration(segment).Value > CriticalMicellarConcentration.Value);

      protected IEnumerable<string> SegmentsWithoutMicellization =>
         _lumenSegments.Where(segment => BileSaltConcentration(segment).Value <= CriticalMicellarConcentration.Value);

      protected const string SOLUBILITY_AQUEOUS = "Solubility (aqueous)";
      protected const string SOLUBILITY_INCREASE_FROM_IONIZATION = "Solubility increase from ionization";
      protected const string FRACTION_UNBOUND_DRUG_LUMEN = "Fraction unbound drug intestinal lumen";
      protected const string BILE_SALT_CONCENTRATION = "Bile Salt concentration";
      protected const string USE_BILE_SALT_MICELLIZATION = "Use bile salt micellization";
   }

   public class When_calculating_the_solubility_in_the_intestinal_lumen : concern_for_intestinal_solubility
   {
      /// <summary>
      ///    S0 + Si = Saq is the defining property of the decomposition of the aqueous solubility into its intrinsic part
      ///    and the increase caused by ionization. It is violated as soon as the intrinsic solubility is calculated at the
      ///    wrong pH, because the Max(Saq - S0; 0) clamp then starts to bind.
      /// </summary>
      //https://github.com/Open-Systems-Pharmacology/PK-Sim/issues/3730#issuecomment-5662251824
      [Observation]
      public void should_decompose_the_aqueous_solubility_into_the_intrinsic_solubility_and_the_increase_from_ionization()
      {
         var intrinsicSolubility = IntrinsicSolubility.Value;

         foreach (var segment in _lumenSegments)
         {
            var aqueousSolubility = AqueousSolubility(segment).Value;
            var solubilityIncrease = SolubilityIncreaseFromIonization(segment).Value;

            (intrinsicSolubility + solubilityIncrease).ShouldBeEqualTo(aqueousSolubility, 1e-9,
               $"S0 + Si != Saq in '{segment}'");
         }
      }

      /// <summary>
      ///    The intrinsic solubility is the minimum of the aqueous solubility over all pH values, so the clamp of the
      ///    solubility increase from ionization must never be active for a correctly calculated intrinsic solubility.
      /// </summary>
      [Observation]
      public void should_never_calculate_an_aqueous_solubility_below_the_intrinsic_solubility()
      {
         var intrinsicSolubility = IntrinsicSolubility.Value;

         foreach (var segment in _lumenSegments)
         {
            AqueousSolubility(segment).Value.ShouldBeGreaterThan(intrinsicSolubility * (1 - 1e-9));
         }
      }

      [Observation]
      public void should_add_the_aqueous_solubility_the_solubility_table_and_the_micellization_term()
      {
         foreach (var segment in _lumenSegments)
         {
            var expected = AqueousSolubility(segment).Value + SolubilityTable(segment).Value + micellizationTermFor(segment);
            TotalSolubility(segment).Value.ShouldBeEqualTo(expected, 1e-9, $"Unexpected solubility in '{segment}'");
         }
      }

      [Observation]
      public void should_calculate_a_total_solubility_above_the_aqueous_solubility_only_where_micelles_are_formed()
      {
         SegmentsWithMicellization.Any().ShouldBeTrue("No lumen segment exceeds the critical micellar concentration");

         foreach (var segment in SegmentsWithMicellization)
         {
            TotalSolubility(segment).Value.ShouldBeGreaterThan(AqueousSolubility(segment).Value);
         }

         foreach (var segment in SegmentsWithoutMicellization)
         {
            var expected = AqueousSolubility(segment).Value + SolubilityTable(segment).Value;
            TotalSolubility(segment).Value.ShouldBeEqualTo(expected, 1e-9, $"Unexpected solubility in '{segment}'");
         }
      }

      [Observation]
      public void should_calculate_the_fraction_unbound_in_the_lumen_as_the_ratio_of_aqueous_and_total_solubility()
      {
         foreach (var segment in _lumenSegments)
         {
            var expected = AqueousSolubility(segment).Value / TotalSolubility(segment).Value;
            FractionUnboundLumen(segment).Value.ShouldBeEqualTo(expected, 1e-9, $"Unexpected fraction unbound in '{segment}'");
         }
      }

      [Observation]
      public void should_calculate_a_fraction_unbound_in_the_lumen_between_zero_and_one()
      {
         foreach (var segment in _lumenSegments)
         {
            var fractionUnbound = FractionUnboundLumen(segment).Value;
            fractionUnbound.ShouldBeGreaterThan(0);
            fractionUnbound.ShouldBeSmallerThan(1 + 1e-9);
         }
      }

      [Observation]
      public void should_calculate_a_fraction_unbound_of_one_where_no_micelles_are_formed()
      {
         foreach (var segment in SegmentsWithoutMicellization)
         {
            FractionUnboundLumen(segment).Value.ShouldBeEqualTo(1, 1e-9, $"Unexpected fraction unbound in '{segment}'");
         }
      }

      private double micellizationTermFor(string segment)
      {
         if (UseBileSaltMicellization.Value != 1)
            return 0;

         var bileSaltConcentration = BileSaltConcentration(segment).Value;
         var criticalMicellarConcentration = CriticalMicellarConcentration.Value;
         if (bileSaltConcentration <= criticalMicellarConcentration)
            return 0;

         var pureWaterConcentration = _simulation.Model.Root.Container(Constants.ORGANISM).Parameter(PURE_WATER_CONCENTRATION).Value;
         var neutralCoefficient = CompoundInSimulation.Parameter(CoreConstants.Parameters.BILE_SALT_PARTITION_COEFFICIENT_NEUTRAL).Value;
         var ionizedCoefficient = CompoundInSimulation.Parameter(CoreConstants.Parameters.BILE_SALT_PARTITION_COEFFICIENT_IONIZED).Value;

         var intrinsicSolubility = IntrinsicSolubility.Value;
         var solubilityIncrease = SolubilityIncreaseFromIonization(segment).Value;

         return (bileSaltConcentration - criticalMicellarConcentration) *
                (intrinsicSolubility / pureWaterConcentration * System.Math.Pow(10, neutralCoefficient) +
                 solubilityIncrease / pureWaterConcentration * System.Math.Pow(10, ionizedCoefficient));
      }

      private const string PURE_WATER_CONCENTRATION = "Pure water concentration (37°C)";
   }

   public class When_calculating_the_aqueous_solubility_in_the_intestinal_lumen : concern_for_intestinal_solubility
   {
      [Observation]
      public void should_use_the_pH_of_the_segment_and_not_the_reference_pH_of_the_compound()
      {
         //a basic compound is more soluble at low pH, so the aqueous solubility must decrease with increasing segment pH
         var segmentsByPH = _lumenSegments.OrderBy(segment => SegmentPH(segment).Value).ToList();

         for (var i = 1; i < segmentsByPH.Count; i++)
         {
            var lowerPHSegment = segmentsByPH[i - 1];
            var higherPHSegment = segmentsByPH[i];

            if (SegmentPH(higherPHSegment).Value - SegmentPH(lowerPHSegment).Value < 1e-9)
               continue;

            AqueousSolubility(lowerPHSegment).Value.ShouldBeGreaterThan(AqueousSolubility(higherPHSegment).Value);
         }
      }

   }

   /// <summary>
   ///    All observations of a context share one simulation instance, so every context which changes the pH of a
   ///    lumen segment has to stay in a class of its own.
   /// </summary>
   public class When_raising_the_pH_of_a_single_lumen_segment : concern_for_intestinal_solubility
   {
      private const string _changedSegment = Constants.Compartment.DUODENUM;
      private Dictionary<string, double> _aqueousSolubilityBefore;

      protected override void Because()
      {
         _aqueousSolubilityBefore = _lumenSegments.ToDictionary(segment => segment, segment => AqueousSolubility(segment).Value);
         SegmentPH(_changedSegment).Value += 1;
      }

      [Observation]
      public void should_reduce_the_aqueous_solubility_of_that_segment()
      {
         //a basic compound is less soluble at a higher pH
         AqueousSolubility(_changedSegment).Value.ShouldBeSmallerThan(_aqueousSolubilityBefore[_changedSegment]);
      }

      [Observation]
      public void should_not_change_the_aqueous_solubility_of_any_other_segment()
      {
         foreach (var segment in _lumenSegments.Where(x => x != _changedSegment))
         {
            AqueousSolubility(segment).Value.ShouldBeEqualTo(_aqueousSolubilityBefore[segment], 1e-9,
               $"Aqueous solubility in '{segment}' changed although only the pH of '{_changedSegment}' was changed");
         }
      }
   }

   public class When_setting_the_pH_of_a_lumen_segment_to_the_reference_pH_of_the_compound : concern_for_intestinal_solubility
   {
      private const string _segment = Constants.Compartment.DUODENUM;

      protected override void Because()
      {
         SegmentPH(_segment).Value = CompoundInSimulation.Parameter(CoreConstants.Parameters.REFERENCE_PH).Value;
      }

      [Observation]
      public void should_calculate_an_aqueous_solubility_equal_to_the_solubility_at_reference_pH()
      {
         var solubilityAtReferencePH = CompoundInSimulation.Parameter(CoreConstants.Parameters.SOLUBILITY_AT_REFERENCE_PH).Value;
         AqueousSolubility(_segment).Value.ShouldBeEqualTo(solubilityAtReferencePH, 1e-9);
      }

      [Observation]
      public void should_calculate_a_solubility_increase_from_ionization_of_the_difference_to_the_intrinsic_solubility()
      {
         var expected = AqueousSolubility(_segment).Value - IntrinsicSolubility.Value;
         SolubilityIncreaseFromIonization(_segment).Value.ShouldBeEqualTo(expected, 1e-9);
      }
   }

   public class When_disabling_the_bile_salt_micellization : concern_for_intestinal_solubility
   {
      protected override void Because()
      {
         UseBileSaltMicellization.Value = 0;
      }

      [Observation]
      public void should_reduce_the_solubility_to_the_aqueous_solubility_and_the_solubility_table()
      {
         foreach (var segment in _lumenSegments)
         {
            var expected = AqueousSolubility(segment).Value + SolubilityTable(segment).Value;
            TotalSolubility(segment).Value.ShouldBeEqualTo(expected, 1e-9, $"Unexpected solubility in '{segment}'");
         }
      }

      [Observation]
      public void should_result_in_a_fraction_unbound_of_one_in_every_segment()
      {
         foreach (var segment in _lumenSegments)
         {
            FractionUnboundLumen(segment).Value.ShouldBeEqualTo(1, 1e-9, $"Unexpected fraction unbound in '{segment}'");
         }
      }
   }

   public class When_creating_a_compound_with_the_bile_salt_micellization_switch : concern_for_intestinal_solubility
   {
      [Observation]
      public void should_enable_the_bile_salt_micellization_by_default()
      {
         UseBileSaltMicellization.Value.ShouldBeEqualTo(1);
      }

      [Observation]
      public void should_define_the_switch_as_visible_and_editable_in_the_advanced_solubility_group()
      {
         var parameter = _compound.Parameter(USE_BILE_SALT_MICELLIZATION);
         parameter.ShouldNotBeNull();
         parameter.Visible.ShouldBeTrue();
         parameter.Editable.ShouldBeTrue();
         parameter.GroupName.ShouldBeEqualTo(CoreConstants.Groups.COMPOUND_ADVANCED_SOLUBILITY);
      }
   }

   /// <summary>
   ///    The micellization term uses Max(BS_C - CMC; 0) instead of a conditional on BS_C > CMC so that the solubility is
   ///    continuous at the critical micellar concentration.
   /// </summary>
   //https://github.com/Open-Systems-Pharmacology/PK-Sim/issues/3721
   public class When_varying_the_bile_salt_concentration_around_the_critical_micellar_concentration : concern_for_intestinal_solubility
   {
      private const string _segment = Constants.Compartment.DUODENUM;

      [Observation]
      public void should_calculate_a_continuous_solubility_at_the_critical_micellar_concentration()
      {
         var criticalMicellarConcentration = CriticalMicellarConcentration.Value;

         BileSaltConcentration(_segment).Value = criticalMicellarConcentration * (1 - 1e-6);
         var solubilityBelow = TotalSolubility(_segment).Value;

         BileSaltConcentration(_segment).Value = criticalMicellarConcentration * (1 + 1e-6);
         var solubilityAbove = TotalSolubility(_segment).Value;

         solubilityAbove.ShouldBeEqualTo(solubilityBelow, 1e-6);
      }

      [Observation]
      public void should_equal_the_aqueous_solubility_and_the_solubility_table_at_the_critical_micellar_concentration()
      {
         BileSaltConcentration(_segment).Value = CriticalMicellarConcentration.Value;

         var expected = AqueousSolubility(_segment).Value + SolubilityTable(_segment).Value;
         TotalSolubility(_segment).Value.ShouldBeEqualTo(expected, 1e-9);
      }

      [Observation]
      public void should_not_increase_the_solubility_below_the_critical_micellar_concentration()
      {
         var expected = AqueousSolubility(_segment).Value + SolubilityTable(_segment).Value;

         foreach (var fraction in new[] {0.0, 0.25, 0.5, 0.75, 0.999})
         {
            BileSaltConcentration(_segment).Value = CriticalMicellarConcentration.Value * fraction;
            TotalSolubility(_segment).Value.ShouldBeEqualTo(expected, 1e-9,
               $"Unexpected solubility at {fraction:P0} of the critical micellar concentration");
         }
      }

      [Observation]
      public void should_increase_the_solubility_monotonically_above_the_critical_micellar_concentration()
      {
         var criticalMicellarConcentration = CriticalMicellarConcentration.Value;
         var previousSolubility = 0.0;

         foreach (var factor in new[] {1.0, 1.5, 2.0, 4.0, 8.0})
         {
            BileSaltConcentration(_segment).Value = criticalMicellarConcentration * factor;
            var solubility = TotalSolubility(_segment).Value;

            solubility.ShouldBeGreaterThan(previousSolubility);
            previousSolubility = solubility;
         }
      }
   }

   /// <summary>
   ///    A table solubility is defined as a pH dependent table. In the lumen the parameter 'Solubility table' is a
   ///    <see cref="TableFormulaWithXArgument" /> evaluating that table at the pH of the segment, and the alternative
   ///    used for a table solubility sets the solubility at reference pH to zero. The total solubility of a segment must
   ///    therefore be exactly the tabulated value for the pH of that segment and nothing else.
   /// </summary>
   public abstract class concern_for_intestinal_solubility_defined_as_a_pH_dependent_table : concern_for_intestinal_solubility
   {
      private const string _tableAlternativeName = "pH dependent solubility";

      //one point per whole pH unit over the range covered by the lumen. The values are strictly decreasing so that
      //every segment interpolates a different solubility and a constant table cannot satisfy the observations.
      private static readonly double[] _tablePHValues = {0, 2, 4, 6, 8, 10, 12, 14};
      private static readonly double[] _tableSolubilityValues = {8e-06, 7e-06, 6e-06, 5e-06, 4e-06, 3e-06, 2e-06, 1e-06}; //[kg/l]

      protected TableFormula _solubilityTableFormula;

      protected override void ConfigureSolubilityAlternatives(ParameterAlternativeGroup solubilityAlternativeGroup)
      {
         var tableAlternative = IoC.Resolve<ICompoundAlternativeTask>()
            .CreateSolubilityTableAlternativeFor(solubilityAlternativeGroup, _tableAlternativeName);

         _solubilityTableFormula = tableAlternative.Parameter(CoreConstants.Parameters.SOLUBILITY_TABLE).Formula.DowncastTo<TableFormula>();
         _solubilityTableFormula.ClearPoints();
         for (var i = 0; i < _tablePHValues.Length; i++)
         {
            _solubilityTableFormula.AddPoint(_tablePHValues[i], _tableSolubilityValues[i]);
         }

         //the alternative used to build the simulation is the default alternative of the group
         solubilityAlternativeGroup.AllAlternatives.Each(x => x.IsDefault = false);
         solubilityAlternativeGroup.AddAlternative(tableAlternative);
         tableAlternative.IsDefault = true;
      }

      protected IParameter SolubilityTableOfCompound => CompoundInSimulation.Parameter(CoreConstants.Parameters.SOLUBILITY_TABLE);

      protected double TabulatedSolubilityAt(double pH) => _solubilityTableFormula.ValueAt(pH);
   }

   public class When_calculating_the_solubility_in_the_intestinal_lumen_for_a_pH_dependent_solubility_table : concern_for_intestinal_solubility_defined_as_a_pH_dependent_table
   {
      [Observation]
      public void should_use_the_table_as_the_only_source_of_the_solubility_in_every_segment()
      {
         foreach (var segment in _lumenSegments)
         {
            TotalSolubility(segment).Value.ShouldBeEqualTo(SolubilityTable(segment).Value, 1e-9,
               $"The solubility in '{segment}' is not the value of the solubility table");
         }
      }

      /// <summary>
      ///    Guards the observation above against a table which is constant over all segments and would make the comparison
      ///    of the total solubility with the table value pass for the wrong reason.
      /// </summary>
      [Observation]
      public void should_evaluate_the_table_at_the_pH_of_each_segment()
      {
         foreach (var segment in _lumenSegments)
         {
            var expected = TabulatedSolubilityAt(SegmentPH(segment).Value);
            SolubilityTable(segment).Value.ShouldBeEqualTo(expected, 1e-9,
               $"The solubility table of '{segment}' is not evaluated at the pH of the segment");
         }

         _lumenSegments.Select(segment => SolubilityTable(segment).Value).Distinct().Count().ShouldBeGreaterThan(1);
      }

      [Observation]
      public void should_not_add_any_aqueous_solubility_to_the_tabulated_value()
      {
         IntrinsicSolubility.Value.ShouldBeEqualTo(0);

         foreach (var segment in _lumenSegments)
         {
            AqueousSolubility(segment).Value.ShouldBeEqualTo(0, 1e-9, $"Unexpected aqueous solubility in '{segment}'");
            SolubilityIncreaseFromIonization(segment).Value.ShouldBeEqualTo(0, 1e-9, $"Unexpected solubility increase from ionization in '{segment}'");
         }
      }

      /// <summary>
      ///    The micellization term is proportional to the intrinsic solubility and to the increase from ionization, so it
      ///    vanishes for a table solubility even in the segments which do form micelles.
      /// </summary>
      [Observation]
      public void should_not_add_any_micellization_term_to_the_tabulated_value()
      {
         UseBileSaltMicellization.Value.ShouldBeEqualTo(1);
         SegmentsWithMicellization.Any().ShouldBeTrue("No lumen segment exceeds the critical micellar concentration");

         foreach (var segment in SegmentsWithMicellization)
         {
            TotalSolubility(segment).Value.ShouldBeEqualTo(SolubilityTable(segment).Value, 1e-9,
               $"The micellization term contributes to the solubility in '{segment}'");
         }
      }

      [Observation]
      public void should_define_the_solubility_table_of_the_compound_as_a_table_formula()
      {
         SolubilityTableOfCompound.Formula.IsTable().ShouldBeTrue();
      }
   }

   public class When_changing_the_pH_of_a_lumen_segment_for_a_pH_dependent_solubility_table : concern_for_intestinal_solubility_defined_as_a_pH_dependent_table
   {
      private const string _changedSegment = Constants.Compartment.DUODENUM;
      private const double _newPH = 11;

      protected override void Because()
      {
         SegmentPH(_changedSegment).Value = _newPH;
      }

      [Observation]
      public void should_read_the_solubility_of_that_segment_from_the_table_at_the_new_pH()
      {
         var expected = TabulatedSolubilityAt(_newPH);

         SolubilityTable(_changedSegment).Value.ShouldBeEqualTo(expected, 1e-9);
         TotalSolubility(_changedSegment).Value.ShouldBeEqualTo(expected, 1e-9);
      }

      [Observation]
      public void should_not_change_the_solubility_of_any_other_segment()
      {
         foreach (var segment in _lumenSegments.Where(x => x != _changedSegment))
         {
            var expected = TabulatedSolubilityAt(SegmentPH(segment).Value);
            TotalSolubility(segment).Value.ShouldBeEqualTo(expected, 1e-9, $"Unexpected solubility in '{segment}'");
         }
      }
   }
}
