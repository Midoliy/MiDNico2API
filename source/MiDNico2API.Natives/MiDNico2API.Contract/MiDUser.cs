using System.IO;
using System.Xml.Linq;

namespace MiDNico2API.Contract
{
    public class MiDUser
    {
        public string ID { get; }
        public string Name { get; }
        public string ThumbnailUrl { get; }

        public MiDUser(
            in Stream data
        )
        {
            if (data == default || data.Length <= 0)
            {
                throw new InvalidDataException("UserInfoデータの取得に失敗しました.");
            }

            // XMLデータを読み込む.
            var doc  = XDocument.Load(data);
            var root = doc.Root.Element("user");
            if (root == default)
            {
                throw new InvalidDataException($"{nameof(data)}: userデータではありません.");
            }

            ID           = root.Element("id")?.Value ?? "";
            Name         = root.Element("nickname")?.Value ?? "";
            ThumbnailUrl = root.Element("thumbnail_url")?.Value ?? "";

        }

    }
}
