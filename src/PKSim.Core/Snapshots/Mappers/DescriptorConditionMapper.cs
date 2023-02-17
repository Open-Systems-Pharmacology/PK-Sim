using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Descriptors;
using OSPSuite.Core.Services;
using PKSim.Assets;
using SnapshotDescriptorCondition = PKSim.Core.Snapshots.DescriptorCondition;

namespace PKSim.Core.Snapshots.Mappers
{
   
   public class DescriptorConditionMapper : SnapshotMapperBase<ITagCondition, SnapshotDescriptorCondition>
   {
      private readonly IOSPSuiteLogger _logger;
      const string MATCH_TAG = "MatchTag";

      public DescriptorConditionMapper(IOSPSuiteLogger logger)
      {
         _logger = logger;
      }

      public override async Task<SnapshotDescriptorCondition> MapToSnapshot(ITagCondition descriptorCondition)
      {
         var tagCondition = descriptorCondition as TagCondition;
         var snapshot = await SnapshotFrom(descriptorCondition);
         snapshot.Tag = tagCondition?.Tag;
         snapshot.Type = typeFrom(descriptorCondition);
         return snapshot;
      }

      public override Task<ITagCondition> MapToModel(SnapshotDescriptorCondition snapshot, SnapshotContext snapshotContext)
      {
         return Task.FromResult(descriptorConditionFrom(snapshot));
      }

      private ITagCondition descriptorConditionFrom(SnapshotDescriptorCondition snapshot)
      {
         var tag = snapshot.Tag;
         switch (snapshot.Type)
         {
            case Constants.IN_CONTAINER:
               return new InContainerCondition(tag);
            case Constants.ALL_TAG:
               return new MatchAllCondition();
            case Constants.IN_PARENT:
               return new InParentCondition();
            case MATCH_TAG:
               return new MatchTagCondition(tag);
            case Constants.NOT_IN_CONTAINER:
               return new NotInContainerCondition(tag);
            case Constants.NOT:
               return new NotMatchTagCondition(tag);
         }

         //This should never happen
         _logger.AddError(PKSimConstants.Error.CannotCreateDescriptorFromSnapshotFor(snapshot.Type));
         return null;
      }

      private string typeFrom(ITagCondition tagCondition)
      {
         switch (tagCondition)
         {
            case InContainerCondition _:
               return Constants.IN_CONTAINER;
            case MatchAllCondition _:
               return Constants.ALL_TAG;
            case MatchTagCondition _:
               return MATCH_TAG;
            case InParentCondition _:
               return Constants.IN_PARENT;
            case NotInContainerCondition _:
               return Constants.NOT_IN_CONTAINER;
            case NotMatchTagCondition _:
               return Constants.NOT;
         }

         _logger.AddError(PKSimConstants.Error.CannotCreateDescriptorSnapshotFor(tagCondition.GetType().FullName));
         return null;
      }
   }
}