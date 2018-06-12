using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model.PopulationAnalyses;
using OSPSuite.Core.Domain;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters;
using PKSim.Presentation.Views;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;

namespace PKSim.Presentation
{
   public abstract class concern_for_ApplicationSettingsPresenter : ContextSpecification<IApplicationSettingsPresenter>
   {
      protected IApplicationSettingsView _view;
      protected IApplicationSettings _applicationSettings;
      protected IDialogCreator _dialogCreator;
      protected ISpeciesRepository _speciesRepository;
      protected ISpeciesDatabaseMapToSpeciesDatabaseMapDTOMapper _speciesDatabaseDTOMapper;
      protected IApplicationSettingsPersistor _applicationSettingsPersistor;
      protected ApplicationSettingsDTO _dto;

      protected override void Context()
      {
         _view=A.Fake<IApplicationSettingsView>();
         _dialogCreator =A.Fake<IDialogCreator>();

         _applicationSettings = new ApplicationSettings();
         _speciesRepository = A.Fake<ISpeciesRepository>();
         A.CallTo(() => _speciesRepository.All()).Returns(new[]
                                                                   {
                                                                      new Species {Name = "Dog", Id = "Dog"},
                                                                      new Species {Name = "Human", Id = "Human"},
                                                                      new Species {Name = "Rat", Id = "Rat"}
                                                                   });
         _speciesDatabaseDTOMapper =new SpeciesDatabaseMapToSpeciesDatabaseMapDTOMapper(_speciesRepository, new RepresentationInfoRepositoryForSpecs());
         _applicationSettingsPersistor =A.Fake<IApplicationSettingsPersistor>();

         sut = new ApplicationSettingsPresenter(_view,_applicationSettings,_speciesRepository,_speciesDatabaseDTOMapper,_dialogCreator,_applicationSettingsPersistor);

         A.CallTo(() => _view.BindTo(A<ApplicationSettingsDTO>._))
            .Invokes(x => _dto = x.GetArgument<ApplicationSettingsDTO>(0));

      }
   }

   
   public class When_the_application_settings_presenter_is_displaying_the_application_settings : concern_for_ApplicationSettingsPresenter
   {
      private SpeciesDatabaseMap _speciesMap1;
      private SpeciesDatabaseMap _speciesMap2;
      private IEnumerable<SpeciesDatabaseMapDTO> _allSpeciesDatabaseMapDTOs;

      protected override void Context()
      {
         base.Context();
         _speciesMap1 = new SpeciesDatabaseMap {Species = "Dog", DatabaseFullPath = "Path1"};
         _speciesMap2 = new SpeciesDatabaseMap {Species = "Human", DatabaseFullPath = "Path2"};
         _applicationSettings.AddSpeciesDatabaseMap(_speciesMap1);
         _applicationSettings.AddSpeciesDatabaseMap(_speciesMap2);

         A.CallTo(() => _view.BindTo(A<IEnumerable<SpeciesDatabaseMapDTO>>._))
          .Invokes(x => _allSpeciesDatabaseMapDTOs = x.GetArgument<IEnumerable<SpeciesDatabaseMapDTO>>(0));
         _applicationSettings.UseWatermark = null;
      }

      protected override void Because()
      {
         sut.EditSettings();
      }

      [Observation]
      public void should_retrieve_the_available_species_and_the_corresponding_path_in_the_settings()
      {
         //one for each species
         _allSpeciesDatabaseMapDTOs.Single(x => x.SpeciesName == _speciesMap1.Species).DatabaseFullPath.ShouldBeEqualTo(_speciesMap1.DatabaseFullPath);
         _allSpeciesDatabaseMapDTOs.Single(x => x.SpeciesName == _speciesMap2.Species).DatabaseFullPath.ShouldBeEqualTo(_speciesMap2.DatabaseFullPath);
      }

