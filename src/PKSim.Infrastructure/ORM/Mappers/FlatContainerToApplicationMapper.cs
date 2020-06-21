using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Descriptors;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Repositories;

namespace PKSim.Infrastructure.ORM.Mappers
{
   public interface IFlatContainerToApplicationMapper : IMapper<FlatContainer, IApplicationBuilder>
   {
   }

   public class FlatContainerToApplicationMapper : FlatContainerIdToContainerMapperBase<IApplicationBuilder>, IFlatContainerToApplicationMapper
   {
      private readonly IParameterContainerTask _parameterContainerTask;
      private readonly IApplicationTransportRepository _applicationTransportRepository;
      private readonly IEntityPathResolver _entityPathResolver;
      private readonly IFlatContainerIdToContainerMapper _flatContainerIdMapper;
      private readonly IFlatContainerToEventBuilderMapper _eventMapper;

      public FlatContainerToApplicationMapper(IParameterContainerTask parameterContainerTask,
         IApplicationTransportRepository applicationTransportRepository,
         IEntityPathResolver entityPathResolver,
         IFlatContainerRepository flatContainerRepository,
         IFlatContainerIdToContainerMapper flatContainerIdMapper,
         IObjectBaseFactory objectBaseFactory,
         IFlatContainerToEventBuilderMapper eventMapper, IFlatContainerTagRepository flatContainerTagRepository) : base(objectBaseFactory, flatContainerRepository, flatContainerTagRepository)
      {
         _parameterContainerTask = parameterContainerTask;
         _applicationTransportRepository = applicationTransportRepository;
         _entityPathResolver = entityPathResolver;
         _flatContainerIdMapper = flatContainerIdMapper;
         _eventMapper = eventMapper;
      }

      public IApplicationBuilder MapFrom(FlatContainer flatApplicationContainer)
      {
         var applicationBuilder = MapCommonPropertiesFrom(flatApplicationContainer);

         //Set molecule name to "DRUG".
         //Must be replaced during simulation building process
         applicationBuilder.MoleculeName = CoreConstants.Molecule.Drug;

         //Add application processes (e.g. infusion)
         addApplicationProcessesTo(applicationBuilder);

         //create parent formulation container of the application
         //(in order to resolve paths of application subobjects correctly)
         createParentFormulation(applicationBuilder, flatApplicationContainer.ParentId);

         //Add substructure (subcontainers with parameters and events)
         addApplicationStructureTo(applicationBuilder);

         //add application (=event group) source criteria
         //Each application will be settled under ApplicationSet-Container of
         //the simulation
         addApplicationSourceCriteria(applicationBuilder);

         return applicationBuilder;
      }

      private static void addApplicationSourceCriteria(IApplicationBuilder applicationBuilder)
      {
         applicationBuilder.SourceCriteria.Add(new MatchTagCondition(Constants.APPLICATIONS));
      }

      private void addEvents(IContainer container)
      {
         var flatEventSubContainers = flatSubContainersFor(container, x => x == CoreConstants.ContainerType.Event);

         foreach (var flatEventContainer in flatEventSubContainers)
         {
            container.Add(_eventMapper.MapFrom(flatEventContainer));
         }
      }

      private void createParentFormulation(IApplicationBuilder applicationBuilder, int? formulationContainerId)
      {
         var flatFormulation = _flatContainerRepository.ContainerFrom(formulationContainerId);
         var formulation = _objectBaseFactory.Create<IContainer>().WithName(flatFormulation.Name);
         formulation.Add(applicationBuilder);
      }

      private void addApplicationStructureTo(IContainer container)
      {
         _parameterContainerTask.AddApplicationParametersTo(container);

         addEvents(container);

         foreach (var subContainer in subContainersFor(container))
         {
            container.Add(subContainer);
            addApplicationStructureTo(subContainer);
         }
      }

      private IEnumerable<IContainer> subContainersFor(IContainer container)
      {
         var flatSubContainers = flatSubContainersFor(container,
            containerType =>
               containerType != CoreConstants.ContainerType.Event &&
               containerType != CoreConstants.ContainerType.Process);

         return flatSubContainers.MapAllUsing(_flatContainerIdMapper);
      }

      private IEnumerable<FlatContainer> flatSubContainersFor(IContainer container, Func<string, bool> containerTypeCondition)
      {
         var pathToParentContainer = _entityPathResolver.PathFor(container);

         //find parent container in repo
         var flatParentContainer = _flatContainerRepository.ContainerFrom(pathToParentContainer);

         return from flatContainer in _flatContainerRepository.All()
            let parentContainer = _flatContainerRepository.ParentContainerFrom(flatContainer.Id)
            where parentContainer != null
            where parentContainer.Id == flatParentContainer.Id
            where containerTypeCondition(flatContainer.Type)
            select flatContainer;
      }

      /// <summary>
      ///    Add application processes to the application builder
      /// </summary>
      private void addApplicationProcessesTo(IApplicationBuilder applicationBuilder)
      {
         var applicationProcess = _applicationTransportRepository.TransportsFor(applicationBuilder.Name);
         applicationProcess.Each(applicationBuilder.AddTransport);
      }
   }
}