using System.Collections.Generic;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Core.Reporting;
using PKSim.Presentation.Mappers;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Services;

namespace PKSim.Presentation.Nodes
{
   public class ToolTipPartCreator : IToolTipPartCreator
   {
      private readonly IReportGenerator _reportGenerator;
      private readonly IReportPartToToolTipPartsMapper _toolTipMapper;

      public ToolTipPartCreator(IReportGenerator reportGenerator, IReportPartToToolTipPartsMapper toolTipMapper)
      {
         _reportGenerator = reportGenerator;
         _toolTipMapper = toolTipMapper;
      }

      public IList<ToolTipPart> ToolTipFor<T>(T objectRequestingToolTip)
      {
         if (objectRequestingToolTip.IsAnImplementationOf<Individual>())
            return toolTipFor<Individual>(objectRequestingToolTip);

         if (objectRequestingToolTip.IsAnImplementationOf<Population>())
            return toolTipFor<Population>(objectRequestingToolTip);

         if (objectRequestingToolTip.IsAnImplementationOf<Compound>())
            return toolTipFor<Compound>(objectRequestingToolTip);

         if (objectRequestingToolTip.IsAnImplementationOf<Formulation>())
            return toolTipFor<Formulation>(objectRequestingToolTip);

         if (objectRequestingToolTip.IsAnImplementationOf<Protocol>())
            return toolTipFor<Protocol>(objectRequestingToolTip);

         if (objectRequestingToolTip.IsAnImplementationOf<PKSimEvent>())
            return toolTipFor<PKSimEvent>(objectRequestingToolTip);

         if (objectRequestingToolTip.IsAnImplementationOf<IndividualSimulationComparison>())
            return toolTipFor<IndividualSimulationComparison>(objectRequestingToolTip);

         if (objectRequestingToolTip.IsAnImplementationOf<PopulationSimulationComparison>())
            return toolTipFor<PopulationSimulationComparison>(objectRequestingToolTip);

         if (objectRequestingToolTip.IsAnImplementationOf<DataRepository>())
            return toolTipFor<DataRepository>(objectRequestingToolTip);

         if (objectRequestingToolTip.IsAnImplementationOf<IClassifiableWrapper>())
            return ToolTipFor(objectRequestingToolTip.DowncastTo<IClassifiableWrapper>().WrappedObject);

         return toolTipFor(objectRequestingToolTip);
      }

      private IList<ToolTipPart> toolTipFor<T>(object objectRequestingToolTip)
      {
         return toolTipFor(objectRequestingToolTip.DowncastTo<T>());
      }

      private IList<ToolTipPart> toolTipFor<T>(T objectRequestingToolTip)
      {
         return _toolTipMapper.MapFrom(_reportGenerator.ReportFor(objectRequestingToolTip));
      }

      public IList<ToolTipPart> ToolTipFor(string toolTipToDisplay)
      {
         return _toolTipMapper.MapFrom(toolTipToDisplay);
      }
   }
}