      [Observation]
      public void should_display_the_path_selection_for_the_available_species()
      {
         //one for each species
         _allSpeciesDatabaseMapDTOs.Count().ShouldBeEqualTo(_speciesRepository.Count());
      }

      [Observation]
      public void should_bound_the_use_watermark_to_false_if_not_defined()
      {
         _dto.UseWatermark.ShouldBeFalse();
      }
   }

   public class When_the_application_settings_presenter_is_displaying_the_application_settings_containing_a_species_that_is_not_licensed_anymore : concern_for_ApplicationSettingsPresenter
   {
      private SpeciesDatabaseMap _speciesMap1;
      private SpeciesDatabaseMap _speciesMap2;
      private IEnumerable<SpeciesDatabaseMapDTO> _allDTOS;
      private SpeciesDatabaseMap _speciesMap3;

      protected override void Context()
      {
         base.Context();
         _speciesMap1 = new SpeciesDatabaseMap { Species = "Dog", DatabaseFullPath = "Path1" };
         _speciesMap2 = new SpeciesDatabaseMap { Species = "Human", DatabaseFullPath = "Path2" };
         _speciesMap3 = new SpeciesDatabaseMap { Species = "Mouse", DatabaseFullPath = "Path3" };
         _applicationSettings.AddSpeciesDatabaseMap(_speciesMap1);
         _applicationSettings.AddSpeciesDatabaseMap(_speciesMap2);
         _applicationSettings.AddSpeciesDatabaseMap(_speciesMap3);
         A.CallTo(() => _view.BindTo(A<IEnumerable<SpeciesDatabaseMapDTO>>._))
          .Invokes(x => _allDTOS = x.GetArgument<IEnumerable<SpeciesDatabaseMapDTO>>(0));
      }

      protected override void Because()
      {
         sut.EditSettings();
      }

      [Observation]
      public void should_have_removed_the_species_from_the_list()
      {
         //one for each species
         _allDTOS.Single(x => x.SpeciesName == _speciesMap1.Species).DatabaseFullPath.ShouldBeEqualTo(_speciesMap1.DatabaseFullPath);
         _allDTOS.Single(x => x.SpeciesName == _speciesMap2.Species).DatabaseFullPath.ShouldBeEqualTo(_speciesMap2.DatabaseFullPath);
      }

      [Observation]
      public void should_display_the_path_selection_for_the_available_species()
      {
         //one for each species
         _allDTOS.Count().ShouldBeEqualTo(_speciesRepository.Count());
      }
   }

   internal class RepresentationInfoRepositoryForSpecs : IRepresentationInfoRepository
   {
      public IEnumerable<RepresentationInfo> All()
      {
         throw new NotSupportedException();
      }

      public void Start()
      {
         throw new NotSupportedException();
      }

      public IEnumerable<RepresentationInfo> AllOfType(RepresentationObjectType objectType)
      {
         throw new NotSupportedException();
      }

      public RepresentationInfo InfoFor(RepresentationObjectType objectType, string objectName)
      {
         throw new NotSupportedException();
      }

      public RepresentationInfo ContainerInfoFor(string objectName)
      {
         throw new NotSupportedException();
      }

      public RepresentationInfo InfoFor(IObjectBase objectBase)
      {
         return new RepresentationInfo {DisplayName = objectBase.Name};
      }

      public string DisplayNameFor(IObjectBase objectBase)
      {
         return objectBase.Name;
      }

      public string DisplayNameFor(StatisticalAggregation statisticalAggregation)
      {
         return statisticalAggregation.Id;
      }

      public string DescriptionFor(IObjectBase objectBase)
      {
         return objectBase.Description;
      }

      public string DescriptionFor(RepresentationObjectType objectType, string objectName)
      {
         return objectName;
      }

      public string DisplayNameFor(RepresentationObjectType objectType, string objectName)
      {
         return objectName;
      }

      public bool ContainsInfoFor(IObjectBase objectBase)
      {
         return false;
      }
   }
}	