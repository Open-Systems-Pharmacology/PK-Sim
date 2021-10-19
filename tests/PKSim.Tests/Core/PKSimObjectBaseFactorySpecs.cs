using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.UnitSystem;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using FakeItEasy;

namespace PKSim.Core
{
   public abstract class concern_for_PKSimObjectBaseFactory : ContextSpecification<IObjectBaseFactory>
   {
      protected IContainer _container;
      protected IIdGenerator _idGenerator;
      protected IParameter _parameter;
      private OSPSuite.Utility.Container.IContainer _ioC;
      private IDimensionRepository _dimensionRepository;
      private ICreationMetaDataFactory _creationMetaDataFactory;

      protected override void Context()
      {
         _idGenerator = A.Fake<IIdGenerator>();
         _parameter = new PKSimParameter();
         _ioC = A.Fake<OSPSuite.Utility.Container.IContainer>();
         _dimensionRepository = A.Fake<IDimensionRepository>();
         _creationMetaDataFactory= A.Fake<ICreationMetaDataFactory>();
         var dimensionFactory = A.Fake<IDimensionFactory>();
         A.CallTo(() => dimensionFactory.NoDimension).Returns(Constants.Dimension.NO_DIMENSION);
         A.CallTo(() => _dimensionRepository.DimensionFactory).Returns(dimensionFactory);
         A.CallTo(() => _ioC.Resolve<IParameter>()).Returns(_parameter);
         sut = new PKSimObjectBaseFactory(_ioC, _dimensionRepository, _idGenerator, _creationMetaDataFactory);
      }
   }

   
   public class When_creating_an_object_for_an_interface_without_any_specified_id : concern_for_PKSimObjectBaseFactory
   {
      private IParameter _result;
      private string _id;

      protected override void Context()
      {
         base.Context();
         _id = "titi";
         A.CallTo(() => _idGenerator.NewId()).Returns(_id);
      }

      protected override void Because()
      {
         _result = sut.Create<IParameter>();
      }

      [Observation]
      public void should_leverage_the_id_generator_to_create_a_new_id()
      {
         A.CallTo(() => _idGenerator.NewId()).MustHaveHappened();
      }

      [Observation]
      public void should_return_a_new_object_with_the_accurate_id()
      {
         _result.Id.ShouldBeEqualTo(_id);
      }
   }

   
   public class When_creating_an_object_for_an_interface_without_a_specified_id : concern_for_PKSimObjectBaseFactory
   {
      private IParameter _result;
      private string _id;

      protected override void Context()
      {
         base.Context();
         _id = "titi";
      }

      protected override void Because()
      {
         _result = sut.Create<IParameter>(_id);
      }

      [Observation]
      public void should_not_leveragte_the_id_generator_to_create_a_new_id()
      {
         A.CallTo(() => _idGenerator.NewId()).MustNotHaveHappened();
      }

      [Observation]
      public void should_return_a_new_object_with_the_accurate_id()
      {
         _result.Id.ShouldBeEqualTo(_id);
      }
   }

   
   public class When_creating_an_object_from_a_type : concern_for_PKSimObjectBaseFactory
   {
      private IParameter _result;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _idGenerator.NewId()).Returns("tralala");
      }

      protected override void Because()
      {
         _result = sut.CreateObjectBaseFrom<PKSimParameter>(typeof (PKSimParameter));
      }

      [Observation]
      public void should_create_a_valid_object()
      {
         _result.ShouldNotBeNull();
      }

      [Observation]
      public void should_leverage_the_id_generator_to_create_a_new_id()
      {
         A.CallTo(() => _idGenerator.NewId()).MustHaveHappened();
      }
   }

   
   public class When_creating_an_object_from_another_object : concern_for_PKSimObjectBaseFactory
   {
      private IParameter _result;
      private PKSimParameter _source;

      protected override void Context()
      {
         base.Context();
         _source = new PKSimParameter();
         A.CallTo(() => _idGenerator.NewId()).Returns("tralala");
      }

      protected override void Because()
      {
         _result = sut.CreateObjectBaseFrom(_source);
      }

      [Observation]
      public void should_create_a_valid_object()
      {
         _result.ShouldNotBeNull();
      }

      [Observation]
      public void should_leverage_the_id_generator_to_create_a_new_id()
      {
         A.CallTo(() => _idGenerator.NewId()).MustHaveHappened();
      }
   }
}