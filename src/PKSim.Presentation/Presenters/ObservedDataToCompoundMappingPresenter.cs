using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Presentation.DTO;
using PKSim.Presentation.DTO.ObservedData;
using PKSim.Presentation.Views;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters
{
   public interface IObservedDataToCompoundMappingPresenter : IDisposablePresenter
   {
      IEnumerable<PKSim.Core.Model.Compound> AllCompounds();
      void UpdateCompoundInObservedDataFor(IPKSimProject project);
   }

   public class ObservedDataToCompoundMappingPresenter : AbstractDisposablePresenter<IObservedDataToCompoundMappingView, IObservedDataToCompoundMappingPresenter>, IObservedDataToCompoundMappingPresenter
   {
      private readonly ICompoundFactory _compoundFactory;
      private readonly PKSim.Core.Model.Compound _undefinedCompound;
      private readonly List<PKSim.Core.Model.Compound> _allAvailableCompounds;
      private List<ObservedDataToCompoundMappingDTO> _mapping;

      public ObservedDataToCompoundMappingPresenter(IObservedDataToCompoundMappingView view, ICompoundFactory compoundFactory) : base(view)
      {
         _compoundFactory = compoundFactory;
         _undefinedCompound = _compoundFactory.Create().WithName(PKSimConstants.UI.Undefined);
         _allAvailableCompounds = new List<PKSim.Core.Model.Compound> {_undefinedCompound};
         view.CancelVisible = false;
      }

      public IEnumerable<PKSim.Core.Model.Compound> AllCompounds()
      {
         return _allAvailableCompounds;
      }

      public void UpdateCompoundInObservedDataFor(IPKSimProject project)
      {
         var allProjectCompound = project.All<PKSim.Core.Model.Compound>();
         _allAvailableCompounds.AddRange(allProjectCompound);

         _mapping = new List<ObservedDataToCompoundMappingDTO>();

         project.AllObservedData.Each(data => _mapping.Add(mapFrom(data, allProjectCompound)));
         _view.BindTo(_mapping);
         _view.Display();

         updateMolWeight();
      }

      private void updateMolWeight()
      {
         foreach (var dto in _mapping)
         {
            foreach (var col in dto.ObservedData.AllButBaseGrid())
            {
               col.QuantityInfo = new QuantityInfo(col.Name, new[] {dto.ObservedData.Name, CoreConstants.ContainerName.ObservedData, PKSimConstants.UI.Undefined, PKSimConstants.UI.Undefined, dto.Compound.Name, col.Name}, QuantityType.Undefined);
               if (dto.Compound == _undefinedCompound) continue;
               col.DataInfo.MolWeight = dto.Compound.MolWeight;
            }
         }
      }

      private ObservedDataToCompoundMappingDTO mapFrom(DataRepository observedData, IEnumerable<PKSim.Core.Model.Compound> allProjectCompound)
      {
         return new ObservedDataToCompoundMappingDTO
            {
               ObservedData = observedData,
               Compound = allProjectCompound.FirstOrDefault() ?? _undefinedCompound
            };
      }
   }
}