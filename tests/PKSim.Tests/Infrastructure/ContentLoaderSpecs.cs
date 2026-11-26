using System.Text;
using OSPSuite.BDDHelper;
using FakeItEasy;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure.Serialization.ORM.Queries;
using PKSim.Infrastructure.Services;
using OSPSuite.Infrastructure.Serialization.ORM.MetaData;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_ContentLoader : ContextSpecification<IContentLoader>
   {
      protected ICompressedSerializationManager _serializationManager;
      protected IBuildingBlockMetaDataContentQuery _buildingBlockMetaDataContentQuery;
      protected IObjectTypeResolver _objectTypeResolver;

      protected override void Context()
      {
         _serializationManager = A.Fake<ICompressedSerializationManager>();
         _buildingBlockMetaDataContentQuery = A.Fake<IBuildingBlockMetaDataContentQuery>();
         _objectTypeResolver = A.Fake<IObjectTypeResolver>();
         sut = new ContentLoader(_buildingBlockMetaDataContentQuery, _serializationManager, _objectTypeResolver);
      }
   }

   public class When_loading_the_content_of_a_lazy_loadable_object : concern_for_ContentLoader
   {
      private IPKSimBuildingBlock _objectToLoad;
      private MetaDataContent _content;

      protected override void Context()
      {
         base.Context();
         _objectToLoad = A.Fake<IPKSimBuildingBlock>();
         _objectToLoad.Id = "tralala";
         _content = new MetaDataContent {Data = Encoding.UTF8.GetBytes("content")};
         A.CallTo(() => _buildingBlockMetaDataContentQuery.ResultFor(_objectToLoad.Id)).Returns(_content);
      }

      protected override void Because()
      {
         sut.LoadContentFor(_objectToLoad);
      }

      [Observation]
      public void should_retrieve_the_meta_data_content()
      {
         A.CallTo(() => _buildingBlockMetaDataContentQuery.ResultFor(_objectToLoad.Id)).MustHaveHappened();
      }

      [Observation]
      public void should_retrieve_the_content_of_the_object_to_load()
      {
         A.CallTo(() => _serializationManager.Deserialize(_objectToLoad, _content.Data, null)).MustHaveHappened();
      }
   }

   public class When_loading_the_content_of_a_lazy_loadable_object_whose_content_is_missing_from_the_project_file : concern_for_ContentLoader
   {
      private Individual _objectToLoad;
      private MetaDataContent _contentWithoutData;

      protected override void Context()
      {
         base.Context();
         _objectToLoad = new Individual {Id = "IND", Name = "Standard male"};
         _contentWithoutData = new MetaDataContent {Data = null};
         A.CallTo(() => _buildingBlockMetaDataContentQuery.ResultFor(_objectToLoad.Id)).Returns(_contentWithoutData);
         A.CallTo(() => _objectTypeResolver.TypeFor((IObjectBase) _objectToLoad)).Returns(PKSimConstants.ObjectTypes.Individual);
      }

      [Observation]
      public void should_throw_an_exception_naming_the_building_block()
      {
         The.Action(() => sut.LoadContentFor(_objectToLoad)).ShouldThrowAn<PKSimException>();
      }

      [Observation]
      public void should_describe_the_missing_content_in_the_exception_message()
      {
         var exception = catchException();
         exception.ShouldNotBeNull();
         exception.Message.ShouldBeEqualTo(PKSimConstants.Error.BuildingBlockContentIsMissing(_objectToLoad.Name, PKSimConstants.ObjectTypes.Individual));
      }

      [Observation]
      public void should_not_pass_the_missing_content_to_the_serializer()
      {
         catchException();
         A.CallTo(() => _serializationManager.Deserialize(_objectToLoad, A<byte[]>._, null)).MustNotHaveHappened();
      }

      private PKSimException catchException()
      {
         try
         {
            sut.LoadContentFor(_objectToLoad);
            return null;
         }
         catch (PKSimException e)
         {
            return e;
         }
      }
   }
}
