using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;

namespace PKSim.Core.Model
{
   public class TransporterExpressionContainer : MoleculeExpressionContainer, ITransporterContainer
   {
      private MembraneLocation _membraneLocation;
      public string CompartmentName { get; set; }
      private readonly IList<string> _allProcessNames = new List<string>();

      public IEnumerable<string> ProcessNames => _allProcessNames;

      public void AddProcessName(string processName)
      {
         _allProcessNames.Add(processName);
      }

      public void ClearProcessNames()
      {
         _allProcessNames.Clear();
      }

      public MembraneLocation MembraneLocation
      {
         get => _membraneLocation;
         set => SetProperty(ref _membraneLocation, value);
      }

      public string OrganName => Name;

      public bool HasPolarizedMembrane
      {
         get
         {
            if (CoreConstants.Organ.PolarizedMembraneOrgans.Contains(OrganName))
               return true;

            return string.Equals(GroupName, CoreConstants.Groups.GI_MUCOSA);
         }
      }

      public void UpdatePropertiesFrom(TransporterContainerTemplate transporterContainerTemplate)
      {
         updatePropertiesFrom(transporterContainerTemplate);
         CompartmentName = transporterContainerTemplate.CompartmentName;
      }

      private void updatePropertiesFrom(ITransporterContainer transporterContainer)
      {
         MembraneLocation = transporterContainer.MembraneLocation;
         _allProcessNames.Clear();
         transporterContainer.ProcessNames.Each(AddProcessName);
      }

      public override void UpdatePropertiesFrom(IUpdatable sourceObject, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(sourceObject, cloneManager);
         if (!(sourceObject is TransporterExpressionContainer sourceTransporterContainer)) return;
         updatePropertiesFrom(sourceTransporterContainer);
         CompartmentName = sourceTransporterContainer.CompartmentName;
      }
   }
}