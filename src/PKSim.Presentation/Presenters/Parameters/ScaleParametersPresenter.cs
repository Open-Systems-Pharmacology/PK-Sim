
using PKSim.Presentation.DTO.Parameters;
using PKSim.Presentation.Views.Parameters;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.Parameters
{
   public interface IScaleParametersPresenter : IPresenter<IScaleParametersView>
   {
      /// <summary>
      /// Initialize the scale parameters presenter with the parent presenter displaying the set of parameters being edited
      /// </summary>
      void InitializeWith(IParameterSetPresenter parameterSetPresenter);

      /// <summary>
      /// Trigger the scale action for the parameters being edited in the parent presenter
      /// </summary>
      void Scale();

      /// <summary>
      /// Trigger the reset action for the parameters being edited in the parent presenter
      /// </summary>
      void Reset();

      /// <summary>
      /// set whether the scale and reset functions are enabled or not
      /// </summary>
      bool Enabled {  set; }
   }

   public class ScaleParametersPresenter : AbstractPresenter<IScaleParametersView, IScaleParametersPresenter>, IScaleParametersPresenter
   {
      private IParameterSetPresenter _parameterSetPresenter;
      private readonly ParameterScaleWithFactorDTO _parameterScaleWithFactorDTO;

      public ScaleParametersPresenter(IScaleParametersView view) : base(view)
      {
         _parameterScaleWithFactorDTO = new ParameterScaleWithFactorDTO {Factor = 1};
         View.BindTo(_parameterScaleWithFactorDTO);
      }

      public void InitializeWith(IParameterSetPresenter parameterSetPresenter)
      {
         _parameterSetPresenter = parameterSetPresenter;
      }

      public void Scale()
      {
         _parameterSetPresenter.ScaleParametersWith(_parameterScaleWithFactorDTO.Factor);
      }

      public void Reset()
      {
         _parameterSetPresenter.ResetParameters();
      }

      public bool Enabled
      {
         set => _view.ReadOnly = !value;
      }
   }
}