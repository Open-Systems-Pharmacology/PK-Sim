using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Services;
using PKSim.Core.Repositories;
using PKSim.Core.Snapshots.Mappers;
using ExplicitFormula = PKSim.Core.Snapshots.ExplicitFormula;
using MoleculeList = PKSim.Core.Snapshots.MoleculeList;
using Observer = PKSim.Core.Snapshots.Observer;

namespace PKSim.Core
{
   public abstract class concern_for_ObserverMapper : ContextSpecificationAsync<ObserverMapper>
   {
      private DescriptorConditionMapper _descriptorConditionMapper;
      private ExplicitFormulaMapper _explicitFormulaMapper;
      private MoleculeListMapper _moleculeListMapper;
      protected IObserverBuilder _amountObserver;
      protected ContainerObserverBuilder _containerObserver;
      protected IDimension _dimension;
      private OSPSuite.Core.Domain.Formulas.ExplicitFormula _formula;
      protected ExplicitFormula _snapshotFormula;
      protected MoleculeList _moleculeListSnapshot;
      protected IObjectBaseFactory _objectBaseFactory;
      protected IDimensionRepository _dimensionRepository;
      protected IOSPSuiteLogger _logger;
      protected OSPSuite.Core.Domain.Formulas.ExplicitFormula _newFormula;
      protected OSPSuite.Core.Domain.Builder.MoleculeList _newMoleculeList;

      protected override Task Context()
      {
         _descriptorConditionMapper= A.Fake<DescriptorConditionMapper>();
         _explicitFormulaMapper= A.Fake<ExplicitFormulaMapper>(); 
         _moleculeListMapper= A.Fake<MoleculeListMapper>();
         _objectBaseFactory= A.Fake<IObjectBaseFactory>();
         _dimensionRepository= A.Fake<IDimensionRepository>();
         _logger= A.Fake<IOSPSuiteLogger>();   

         _dimension = DomainHelperForSpecs.TimeDimensionForSpecs();

         _amountObserver = new AmountObserverBuilder {Dimension = _dimension};
         _containerObserver = new ContainerObserverBuilder { Dimension = _dimension };
         _formula = new OSPSuite.Core.Domain.Formulas.ExplicitFormula("1+2");
         _amountObserver.Formula = _formula;
         _containerObserver.Formula = _formula;

         _moleculeListSnapshot=new MoleculeList();
         _snapshotFormula = new ExplicitFormula();

         _newMoleculeList = new OSPSuite.Core.Domain.Builder.MoleculeList();
         _newMoleculeList.AddMoleculeNameToExclude("A");
         _newMoleculeList.AddMoleculeName("B");
         _newMoleculeList.ForAll = false;
         _newFormula =new OSPSuite.Core.Domain.Formulas.ExplicitFormula("New");

         sut = new ObserverMapper(
            _descriptorConditionMapper,
            _explicitFormulaMapper, 
            _moleculeListMapper,
            _objectBaseFactory,
            _dimensionRepository,
            _logger);

         A.CallTo(() => _explicitFormulaMapper.MapToSnapshot(_formula)).Returns(_snapshotFormula);
         A.CallTo(() => _explicitFormulaMapper.MapToModel(_snapshotFormula, A<SnapshotContext>._)).Returns(_newFormula);
         A.CallTo(() => _moleculeListMapper.MapToSnapshot(_amountObserver.MoleculeList)).Returns(_moleculeListSnapshot);
         A.CallTo(() => _moleculeListMapper.MapToSnapshot(_containerObserver.MoleculeList)).Returns(_moleculeListSnapshot);
         A.CallTo(() => _moleculeListMapper.MapToModel(_moleculeListSnapshot, A<SnapshotContext>._)).Returns(_newMoleculeList);

         A.CallTo(() => _dimensionRepository.DimensionByName(_dimension.Name)).Returns(_dimension);
         A.CallTo(() => _objectBaseFactory.Create<AmountObserverBuilder>()).Returns(new AmountObserverBuilder());
         A.CallTo(() => _objectBaseFactory.Create<ContainerObserverBuilder>()).Returns(new ContainerObserverBuilder());

         return _completed;
      }
   }

   public class When_mapping_a_container_observer_to_snapshot : concern_for_ObserverMapper
   {
      private Observer _result;

      protected override async Task Because()
      {
         _result = await  sut.MapToSnapshot(_containerObserver);
      }

      [Observation]
      public void should_return_a_valid_snapshot()
      {
         _result.Type.ShouldBeEqualTo("Container");
         _result.Dimension.ShouldBeEqualTo(_dimension.Name);
         _result.Formula.ShouldBeEqualTo(_snapshotFormula);
         _result.MoleculeList.ShouldBeEqualTo(_moleculeListSnapshot);
      }
   }

   public class When_mapping_a_container_observer_snapshot_to_observer : concern_for_ObserverMapper
   {
      private Observer _snapshot;
      private IObserverBuilder _result;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_containerObserver);
      }


      protected override async Task Because()
      {
         _result = await sut.MapToModel(_snapshot, new SnapshotContext());
      }

      [Observation]
      public void should_return_a_valid_model()
      {
         _result.ShouldBeAnInstanceOf<ContainerObserverBuilder>();
         _result.Dimension.ShouldBeEqualTo(_dimension);
         _result.Formula.ShouldBeEqualTo(_newFormula);
         _result.MoleculeList.ForAll.ShouldBeEqualTo(_newMoleculeList.ForAll);
         _result.MoleculeList.MoleculeNames.ShouldOnlyContain(_newMoleculeList.MoleculeNames);
         _result.MoleculeList.MoleculeNamesToExclude.ShouldOnlyContain(_newMoleculeList.MoleculeNamesToExclude);
      }
   }

   public class When_mapping_an_amount_observer_to_snapshot : concern_for_ObserverMapper
   {
      private Observer _result;

      protected override async Task Because()
      {
         _result = await sut.MapToSnapshot(_amountObserver);
      }

      [Observation]
      public void should_return_a_valid_snapshot()
      {
         _result.Type.ShouldBeEqualTo("Amount");
         _result.Dimension.ShouldBeEqualTo(_dimension.Name);
         _result.Formula.ShouldBeEqualTo(_snapshotFormula);
         _result.MoleculeList.ShouldBeEqualTo(_moleculeListSnapshot);

      }
   }

   public class When_mapping_a_amount_observer_snapshot_to_observer : concern_for_ObserverMapper
   {
      private Observer _snapshot;
      private IObserverBuilder _result;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_amountObserver);
      }


      protected override async Task Because()
      {
         _result = await sut.MapToModel(_snapshot, new SnapshotContext());
      }

      [Observation]
      public void should_return_a_valid_model()
      {
         _result.ShouldBeAnInstanceOf<AmountObserverBuilder>();
         _result.Dimension.ShouldBeEqualTo(_dimension);
         _result.Formula.ShouldBeEqualTo(_newFormula);
         _result.MoleculeList.ForAll.ShouldBeEqualTo(_newMoleculeList.ForAll);
         _result.MoleculeList.MoleculeNames.ShouldOnlyContain(_newMoleculeList.MoleculeNames);
         _result.MoleculeList.MoleculeNamesToExclude.ShouldOnlyContain(_newMoleculeList.MoleculeNamesToExclude);
      }
   }
}