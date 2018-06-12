using System;
using OSPSuite.Presentation.Presenters.Nodes;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Presentation.Nodes;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface IPartialProcessToRootNodeTypeMapper : IMapper<PartialProcess, RootNodeType>
   {
   }

   public class PartialProcessToRootNodeTypeMapper : IPartialProcessToRootNodeTypeMapper
   {
      public RootNodeType MapFrom(PartialProcess partialProcess)
      {
         var type = partialProcess.GetType();

         if (type.IsAnImplementationOf<EnzymaticProcess>())
            return PKSimRootNodeTypes.CompoundMetabolizingEnzymes;

         if (type.IsAnImplementationOf<SpecificBindingPartialProcess>())
            return PKSimRootNodeTypes.CompoundProteinBindingPartners;

         if (type.IsAnImplementationOf<InhibitionProcess>())
            return PKSimRootNodeTypes.InhibitionProcess;

         if (type.IsAnImplementationOf<InductionProcess>())
            return PKSimRootNodeTypes.InductionProcess;

         if (type.IsAnImplementationOf<TransportPartialProcess>())
            return PKSimRootNodeTypes.CompoundTransportProteins;

         throw new ArgumentException(partialProcess.ToString());
      }
   }
}