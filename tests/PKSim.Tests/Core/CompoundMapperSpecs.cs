using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;
using CalculationMethodCache = PKSim.Core.Snapshots.CalculationMethodCache;
using Compound = PKSim.Core.Snapshots.Compound;
using CompoundProcess = PKSim.Core.Snapshots.CompoundProcess;

namespace PKSim.Core
{
   public abstract class concern_for_CompoundMapper : ContextSpecificationAsync<CompoundMapper>
   {
      protected AlternativeMapper _alternativeMapper;
      protected ParameterMapper _parameterMapper;
      protected Compound _snapshot;
      protected Model.Compound _compound;
      protected CalculationMethodCacheMapper _calculationMethodCacheMapper;
      protected CalculationMethodCache _calculationMethodCacheSnapshot;
      protected CompoundProcessMapper _processMapper;
      protected CompoundProcess _snapshotProcess1;
      protected CompoundProcess _snapshotProcess2;
      protected EnzymaticProcess _partialProcess;
      private SystemicProcess _systemicProcess;
      protected ICompoundFactory _compoundFactory;

      protected override Task Context()
      {
         _alternativeMapper = A.Fake<AlternativeMapper>();
         _parameterMapper = A.Fake<ParameterMapper>();
         _calculationMethodCacheMapper = A.Fake<CalculationMethodCacheMapper>();
         _processMapper = A.Fake<CompoundProcessMapper>();
         _compoundFactory = A.Fake<ICompoundFactory>();
         sut = new CompoundMapper(_parameterMapper, _alternativeMapper, _calculationMethodCacheMapper, _processMapper, _compoundFactory);

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
         _compound.Add(DomainHelperForSpecs.ConstantParameterWithValue((int) PlasmaProteinBindingPartner.Glycoprotein).WithName(CoreConstants.Parameter.PLASMA_PROTEIN_BINDING_PARTNER));

         _partialProcess = new EnzymaticProcess().WithName("EnzymaticProcess");
         _systemicProcess = new SystemicProcess().WithName("SystemicProcess");
         _compound.AddProcess(_partialProcess);
         _compound.AddProcess(_systemicProcess);

         _calculationMethodCacheSnapshot = new CalculationMethodCache();
         A.CallTo(() => _calculationMethodCacheMapper.MapToSnapshot(_compound.CalculationMethodCache)).Returns(_calculationMethodCacheSnapshot);

         _snapshotProcess1 = new CompoundProcess();
         _snapshotProcess2 = new CompoundProcess();
         A.CallTo(() => _processMapper.MapToSnapshot(_partialProcess)).Returns(_snapshotProcess1);
         A.CallTo(() => _processMapper.MapToSnapshot(_systemicProcess)).Returns(_snapshotProcess2);

         return _completed;
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
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_compound);
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
         _snapshot.CalculationMethods.ShouldBeEqualTo(_calculationMethodCacheSnapshot);
      }

      [Observation]
      public void should_have_saved_the_is_small_molecule_flag_when_different_from_the_default_value_true()
      {
         _snapshot.IsSmallMolecule.ShouldBeNull();
      }

      [Observation]
      public void should_have_saved_the_plasma_protein_binding_partner()
      {
         _snapshot.PlasmaProteinBindingPartner.ShouldBeEqualTo(_compound.PlasmaProteinBindingPartner);
      }

      [Observation]
      public void should_have_saved_the_processes()
      {
         _snapshot.Processes.ShouldOnlyContain(_snapshotProcess1, _snapshotProcess2);
      }

      [Observation]
      public void should_have_saved_the_defined_alternative_into_the_snapshot()
      {
         _snapshot.Lipophilicity.Length.ShouldBeEqualTo(1);
         _snapshot.FractionUnbound.Length.ShouldBeEqualTo(1);
         _snapshot.Solubility.Length.ShouldBeEqualTo(1);
         _snapshot.IntestinalPermeability.Length.ShouldBeEqualTo(1);
         _snapshot.Permeability.Length.ShouldBeEqualTo(1);
      }

