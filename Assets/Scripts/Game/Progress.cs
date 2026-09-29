using UnityEngine;

namespace LittleClass.Game
{
    /// <summary>What the child has finished, kept on the device. English only for now.</summary>
    public static class Progress
    {
        const string Key = "littleclass.done.";

        public static bool Done(Core.ActivityId id)
        {
            return PlayerPrefs.GetInt(Key + (int)id, 0) == 1;
        }

        public static void MarkDone(Core.ActivityId id)
        {
            PlayerPrefs.SetInt(Key + (int)id, 1);
            PlayerPrefs.Save();
        }

        public static int FinishedCount()
        {
            int n = 0;
            foreach (Core.ActivityId id in System.Enum.GetValues(typeof(Core.ActivityId)))
                if (Done(id)) n++;
            return n;
        }
    }
}
