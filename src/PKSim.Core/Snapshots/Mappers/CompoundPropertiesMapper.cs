using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using SnapshotCompoundProperties = PKSim.Core.Snapshots.CompoundProperties;
using ModelCompoundProperties = PKSim.Core.Model.CompoundProperties;

namespace PKSim.Core.Snapshots.Mappers
{
   public class CompoundPropertiesMapper : SnapshotMapperBase<ModelCompoundProperties, SnapshotCompoundProperties>
   {
      private readonly CalculationMethodCacheMapper _calculationMethodCacheMapper;
      private readonly IBuildingBlockRepository _buildingBlockRepository;

      public CompoundPropertiesMapper(CalculationMethodCacheMapper calculationMethodCacheMapper, IBuildingBlockRepository buildingBlockRepository)
      {
         _calculationMethodCacheMapper = calculationMethodCacheMapper;
         _buildingBlockRepository = buildingBlockRepository;
      }

      public override async Task<SnapshotCompoundProperties> MapToSnapshot(ModelCompoundProperties modelCompoundProperties)
      {
         var compound = modelCompoundProperties.Compound;
         var snapshot = await SnapshotFrom(modelCompoundProperties, x => { x.Name = compound.Name; });

         snapshot.CalculationMethods = await _calculationMethodCacheMapper.MapToSnapshot(modelCompoundProperties.CalculationMethodCache);
         snapshot.Alternatives = alternativeSelectionsFrom(compound, modelCompoundProperties);
         snapshot.Processes = compoundProcessSelection(modelCompoundProperties.Processes);
         snapshot.Protocol = protocolPropertiesFrom(modelCompoundProperties.ProtocolProperties);
         return snapshot;
      }

      private ProtocolSelection protocolPropertiesFrom(ProtocolProperties protocolProperties)
      {
         if (protocolProperties.Protocol == null)
            return null;

         return new ProtocolSelection
         {
            Name = protocolProperties.Protocol.Name,
            Formulations = formulationMappingFrom(protocolProperties.FormulationMappings)
         };
      }

      private CompoundProcessSelection[] compoundProcessSelection(CompoundProcessesSelection compoundProcessesSelection)
      {
         return compoundProcessesSelection.AllProcessSelectionGroups
            .SelectMany(processSelectionsForGroup)
            .ToArray();
      }

      private CompoundProcessSelection compoundProcessSelectionFor(IReactionMapping reactionMapping)
      {
         var compoundProcessSelection = new CompoundProcessSelection
         {
            Name = reactionMapping.ProcessName
         };

         switch (reactionMapping)
         {
            case EnzymaticProcessSelection enzymaticProcessSelection:
               compoundProcessSelection.MoleculeName = SnapshotValueFor(enzymaticProcessSelection.MoleculeName);
               compoundProcessSelection.MetaboliteName = SnapshotValueFor(enzymaticProcessSelection.MetaboliteName);
               break;
            case ProcessSelection processSelection:
               compoundProcessSelection.MoleculeName = SnapshotValueFor(processSelection.MoleculeName);
               break;
         }

         return compoundProcessSelection;
      }

      private CompoundGroupSelection[] alternativeSelectionsFrom(Model.Compound compound, ModelCompoundProperties modelCompoundProperties)
      {
         var alternativesSelections = new List<CompoundGroupSelection>();
         modelCompoundProperties.CompoundGroupSelections.Each(x =>
         {
            var compoundProperties = compound.ParameterAlternativeGroup(x.GroupName);
            if (compoundProperties.AllAlternatives.Count() > 1)
               alternativesSelections.Add(x);
         });

         return !alternativesSelections.Any() ? null : alternativesSelections.ToArray();
      }

      private IEnumerable<CompoundProcessSelection> processSelectionsForGroup(ProcessSelectionGroup processSelectionGroup)
      {
         return processSelectionGroup.AllEnabledProcesses().Select(compoundProcessSelectionFor);
      }

      private FormulationSelection[] formulationMappingFrom(IReadOnlyList<FormulationMapping> formulationMappings)
      {
         if (!formulationMappings.Any())
            return null;

         return formulationMappings
            .Select(formulationSelectionFrom)
            .ToArray();
      }

      private FormulationSelection formulationSelectionFrom(FormulationMapping formulationMapping)
      {
         var formulation = _buildingBlockRepository.ById(formulationMapping.TemplateFormulationId);
         return new FormulationSelection {Name = formulation.Name, Key = formulationMapping.FormulationKey};
      }

      public override Task<ModelCompoundProperties> MapToModel(SnapshotCompoundProperties snapshot)
      {
         throw new NotImplementedException();
      }
   }
}