      [Observation]
      public void should_export_all_pka_type_values_that_are_not_neutral()
      {
         _snapshot.PkaTypes.Length.ShouldBeEqualTo(2);
         _snapshot.PkaTypes[0].Pka.ShouldBeEqualTo(8);
         _snapshot.PkaTypes[0].Type.ShouldBeEqualTo(CompoundType.Base);

         _snapshot.PkaTypes[1].Pka.ShouldBeEqualTo(4);
         _snapshot.PkaTypes[1].Type.ShouldBeEqualTo(CompoundType.Acid);
      }
   }

   public class When_mapping_a_valid_compound_snapshot_to_a_compound : concern_for_CompoundMapper
   {
      private Model.Compound _newCompound;
      private ParameterAlternative _alternative;
      private Model.CompoundProcess _newProcess;
      private ParameterAlternativeGroup _fractionUnboundParameterGroup;

      protected override async Task Context()
      {
         await base.Context();
         A.CallTo(() => _compoundFactory.Create()).Returns(_compound);
         clearCompound();

         _snapshot = await sut.MapToSnapshot(_compound);
         _snapshot.PlasmaProteinBindingPartner = PlasmaProteinBindingPartner.Albumin;
         _snapshot.IsSmallMolecule = false;
         _snapshot.PkaTypes = new[]
         {
            new PkaType {Pka = 1, Type = CompoundType.Acid},
            new PkaType {Pka = 2, Type = CompoundType.Base},
            new PkaType {Pka = 3, Type = CompoundType.Acid},
         };

         _alternative = new ParameterAlternative().WithName("Alternative");
         _fractionUnboundParameterGroup = _compound.ParameterAlternativeGroup(CoreConstants.Groups.COMPOUND_FRACTION_UNBOUND);
         A.CallTo(() => _alternativeMapper.MapToModel(_snapshot.FractionUnbound[0], _fractionUnboundParameterGroup)).Returns(_alternative);

         _snapshot.Processes = new[] {_snapshotProcess1};
         _newProcess = new EnzymaticProcess();
         A.CallTo(() => _processMapper.MapToModel(_snapshotProcess1)).Returns(_newProcess);
      }

      private void clearCompound()
      {
         _compound.AllProcesses().ToList().Each(_compound.RemoveProcess);
      }

      protected override async Task Because()
      {
         _newCompound = await sut.MapToModel(_snapshot);
      }

      [Observation]
      public void should_have_created_an_compound_with_the_expected_properties()
      {
         _newCompound.Name.ShouldBeEqualTo(_snapshot.Name);
         _newCompound.Description.ShouldBeEqualTo(_snapshot.Description);
      }

      [Observation]
      public void should_load_the_calculation_methods()
      {
         A.CallTo(() => _calculationMethodCacheMapper.UpdateCalculationMethodCache(_newCompound, _snapshot.CalculationMethods)).MustHaveHappened();
      }

      [Observation]
      public void should_load_the_is_small_molecule_flag()
      {
         _newCompound.IsSmallMolecule.ShouldBeFalse();
      }

      [Observation]
      public void should_load_the_binding_partner()
      {
         _newCompound.PlasmaProteinBindingPartner.ShouldBeEqualTo(PlasmaProteinBindingPartner.Albumin);
      }

      [Observation]
      public void should_have_created_the_expected_processes()
      {
         _newCompound.AllProcesses().ShouldOnlyContain(_newProcess);
      }

      [Observation]
      public void should_have_created_the_expected_alternatives()
      {
         _newCompound.ParameterAlternativeGroup(CoreConstants.Groups.COMPOUND_FRACTION_UNBOUND).AllAlternatives.ShouldContain(_alternative);
      }

      [Observation]
      public void should_have_loaded_the_expected_pka_type_values()
      {
         _newCompound.Parameter(CoreConstants.Parameter.PARAMETER_PKA1).Value.ShouldBeEqualTo(_snapshot.PkaTypes[0].Pka);
         _newCompound.Parameter(CoreConstants.Parameter.PARAMETER_PKA2).Value.ShouldBeEqualTo(_snapshot.PkaTypes[1].Pka);
         _newCompound.Parameter(CoreConstants.Parameter.PARAMETER_PKA3).Value.ShouldBeEqualTo(_snapshot.PkaTypes[2].Pka);

         _newCompound.Parameter(CoreConstants.Parameter.COMPOUND_TYPE1).Value.ShouldBeEqualTo((int) _snapshot.PkaTypes[0].Type);
         _newCompound.Parameter(CoreConstants.Parameter.COMPOUND_TYPE2).Value.ShouldBeEqualTo((int) _snapshot.PkaTypes[1].Type);
         _newCompound.Parameter(CoreConstants.Parameter.COMPOUND_TYPE3).Value.ShouldBeEqualTo((int) _snapshot.PkaTypes[2].Type);
      }
   }
}