using System.Threading.Tasks;
using Castle.Core.Internal;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;
using PKSim.Extensions;
using Compound = PKSim.Core.Model.Compound;
using CompoundProperties = PKSim.Core.Model.CompoundProperties;
using Formulation = PKSim.Core.Model.Formulation;
using Protocol = PKSim.Core.Model.Protocol;

namespace PKSim.Core
{
   public abstract class concern_for_CompoundPropertiesMapper : ContextSpecificationAsync<CompoundPropertiesMapper>
   {
      private CalculationMethodCacheMapper _calculationMethodCacheMapper;
      private IBuildingBlockRepository _buildingBlockRepository;
      protected Snapshots.CompoundProperties _snapshot;
      protected CompoundProperties _compoundProperties;
      private CompoundGroupSelection _compoungGroupSelectionOneAlternative;
      protected Compound _compound;
      protected CalculationMethodCache _calculationMethodSnapshot;
      protected EnzymaticProcessSelection _metabolizationEnzymaticPartialProcess;
      protected ProcessSelection _specificBindingPartialProcess;
      protected SystemicProcessSelection _transportSystemicProcess;
      protected Protocol _protocol;
      protected Formulation _formulation;
      private ParameterAlternativeGroup _parameterAlternativeGroupWithOneAlternative;
      private ParameterAlternativeGroup _parameterAlternativeGroupWithTwoAlternatives;
      protected CompoundGroupSelection _compoungGroupSelectioTwoAlternatives;

      protected override Task Context()
      {
         _calculationMethodCacheMapper= A.Fake<CalculationMethodCacheMapper>();
         _buildingBlockRepository= A.Fake<IBuildingBlockRepository>();
         _calculationMethodSnapshot=new CalculationMethodCache();
         sut = new CompoundPropertiesMapper(_calculationMethodCacheMapper,_buildingBlockRepository);

         _compoungGroupSelectionOneAlternative = new CompoundGroupSelection
         {
            AlternativeName = "ALT1",
            GroupName = "ALTERNATIVE_GROUP_1"
         };

         _compoungGroupSelectioTwoAlternatives = new CompoundGroupSelection
         {
            AlternativeName = "ALT2",
            GroupName = "ALTERNATIVE_GROUP_2"
         };

         _compound = new Compound
         {
            Name = "COMP",
         };

         _protocol = new SimpleProtocol
         {
            Name = "PROTOCOL"
         };

         _parameterAlternativeGroupWithOneAlternative = new ParameterAlternativeGroup {Name = _compoungGroupSelectionOneAlternative.GroupName};
         _parameterAlternativeGroupWithTwoAlternatives = new ParameterAlternativeGroup {Name = _compoungGroupSelectioTwoAlternatives.GroupName};

         _parameterAlternativeGroupWithTwoAlternatives.AddAlternative(new ParameterAlternative{Name = "ALT1"});
         _parameterAlternativeGroupWithTwoAlternatives.AddAlternative(new ParameterAlternative{Name = "ALT2"});

         _compound.AddParameterAlternativeGroup(_parameterAlternativeGroupWithOneAlternative);
         _compound.AddParameterAlternativeGroup(_parameterAlternativeGroupWithTwoAlternatives);

         _compoundProperties = new CompoundProperties();

         _compoundProperties.AddCompoundGroupSelection(_compoungGroupSelectionOneAlternative);
         _compoundProperties.AddCompoundGroupSelection(_compoungGroupSelectioTwoAlternatives);
         _compoundProperties.Compound = _compound;

         _metabolizationEnzymaticPartialProcess = new EnzymaticProcessSelection
         {
            CompoundName = _compound.Name,
            MetaboliteName = "META",
            MoleculeName = "CYP",
            ProcessName = "MetaProcess"
         };

         _specificBindingPartialProcess = new ProcessSelection
         {
            CompoundName = _compound.Name,
            MoleculeName = "BINDER",
            ProcessName = "BindingProcess"
         };

         _transportSystemicProcess = new SystemicProcessSelection
         {
            CompoundName = _compound.Name,
            ProcessName = "SystemicTransport",
            ProcessType = SystemicProcessTypes.Biliary
         };

         _compoundProperties.Processes.MetabolizationSelection.AddPartialProcessSelection(_metabolizationEnzymaticPartialProcess);
         _compoundProperties.Processes.SpecificBindingSelection.AddPartialProcessSelection(_specificBindingPartialProcess);
         _compoundProperties.Processes.TransportAndExcretionSelection.AddSystemicProcessSelection(_transportSystemicProcess);


         _formulation = new Model.Formulation
         {
            Id = "123456"
         };
         _compoundProperties.ProtocolProperties.Protocol = _protocol;
         _compoundProperties.ProtocolProperties.AddFormulationMapping(new FormulationMapping
         {
            FormulationKey = "F1",
            TemplateFormulationId = _formulation.Id
         });

         A.CallTo(() => _buildingBlockRepository.ById(_formulation.Id)).Returns(_formulation);
         A.CallTo(() => _calculationMethodCacheMapper.MapToSnapshot(_compoundProperties.CalculationMethodCache)).ReturnsAsync(_calculationMethodSnapshot);

         return Task.FromResult(true);
      }
   }

