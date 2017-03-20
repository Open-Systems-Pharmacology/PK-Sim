using System.Collections.Generic;
using System.Linq;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Services
{
   public interface IParameterGroupTask
   {
      /// <summary>
      ///    Returns all the top groups used by the parameters
      /// </summary>
      /// <param name="allParameters">Parameters for which the used groups should be retrieved</param>
      IEnumerable<IGroup> TopGroupsUsedBy(IEnumerable<IParameter> allParameters);

      /// <summary>
      ///    Returns all the  groups used by parameters (only the groups actually containing parameters)
      /// </summary>
      /// <param name="allParameters">Parameters for which the used groups should be retrieved</param>
      IEnumerable<IGroup> GroupsUsedBy(IEnumerable<IParameter> allParameters);

      /// <summary>
      ///    Return for the given group the subset from <paramref name="allParameters" />  belonging to that group.
      /// </summary>
      /// <param name="group">Group for which the subset of parmeters should be retrieved</param>
      /// <param name="allParameters">all existing parameters (some of which may or may not belong to the group</param>
      IEnumerable<IParameter> ParametersIn(IGroup group, IEnumerable<IParameter> allParameters);

      /// <summary>
      ///    Returns for the given group the subset from <paramref name="allParameters" />  belonging to that group.
      /// </summary>
      /// <param name="groupName">Name of the group for which the subset of parmeters should be retrieved</param>
      /// <param name="allParameters">all existing parameters (some of which may or may not belong to the group</param>
      IEnumerable<IParameter> ParametersIn(string groupName, IEnumerable<IParameter> allParameters);

      /// <summary>
      ///    Returns for the given top group the subset from <paramref name="allParameters" />  belonging to that top group.
      ///    (e.g. there are in a group whose root is the group with the name topGroupName)
      /// </summary>
      /// <param name="group">Group for which the subset of parmeters should be retrieved</param>
      /// <param name="allParameters">all existing parameters (some of which may or may not belong to the group</param>
      IEnumerable<IParameter> ParametersInTopGroup(IGroup group, IEnumerable<IParameter> allParameters);

      /// <summary>
      ///    Returns for the given top group the subset from <paramref name="allParameters" />  belonging to that top group.
      ///    (e.g. there are in a group whose root is the group with the name topGroupName)
      /// </summary>
      /// <param name="topGroupName">Name of the top group for which the subset of parmeters should be retrieved</param>
      /// <param name="allParameters">all existing parameters (some of which may or may not belong to the group</param>
      IEnumerable<IParameter> ParametersInTopGroup(string topGroupName, IEnumerable<IParameter> allParameters);

      /// <summary>
      ///    Returns true if at least one parameter in <paramref name="allParameters" /> is child of the given group
      ///    <paramref
      ///       name="group" />
      /// </summary>
      bool GroupHasParameter(IGroup group, IEnumerable<IParameter> allParameters);
   }

   public class ParameterGroupTask : IParameterGroupTask
   {
      private readonly IGroupRepository _groupRepository;

      public ParameterGroupTask(IGroupRepository groupRepository)
      {
         _groupRepository = groupRepository;
      }

      public IEnumerable<IGroup> TopGroupsUsedBy(IEnumerable<IParameter> allParameters)
      {
         var groupIdUsed = allParameters
            .Select(param => param.GroupName)
            .Distinct();

         return groupIdUsed.Select(topGroupWithName).Distinct();
      }

      public IEnumerable<IGroup> GroupsUsedBy(IEnumerable<IParameter> allParameters)
      {
         var groupNameUsed = allParameters
            .Select(param => param.GroupName)
            .Distinct();

         return groupNameUsed.Select(name => _groupRepository.GroupByName(name));
      }

      public IEnumerable<IParameter> ParametersIn(IGroup parameterGroup, IEnumerable<IParameter> allParameters)
      {
         var dynamicGroup = parameterGroup as DynamicGroup;
         return dynamicGroup == null ? ParametersIn(parameterGroup.Name, allParameters) : dynamicGroup.Parameters;
      }

      public IEnumerable<IParameter> ParametersIn(string groupName, IEnumerable<IParameter> allParameters)
      {
         return from parameter in allParameters
            where parameter.GroupName.Equals(groupName)
            select parameter;
      }

      public IEnumerable<IParameter> ParametersInTopGroup(IGroup group, IEnumerable<IParameter> allParameters)
      {
         return ParametersInTopGroup(group.Name, allParameters);
      }

      public IEnumerable<IParameter> ParametersInTopGroup(string topGroupName, IEnumerable<IParameter> allParameters)
      {
         return from parameter in allParameters
            where groupBelongsIn(parameter.GroupName, topGroupName)
            select parameter;
      }

      public bool GroupHasParameter(IGroup group, IEnumerable<IParameter> allParameters)
      {
         var groupName = group.Name;
         return allParameters.Any(p => p.GroupName.Equals(groupName));
      }

      private bool groupBelongsIn(string groupName, string parentGroupName)
      {
         return _groupRepository.GroupByName(groupName).HasAncestorNamed(parentGroupName);
      }

      private IGroup topGroupWithName(string groupName)
      {
         return _groupRepository.GroupByName(groupName).Root;
      }
   }
}