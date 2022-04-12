using System;
using OSPSuite.Assets;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Validation;
using PKSim.Assets;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.ExpressionProfiles;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Views.ExpressionProfiles;

namespace PKSim.Presentation.Presenters.ExpressionProfiles
{
   public interface ICreateExpressionProfilePresenter : ICreateBuildingBlockPresenter<ExpressionProfile>
   {
      ExpressionProfile ExpressionProfile { get; }
      IPKSimCommand Create<TMolecule>() where TMolecule : IndividualMolecule;
   }

   public class CreateExpressionProfilePresenter : AbstractDisposableCommandCollectorPresenter<ICreateExpressionProfileView, ICreateExpressionProfilePresenter>, ICreateExpressionProfilePresenter
   {
      private readonly IExpressionProfileFactory _expressionProfileFactory;
      private readonly IExpressionProfileToExpressionProfileDTOMapper _expressionProfileDTOMapper;
      private readonly IMoleculeParameterTask _moleculeParameterTask;
      private ExpressionProfileDTO _dto;
      public ExpressionProfile ExpressionProfile { get; private set; }

      public CreateExpressionProfilePresenter(
         ICreateExpressionProfileView view,
         IExpressionProfileFactory expressionProfileFactory,
         IExpressionProfileToExpressionProfileDTOMapper expressionProfileDTOMapper, 
         IMoleculeParameterTask moleculeParameterTask) : base(view)
      {
         _expressionProfileFactory = expressionProfileFactory;
         _expressionProfileDTOMapper = expressionProfileDTOMapper;
         _moleculeParameterTask = moleculeParameterTask;
      }

      public IPKSimCommand Create<TMolecule>() where TMolecule : IndividualMolecule
      {
         //Just for edit
         ExpressionProfile = _expressionProfileFactory.Create<TMolecule>();
         _dto = _expressionProfileDTOMapper.MapFrom(ExpressionProfile);
         _view.Caption = PKSimConstants.UI.CreateExpressionProfile;
         _view.BindTo(_dto);
         _view.Display();
         if(_view.Canceled)   
            return new PKSimEmptyCommand();

         //we create a new one with all new features
         ExpressionProfile = _expressionProfileFactory.Create<TMolecule>(_dto.Species, _dto.MoleculeName);
         ExpressionProfile.Category = _dto.Category;

         _moleculeParameterTask.SetDefaultFor(ExpressionProfile);

         //Action 
         return new PKSimMacroCommand();
      }

      public IPKSimCommand Create() => Create<IndividualEnzyme>();

      public ExpressionProfile BuildingBlock => ExpressionProfile;

      public override void ViewChanged()
      {
         base.ViewChanged();
         View.OkEnabled = CanClose;
      }

      public override bool CanClose => base.CanClose && _dto.IsValid();
   }
}