   public class When_mapping_compound_properties_to_snapshot : concern_for_CompoundPropertiesMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_compoundProperties);
      }

      [Observation]
      public void should_return_a_snapshot_having_the_expected_properties()
      {
         _snapshot.Name.ShouldBeEqualTo(_compound.Name);
      }

      [Observation]
      public void should_save_the_calculation_methods_to_snapshot()
      {
         _snapshot.CalculationMethods.ShouldBeEqualTo(_calculationMethodSnapshot);
      }

      [Observation]
      public void should_save_the_protocol_selection_to_snapshot()
      {
         _snapshot.Protocol.Name.ShouldBeEqualTo(_protocol.Name);
         _snapshot.Protocol.Formulations.Length.ShouldBeEqualTo(1);
         _snapshot.Protocol.Formulations[0].Key.ShouldBeEqualTo("F1");
         _snapshot.Protocol.Formulations[0].Name.ShouldBeEqualTo(_formulation.Name);
      }

      [Observation]
      public void should_save_the_used_processes_to_snapshot()
      {
         _snapshot.Processes.Length.ShouldBeEqualTo(3);
         var snapshotProcess= _snapshot.Processes.Find(x => x.Name == _metabolizationEnzymaticPartialProcess.ProcessName);
         snapshotProcess.MoleculeName.ShouldBeEqualTo(_metabolizationEnzymaticPartialProcess.MoleculeName);
         snapshotProcess.MetaboliteName.ShouldBeEqualTo(_metabolizationEnzymaticPartialProcess.MetaboliteName);

         snapshotProcess = _snapshot.Processes.Find(x => x.Name == _specificBindingPartialProcess.ProcessName);
         snapshotProcess.MoleculeName.ShouldBeEqualTo(_specificBindingPartialProcess.MoleculeName);
         snapshotProcess.MetaboliteName.ShouldBeNull();

         snapshotProcess = _snapshot.Processes.Find(x => x.Name == _transportSystemicProcess.ProcessName);
         snapshotProcess.MoleculeName.ShouldBeNull();
         snapshotProcess.MetaboliteName.ShouldBeNull();
      }

      [Observation]
      public void should_save_alternatives_with_at_least_two_alternatives()
      {
         _snapshot.Alternatives.Length.ShouldBeEqualTo(1);
         _snapshot.Alternatives[0].AlternativeName.ShouldBeEqualTo(_compoungGroupSelectioTwoAlternatives.AlternativeName);
         _snapshot.Alternatives[0].GroupName.ShouldBeEqualTo(_compoungGroupSelectioTwoAlternatives.GroupName);
      }
   }
}	