using OSPSuite.Presentation.Mappers.ParameterIdentifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// TODO: Remove
namespace PKSim.Infrastructure.Extensions
{
  public static class FunctionExtensions
  {
    public static T Compose<T>(this Func<T, T> f1, Func<T, T> f2, T value)
    {
      return f2.Invoke(f1.Invoke(value));
    }
  }
}
