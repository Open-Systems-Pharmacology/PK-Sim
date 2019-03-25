﻿using System.Collections.Generic;
using OSPSuite.Presentation.DTO;
using OSPSuite.Utility.Validation;
using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Presentation.DTO.Simulations
{
   public class ObserverSetMappingDTO : DxValidatableDTO
   {
      public ObserverSetMapping ObserverSetMapping { get; }
      private ObserverSet _observerSet;

      public ObserverSetMappingDTO(ObserverSetMapping observerSetMapping)
      {
         ObserverSetMapping = observerSetMapping;
         Rules.AddRange(AllRules.All());
      }

      public ObserverSet ObserverSet
      {
         get => _observerSet;
         set
         {
            ObserverSetMapping.TemplateObserverSetId = value?.Id ?? string.Empty;
            SetProperty(ref _observerSet, value);
         }
      }

      private static class AllRules
      {
         private static IBusinessRule buildingBlockNotNull { get; } = CreateRule.For<ObserverSetMappingDTO>()
            .Property(item => item.ObserverSet)
            .WithRule((dto, ev) => ev != null)
            .WithError((dto, block) => PKSimConstants.Error.BuildingBlockNotDefined(PKSimConstants.ObjectTypes.ObserverSet));

         internal static IEnumerable<IBusinessRule> All()
         {
            yield return buildingBlockNotNull;
         }
      }
   }
}