using System.Collections.Generic;

namespace Cyens.ReInherit.Extensions {
    public static class ListExtensions {
        public static T Pop<T>(this List<T> list) {
            var lastIndex = list.Count - 1;
            var value = list[lastIndex];
            list.RemoveAt(lastIndex);
            return value;
        }

        public static bool TryPop<T>(this List<T> list, out T value) {
            if (list.Count == 0) {
                value = default;
                return false;
            }

            value = list.Pop();
            return true;
        }

        public static T Pop<T>(this List<T> list, int index) {
            var value = list[index];
            list.RemoveAt(index);
            return value;
        }

        public static void ReserveCapacity<T>(this List<T> list, int capacity) {
            if (list.Capacity < capacity) {
                list.Capacity = capacity;
            }
        }
    }
}