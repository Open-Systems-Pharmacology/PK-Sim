using System.Collections.Generic;
using System.Data;
using System.Linq;
using OSPSuite.Infrastructure.Reporting;
using OSPSuite.TeXReporting.Builder;
using OSPSuite.TeXReporting.Items;
using OSPSuite.TeXReporting.TeX.Converter;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Infrastructure.Reporting.TeX.Builders
{
   public class UsedBuildingBlockTeXBuilder : OSPSuiteTeXBuilder<IEnumerable<UsedBuildingBlock>>
   {
      private readonly ITeXBuilderRepository _builderRepository;
      private readonly IBuildingBlockRepository _buildingBlockRepository;

      public UsedBuildingBlockTeXBuilder(ITeXBuilderRepository builderRepository, IBuildingBlockRepository buildingBlockRepository)
      {
         _builderRepository = builderRepository;
         _buildingBlockRepository = buildingBlockRepository;
      }

      public override void Build(IEnumerable<UsedBuildingBlock> usedBuildingBlocks, OSPSuiteTracker buildTracker)
      {
         var table = new DataTable();
         table.AddColumn(PKSimConstants.UI.BuildingBlock);
         table.AddColumn<Text>(PKSimConstants.UI.Name);

         foreach (var usedBuildingBlock in usedBuildingBlocks)
         {
            var row = table.NewRow();
            var defaultText = new Text(usedBuildingBlock.Name);
            row[PKSimConstants.UI.BuildingBlock] = usedBuildingBlock.BuildingBlockType.ToString();

            var templateBuildingBlock = _buildingBlockRepository.ById(usedBuildingBlock.TemplateId);
            //should never happen
            if (templateBuildingBlock == null)
               row[PKSimConstants.UI.Name] = defaultText;

            else if (templateBuildingBlock.Version == usedBuildingBlock.Version)
            {
               var reference = buildTracker.ReferenceFor(templateBuildingBlock);
               if (reference == null)
                  row[PKSimConstants.UI.Name] = defaultText;
               else
               {
                  var partBuildingBlocks = getReferenceForPartBuildingBlocks(buildTracker);
                  row[PKSimConstants.UI.Name] = new Text("{0} (see {1} in {2})", defaultText,
                                                         reference, partBuildingBlocks) {Converter = NoConverter.Instance};
               }
            }

            else
               row[PKSimConstants.UI.Name] = defaultText;

            table.Rows.Add(row);
         }

         _builderRepository.Report(new Table(table.DefaultView, PKSimConstants.UI.BuildingBlock), buildTracker);
      }

      private Reference getReferenceForPartBuildingBlocks(OSPSuiteTracker buildTracker)
      {
         return (from part in buildTracker.TrackedObjects.OfType<Part>() 
                 where part.Name == PKSimConstants.UI.BuildingBlocks 
                 select new Reference(part)).FirstOrDefault();
      }
   }
}