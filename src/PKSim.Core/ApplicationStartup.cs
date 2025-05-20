using System;
using System.Globalization;
using System.Reflection;

namespace PKSim.Core;

public class ApplicationStartup
{
   protected static void RedirectAssembly(string shortName, Version targetVersion, string publicKeyToken)
   {
      Assembly Handler(object sender, ResolveEventArgs args)
      {
         var requestedAssembly = new AssemblyName(args.Name);
         if (requestedAssembly.Name != shortName) return null;

         requestedAssembly.Version = targetVersion;
         requestedAssembly.SetPublicKeyToken(new AssemblyName("x, PublicKeyToken=" + publicKeyToken).GetPublicKeyToken());
         requestedAssembly.CultureInfo = CultureInfo.InvariantCulture;

         //once found, not need to react to event anymore
         AppDomain.CurrentDomain.AssemblyResolve -= Handler;

         return Assembly.Load(requestedAssembly);
      }

      AppDomain.CurrentDomain.AssemblyResolve += Handler;
   }
}