using OSPSuite.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Presenters
{
   public interface ICloneBuildingBlockPresenter : IDisposablePresenter
   {
      IPKSimBuildingBlock CreateCloneFor(IPKSimBuildingBlock buildingBlockToClone);
   }

   public class CloneBuildingBlockPresenter : AbstractClonePresenter<IPKSimBuildingBlock>, ICloneBuildingBlockPresenter
   {
      private readonly ICloner _cloner;

      public CloneBuildingBlockPresenter(
         IObjectBaseView view, 
         IObjectTypeResolver objectTypeResolver, 
         IRenameObjectDTOFactory renameObjectBaseDTOFactory, 
         ICloner cloner, 
         IOSPSuiteExecutionContext executionContext)
         : base(view, objectTypeResolver, renameObjectBaseDTOFactory, executionContext)
      {
         _cloner = cloner;
      }

      protected override IPKSimBuildingBlock Clone(IPKSimBuildingBlock buildingBlock)
      {
         return _cloner.Clone(buildingBlock);
      }
   }
}