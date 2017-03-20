using System.Collections.Generic;
using PKSim.Assets;
using OSPSuite.Utility.Validation;
using PKSim.Core;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;

namespace PKSim.Presentation.DTO.Compounds
{
   public class PartialProcessDTO : CompoundProcessDTO
   {
      private string _moleculeName;

      public PartialProcessDTO(CompoundProcess process) : base(process)
      {
         Rules.AddRange(PartialProcessRules.All());
      }

      public virtual string MoleculeName
      {
         get { return _moleculeName; }
         set
         {
            _moleculeName = value.TrimmedValue();
            OnPropertyChanged(() => MoleculeName);
         }
      }

      public override string Name
      {
         get { return createName(MoleculeName, DataSource); }
      }

      private string createName(string proteinName, string dataSource)
      {
         var trimProteinName = string.IsNullOrEmpty(proteinName) ? proteinName : proteinName.Trim();
         var trimDataSource = string.IsNullOrEmpty(dataSource) ? dataSource : dataSource.Trim();
         return CoreConstants.ContainerName.PartialProcessName(trimProteinName, trimDataSource);
      }

      private static class PartialProcessRules
      {
         private static IBusinessRule proteinNotEmpty
         {
            get { return GenericRules.NonEmptyRule<PartialProcessDTO>(x => x.MoleculeName, PKSimConstants.Error.MoleculeIsRequired); }
         }

         private static IBusinessRule dataSourceNotEmpty
         {
            get { return GenericRules.NonEmptyRule<PartialProcessDTO>(x => x.DataSource, PKSimConstants.Error.DataSourceIsRequired); }
         }

         private static IBusinessRule proteinNameValid
         {
            get
            {
               return CreateRule.For<PartialProcessDTO>()
                  .Property(item => item.MoleculeName)
                  .WithRule((dto, proteinName) => dto.proteinDataSourceValid(proteinName, dto.DataSource))
                  .WithError((dto, proteinName) => PKSimConstants.Error.NameAlreadyExistsInContainerType(proteinName, PKSimConstants.ObjectTypes.Compound));
            }
         }

         private static IBusinessRule dataSourceValid
         {
            get
            {
               return CreateRule.For<PartialProcessDTO>()
                  .Property(item => item.DataSource)
                  .WithRule((dto, dataSource) => dto.proteinDataSourceValid(dto.MoleculeName, dataSource))
                  .WithError((dto, dataSource) => PKSimConstants.Error.NameAlreadyExistsInContainerType(dataSource, PKSimConstants.ObjectTypes.Compound));
            }
         }

         internal static IEnumerable<IBusinessRule> All()
         {
            yield return proteinNotEmpty;
            yield return proteinNameValid;
            yield return dataSourceNotEmpty;
            yield return dataSourceValid;
         }
      }

      private bool proteinDataSourceValid(string proteinName, string dataSource)
      {
         return !Compound.ProcessExistsByName(createName(proteinName, dataSource));
      }
   }
}