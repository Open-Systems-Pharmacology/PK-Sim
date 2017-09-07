using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Utility;
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

      public override SnapshotCompound MapToSnapshot(ModelCompound compound)
      {
         return SnapshotFrom(compound, snapshot =>
         {
            snapshot.CalculationMethods = _calculationMethodCacheMapper.MapToSnapshot(compound.CalculationMethodCache);

            snapshot.Lipophilicity = mapAlternatives(compound, COMPOUND_LIPOPHILICITY);
            snapshot.FractionUnbound = mapAlternatives(compound, COMPOUND_FRACTION_UNBOUND);
            snapshot.Solubility = mapAlternatives(compound, COMPOUND_SOLUBILITY);
            snapshot.IntestinalPermeability = mapAlternatives(compound, COMPOUND_INTESTINAL_PERMEABILITY);
            snapshot.Permeability = mapAlternatives(compound, COMPOUND_PERMEABILITY);

            snapshot.PkaTypes = mapPkaTypes(compound);
            snapshot.Processes = mapProcesses(compound);
            snapshot.IsSmallMolecule = compound.IsSmallMolecule;
            snapshot.PlasmaProteinBindingPartner = compound.PlasmaProteinBindingPartner.ToString();
         });
      }

      public override ModelCompound MapToModel(SnapshotCompound snapshot)
      {
         var compound = _compoundFactory.Create();
         MapSnapshotPropertiesToModel(snapshot, compound);
         _calculationMethodCacheMapper.UpdateCalculationMethodCache(compound, snapshot.CalculationMethods);
         
         updateAlternatives(compound, snapshot.Lipophilicity, COMPOUND_LIPOPHILICITY);
         updateAlternatives(compound, snapshot.FractionUnbound, COMPOUND_FRACTION_UNBOUND);
         updateAlternatives(compound, snapshot.Solubility, COMPOUND_SOLUBILITY);
         updateAlternatives(compound, snapshot.IntestinalPermeability, COMPOUND_INTESTINAL_PERMEABILITY);
         updateAlternatives(compound, snapshot.Permeability, COMPOUND_PERMEABILITY);

         updatePkaTypes(compound, snapshot);
         compound.AddChildren(snapshot.Processes.Select(_processMapper.MapToModel));
         compound.IsSmallMolecule = snapshot.IsSmallMolecule;
         compound.PlasmaProteinBindingPartner = EnumHelper.ParseValue<PlasmaProteinBindingPartner>(snapshot.PlasmaProteinBindingPartner);
         UpdateParametersFromSnapshot(snapshot, compound, PKSimConstants.ObjectTypes.Compound);

         return compound;
      }

      private void updateAlternatives(ModelCompound compound, List<Alternative> snapshotAlternatives, string alternativeGroupName)
      {
         var alternativeGroup = compound.ParameterAlternativeGroup(alternativeGroupName);

         //Remove all alternatives except calculated ones
         alternativeGroup.AllAlternatives.ToList().Where(x=>!x.IsCalculated).Each(alternativeGroup.RemoveAlternative);
         alternativeGroup.AddChildren(snapshotAlternatives.Select(x=>_alternativeMapper.MapToModel(x, alternativeGroup)));
      }

      private void updatePkaTypes(ModelCompound compound, SnapshotCompound snapshot)
      {
         snapshot.PkaTypes.Each((pkaType, i) =>
         {
            var compoundType = EnumHelper.ParseValue<CompoundType>(pkaType.Type);
            compound.Parameter(ParameterCompoundType(i)).Value = (int) compoundType;
            compound.Parameter(ParameterPKa(i)).Value = pkaType.Pka;
         });
      }

      private List<CompoundProcess> mapProcesses(ModelCompound compound)
      {
         return compound.AllProcesses().Select(_processMapper.MapToSnapshot).ToList();
      }

      private List<PkaType> mapPkaTypes(ModelCompound compound)
      {
         var pkaTypes = new List<PkaType>();

         for (int i = 0; i < CoreConstants.NUMBER_OF_PKA_PARAMETERS; i++)
         {
            var compoundTypeParameter = compound.Parameter(ParameterCompoundType(i));
            var pkA = compound.Parameter(ParameterPKa(i)).Value;
            var compoundType = (CompoundType) compoundTypeParameter.Value;
            if (compoundType == CompoundType.Neutral)
               continue;

            pkaTypes.Add(new PkaType {Pka = pkA, Type = compoundType.ToString()});
         }

         return pkaTypes;
      }

      protected override void AddModelParametersToSnapshot(ModelCompound compound, SnapshotCompound snapshot)
      {
         var parameters = parameterOverwrittenByUserIn(compound);
         AddParametersToSnapshot(parameters, snapshot);
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
            .Where(ParameterHasChanged);
      }

      private List<Alternative> mapAlternatives(ModelCompound compound, string alternativeGroupName)
      {
         var alternativeGroup = compound.ParameterAlternativeGroup(alternativeGroupName);
         return alternativeGroup.AllAlternatives.Select(_alternativeMapper.MapToSnapshot)
            .Where(x => x != null).ToList();
      }
   }
}