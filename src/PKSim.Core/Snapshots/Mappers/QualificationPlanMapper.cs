using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using SnapshotQualificationPlan = PKSim.Core.Snapshots.QualificationPlan;
using ModelQualificationPlan = PKSim.Core.Model.QualificationPlan;

namespace PKSim.Core.Snapshots.Mappers
{
   public class QualificationPlanMapper : ObjectBaseSnapshotMapperBase<ModelQualificationPlan, SnapshotQualificationPlan, PKSimProject>
   {
      private readonly QualificationStepMapper _qualificationStepMapper;
      private readonly IObjectBaseFactory _objectBaseFactory;

      public QualificationPlanMapper(QualificationStepMapper qualificationStepMapper, IObjectBaseFactory objectBaseFactory)
      {
         _qualificationStepMapper = qualificationStepMapper;
         _objectBaseFactory = objectBaseFactory;
      }

      public override async Task<SnapshotQualificationPlan> MapToSnapshot(ModelQualificationPlan qualificationPlan)
      {
         var snaphsot = await SnapshotFrom(qualificationPlan);
         snaphsot.Steps = await _qualificationStepMapper.MapToSnapshots(qualificationPlan.Steps);
         return snaphsot;
      }

      public override async Task<ModelQualificationPlan> MapToModel(SnapshotQualificationPlan snapshot, PKSimProject project)
      {
         var qualificationPlan = _objectBaseFactory.Create<ModelQualificationPlan>();
         MapSnapshotPropertiesToModel(snapshot, qualificationPlan);

         var qualificationSteps = await _qualificationStepMapper.MapToModels(snapshot.Steps, project);
         qualificationSteps?.Each(qualificationPlan.Add);
         return qualificationPlan;
      }
   }
}