using UnityEngine;
using System.IO;

namespace IsonoGame
{
    public class ResourceManager
    {
        public static T Load<T>(string path) where T : Object
        {
            var extension = Path.GetExtension(path);
            if (extension != "")
            {
                path = path.Replace(extension, "");
            }

            var asset = Resources.Load<T>(path);
            return asset;
        }
    }
}