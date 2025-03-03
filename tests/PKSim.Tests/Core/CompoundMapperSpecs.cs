using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;
using CalculationMethodCache = PKSim.Core.Snapshots.CalculationMethodCache;
using Compound = PKSim.Core.Snapshots.Compound;
using CompoundProcess = PKSim.Core.Snapshots.CompoundProcess;
using ValueOrigin = PKSim.Core.Snapshots.ValueOrigin;

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
      private ParameterAlternativeGroup _compoundIntestinalPermeabilityAlternativeGroup;
      private ParameterAlternative _calculatedAlternative;
      protected ValueOriginMapper _valueOriginMapper;
      protected ValueOrigin _snapshotValueOrigin;
      protected OSPSuite.Core.Domain.ValueOrigin _pkaValueOrigin;
      private IParameter _molweightParameter;
      private IParameter _halogenFParameter;

      protected override Task Context()
      {
         _parameterMapper = A.Fake<ParameterMapper>();
         _alternativeMapper = A.Fake<AlternativeMapper>();
         _calculationMethodCacheMapper = A.Fake<CalculationMethodCacheMapper>();
         _processMapper = A.Fake<CompoundProcessMapper>();
         _compoundFactory = A.Fake<ICompoundFactory>();
         _valueOriginMapper = A.Fake<ValueOriginMapper>();
         sut = new CompoundMapper(_parameterMapper, _alternativeMapper, _calculationMethodCacheMapper, _processMapper, _valueOriginMapper, _compoundFactory);

         _compound = new Model.Compound
         {
            Name = "Compound",
            Description = "Description"
         };
         _pkaValueOrigin = new OSPSuite.Core.Domain.ValueOrigin { Method = ValueOriginDeterminationMethods.InVitro, Description = "PKA" };
         _snapshotValueOrigin = new ValueOrigin { Method = ValueOriginDeterminationMethodId.InVivo, Description = "PKA" };

         addPkAParameters(_compound, 0, 8, CompoundType.Base);
         addPkAParameters(_compound, 1, 4, CompoundType.Acid);
         addPkAParameters(_compound, 2, 7, CompoundType.Neutral);

         _compoundIntestinalPermeabilityAlternativeGroup = createParameterAlternativeGroup(CoreConstants.Groups.COMPOUND_INTESTINAL_PERMEABILITY);
         _compound.AddParameterAlternativeGroup(createParameterAlternativeGroup(CoreConstants.Groups.COMPOUND_LIPOPHILICITY));
         _compound.AddParameterAlternativeGroup(createParameterAlternativeGroup(CoreConstants.Groups.COMPOUND_FRACTION_UNBOUND));
         _compound.AddParameterAlternativeGroup(createParameterAlternativeGroup(CoreConstants.Groups.COMPOUND_SOLUBILITY));
         _compound.AddParameterAlternativeGroup(createParameterAlternativeGroup(CoreConstants.Groups.COMPOUND_PERMEABILITY));
         _compound.AddParameterAlternativeGroup(_compoundIntestinalPermeabilityAlternativeGroup);

         _compoundIntestinalPermeabilityAlternativeGroup.DefaultAlternative.IsDefault = true;
         //Calculated alternative will not be the default alternative for intestinal perm
         _calculatedAlternative = new ParameterAlternative { Name = PKSimConstants.UI.CalculatedAlernative, IsDefault = false };
         _compoundIntestinalPermeabilityAlternativeGroup.AddAlternative(_calculatedAlternative);
         //Mapping of a calculated alternative returns null
         A.CallTo(() => _alternativeMapper.MapToSnapshot(_calculatedAlternative)).Returns(Task.FromResult<Alternative>(null));

         _compound.Add(DomainHelperForSpecs.ConstantParameterWithValue(1).WithName(Constants.Parameters.IS_SMALL_MOLECULE));
         _compound.Add(DomainHelperForSpecs.ConstantParameterWithValue((int)PlasmaProteinBindingPartner.Glycoprotein).WithName(Constants.Parameters.PLASMA_PROTEIN_BINDING_PARTNER));

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

         _molweightParameter = DomainHelperForSpecs.ConstantParameterWithValue(400).WithName(Constants.Parameters.MOL_WEIGHT);
         _molweightParameter.ValueOrigin.Method = ValueOriginDeterminationMethods.InVivo;

         //Do not update F value origin to ensure that it's being synchronized when mapping from snapshot
         _halogenFParameter = DomainHelperForSpecs.ConstantParameterWithValue(5).WithName(Constants.Parameters.F);

         _compound.Add(_molweightParameter);
         _compound.Add(_halogenFParameter);
         return _completed;
      }

      private void addPkAParameters(Model.Compound compound, int index, double pkA, CompoundType compoundType)
      {
         var pkaParameter = DomainHelperForSpecs.ConstantParameterWithValue(pkA).WithName(CoreConstants.Parameters.ParameterPKa(index));
         pkaParameter.ValueOrigin.UpdateFrom(_pkaValueOrigin);
         A.CallTo(() => _valueOriginMapper.MapToSnapshot(pkaParameter.ValueOrigin)).Returns(_snapshotValueOrigin);
         compound.Add(pkaParameter);
         var compoundTypeParameter = DomainHelperForSpecs.ConstantParameterWithValue((int)compoundType).WithName(Constants.Parameters.ParameterCompoundType(index));
         compoundTypeParameter.ValueOrigin.UpdateFrom(_pkaValueOrigin);
         A.CallTo(() => _valueOriginMapper.MapToSnapshot(compoundTypeParameter.ValueOrigin)).Returns(_snapshotValueOrigin);
         compound.Add(compoundTypeParameter);
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
      public void should_have_saved_the_is_small_molecule_flag_even_for_a_small_molecule()
      {
         _snapshot.IsSmallMolecule.ShouldBeEqualTo(true);
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
         _snapshot.PkaTypes[0].ValueOrigin.ShouldBeEqualTo(_snapshotValueOrigin);

         _snapshot.PkaTypes[1].Pka.ShouldBeEqualTo(4);
         _snapshot.PkaTypes[1].Type.ShouldBeEqualTo(CompoundType.Acid);
         _snapshot.PkaTypes[1].ValueOrigin.ShouldBeEqualTo(_snapshotValueOrigin);
      }
   }

   public class When_mapping_a_valid_compound_snapshot_to_a_compound : concern_for_CompoundMapper
   {
      private Model.Compound _newCompound;
      private ParameterAlternative _fractionUnboundAlternative;
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
            new PkaType { Pka = 1, Type = CompoundType.Acid, ValueOrigin = _snapshotValueOrigin },
            new PkaType { Pka = 2, Type = CompoundType.Base, ValueOrigin = _snapshotValueOrigin },
            new PkaType { Pka = 3, Type = CompoundType.Acid, ValueOrigin = _snapshotValueOrigin },
         };

         _fractionUnboundAlternative = new ParameterAlternative().WithName("Alternative");
         _fractionUnboundParameterGroup = _compound.ParameterAlternativeGroup(CoreConstants.Groups.COMPOUND_FRACTION_UNBOUND);
         A.CallTo(() => _alternativeMapper.MapToModel(_snapshot.FractionUnbound[0], A<AlternativeMapperSnapshotContext>.That.Matches(x => x.ParameterAlternativeGroup == _fractionUnboundParameterGroup)))
            .Returns(_fractionUnboundAlternative);

         _snapshot.Processes = new[] { _snapshotProcess1 };
         _newProcess = new EnzymaticProcess();
         A.CallTo(() => _processMapper.MapToModel(_snapshotProcess1, A<SnapshotContext>._)).Returns(_newProcess);
      }

      private void clearCompound()
      {
         _compound.AllProcesses().ToList().Each(_compound.RemoveProcess);
      }

      protected override async Task Because()
      {
         _newCompound = await sut.MapToModel(_snapshot, new SnapshotContext());
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
         _newCompound.ParameterAlternativeGroup(CoreConstants.Groups.COMPOUND_FRACTION_UNBOUND).AllAlternatives.ShouldContain(_fractionUnboundAlternative);
      }

      [Observation]
      public void should_have_set_the_alternative_as_default_alternative()
      {
         _fractionUnboundAlternative.IsDefault.ShouldBeTrue();
         _newCompound.ParameterAlternativeGroup(CoreConstants.Groups.COMPOUND_INTESTINAL_PERMEABILITY).DefaultAlternative.IsDefault.ShouldBeTrue();
      }

      [Observation]
      public void should_have_loaded_the_expected_pka_type_values()
      {
         _newCompound.Parameter(CoreConstants.Parameters.PARAMETER_PKA1).Value.ShouldBeEqualTo(_snapshot.PkaTypes[0].Pka);
         _newCompound.Parameter(CoreConstants.Parameters.PARAMETER_PKA1).ValueOrigin.ShouldBeEqualTo(_pkaValueOrigin);

         _newCompound.Parameter(CoreConstants.Parameters.PARAMETER_PKA2).Value.ShouldBeEqualTo(_snapshot.PkaTypes[1].Pka);
         _newCompound.Parameter(CoreConstants.Parameters.PARAMETER_PKA2).ValueOrigin.ShouldBeEqualTo(_pkaValueOrigin);

         _newCompound.Parameter(CoreConstants.Parameters.PARAMETER_PKA3).Value.ShouldBeEqualTo(_snapshot.PkaTypes[2].Pka);
         _newCompound.Parameter(CoreConstants.Parameters.PARAMETER_PKA3).ValueOrigin.ShouldBeEqualTo(_pkaValueOrigin);

         _newCompound.Parameter(Constants.Parameters.COMPOUND_TYPE1).Value.ShouldBeEqualTo((int)_snapshot.PkaTypes[0].Type);
         _newCompound.Parameter(Constants.Parameters.COMPOUND_TYPE1).ValueOrigin.ShouldBeEqualTo(_pkaValueOrigin);

         _newCompound.Parameter(Constants.Parameters.COMPOUND_TYPE2).Value.ShouldBeEqualTo((int)_snapshot.PkaTypes[1].Type);
         _newCompound.Parameter(Constants.Parameters.COMPOUND_TYPE2).ValueOrigin.ShouldBeEqualTo(_pkaValueOrigin);

         _newCompound.Parameter(Constants.Parameters.COMPOUND_TYPE3).Value.ShouldBeEqualTo((int)_snapshot.PkaTypes[2].Type);
         _newCompound.Parameter(Constants.Parameters.COMPOUND_TYPE3).ValueOrigin.ShouldBeEqualTo(_pkaValueOrigin);
      }

      [Observation]
      public void should_have_ensured_that_the_mol_weight_and_halogen_parameters_share_the_same_value_origin()
      {
         _fractionUnboundAlternative.IsDefault.ShouldBeTrue();
         var molWeight = _newCompound.Parameter(Constants.Parameters.MOL_WEIGHT);
         var F = _newCompound.Parameter(Constants.Parameters.F);

         F.ValueOrigin.ShouldBeEqualTo(molWeight.ValueOrigin);
      }
   }

   public class ParameterAlternativeEqualityComparer : GenericEqualityComparer<ParameterAlternative>
   {
   }

   public class SystemicProcessEqualityComparer : GenericEqualityComparer<SystemicProcess>
   {
   }
}