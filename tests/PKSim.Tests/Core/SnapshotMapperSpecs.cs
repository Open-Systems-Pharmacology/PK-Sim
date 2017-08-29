using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Collections;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;

namespace PKSim.Core
{
   public abstract class concern_for_SnapshotMapper : ContextSpecification<ISnapshotMapper>
   {
      private IRepository<ISnapshotMapperSpecification> _snapshotMapperRepository;
      protected ISnapshotMapperSpecification _snapshotMapper1;
      protected ISnapshotMapperSpecification _snapshotMapper2;

      protected override void Context()
      {
         _snapshotMapperRepository= A.Fake<IRepository<ISnapshotMapperSpecification>>();
         _snapshotMapper1= A.Fake<ISnapshotMapperSpecification>();
         _snapshotMapper2= A.Fake<ISnapshotMapperSpecification>();

         A.CallTo(() => _snapshotMapperRepository.All()).Returns(new []{_snapshotMapper1, _snapshotMapper2, });

         sut = new SnapshotMapper(_snapshotMapperRepository);
      }
   }

   public class When_mapping_an_object_for_a_which_a_snapshot_can_be_found_to_a_snapshot_object : concern_for_SnapshotMapper
   {
      private object _model;
      private object _snapshot;

      protected override void Context()
      {
         base.Context();
         _model = new Model.Formulation();
         _snapshot = new Snapshots.Formulation();
         A.CallTo(() => _snapshotMapper1.IsSatisfiedBy(_model.GetType())).Returns(false);
         A.CallTo(() => _snapshotMapper2.IsSatisfiedBy(_model.GetType())).Returns(true);

         A.CallTo(() => _snapshotMapper2.MapToSnapshot(_model)).Returns(_snapshot);
      }

      [Observation]
      public void should_return_the_expected_snapshot()
      {
         sut.MapToSnapshot(_model).ShouldBeEqualTo(_snapshot);         
      }
   }

   public class When_mapping_an_object_for_a_which_no_snapshot_can_be_found_to_a_snapshot_object : concern_for_SnapshotMapper
   {
      private object _model;
     
      protected override void Context()
      {
         base.Context();
         _model = new Model.Formulation();
      }

      [Observation]
      public void should_throw_a_no_mapper_can_be_found_exception()
      {
         The.Action(()=>sut.MapToSnapshot(_model)).ShouldThrowAn<SnapshotNotFoundException>();
      }
   }
}	