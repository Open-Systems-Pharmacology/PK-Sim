using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatGenderRepository : IMetaDataRepository<FlatGender>
   {
   }

   public class FlatGenderRepository : MetaDataRepository<FlatGender>, IFlatGenderRepository
   {
      public FlatGenderRepository(IDbGateway dbGateway, IDataTableToMetaDataMapper<FlatGender> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.ViewGenders)
      {
      }
   }

   public class GenderRepository : StartableRepository<Gender>, IGenderRepository
   {
      private readonly IFlatGenderRepository _flatGenderRepository;
      private readonly IFlatGenderToGenderMapper _genderMapper;
      private readonly IRepresentationInfoRepository _representationInfoRepository;
      private IReadOnlyList<Gender> _allGenders;

      public GenderRepository(IFlatGenderRepository flatGenderRepository, IFlatGenderToGenderMapper genderMapper, IRepresentationInfoRepository representationInfoRepository)
      {
         _flatGenderRepository = flatGenderRepository;
         _genderMapper = genderMapper;
         _representationInfoRepository = representationInfoRepository;
      }

      public override IEnumerable<Gender> All()
      {
         Start();
         return _allGenders;
      }

      protected override void DoStart()
      {
         var flatGenders = _flatGenderRepository.All();
         _allGenders = flatGenders.MapAllUsing(_genderMapper);
         _allGenders.Each(updateDisplayInfo);
      }

      private void updateDisplayInfo(Gender gender)
      {
         var representationInfo = _representationInfoRepository.InfoFor(gender);
         gender.Description = representationInfo.Description;
         gender.DisplayName = representationInfo.DisplayName;
      }

      public Gender Male => this.FindByName(CoreConstants.Gender.Male);

      public Gender Female => this.FindByName(CoreConstants.Gender.Female);

      public Gender FindByIndex(int index)
      {
         return All().FirstOrDefault(x => x.Index == index);
      }
   }
}