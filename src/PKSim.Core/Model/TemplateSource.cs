using System;
using System.Security.Cryptography;
using System.Text;

namespace PKSim.Core.Model
{
   /// <summary>
   ///    An additional building block template library configured by the user (see <see cref="ICoreUserSettings.TemplateSources" />).
   ///    A source only carries a display name and the location of its summary file (templates.json).
   ///    Credentials are never part of a source: they are resolved at download time (see <c>ITemplateCredentialProvider</c>).
   /// </summary>
   public class TemplateSource
   {
      public string Name { get; set; }

      /// <summary>
      ///    Location of the summary file. Either an absolute http(s) url or a local/UNC path
      /// </summary>
      public string Url { get; set; }

      public TemplateSource()
      {
      }

      public TemplateSource(string name, string url)
      {
         Name = name;
         Url = url;
      }

      /// <summary>
      ///    Absolute uri of the summary file. Local and UNC paths are returned as file uris. Null if <see cref="Url" /> is not a
      ///    valid absolute uri
      /// </summary>
      public Uri Uri => UriFor(Url);

      /// <summary>
      ///    Returns true if <see cref="Url" /> is an absolute http(s) url or a file path
      /// </summary>
      public bool HasValidUrl => IsValidUrl(Url);

      /// <summary>
      ///    Stable identifier derived from the url and used to isolate downloaded files of this source on disk
      /// </summary>
      public string Key
      {
         get
         {
            var normalizedUrl = (Url ?? string.Empty).Trim().ToLowerInvariant();
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(normalizedUrl));
            return Convert.ToHexString(hash, 0, 8).ToLowerInvariant();
         }
      }

      public static Uri UriFor(string url)
      {
         if (string.IsNullOrWhiteSpace(url))
            return null;

         return Uri.TryCreate(url.Trim(), UriKind.Absolute, out var uri) ? uri : null;
      }

      public static bool IsValidUrl(string url)
      {
         var uri = UriFor(url);
         if (uri == null)
            return false;

         return uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps || uri.IsFile;
      }

      public TemplateSource Clone() => new TemplateSource().WithUpdatedPropertiesFrom(this);

      public TemplateSource WithUpdatedPropertiesFrom(TemplateSource source)
      {
         Name = source.Name;
         Url = source.Url;
         return this;
      }

      public override bool Equals(object obj)
      {
         if (ReferenceEquals(this, obj))
            return true;

         return obj is TemplateSource other &&
                string.Equals(Name, other.Name) &&
                string.Equals(Url, other.Url);
      }

      public override int GetHashCode() => HashCode.Combine(Name, Url);

      public override string ToString() => $"{Name} ({Url})";
   }
}
