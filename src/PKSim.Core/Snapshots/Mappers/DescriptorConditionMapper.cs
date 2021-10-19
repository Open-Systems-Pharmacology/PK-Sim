using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Descriptors;
using OSPSuite.Core.Services;
using PKSim.Assets;
using SnapshotDescriptorCondition = PKSim.Core.Snapshots.DescriptorCondition;

namespace PKSim.Core.Snapshots.Mappers
{
   
   public class DescriptorConditionMapper : SnapshotMapperBase<IDescriptorCondition, SnapshotDescriptorCondition>
   {
      private readonly IOSPSuiteLogger _logger;
      const string MATCH_TAG = "MatchTag";

      public DescriptorConditionMapper(IOSPSuiteLogger logger)
      {
         _logger = logger;
      }

      public override async Task<SnapshotDescriptorCondition> MapToSnapshot(IDescriptorCondition descriptorCondition)
      {
         var tagCondition = descriptorCondition as TagCondition;
         var snapshot = await SnapshotFrom(descriptorCondition);
         snapshot.Tag = tagCondition?.Tag;
         snapshot.Type = typeFrom(descriptorCondition);
         return snapshot;
      }

      public override Task<IDescriptorCondition> MapToModel(SnapshotDescriptorCondition snapshot)
      {
         return Task.FromResult(descriptorConditionFrom(snapshot));
      }

      private IDescriptorCondition descriptorConditionFrom(SnapshotDescriptorCondition snapshot)
      {
         var tag = snapshot.Tag;
         switch (snapshot.Type)
         {
            case Constants.IN_CONTAINER:
               return new InContainerCondition(tag);
            case Constants.ALL_TAG:
               return new MatchAllCondition();
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

      private string typeFrom(IDescriptorCondition descriptorCondition)
      {
         switch (descriptorCondition)
         {
            case InContainerCondition _:
               return Constants.IN_CONTAINER;
            case MatchAllCondition _:
               return Constants.ALL_TAG;
            case MatchTagCondition _:
               return MATCH_TAG;
            case NotInContainerCondition _:
               return Constants.NOT_IN_CONTAINER;
            case NotMatchTagCondition _:
               return Constants.NOT;
         }

         _logger.AddError(PKSimConstants.Error.CannotCreateDescriptorSnapshotFor(descriptorCondition.GetType().FullName));
         return null;
      }
   }
}