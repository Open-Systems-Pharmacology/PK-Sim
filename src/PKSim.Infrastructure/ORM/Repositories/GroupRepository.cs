using System.Collections.Generic;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;
using OSPSuite.Core.Domain;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public class GroupRepository : StartableRepository<IGroup>, IGroupRepository
   {
      private readonly IFlatGroupRepository _flatGroupRepository;
      private readonly IFlatGroupToGroupMapper _groupMapper;
      private readonly ICache<string, IGroup> _allGroupsByName;
      private readonly ICache<string, IGroup> _allGroupsById;

      public GroupRepository(IFlatGroupRepository flatGroupRepository, IFlatGroupToGroupMapper groupMapper)
      {
         _flatGroupRepository = flatGroupRepository;
         _groupMapper = groupMapper;
         _allGroupsByName = new Cache<string, IGroup>(x => x.Name, onMissingKey: retrieveUndefinedGroup);
         _allGroupsById = new Cache<string, IGroup>(x => x.Id, onMissingKey: retrieveUndefinedGroup);
      }

      private IGroup retrieveUndefinedGroup(string groupName)
      {
         return _allGroupsByName[Constants.Groups.UNDEFINED];
      }

      public override IEnumerable<IGroup> All()
      {
         Start();
         return _allGroupsByName;
      }

      protected override void DoStart()
      {
         _flatGroupRepository.Where(g => string.IsNullOrEmpty(g.ParentGroup))
            .Each(g => addGroup(g));
      }

      public IGroup GroupByName(string groupName)
      {
         Start();
         return _allGroupsByName[groupName];
      }

      public IGroup GroupById(string groupId)
      {
         Start();
         return _allGroupsById[groupId];
      }

      public void Clear()
      {
         _allGroupsByName.Clear();
         _allGroupsById.Clear();
      }

      public void AddGroup(IGroup group)
      {
         _allGroupsByName.Add(group);
         _allGroupsById.Add(group);
      }

      private IGroup addGroup(FlatGroup flatGroup)
      {
         var group = _groupMapper.MapFrom(flatGroup);
         AddGroup(group);
         foreach (var childGroup in _flatGroupRepository.Where(g => g.ParentGroup == flatGroup.Name))
         {
            group.AddChild(addGroup(childGroup));
         }

         return group;
      }
   }
}