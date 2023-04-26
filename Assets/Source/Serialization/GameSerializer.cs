using System;
using System.IO;
using Cyens.ReInherit.Architect;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Index = Cyens.ReInherit.Architect.Index;

namespace Cyens.ReInherit.Serialization
{
    public class GameSerializer : MonoBehaviour
    {
        public static readonly string SavePath;
        private static readonly JsonSerializerSettings SerializerSettings;

        static GameSerializer()
        {
            SavePath = Path.Join(Application.persistentDataPath, "save.json");
            SerializerSettings = new JsonSerializerSettings {
                Converters = new JsonConverter[] {
                    new IndexConverter(),
                },
            };
        }

        public static void Save(RoomGraph roomGraph)
        {
            var jsonObject = roomGraph.WriteJsonData();
            var jsonString = JsonConvert.SerializeObject(jsonObject, Formatting.Indented, SerializerSettings);

            File.WriteAllText(SavePath, jsonString);
        }

        public static void Load(RoomGraph roomGraph)
        {
            try {
                var text = File.ReadAllText(SavePath);
                var obj = JsonConvert.DeserializeObject<RoomGraphJsonData>(text, SerializerSettings);
                roomGraph.LoadJsonData(obj);
            } catch (FileNotFoundException) {
                // ignored (Save does not exist)
            }
        }

        private class IndexConverter : JsonConverter
        {
            private static readonly int[] Buffer = new int[2];
        
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                var index = (Index)value;
                Buffer[0] = index.x;
                Buffer[1] = index.y;
                JArray.FromObject(Buffer).WriteTo(writer);
            }
        
            public override object ReadJson(
                JsonReader reader,
                Type objectType,
                object existingValue,
                JsonSerializer serializer)
            {
                var array = JArray.Load(reader).ToObject<int[]>();
                return new Index(array[0], array[1]);
            }
        
            public override bool CanConvert(Type objectType) => objectType == typeof(Index);
        }
    }
}