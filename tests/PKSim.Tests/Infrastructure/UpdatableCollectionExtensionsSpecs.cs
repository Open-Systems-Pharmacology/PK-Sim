using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using NHibernate;
using PKSim.Infrastructure.Serialization.ORM.MetaData;
using OSPSuite.Infrastructure.Serialization.ORM.MetaData;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_UpdatableCollectionExtensions : StaticContextSpecification
   {
      protected ICollection<DummyUpdatable> _sourceCollection;
      protected ICollection<DummyUpdatable> _targetDictionary;
      protected DummyUpdatable _target1;
      protected DummyUpdatable _target2;

      protected override void Context()
      {
         _sourceCollection = new HashSet<DummyUpdatable>();
         _targetDictionary = new HashSet<DummyUpdatable>();
         _target1 = new DummyUpdatable {Id = "key1"};
         _target2 = new DummyUpdatable {Id = "key2"};
         _targetDictionary.Add(_target1);
         _targetDictionary.Add(_target2);
      }
   }

   
   public class When_updating_a_target_dictionary_from_a_source_dictionary_containing_the_same_items : concern_for_UpdatableCollectionExtensions
   {
      private DummyUpdatable _source1;
      private DummyUpdatable _source2;
      private ISession _session;

      protected override void Context()
      {
         base.Context();
         _session =A.Fake<ISession>();
         _source1 = new DummyUpdatable {Id = "key1"};
         _source2 = new DummyUpdatable {Id = "key2"};
         _sourceCollection.Add(_source1);
         _sourceCollection.Add(_source2);
      }

      protected override void Because()
      {
         _targetDictionary.UpdateFrom<string, DummyUpdatable>(_sourceCollection, _session);
      }

      [Observation]
      public void should_not_remove_any_item_from_the_dictionary()
      {
         _targetDictionary.Contains(_target1).ShouldBeTrue();
         _targetDictionary.Contains(_target2).ShouldBeTrue();
      }

      [Observation]
      public void should_update_each_target_item_with_the_source_item()
      {
         _target1.Updated.ShouldBeTrue();
         _target2.Updated.ShouldBeTrue();
      }
   }

   
   public class When_updating_a_target_dictionary_from_a_source_dictionary_containing_different_items : concern_for_UpdatableCollectionExtensions
   {
      private DummyUpdatable _source3;
      private DummyUpdatable _source2;
      private ISession _session;

      protected override void Context()
      {
         base.Context();
         _source3 = new DummyUpdatable {Id = "key3"};
         _source2 = new DummyUpdatable {Id = "key2"};
         _sourceCollection.Add(_source3);
         _sourceCollection.Add(_source2);
         _session =A.Fake<ISession>();
      }

      protected override void Because()
      {
         _targetDictionary.UpdateFrom<string, DummyUpdatable>(_sourceCollection, _session);
      }

      [Observation]
      public void should_remove_the_items_that_does_not_exist_in_the_source_item_anymore()
      {
         _targetDictionary.Contains(_target1).ShouldBeFalse();
         _targetDictionary.Contains(_source2).ShouldBeTrue();
      }

      [Observation]
      public void should_add_the_items_that_where_added_to_the_source_item()
      {
         _targetDictionary.Contains(_source3).ShouldBeTrue();
      }

      [Observation]
      public void should_not_call_the_update_function_on_the_newly_added_items()
      {
         _source3.Updated.ShouldBeFalse();
      }

      [Observation]
      public void should_call_the_update_function_on_existing_items()
      {
         _target2.Updated.ShouldBeTrue();
      }
   }

 

   public class DummyUpdatable : MetaData<string>, IUpdatableFrom<DummyUpdatable>
   {
      public bool Updated { get; private set; }

      public void UpdateFrom(DummyUpdatable source, ISession session)
      {
         Updated = true;
      }
   }
}