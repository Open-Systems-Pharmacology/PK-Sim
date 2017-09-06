using System;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using SnapshotCompoundProcess = PKSim.Core.Snapshots.CompoundProcess;
using ModelCompoundProcess = PKSim.Core.Model.CompoundProcess;

namespace PKSim.Core.Snapshots.Mappers
{
   public class CompoundProcessMapper : ParameterContainerSnapshotMapperBase<ModelCompoundProcess, SnapshotCompoundProcess>
   {
      private readonly IRepresentationInfoRepository _representationInfoRepository;

      public CompoundProcessMapper(ParameterMapper parameterMapper, IRepresentationInfoRepository representationInfoRepository) : base(parameterMapper)
      {
         _representationInfoRepository = representationInfoRepository;
      }

      public override SnapshotCompoundProcess MapToSnapshot(ModelCompoundProcess compoundProcess)
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

      private string metaboliteNameFor(ModelCompoundProcess process) => (process as EnzymaticProcess)?.MetaboliteName;

      private string moleculeNameFor(ModelCompoundProcess process) => (process as PartialProcess)?.MoleculeName;

      private string speciesNameFor(ModelCompoundProcess pro) => (pro as ISpeciesDependentCompoundProcess)?.Species.Name;

      public override ModelCompoundProcess MapToModel(SnapshotCompoundProcess snapshot)
      {
         throw new NotImplementedException();
      }
   }
}