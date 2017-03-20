using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using OSPSuite.Core.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Views;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters
{
   public interface IApplicationSettingsPresenter : IPresenter<IApplicationSettingsView>, ISettingsItemPresenter
   {
      void SelectDatabasePathFor(SpeciesDatabaseMapDTO speciesDatabaseMapDTO);
      void RemoveDatabasePathFor(SpeciesDatabaseMapDTO speciesDatabaseMapDTO);
   }

   public class ApplicationSettingsPresenter : AbstractSubPresenter<IApplicationSettingsView, IApplicationSettingsPresenter>, IApplicationSettingsPresenter
   {
      private readonly IApplicationSettings _applicationSettings;
      private readonly ISpeciesRepository _speciesRepository;
      private readonly ISpeciesDatabaseMapToSpeciesDatabaseMapDTOMapper _speciesMapper;
      private readonly IDialogCreator _dialogCreator;
      private readonly IApplicationSettingsPersitor _applicationSettingsPersitor;
      private IEnumerable<SpeciesDatabaseMapDTO> _databaseMapDTOs;

      public ApplicationSettingsPresenter(IApplicationSettingsView view, IApplicationSettings applicationSettings,
         ISpeciesRepository speciesRepository, ISpeciesDatabaseMapToSpeciesDatabaseMapDTOMapper speciesMapper,
         IDialogCreator dialogCreator, IApplicationSettingsPersitor applicationSettingsPersitor) : base(view)
      {
         _applicationSettings = applicationSettings;
         _speciesRepository = speciesRepository;
         _speciesMapper = speciesMapper;
         _dialogCreator = dialogCreator;
         _applicationSettingsPersitor = applicationSettingsPersitor;
      }

      public void EditSettings()
      {
         var definedMapping = _applicationSettings.SpeciesDataBaseMaps.ToList();
         addMissingSpeciesTo(definedMapping);
         removeUnlicensedSpeciesFrom(definedMapping);
         _databaseMapDTOs = definedMapping.MapAllUsing(_speciesMapper).OrderBy(x => x.SpeciesDisplayName);
         _view.BindTo(_databaseMapDTOs);
         _view.BindTo(_applicationSettings);
      }

      private void removeUnlicensedSpeciesFrom(List<SpeciesDatabaseMap> speciesMaps)
      {
         foreach (var speciesMap in speciesMaps.ToList())
         {
            if (_speciesRepository.FindByName(speciesMap.Species) == null)
               speciesMaps.Remove(speciesMap);
         }
      }

      private void addMissingSpeciesTo(IList<SpeciesDatabaseMap> definedMapping)
      {
         foreach (var species in _speciesRepository.All().Where(species => !_applicationSettings.HasExpressionsDatabaseFor(species)))
         {
            definedMapping.Add(new SpeciesDatabaseMap {Species = species.Name});
         }
      }

      public void SaveSettings()
      {
         //Update the user settings according to the selection
         foreach (var speciesDatabaseMapDTO in _databaseMapDTOs)
         {
            if (speciesDatabaseMapDTO.WasDeleted)
               _applicationSettings.RemoveSpeciesDatabaseMap(speciesDatabaseMapDTO.SpeciesName);
            else
               _applicationSettings.SpeciesDatabaseMapsFor(speciesDatabaseMapDTO.SpeciesName).DatabaseFullPath = speciesDatabaseMapDTO.DatabaseFullPath;
         }

         _applicationSettingsPersitor.Save(_applicationSettings);
      }

      public void SelectDatabasePathFor(SpeciesDatabaseMapDTO speciesDatabaseMapDTO)
      {
         var dataBaseFile = _dialogCreator.AskForFileToOpen(PKSimConstants.UI.SelectDatabasePathFor(speciesDatabaseMapDTO.SpeciesDisplayName), CoreConstants.Filter.EXPRESSION_DATABASE_FILE_FILTER, CoreConstants.DirectoryKey.DATABASE);
         if (string.IsNullOrEmpty(dataBaseFile)) return;
         speciesDatabaseMapDTO.DatabaseFullPath = dataBaseFile;
      }

      public void RemoveDatabasePathFor(SpeciesDatabaseMapDTO speciesDatabaseMapDTO)
      {
         speciesDatabaseMapDTO.DatabaseFullPath = string.Empty;
      }
   }
}