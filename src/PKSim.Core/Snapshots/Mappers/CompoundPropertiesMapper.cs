using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using SnapshotCompoundProperties = PKSim.Core.Snapshots.CompoundProperties;
using ModelCompoundProperties = PKSim.Core.Model.CompoundProperties;

namespace PKSim.Core.Snapshots.Mappers
{
   public class CompoundPropertiesContext
   {
      public PKSimProject Project { get;  }
      public Model.Simulation Simulation { get;  }

      public CompoundPropertiesContext(PKSimProject project, Model.Simulation simulation)
      {
         Project = project;
         Simulation = simulation;
      }
   }

   public class CompoundPropertiesMapper : SnapshotMapperBase<ModelCompoundProperties, SnapshotCompoundProperties, CompoundPropertiesContext, PKSimProject>
   {
      private readonly CalculationMethodCacheMapper _calculationMethodCacheMapper;

      public CompoundPropertiesMapper(CalculationMethodCacheMapper calculationMethodCacheMapper)
      {
         _calculationMethodCacheMapper = calculationMethodCacheMapper;
      }

      public override async Task<SnapshotCompoundProperties> MapToSnapshot(ModelCompoundProperties modelCompoundProperties, PKSimProject project)
      {
         var compound = modelCompoundProperties.Compound;
         var snapshot = await SnapshotFrom(modelCompoundProperties, x => { x.Name = compound.Name; });

         snapshot.CalculationMethods = await _calculationMethodCacheMapper.MapToSnapshot(modelCompoundProperties.CalculationMethodCache);
         snapshot.Alternatives = alternativeSelectionsFrom(compound, modelCompoundProperties);
         snapshot.Processes = snapshotProcessSelectionFrom(modelCompoundProperties.Processes);
         snapshot.Protocol = protocolPropertiesFrom(modelCompoundProperties.ProtocolProperties, project);
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

      private CompoundProcessSelection[] snapshotProcessSelectionFrom(CompoundProcessesSelection compoundProcessesSelection)
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

      private IReactionMapping reactionMappingFrom(CompoundProcessSelection compoundProcessSelection, Model.CompoundProcess process)
      {
         switch (process)
         {
            case SystemicProcess systemicProcess:
               return new SystemicProcessSelection
               {
                  ProcessType = systemicProcess.SystemicProcessType,
               };
            case EnzymaticProcess _:
               return new EnzymaticProcessSelection
               {
                  MoleculeName = compoundProcessSelection.MoleculeName,
                  MetaboliteName = compoundProcessSelection.MetaboliteName,
               };
            default:
               return new ProcessSelection
               {
                  MoleculeName = compoundProcessSelection.MoleculeName,
               };
         }
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
         var formulation = project.BuildingBlockById(formulationMapping.TemplateFormulationId);
         return new FormulationSelection {Name = formulation.Name, Key = formulationMapping.FormulationKey};
      }

      public override async Task<ModelCompoundProperties> MapToModel(SnapshotCompoundProperties snapshot, CompoundPropertiesContext context)
      {
         var simulation = context.Simulation;
         var compoundProperties = simulation.CompoundPropertiesFor(snapshot.Name);

         await _calculationMethodCacheMapper.MapToModel(snapshot.CalculationMethods, compoundProperties.CalculationMethodCache);
         updateAlternativeSelections(snapshot.Alternatives, compoundProperties);
         compoundProperties.Processes = modelProcessSelectionFrom(snapshot.Processes, compoundProperties.Compound);
         compoundProperties.ProtocolProperties = modelProtocolPropertiesFrom(snapshot.Protocol, simulation, context.Project);

         return compoundProperties;
      }

      private ProtocolProperties modelProtocolPropertiesFrom(ProtocolSelection snapshotProtocol, Model.Simulation simulation, PKSimProject project)
      {
         var protocolProperties = new ProtocolProperties();
         if(snapshotProtocol==null)
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

      private CompoundProcessesSelection modelProcessSelectionFrom(CompoundProcessSelection[] snapshotProcesses, Model.Compound compound)
      {
         var compoundProcessesSelection = new CompoundProcessesSelection();
         snapshotProcesses?.Each(x =>
         {
            var process = compound.ProcessByName(x.Name);
            if (process == null)
               throw new SnapshotOutdatedException(PKSimConstants.Error.ProcessNotFoundInCompound(x.Name, compound.Name));

            addProcessToProcessSelection(compoundProcessesSelection, x, process);
         });

         return compoundProcessesSelection;
      }

      private void addProcessToProcessSelection(CompoundProcessesSelection compoundProcessesSelection, CompoundProcessSelection compoundProcessSelection, Model.CompoundProcess process)
      {
         var processSelectionGroup = selectionGroupFor(compoundProcessesSelection, process);
         addProcessToProcessSelectionGroup(processSelectionGroup, compoundProcessSelection, process);
      }

      private void addProcessToProcessSelectionGroup(ProcessSelectionGroup processSelectionGroup, CompoundProcessSelection compoundProcessSelection, Model.CompoundProcess process)
      {
         var reactionMapping = reactionMappingFrom(compoundProcessSelection, process);
         reactionMapping.ProcessName = compoundProcessSelection.Name;
         reactionMapping.CompoundName = process.ParentCompound.Name;
         processSelectionGroup.AddProcessSelection(reactionMapping);
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
         var modelGroupSelection = compoundProperties.CompoundGroupSelections.Find(x => x.GroupName == snapshotCompoundGroupSelection.GroupName);
         //TODO throw?
         if (modelGroupSelection == null)
            return;

         modelGroupSelection.AlternativeName = snapshotCompoundGroupSelection.AlternativeName;
      }
   }
}