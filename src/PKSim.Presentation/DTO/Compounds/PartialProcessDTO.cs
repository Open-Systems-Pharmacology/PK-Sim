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
         get => _moleculeName;
         set => SetProperty(ref _moleculeName, value.TrimmedValue());
      }

      public override string Name => createName(MoleculeName, DataSource);

      private string createName(string proteinName, string dataSource) => CoreConstants.CompositeNameFor(proteinName, dataSource);

      private static class PartialProcessRules
      {
         private static IBusinessRule moleculeNotEmpty
         {
            get { return GenericRules.NonEmptyRule<PartialProcessDTO>(x => x.MoleculeName, PKSimConstants.Error.MoleculeIsRequired); }
         }

         private static IBusinessRule dataSourceNotEmpty
         {
            get { return GenericRules.NonEmptyRule<PartialProcessDTO>(x => x.DataSource, PKSimConstants.Error.DataSourceIsRequired); }
         }

         private static IBusinessRule moleculeNameValid
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
            yield return moleculeNotEmpty;
            yield return moleculeNameValid;
            yield return dataSourceNotEmpty;
            yield return dataSourceValid;
         }
      }

      private bool proteinDataSourceValid(string moleculeName, string dataSource)
      {
         return !Compound.ProcessExistsByName(createName(moleculeName, dataSource));
      }
   }
}