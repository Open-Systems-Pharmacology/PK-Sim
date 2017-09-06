using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Snapshots.Mappers;
using CalculationMethod = PKSim.Core.Snapshots.CalculationMethod;
using Compound = PKSim.Core.Snapshots.Compound;
using CompoundProcess = PKSim.Core.Snapshots.CompoundProcess;

namespace PKSim.Core
{
   public abstract class concern_for_CompoundMapper : ContextSpecification<CompoundMapper>
   {
      protected AlternativeMapper _alernativeMapper;
      protected ParameterMapper _parameterMapper;
      protected Compound _snapshot;
      protected Model.Compound _compound;
      private CalculationMethodMapper _calculationMethodMapper;
      protected List<CalculationMethod> _calculationMethodsSnapshot;
      protected CompoundProcessMapper _processMapper;
      protected CompoundProcess _snapshotProcess1;
      protected CompoundProcess _snapshotProcess2;
      protected EnzymaticProcess _partialProcess;
      private SystemicProcess _systemicProcess;

      protected override void Context()
      {
         _alernativeMapper = A.Fake<AlternativeMapper>();
         _parameterMapper = A.Fake<ParameterMapper>();
         _calculationMethodMapper = A.Fake<CalculationMethodMapper>();
         _processMapper = A.Fake<CompoundProcessMapper>();
         sut = new CompoundMapper(_parameterMapper, _alernativeMapper, _calculationMethodMapper, _processMapper);

         _compound = new Model.Compound
         {
            Name = "Compound",
            Description = "Description"
         };

         addPkAParameters(_compound, 0, 8, CompoundType.Base);
         addPkAParameters(_compound, 1, 4, CompoundType.Acid);
         addPkAParameters(_compound, 2, 7, CompoundType.Neutral);

         _compound.AddParameterAlternativeGroup(createParameterAlternativeGroup(CoreConstants.Groups.COMPOUND_LIPOPHILICITY));
         _compound.AddParameterAlternativeGroup(createParameterAlternativeGroup(CoreConstants.Groups.COMPOUND_FRACTION_UNBOUND));
         _compound.AddParameterAlternativeGroup(createParameterAlternativeGroup(CoreConstants.Groups.COMPOUND_SOLUBILITY));
         _compound.AddParameterAlternativeGroup(createParameterAlternativeGroup(CoreConstants.Groups.COMPOUND_INTESTINAL_PERMEABILITY));
         _compound.AddParameterAlternativeGroup(createParameterAlternativeGroup(CoreConstants.Groups.COMPOUND_PERMEABILITY));

         _compound.Add(DomainHelperForSpecs.ConstantParameterWithValue(1).WithName(CoreConstants.Parameter.IS_SMALL_MOLECULE));
         _compound.Add(DomainHelperForSpecs.ConstantParameterWithValue((int)PlasmaProteinBindingPartner.Glycoprotein).WithName(CoreConstants.Parameter.PLASMA_PROTEIN_BINDING_PARTNER));

         _partialProcess = new EnzymaticProcess().WithName("EnzymaticProcess");
         _systemicProcess = new SystemicProcess().WithName("SystemicProcess");
         _compound.AddProcess(_partialProcess);
         _compound.AddProcess(_systemicProcess);

         _calculationMethodsSnapshot = new List<CalculationMethod>();
         A.CallTo(() => _calculationMethodMapper.MapToSnapshot(_compound.CalculationMethodCache)).Returns(_calculationMethodsSnapshot);

         _snapshotProcess1= new CompoundProcess();
         _snapshotProcess2 = new CompoundProcess();
         A.CallTo(() => _processMapper.MapToSnapshot(_partialProcess)).Returns(_snapshotProcess1);
         A.CallTo(() => _processMapper.MapToSnapshot(_systemicProcess)).Returns(_snapshotProcess2);
      }

      private void addPkAParameters(Model.Compound compound, int index, double pkA, CompoundType compoundType)
      {
         compound.Add(DomainHelperForSpecs.ConstantParameterWithValue(pkA).WithName(CoreConstants.Parameter.ParameterPKa(index)));
         compound.Add(DomainHelperForSpecs.ConstantParameterWithValue((int) compoundType).WithName(CoreConstants.Parameter.ParameterCompoundType(index)));
      }

      private ParameterAlternativeGroup createParameterAlternativeGroup(string parameterAlternativeGroupName)
      {
         return new ParameterAlternativeGroup
         {
            new ParameterAlternative
            {
               Name = parameterAlternativeGroupName
            }
         }.WithName(parameterAlternativeGroupName);
      }
   }

   public class When_mapping_a_compound_to_snapshot : concern_for_CompoundMapper
   {
      protected override void Because()
      {
         _snapshot = sut.MapToSnapshot(_compound);
      }

      [Observation]
      public void should_save_the_compound_properties()
      {
         _snapshot.Name.ShouldBeEqualTo(_compound.Name);
         _snapshot.Description.ShouldBeEqualTo(_compound.Description);
      }

      [Observation]
      public void should_save_the_calculation_methods_used_in_the_compound()
      {
         _snapshot.CalculationMethods.ShouldBeEqualTo(_calculationMethodsSnapshot);
      }

      [Observation]
      public void should_have_saved_the_is_small_molecule_flag()
      {
         _snapshot.IsSmallMolecule.ShouldBeTrue();
      }

      [Observation]
      public void should_have_saved_the_plasma_protein_binding_partner()
      {
         _snapshot.PlasmaProteinBindingPartner.ShouldBeEqualTo(_compound.PlasmaProteinBindingPartner.ToString());
      }

      [Observation]
      public void should_have_saved_the_processes()
      {
         _snapshot.Processes.ShouldOnlyContain(_snapshotProcess1, _snapshotProcess2);
      }

      [Observation]
      public void should_have_saved_the_defined_alternative_into_the_snapshot()
      {
         _snapshot.Lipophilicity.Count.ShouldBeEqualTo(1);
         _snapshot.FractionUnbound.Count.ShouldBeEqualTo(1);
         _snapshot.Solubility.Count.ShouldBeEqualTo(1);
         _snapshot.IntestinalPermeability.Count.ShouldBeEqualTo(1);
         _snapshot.Permeability.Count.ShouldBeEqualTo(1);
      }

      [Observation]
      public void should_export_all_pka_type_values_that_are_not_neutral()
      {
         _snapshot.PkaTypes.Count.ShouldBeEqualTo(2);
         _snapshot.PkaTypes[0].Pka.ShouldBeEqualTo(8);
         _snapshot.PkaTypes[0].Type.ShouldBeEqualTo(CompoundType.Base.ToString());

         _snapshot.PkaTypes[1].Pka.ShouldBeEqualTo(4);
         _snapshot.PkaTypes[1].Type.ShouldBeEqualTo(CompoundType.Acid.ToString());
      }
   }
}