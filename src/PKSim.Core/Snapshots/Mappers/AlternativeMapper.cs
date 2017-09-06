using PKSim.Core.Model;

namespace PKSim.Core.Snapshots.Mappers
{
   public class AlternativeMapper : ParameterContainerSnapshotMapperBase<ParameterAlternative, Alternative>
   {
      public AlternativeMapper(ParameterMapper parameterMapper) : base(parameterMapper)
      {
      }

      public override Alternative MapToSnapshot(ParameterAlternative parameterAlternative)
      {
         if (parameterAlternative.IsCalculated)
            return null;

         return SnapshotFrom(parameterAlternative, snapshot =>
         {
            snapshot.IsDefault = parameterAlternative.IsDefault;
            snapshot.Species = (parameterAlternative as ParameterAlternativeWithSpecies)?.Species.Name;
         });
      }

      public override ParameterAlternative MapToModel(Alternative snapshot)
      {
         throw new System.NotImplementedException();
      }
   }
}