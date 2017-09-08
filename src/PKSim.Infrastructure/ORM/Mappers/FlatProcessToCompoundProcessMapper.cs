using System;
using PKSim.Assets;
using OSPSuite.Utility;
using OSPSuite.Core.Domain;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.FlatObjects;

namespace PKSim.Infrastructure.ORM.Mappers
{
   public interface IFlatProcessToCompoundProcessMapper : IMapper<FlatProcess, CompoundProcess>
   {
   }

   public class FlatProcessToCompoundProcessMapper : IFlatProcessToCompoundProcessMapper
   {
      private readonly IObjectBaseFactory _entityBaseFactory;
      private readonly ISpeciesRepository _speciesRepository;
      private readonly IRepresentationInfoRepository _representationInfoRepository;

      public FlatProcessToCompoundProcessMapper(IObjectBaseFactory entityBaseFactory,
                                                ISpeciesRepository speciesRepository,IRepresentationInfoRepository representationInfoRepository)
      {
         _entityBaseFactory = entityBaseFactory;
         _speciesRepository = speciesRepository;
         _representationInfoRepository = representationInfoRepository;
      }

      public CompoundProcess MapFrom(FlatProcess flatProcess)
      {
         if (!flatProcess.IsTemplate)
            throw new ArgumentException(PKSimConstants.Error.CompoundProcessDeclaredAsNotTemplate(flatProcess.Name));

         if (flatProcess.GroupName == CoreConstants.Groups.ENZYMATIC_STABILITY_INTRINSIC)
            return createProcessBaseOn<EnzymaticProcessWithSpecies>(flatProcess);

         if (flatProcess.GroupName == CoreConstants.Groups.ENZYMATIC_STABILITY_PARTIAL)
            return createProcessBaseOn<EnzymaticProcess>(flatProcess);

         if (flatProcess.GroupName == CoreConstants.Groups.SPECIFIC_BINDING)
            return createProcessBaseOn<SpecificBindingPartialProcess>(flatProcess);

         if (flatProcess.GroupName == CoreConstants.Groups.ACTIVE_TRANSPORT_INTRINSIC)
            return createProcessBaseOn<TransportPartialProcessWithSpecies>(flatProcess);

         if (flatProcess.GroupName == CoreConstants.Groups.ACTIVE_TRANSPORT)
            return createProcessBaseOn<TransportPartialProcess>(flatProcess);

         if (flatProcess.GroupName == CoreConstants.Groups.IN_VITRO_PROCESSES_ENZYMATIC_STABILITY)
            return createProcessBaseOn<EnzymaticProcessWithSpecies>(flatProcess);

         if (flatProcess.GroupName == CoreConstants.Groups.INHIBITION_PROCESSES)
            return createInteractionProcess<InhibitionProcess>(flatProcess);

         if (flatProcess.GroupName == CoreConstants.Groups.INDUCTION_PROCESSES)
            return createInteractionProcess<InductionProcess>(flatProcess);

         if (flatProcess.GroupName == CoreConstants.Groups.IN_VITRO_PROCESSES_TOTAL_HEPATIC)
            return createSystemicProcess(flatProcess, SystemicProcessTypes.Hepatic);

         if (flatProcess.GroupName == CoreConstants.Groups.SYSTEMIC_PROCESSES)
            return createSystemicProcess(flatProcess, typeFrom(flatProcess));

         throw new ArgumentException($"Group '{flatProcess.GroupName}' unknown for compound");
      }

      private SystemicProcess createSystemicProcess(FlatProcess flatProcess, SystemicProcessType processType)
      {
         var systemicProcess = createProcessBaseOn<SystemicProcess>(flatProcess);
         systemicProcess.SystemicProcessType = processType;
         systemicProcess.Icon = processType.IconName;
         setDefaultSpeciesForProcess(systemicProcess);
         return systemicProcess;
      }

      private void setDefaultSpeciesForProcess(ISpeciesDependentCompoundProcess speciesDependentCompoundProcess)
      {
         if (speciesDependentCompoundProcess == null) return;
         speciesDependentCompoundProcess.Species = _speciesRepository.DefaultSpecies;
      }

      private SystemicProcessType typeFrom(FlatProcess flatProcess)
      {
         if (flatProcess.ProcessType == CoreConstants.ProcessType.ELIMINATION_GFR)
            return SystemicProcessTypes.GFR;

         if (flatProcess.ProcessType == CoreConstants.ProcessType.ELIMINATION)
            return SystemicProcessTypes.Renal;

         if (flatProcess.ProcessType == CoreConstants.ProcessType.SECRETION)
            return SystemicProcessTypes.Biliary;

         if (flatProcess.ProcessType == CoreConstants.ProcessType.METABOLIZATION)
            return SystemicProcessTypes.Hepatic;

         throw new ArgumentException($"Systemic process type for process '{flatProcess.Name}' of type '{flatProcess.ProcessType}' unknown.");
      }

      private TProcess createProcessBaseOn<TProcess>(FlatProcess flatProcess) where TProcess : CompoundProcess
      {
         var newProcess = _entityBaseFactory.Create<TProcess>();
         var repInfo = _representationInfoRepository.InfoFor(RepresentationObjectType.PROCESS, flatProcess.Name);
         newProcess.Name = flatProcess.Name;
         newProcess.InternalName = flatProcess.Name;
         newProcess.Description = repInfo.Description;
         newProcess.Icon = repInfo.IconName;
         setDefaultSpeciesForProcess(newProcess as ISpeciesDependentCompoundProcess);
         return newProcess;
      }

      private TInteractionProcess createInteractionProcess<TInteractionProcess>(FlatProcess flatProcess) where TInteractionProcess : InteractionProcess
      {
         var interactionProcess = createProcessBaseOn<TInteractionProcess>(flatProcess);
         interactionProcess.InteractionType = mapInteractionTypeFrom(flatProcess);
         return interactionProcess;
      }

      private InteractionType mapInteractionTypeFrom(FlatProcess flatProcess)
      {
         return EnumHelper.ParseValue<InteractionType>(flatProcess.ProcessType);
      }
   }
}