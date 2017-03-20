using PKSim.Presentation.Core;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Services;
using OSPSuite.Presentation.UICommands;

namespace PKSim.Presentation.UICommands
{
   public class EditSubjectUICommand<T> : ObjectUICommand<T> where T : class
   {
      private readonly ISingleStartPresenterTask _singleStartPresenterTask;

      public EditSubjectUICommand(ISingleStartPresenterTask singleStartPresenterTask)
      {
         _singleStartPresenterTask = singleStartPresenterTask;
      }

      protected override void PerformExecute()
      {
         _singleStartPresenterTask.StartForSubject(Subject);
      }
   }
}