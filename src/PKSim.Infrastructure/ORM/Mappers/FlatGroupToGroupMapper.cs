using System.Globalization;
using OSPSuite.Utility;
using PKSim.Infrastructure.ORM.FlatObjects;
using OSPSuite.Core.Domain;

namespace PKSim.Infrastructure.ORM.Mappers
{
   public interface IFlatGroupToGroupMapper : IMapper<FlatGroup, IGroup>
   {
   }

   public class FlatGroupToGroupMapper : IFlatGroupToGroupMapper
   {
      public IGroup MapFrom(FlatGroup flatGroup)
      {
         return new Group
            {
               Name = flatGroup.Name,
               DisplayName = flatGroup.DisplayName,
               Description = flatGroup.Description,
               IconName = flatGroup.IconName,
               Visible = flatGroup.Visible,
               Sequence = flatGroup.Sequence,
               IsAdvanced = flatGroup.IsAdvanced,
               FullName = flatGroup.FullName,
               Id = flatGroup.Id.ToString(CultureInfo.InvariantCulture),
               PopDisplayName = string.IsNullOrEmpty(flatGroup.PopDisplayName) ? flatGroup.DisplayName : flatGroup.PopDisplayName
            };
      }
   }
}