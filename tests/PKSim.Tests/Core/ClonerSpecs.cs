using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using IContainer = OSPSuite.Utility.Container.IContainer;

namespace PKSim.Core
{
   public abstract class concern_for_Cloner : ContextSpecification<ICloner>
   {
      protected ICloneManagerForModel _cloneManagerForModel;
      protected IBuildingBlockFinalizer _buildingBlockFinalizer;
      protected ICloneManagerForBuildingBlock _cloneManagerForBuildingBlock;
      protected ISerializationManager _serializationManager;
      protected IObjectIdResetter _objectIdResetter;
      protected IContainer _container;

      protected override void Context()
      {
         _buildingBlockFinalizer = A.Fake<IBuildingBlockFinalizer>();
         _cloneManagerForModel = A.Fake<ICloneManagerForModel>();
         _cloneManagerForBuildingBlock = A.Fake<ICloneManagerForBuildingBlock>();
         _objectIdResetter= A.Fake<IObjectIdResetter>();
         _serializationManager = A.Fake<ISerializationManager>();
         _container = A.Fake<IContainer>();
         A.CallTo(() => _container.Resolve<ICloneManagerForBuildingBlock>()).Returns(_cloneManagerForBuildingBlock);
         sut = new Cloner(_cloneManagerForModel, _buildingBlockFinalizer, _serializationManager, _objectIdResetter, _container);
      }
   }

   public class When_asked_to_clone_an_object_base : concern_for_Cloner
   {
      private IObjectBase _clone;
      private IObjectBase _objectToClone;

      protected override void Context()
      {
         base.Context();
         _clone = A.Fake<IObjectBase>();
         _objectToClone = A.Fake<IObjectBase>();
         A.CallTo(() => _cloneManagerForBuildingBlock.Clone(_objectToClone)).Returns(_clone);
      }

      [Observation]
      public void should_leverage_the_clone_manager_for_buildingblock_to_clone_the_object_base()
      {
         sut.Clone(_objectToClone).ShouldBeEqualTo(_clone);
      }
   }

   public class When_asked_to_clone_a_building_block : concern_for_Cloner
   {
      private Individual _clone;
      private Individual _individualToClone;
      private Individual _result;

      protected override void Context()
      {
         base.Context();
         _clone = A.Fake<Individual>();
         _individualToClone = A.Fake<Individual>();
         A.CallTo(() => _cloneManagerForBuildingBlock.Clone(_individualToClone)).Returns(_clone);
      }

      protected override void Because()
      {
         _result = sut.Clone(_individualToClone);
      }

      [Observation]
      public void should_leverage_the_clone_manager_for_buildingblock_to_clone_the_building_block()
      {
         _result.ShouldBeEqualTo(_clone);
      }

      [Observation]
      public void should_resolve_the_references_define_in_the_building_block()
      {
         A.CallTo(() => _buildingBlockFinalizer.Finalize(_result)).MustHaveHappened();
      }
   }

   public class When_cloning_several_objects : concern_for_Cloner
   {
      protected override void Context()
      {
         base.Context();
         //each call must get its own clone manager: it carries per-operation state (FormulaCache)
         A.CallTo(() => _container.Resolve<ICloneManagerForBuildingBlock>()).ReturnsLazily(() => A.Fake<ICloneManagerForBuildingBlock>());
      }

      protected override void Because()
      {
         sut.Clone(A.Fake<Individual>());
         sut.Clone(A.Fake<Individual>());
      }

      [Observation]
      public void should_resolve_a_fresh_clone_manager_for_each_clone_operation()
      {
         A.CallTo(() => _container.Resolve<ICloneManagerForBuildingBlock>()).MustHaveHappenedTwiceExactly();
      }
   }

   public class When_cloning_an_object_that_is_not_updtable : concern_for_Cloner
   {
      private IWithId _objectWithId;
      private IWithId _result;
      private IWithId _derserialziedObject;

      protected override void Context()
      {
         base.Context();
         _derserialziedObject= A.Fake<IWithId>();
         _objectWithId= A.Fake<IWithId>();
         var stream=new byte[]{1,2};
         A.CallTo(() => _serializationManager.Serialize(_objectWithId)).Returns(stream);
         A.CallTo(() => _serializationManager.Deserialize<IWithId>(stream, null)).Returns(_derserialziedObject);
      }

      protected override void Because()
      {
         _result = sut.CloneObject(_objectWithId);
      }

      [Observation]
      public void should_clone_the_object_using_the_serialization_and_deserialization_workflow()
      {
         _result.ShouldBeEqualTo(_derserialziedObject);  
      }

      [Observation]
      public void should_update_the_id()
      {
         A.CallTo(() => _objectIdResetter.ResetIdFor(_result)).MustHaveHappened();
      }
   }
}