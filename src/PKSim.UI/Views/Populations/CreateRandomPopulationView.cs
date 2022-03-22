﻿using OSPSuite.Assets;
using PKSim.Assets;
using PKSim.Presentation.Presenters.Populations;
using PKSim.Presentation.Views.Populations;
using PKSim.UI.Views.Core;

namespace PKSim.UI.Views.Populations
{
   public partial class CreateRandomPopulationView : BuildingBlockWizardView, ICreateRandomPopulationView
   {
      public CreateRandomPopulationView(Shell shell) : base(shell)
      {
         InitializeComponent();
      }

      public void AttachPresenter(ICreateRandomPopulationPresenter presenter)
      {
         WizardPresenter = presenter;
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         Icon = ApplicationIcons.Population.WithSize(IconSizes.Size16x16);
         Caption = PKSimConstants.UI.CreatePopulation;
      }
   }
}