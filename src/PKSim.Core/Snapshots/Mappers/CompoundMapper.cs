using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using static PKSim.Core.CoreConstants.Groups;
using static PKSim.Core.CoreConstants.Parameter;
using SnapshotCompound = PKSim.Core.Snapshots.Compound;
using ModelCompound = PKSim.Core.Model.Compound;

namespace PKSim.Core.Snapshots.Mappers
{
   public class CompoundMapper : ParameterContainerSnapshotMapperBase<ModelCompound, SnapshotCompound>
   {
      private readonly AlternativeMapper _alternativeMapper;
      private readonly CalculationMethodCacheMapper _calculationMethodCacheMapper;
      private readonly CompoundProcessMapper _processMapper;
      private readonly ICompoundFactory _compoundFactory;

      public CompoundMapper(
         ParameterMapper parameterMapper,
         AlternativeMapper alternativeMapper,
         CalculationMethodCacheMapper calculationMethodCacheMapper,
         CompoundProcessMapper processMapper,
         ICompoundFactory compoundFactory) : base(parameterMapper)
      {
         _alternativeMapper = alternativeMapper;
         _calculationMethodCacheMapper = calculationMethodCacheMapper;
         _processMapper = processMapper;
         _compoundFactory = compoundFactory;
      }

      public override async Task<SnapshotCompound> MapToSnapshot(ModelCompound compound)
      {
         var snapshot = await SnapshotFrom(compound);
         snapshot.CalculationMethods = await _calculationMethodCacheMapper.MapToSnapshot(compound.CalculationMethodCache);
         snapshot.Lipophilicity = await mapAlternatives(compound, COMPOUND_LIPOPHILICITY);
         snapshot.FractionUnbound = await mapAlternatives(compound, COMPOUND_FRACTION_UNBOUND);
         snapshot.Solubility = await mapAlternatives(compound, COMPOUND_SOLUBILITY);
         snapshot.IntestinalPermeability = await mapAlternatives(compound, COMPOUND_INTESTINAL_PERMEABILITY);
         snapshot.Permeability = await mapAlternatives(compound, COMPOUND_PERMEABILITY);
         snapshot.PkaTypes = mapPkaTypes(compound);
         snapshot.Processes = await mapProcesses(compound);
         snapshot.IsSmallMolecule = SnapshotValueFor(compound.IsSmallMolecule, true);
         snapshot.PlasmaProteinBindingPartner = SnapshotValueFor(compound.PlasmaProteinBindingPartner, PlasmaProteinBindingPartner.Unknown);
         return snapshot;
      }

      public override async Task<ModelCompound> MapToModel(SnapshotCompound snapshot)
      {
         var compound = _compoundFactory.Create();
         MapSnapshotPropertiesToModel(snapshot, compound);
         _calculationMethodCacheMapper.UpdateCalculationMethodCache(compound, snapshot.CalculationMethods);

         await updateAlternatives(compound, snapshot.Lipophilicity, COMPOUND_LIPOPHILICITY);
         await updateAlternatives(compound, snapshot.FractionUnbound, COMPOUND_FRACTION_UNBOUND);
         await updateAlternatives(compound, snapshot.Solubility, COMPOUND_SOLUBILITY);
         await updateAlternatives(compound, snapshot.IntestinalPermeability, COMPOUND_INTESTINAL_PERMEABILITY);
         await updateAlternatives(compound, snapshot.Permeability, COMPOUND_PERMEABILITY);

         updatePkaTypes(compound, snapshot);

         await updateProcesses(snapshot, compound);
         await UpdateParametersFromSnapshot(snapshot, compound);

         return compound;
      }

      protected override void MapSnapshotPropertiesToModel(SnapshotCompound snapshot, ModelCompound compound)
      {
         base.MapSnapshotPropertiesToModel(snapshot, compound);
         compound.IsSmallMolecule = ModelValueFor(snapshot.IsSmallMolecule, true);
         compound.PlasmaProteinBindingPartner = ModelValueFor(snapshot.PlasmaProteinBindingPartner, PlasmaProteinBindingPartner.Unknown);
      }

      private async Task updateProcesses(SnapshotCompound snapshot, ModelCompound compound)
      {
         var proceses = await _processMapper.MapToModels(snapshot.Processes);
         proceses?.Each(compound.AddProcess);
      }

      private async Task updateAlternatives(ModelCompound compound, Alternative[] snapshotAlternatives, string alternativeGroupName)
      {
         if (snapshotAlternatives == null)
            return;

         var alternativeGroup = compound.ParameterAlternativeGroup(alternativeGroupName);

         //Remove all alternatives except calculated ones
         alternativeGroup.AllAlternatives.ToList().Where(x => !x.IsCalculated).Each(alternativeGroup.RemoveAlternative);

         var alternatives = await _alternativeMapper.MapToModels(snapshotAlternatives, alternativeGroup);

         alternatives?.Each(alternativeGroup.AddAlternative);
      }

      private void updatePkaTypes(ModelCompound compound, SnapshotCompound snapshot)
      {
         snapshot.PkaTypes?.Each((pkaType, i) =>
         {
            var compoundType = pkaType.Type;
            compound.Parameter(ParameterCompoundType(i)).Value = (int) compoundType;
            compound.Parameter(ParameterPKa(i)).Value = pkaType.Pka;
         });
      }

      private Task<CompoundProcess[]> mapProcesses(ModelCompound compound) => _processMapper.MapToSnapshots(compound.AllProcesses());

      private PkaType[] mapPkaTypes(ModelCompound compound)
      {
         var pkaTypes = new List<PkaType>();

         for (int i = 0; i < CoreConstants.NUMBER_OF_PKA_PARAMETERS; i++)
         {
            var compoundTypeParameter = compound.Parameter(ParameterCompoundType(i));
            var pkA = compound.Parameter(ParameterPKa(i)).Value;
            var compoundType = (CompoundType) compoundTypeParameter.Value;
            if (compoundType == CompoundType.Neutral)
               continue;

            pkaTypes.Add(new PkaType {Pka = pkA, Type = compoundType});
         }

         if (pkaTypes.Any())
            return pkaTypes.ToArray();

         return null;
      }

      protected override Task AddModelParametersToSnapshot(ModelCompound compound, SnapshotCompound snapshot)
      {
         var parameters = parameterOverwrittenByUserIn(compound);
         return AddParametersToSnapshot(parameters, snapshot);
      }

      private IReadOnlyList<IParameter> parameterOverwrittenByUserIn(ModelCompound compound)
      {
         var parameters = new List<IParameter>();
         //Molecular Weight
         parameters.AddRange(changedGroupParameters(compound, COMPOUND_MW));

         //advanced parameters
         parameters.AddRange(changedGroupParameters(compound, COMPOUND_TWO_PORE));
         parameters.AddRange(changedGroupParameters(compound, COMPOUND_DISSOLUTION));

         return parameters;
      }

      private IEnumerable<IParameter> changedGroupParameters(IContainer container, string groupName)
      {
         return container.AllParameters(x => string.Equals(x.GroupName, groupName))
            .Where(x => x.ParameterHasChanged());
      }

      private async Task<Alternative[]> mapAlternatives(ModelCompound compound, string alternativeGroupName)
      {
         var alternativeGroup = compound.ParameterAlternativeGroup(alternativeGroupName);
         var alteratives = await _alternativeMapper.MapToSnapshots(alternativeGroup.AllAlternatives);
         var definedAlternatives = alteratives?.ToArray();
         if (definedAlternatives == null || !definedAlternatives.Any())
            return null;

         return definedAlternatives;
      }
   }
}