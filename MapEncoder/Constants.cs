using System.Text;

namespace TwelveEngine {
    public partial class Constants {
        public const string MapDatabaseExtension = "temdb"; /* Twelve Engine Map Database */      
        public const string MapDatabase = "maps." + MapDatabaseExtension;
        internal static readonly Encoding MapStringEncoding = Encoding.UTF8;
    }
}
