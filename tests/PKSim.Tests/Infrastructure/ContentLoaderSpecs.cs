using System.Text;
using OSPSuite.BDDHelper;
using FakeItEasy;
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

      protected override void Context()
      {
         _serializationManager = A.Fake<ICompressedSerializationManager>();
         _buildingBlockMetaDataContentQuery = A.Fake<IBuildingBlockMetaDataContentQuery>();
         sut = new ContentLoader(_buildingBlockMetaDataContentQuery, _serializationManager);
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
}