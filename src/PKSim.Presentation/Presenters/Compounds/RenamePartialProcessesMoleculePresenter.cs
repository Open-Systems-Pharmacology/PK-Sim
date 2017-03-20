using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using PKSim.Presentation.DTO.Compounds;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Presenters.Compounds
{
   public interface IRenamePartialProcessesMoleculePresenter : IObjectBasePresenter<PKSim.Core.Model.Compound>
   {
      string NewMoleculeName { get; }
      IEnumerable<PKSim.Core.Model.PartialProcess> AllProcessesToRename { get; set; }
   }

   public class RenamePartialProcessesMoleculePresenter : ObjectBasePresenter<PKSim.Core.Model.Compound>, IRenamePartialProcessesMoleculePresenter
   {
      private readonly RenameMoleculeInPartialProcessDTO _renameDTO;
      public IEnumerable<PKSim.Core.Model.PartialProcess> AllProcessesToRename { get; set; }

      public string NewMoleculeName
      {
         get { return _renameDTO.Name; }
      }

      public RenamePartialProcessesMoleculePresenter(IObjectBaseView view) : base(view, false)
      {
         _renameDTO = new RenameMoleculeInPartialProcessDTO();
         AllProcessesToRename = Enumerable.Empty<PKSim.Core.Model.PartialProcess>();
      }

      protected override void InitializeResourcesFor(PKSim.Core.Model.Compound compound)
      {
         _view.Caption = PKSimConstants.UI.Rename;
         _view.NameDescription = PKSimConstants.UI.RenamePartialProcessesMolecule(PKSimConstants.UI.Molecule);
      }

      protected override ObjectBaseDTO CreateDTOFor(PKSim.Core.Model.Compound compound)
      {
         _renameDTO.Name = AllProcessesToRename.Select(x=>x.MoleculeName).FirstOrDefault();
         _renameDTO.AddUsedDataSource(AllProcessesToRename.Select(x => x.DataSource));
         _renameDTO.AddUsedNames(compound.AllProcesses<PKSim.Core.Model.CompoundProcess>().Select(x => x.Name));
         return _renameDTO;
      }
   }
}