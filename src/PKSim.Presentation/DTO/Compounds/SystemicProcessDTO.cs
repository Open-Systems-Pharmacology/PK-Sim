using System.Collections.Generic;
using PKSim.Assets;
using OSPSuite.Utility.Validation;
using PKSim.Core;
using PKSim.Core.Model;
using OSPSuite.Core.Extensions;

namespace PKSim.Presentation.DTO.Compounds
{
   public class SystemicProcessDTO : CompoundProcessDTO
   {
      public string SystemicProcessType { get; set; }

      public SystemicProcessDTO(CompoundProcess process) : base(process)
      {
         Rules.AddRange(SystemicProcessRules.All());
      }

      public override string Name
      {
         get { return createName(DataSource); }
      }

      private string createName(string dataSource)
      {
         var trimDataSource = string.IsNullOrEmpty(dataSource) ? dataSource : dataSource.Trim();
         return CoreConstants.ContainerName.PartialProcessName(SystemicProcessType, trimDataSource);
      }

      private static class SystemicProcessRules
      {
         private static IBusinessRule dataSourceNotEmpty
         {
            get
            {
               return CreateRule.For<SystemicProcessDTO>()
                  .Property(item => item.DataSource)
                  .WithRule((dto, dataSource) => dataSource.StringIsNotEmpty())
                  .WithError(PKSimConstants.Error.DataSourceIsRequired);
            }
         }

         private static IBusinessRule dataSourceValid
         {
            get
            {
               return CreateRule.For<SystemicProcessDTO>()
                  .Property(item => item.DataSource)
                  .WithRule((dto, dataSource) => dto.dataSourceValid(dataSource))
                  .WithError((dto, dataSource) => PKSimConstants.Error.NameAlreadyExistsInContainerType(dataSource, PKSimConstants.ObjectTypes.Compound));
            }
         }

         internal static IEnumerable<IBusinessRule> All()
         {
            yield return dataSourceNotEmpty;
            yield return dataSourceValid;
         }
      }

      private bool dataSourceValid(string dataSource)
      {
         return !Compound.ProcessExistsByName(createName(dataSource));
      }
   }
}