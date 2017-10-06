using System.Threading.Tasks;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using SnapshotCompoundProcess = PKSim.Core.Snapshots.CompoundProcess;
using ModelCompoundProcess = PKSim.Core.Model.CompoundProcess;

namespace PKSim.Core.Snapshots.Mappers
{
   public class CompoundProcessMapper : ParameterContainerSnapshotMapperBase<ModelCompoundProcess, SnapshotCompoundProcess>
   {
      private readonly IRepresentationInfoRepository _representationInfoRepository;
      private readonly ICompoundProcessRepository _compoundProcessRepository;
      private readonly ICloner _cloner;
      private readonly ISpeciesRepository _speciesRepository;
      private readonly ICompoundProcessTask _compoundProcessTask;

      public CompoundProcessMapper(ParameterMapper parameterMapper,
         IRepresentationInfoRepository representationInfoRepository,
         ICompoundProcessRepository compoundProcessRepository,
         ICloner cloner,
         ISpeciesRepository speciesRepository,
         ICompoundProcessTask compoundProcessTask) : base(parameterMapper)
      {
         _representationInfoRepository = representationInfoRepository;
         _compoundProcessRepository = compoundProcessRepository;
         _cloner = cloner;
         _speciesRepository = speciesRepository;
         _compoundProcessTask = compoundProcessTask;
      }

      public override Task<SnapshotCompoundProcess> MapToSnapshot(ModelCompoundProcess compoundProcess)
      {
         return SnapshotFrom(compoundProcess, snapshot =>
         {
            //name is never saved for processes as it is a field calculated programatically
            snapshot.Name = null;
            snapshot.Description = descriptionFor(compoundProcess);
            snapshot.InternalName = compoundProcess.InternalName;
            snapshot.DataSource = compoundProcess.DataSource;
            snapshot.Species = speciesNameFor(compoundProcess);
            snapshot.Molecule = moleculeNameFor(compoundProcess);
            snapshot.Metabolite = metaboliteNameFor(compoundProcess);
         });
      }

      private string descriptionFor(ModelCompoundProcess compoundProcess)
      {
         var databaseDescription = _representationInfoRepository.DescriptionFor(RepresentationObjectType.PROCESS, compoundProcess.InternalName);
         return string.Equals(databaseDescription, compoundProcess.Description) ? null : compoundProcess.Description;
      }

      private string metaboliteNameFor(ModelCompoundProcess process) => SnapshotValueFor((process as EnzymaticProcess)?.MetaboliteName);

      private string moleculeNameFor(ModelCompoundProcess process) => SnapshotValueFor((process as PartialProcess)?.MoleculeName);

      private string speciesNameFor(ModelCompoundProcess pro) => SnapshotValueFor((pro as ISpeciesDependentCompoundProcess)?.Species.Name);

      public override async Task<ModelCompoundProcess> MapToModel(SnapshotCompoundProcess snapshot)
      {
         var process = await retrieveProcessFrom(snapshot);
         if (!string.IsNullOrEmpty(snapshot.Description))
            process.Description = snapshot.Description;

         updatePartialProcessProperties(process as PartialProcess, snapshot);
         process.RefreshName();

         return process;
      }

      private void updatePartialProcessProperties(PartialProcess partialProcess, SnapshotCompoundProcess snapshot)
      {
         if (partialProcess == null)
            return;

         partialProcess.MoleculeName = snapshot.Molecule;

         if (partialProcess is EnzymaticProcess enzymaticProcess)
            enzymaticProcess.MetaboliteName = snapshot.Metabolite;
      }

      private async Task<ModelCompoundProcess> retrieveProcessFrom(SnapshotCompoundProcess snapshot)
      {
         var template = _compoundProcessRepository.ProcessByName(snapshot.InternalName);
         if (template == null)
            throw new SnapshotOutdatedException(PKSimConstants.Error.SnapshotProcessNameNotFound(snapshot.InternalName));

         var process = _cloner.Clone(template);
         process.DataSource = snapshot.DataSource;

         if (process.IsAnImplementationOf<ISpeciesDependentCompoundProcess>())
            updateSpeciesDependentParameter(process, snapshot);

         await UpdateParametersFromSnapshot(snapshot, process, process.InternalName);

         return process;
      }

      private void updateSpeciesDependentParameter(ModelCompoundProcess process, SnapshotCompoundProcess snapshot)
      {
         if (string.IsNullOrEmpty(snapshot.Species))
            return;

         var species = _speciesRepository.FindByName(snapshot.Species);
    
         _compoundProcessTask.SetSpeciesForProcess(process, species);
      }
   }
}