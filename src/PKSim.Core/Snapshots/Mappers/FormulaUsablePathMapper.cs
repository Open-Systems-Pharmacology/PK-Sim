using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;
using PKSim.Core.Repositories;
using SnapshotFormulaUsablePath = PKSim.Core.Snapshots.FormulaUsablePath;

namespace PKSim.Core.Snapshots.Mappers
{
   public class FormulaUsablePathMapper : SnapshotMapperBase<IFormulaUsablePath, SnapshotFormulaUsablePath>
   {
      private readonly IDimensionRepository _dimensionRepository;

      public FormulaUsablePathMapper(IDimensionRepository dimensionRepository)
      {
         _dimensionRepository = dimensionRepository;
      }

      public override Task<SnapshotFormulaUsablePath> MapToSnapshot(IFormulaUsablePath formulaUsablePath)
      {
         return SnapshotFrom(formulaUsablePath, x =>
         {
            x.Alias = formulaUsablePath.Alias;
            x.Dimension = formulaUsablePath.Dimension?.Name;
            x.Path = formulaUsablePath.PathAsString;
         });
      }

      public override Task<IFormulaUsablePath> MapToModel(SnapshotFormulaUsablePath snapshot)
      {
         IFormulaUsablePath formulaUsablePath;
         if (snapshot.Alias == Constants.TIME)
            formulaUsablePath = new TimePath();
         else
         {
            formulaUsablePath = new OSPSuite.Core.Domain.FormulaUsablePath(snapshot.Path.ToPathArray())
            {
               Alias = snapshot.Alias,
               Dimension = _dimensionRepository.DimensionByName(snapshot.Dimension),
            };
         }

         return Task.FromResult(formulaUsablePath);
      }
   }
}