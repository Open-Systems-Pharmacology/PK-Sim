﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using SnapshotCompoundProperties = PKSim.Core.Snapshots.CompoundProperties;
using ModelCompoundProperties = PKSim.Core.Model.CompoundProperties;

namespace PKSim.Core.Snapshots.Mappers
{
   public class CompoundPropertiesContext
   {
      public PKSimProject Project { get; }
      public Model.Simulation Simulation { get; }

      public CompoundPropertiesContext(PKSimProject project, Model.Simulation simulation)
      {
         Project = project;
         Simulation = simulation;
      }
   }

   public class CompoundPropertiesMapper : SnapshotMapperBase<ModelCompoundProperties, SnapshotCompoundProperties, CompoundPropertiesContext, PKSimProject>
   {
      private readonly CalculationMethodCacheMapper _calculationMethodCacheMapper;
      private readonly ProcessMappingMapper _processMappingMapper;
      private readonly ILogger _logger;

      public CompoundPropertiesMapper(CalculationMethodCacheMapper calculationMethodCacheMapper, ProcessMappingMapper processMappingMapper, ILogger logger)
      {
         _calculationMethodCacheMapper = calculationMethodCacheMapper;
         _processMappingMapper = processMappingMapper;
         _logger = logger;
      }

      public override async Task<SnapshotCompoundProperties> MapToSnapshot(ModelCompoundProperties modelCompoundProperties, PKSimProject project)
      {
         var compound = modelCompoundProperties.Compound;
         var snapshot = await SnapshotFrom(modelCompoundProperties, x =>
         {
            x.Name = compound.Name;
            x.Alternatives = alternativeSelectionsFrom(compound, modelCompoundProperties);
            x.Protocol = protocolPropertiesFrom(modelCompoundProperties.ProtocolProperties, project);
         });

         snapshot.CalculationMethods = await _calculationMethodCacheMapper.MapToSnapshot(modelCompoundProperties.CalculationMethodCache);
         snapshot.Processes = await snapshotProcessSelectionFrom(modelCompoundProperties.Processes);
         return snapshot;
      }

      private ProtocolSelection protocolPropertiesFrom(ProtocolProperties protocolProperties, PKSimProject project)
      {
         if (protocolProperties.Protocol == null)
            return null;

         return new ProtocolSelection
         {
            Name = protocolProperties.Protocol.Name,
            Formulations = formulationMappingFrom(protocolProperties.FormulationMappings, project)
         };
      }

