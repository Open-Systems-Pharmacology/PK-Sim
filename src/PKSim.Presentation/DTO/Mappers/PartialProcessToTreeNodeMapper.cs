using OSPSuite.Presentation.Nodes;
using OSPSuite.Utility;
using PKSim.Core.Model;
using PKSim.Presentation.Nodes;
using OSPSuite.Assets;
using ITreeNodeFactory = PKSim.Presentation.Nodes.ITreeNodeFactory;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface IPartialProcessToTreeNodeMapper : IMapper<PartialProcess, ITreeNode>
   {
   }

   public class PartialProcessToTreeNodeMapper : IPartialProcessToTreeNodeMapper
   {
      private readonly ITreeNodeFactory _treeNodeFactory;

      public PartialProcessToTreeNodeMapper(ITreeNodeFactory treeNodeFactory)
      {
         _treeNodeFactory = treeNodeFactory;
      }

      public ITreeNode MapFrom(PartialProcess partialProcess)
      {
         //protein id should be created using the process type as well since the same protein might be used for e.g transporter and metabolism
         var moleculeNode = _treeNodeFactory.CreateFor(partialProcess, partialProcess.MoleculeName);
         moleculeNode.AddChild(_treeNodeFactory.CreateFor(partialProcess).WithIcon(ApplicationIcons.IconByName(partialProcess.Icon)));
         return moleculeNode;
      }
   }
}