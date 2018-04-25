using System;
using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public class CompoundProcessRepository : StartableRepository<CompoundProcess>, ICompoundProcessRepository
   {
      private readonly IFlatProcessRepository _processRepository;
      private readonly IParameterContainerTask _parameterContainerTask;
      private readonly IFlatProcessToCompoundProcessMapper _flatProcessToCompoundProcessMapper;
      private readonly IGroupRepository _groupRepository;
      private List<CompoundProcess> _allCompoundProcesses;

      public CompoundProcessRepository(IFlatProcessRepository processRepository,
                                       IParameterContainerTask parameterContainerTask,
                                       IFlatProcessToCompoundProcessMapper flatProcessToCompoundProcessMapper,
                                       IGroupRepository groupRepository)
      {
         _processRepository = processRepository;
         _parameterContainerTask = parameterContainerTask;
         _flatProcessToCompoundProcessMapper = flatProcessToCompoundProcessMapper;
         _groupRepository = groupRepository;
      }

      public override IEnumerable<CompoundProcess> All()
      {
         Start();
         return _allCompoundProcesses;
      }

      protected override void DoStart()
      {
         _allCompoundProcesses = allCompoundProcesses().MapAllUsing(_flatProcessToCompoundProcessMapper).ToList();
         _allCompoundProcesses.Each(addParametersToTemplate);

      }

      private IEnumerable<FlatProcess> allCompoundProcesses()
      {
         var compoundProcessGroup = _groupRepository.GroupByName(CoreConstants.Groups.COMPOUND_PROCESSES);
         return _processRepository.Where(proc => compoundProcessGroup.ContainsGroup(proc.GroupName));
      }

      private void addParametersToTemplate(CompoundProcess compoundProcess)
      {
         _parameterContainerTask.AddActiveProcessParametersTo(compoundProcess);
      }

      public IEnumerable<TProcessType> All<TProcessType>() where TProcessType : CompoundProcess
      {
         Start();
         return _allCompoundProcesses.OfType<TProcessType>();
      }

      public CompoundProcess ProcessByName(string processTemplateName)
      {
         Start();
         return All().FindByName(processTemplateName);
      }

      public TProcessType ProcessByName<TProcessType>(string processTemplateName) where TProcessType : CompoundProcess
      {
         return ProcessByName(processTemplateName).DowncastTo<TProcessType>();
      }
   }
}