      private Task<CompoundProcessSelection[]> snapshotProcessSelectionFrom(CompoundProcessesSelection compoundProcessesSelection)
      {
         return _processMappingMapper.MapToSnapshots(compoundProcessesSelection.AllEnabledProcesses());
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

      private FormulationSelection[] formulationMappingFrom(IReadOnlyList<FormulationMapping> formulationMappings, PKSimProject project)
      {
         if (!formulationMappings.Any())
            return null;

         return formulationMappings
            .Select(x => formulationSelectionFrom(x, project))
            .ToArray();
      }

      private FormulationSelection formulationSelectionFrom(FormulationMapping formulationMapping, PKSimProject project)
      {
         var formulation = project.BuildingBlockById(formulationMapping.TemplateFormulationId) ?? formulationMapping.Formulation;
         return new FormulationSelection {Name = formulation.Name, Key = formulationMapping.FormulationKey};
      }

      public override async Task<ModelCompoundProperties> MapToModel(SnapshotCompoundProperties snapshot, CompoundPropertiesContext context)
      {
         var simulation = context.Simulation;
         var compoundProperties = simulation.CompoundPropertiesFor(snapshot.Name);

         await _calculationMethodCacheMapper.MapToModel(snapshot.CalculationMethods, compoundProperties.CalculationMethodCache);
         updateAlternativeSelections(snapshot.Alternatives, compoundProperties);
         compoundProperties.Processes = await modelProcessSelectionFrom(snapshot.Processes, compoundProperties.Compound);
         compoundProperties.ProtocolProperties = modelProtocolPropertiesFrom(snapshot.Protocol, context.Project);

         return compoundProperties;
      }

      private ProtocolProperties modelProtocolPropertiesFrom(ProtocolSelection snapshotProtocol, PKSimProject project)
      {
         var protocolProperties = new ProtocolProperties();
         if (snapshotProtocol == null)
            return protocolProperties;

         var protocol = project.BuildingBlockByName<Model.Protocol>(snapshotProtocol.Name);
         protocolProperties.Protocol = protocol;
         updateFormulationMapping(protocolProperties, snapshotProtocol.Formulations, project);
         return protocolProperties;
      }

      private void updateFormulationMapping(ProtocolProperties protocolProperties, FormulationSelection[] snapshotProtocolFormulations, PKSimProject project)
      {
         snapshotProtocolFormulations?.Each(x =>
         {
            var formulation = project.BuildingBlockByName<Model.Formulation>(x.Name);
            var formulationMapping = new FormulationMapping
            {
               Formulation = formulation,
               FormulationKey = x.Key,
               TemplateFormulationId = formulation.Id
            };
            protocolProperties.AddFormulationMapping(formulationMapping);
         });
      }

      private async Task<CompoundProcessesSelection> modelProcessSelectionFrom(CompoundProcessSelection[] snapshotProcesses, Model.Compound compound)
      {
         var compoundProcessesSelection = new CompoundProcessesSelection();
         if (snapshotProcesses == null)
            return compoundProcessesSelection;

         foreach (var snapshotProcess in snapshotProcesses)
         {
            var process = compound.ProcessByName(snapshotProcess.Name);
            if (process == null)
               _logger.AddWarning(PKSimConstants.Error.ProcessNotFoundInCompound(snapshotProcess.Name, compound.Name));
            else
               await addProcessToProcessSelection(compoundProcessesSelection, snapshotProcess, process);
         }

         return compoundProcessesSelection;
      }

      private async Task addProcessToProcessSelection(CompoundProcessesSelection compoundProcessesSelection, CompoundProcessSelection compoundProcessSelection, Model.CompoundProcess process)
      {
         var processSelectionGroup = selectionGroupFor(compoundProcessesSelection, process);
         var processSelection = await _processMappingMapper.MapToModel(compoundProcessSelection, process);
         processSelectionGroup.AddProcessSelection(processSelection);
      }

      private ProcessSelectionGroup selectionGroupFor(CompoundProcessesSelection compoundProcessesSelection, Model.CompoundProcess process)
      {
         return selectionGroup<EnzymaticProcess>(compoundProcessesSelection.MetabolizationSelection, process, SystemicProcessTypes.Hepatic) ??
                selectionGroup<TransportPartialProcess>(compoundProcessesSelection.TransportAndExcretionSelection, process, SystemicProcessTypes.GFR, SystemicProcessTypes.Biliary, SystemicProcessTypes.Renal) ??
                selectionGroup<SpecificBindingPartialProcess>(compoundProcessesSelection.SpecificBindingSelection, process);
      }

      private ProcessSelectionGroup selectionGroup<TPartialProcess>(ProcessSelectionGroup processSelectionGroup, Model.CompoundProcess process, params SystemicProcessType[] systemicProcessTypes) where TPartialProcess : PartialProcess
      {
         if (process.IsAnImplementationOf<TPartialProcess>())
            return processSelectionGroup;

         var systemicProcess = process as SystemicProcess;
         if (systemicProcess == null)
            return null;

         if (systemicProcess.SystemicProcessType.IsOneOf(systemicProcessTypes))
            return processSelectionGroup;

         return null;
      }

      private void updateAlternativeSelections(CompoundGroupSelection[] snapshotAlternatives, ModelCompoundProperties compoundProperties)
      {
         snapshotAlternatives?.Each(x => updateAlternativeSelection(x, compoundProperties));
      }

      private void updateAlternativeSelection(CompoundGroupSelection snapshotCompoundGroupSelection, ModelCompoundProperties compoundProperties)
      {
         var compoundGroupSelection = compoundProperties.CompoundGroupSelections.Find(x => x.GroupName == snapshotCompoundGroupSelection.GroupName);
         if (compoundGroupSelection == null)
         {
            _logger.AddError(PKSimConstants.Error.CompoundGroupNotFoundFor(snapshotCompoundGroupSelection.GroupName, compoundProperties.Compound.Name));
            return;
         }

         var alternativeGroup = compoundProperties.Compound.ParameterAlternativeGroup(snapshotCompoundGroupSelection.GroupName);
         if (alternativeGroup == null)
         {
            _logger.AddError(PKSimConstants.Error.CompoundGroupNotFoundFor(snapshotCompoundGroupSelection.GroupName, compoundProperties.Compound.Name));
            return;
         }

         var alternative = alternativeGroup.AlternativeByName(snapshotCompoundGroupSelection.AlternativeName);
         if (alternative == null)
         {
            _logger.AddError(PKSimConstants.Error.CompoundAlternativeNotFoundFor(snapshotCompoundGroupSelection.AlternativeName, alternativeGroup.DefaultAlternative?.Name, snapshotCompoundGroupSelection.GroupName, compoundProperties.Compound.Name));
            return;
         }
         compoundGroupSelection.AlternativeName = snapshotCompoundGroupSelection.AlternativeName;
      }
   }
}