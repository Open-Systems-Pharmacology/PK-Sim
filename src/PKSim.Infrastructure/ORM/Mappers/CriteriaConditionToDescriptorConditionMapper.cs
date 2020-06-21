using System;
using OSPSuite.Core.Domain.Descriptors;
using PKSim.Infrastructure.ORM.FlatObjects;

namespace PKSim.Infrastructure.ORM.Mappers
{
   public interface ICriteriaConditionToDescriptorConditionMapper
   {
      IDescriptorCondition MapFrom(CriteriaCondition criteriaCondition, string tag);
   }

   public class CriteriaConditionToDescriptorConditionMapper : ICriteriaConditionToDescriptorConditionMapper
   {
      public IDescriptorCondition MapFrom(CriteriaCondition criteriaCondition, string tag)
      {
         switch (criteriaCondition)
         {
            case CriteriaCondition.Has:
               return new MatchTagCondition(tag);
            case CriteriaCondition.DoesNotHave:
               return new NotMatchTagCondition(tag);
            case CriteriaCondition.InContainer:
               return new InContainerCondition(tag);
            case CriteriaCondition.NotInContainer:
               return new NotInContainerCondition(tag);
            default:
               throw new ArgumentOutOfRangeException(nameof(criteriaCondition), criteriaCondition, null);
         }
      }
   }
}