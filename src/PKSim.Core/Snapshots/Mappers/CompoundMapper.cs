using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using SnapshotCompound = PKSim.Core.Snapshots.Compound;
using ModelCompound = PKSim.Core.Model.Compound;

namespace PKSim.Core.Snapshots.Mappers
{
   public class CompoundMapper : ParameterContainerSnapshotMapperBase<ModelCompound, SnapshotCompound>
   {
      private readonly AlternativeMapper _alternativeMapper;
      private readonly CalculationMethodMapper _calculationMethodMapper;
      private readonly CompoundProcessMapper _processMapper;

      public CompoundMapper(ParameterMapper parameterMapper, 
         AlternativeMapper alternativeMapper, 
         CalculationMethodMapper calculationMethodMapper,
         CompoundProcessMapper processMapper) : base(parameterMapper)
      {
         _alternativeMapper = alternativeMapper;
         _calculationMethodMapper = calculationMethodMapper;
         _processMapper = processMapper;
      }

      public override SnapshotCompound MapToSnapshot(ModelCompound compound)
      {
         return SnapshotFrom(compound, snapshot =>
         {
            snapshot.CalculationMethods = _calculationMethodMapper.MapToSnapshot(compound.CalculationMethodCache);
            snapshot.Lipophilicity = mapAlternatives(compound, CoreConstants.Groups.COMPOUND_LIPOPHILICITY);
            snapshot.FractionUnbound = mapAlternatives(compound, CoreConstants.Groups.COMPOUND_FRACTION_UNBOUND);
            snapshot.Solubility = mapAlternatives(compound, CoreConstants.Groups.COMPOUND_SOLUBILITY);
            snapshot.IntestinalPermeability = mapAlternatives(compound, CoreConstants.Groups.COMPOUND_INTESTINAL_PERMEABILITY);
            snapshot.Permeability = mapAlternatives(compound, CoreConstants.Groups.COMPOUND_PERMEABILITY);
            snapshot.PkaTypes = mapPkaTypes(compound);
            snapshot.Processes = mapProcesses(compound);
            snapshot.IsSmallMolecule = compound.IsSmallMolecule;
            snapshot.PlasmaProteinBindingPartner = compound.PlasmaProteinBindingPartner.ToString();
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
            var compoundTypeParameter = compound.Parameter(CoreConstants.Parameter.ParameterCompoundType(i));
            var pkA = compound.Parameter(CoreConstants.Parameter.ParameterPKa(i)).Value;
            var compoundType = (CompoundType) compoundTypeParameter.Value;
            if (compoundType == CompoundType.Neutral)
               continue;

            pkaTypes.Add(new PkaType {Pka = pkA, Type = compoundType.ToString()});
         }

         return pkaTypes;
      }

      public override ModelCompound MapToModel(SnapshotCompound snapshot)
      {
         throw new NotImplementedException();
      }

      protected override void MapModelParameters(ModelCompound compound, SnapshotCompound snapshot)
      {
         var parameters = parameterOverwrittenByUserIn(compound);
         MapParameters(parameters, snapshot);
      }

      private IReadOnlyList<IParameter> parameterOverwrittenByUserIn(ModelCompound compound)
      {
         var parameters = new List<IParameter>();
         //Molecular Weight
         parameters.AddRange(changedGroupParameters(compound, CoreConstants.Groups.COMPOUND_MW));

         //advanced parameters
         parameters.AddRange(changedGroupParameters(compound, CoreConstants.Groups.COMPOUND_TWO_PORE));
         parameters.AddRange(changedGroupParameters(compound, CoreConstants.Groups.COMPOUND_DISSOLUTION));

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
            .Where(x=>x!=null).ToList();
      }
   }
}