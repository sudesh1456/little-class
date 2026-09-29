using System.Collections;
using System.Collections.Generic;
using LittleClass.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LittleClass.Game
{
    /// <summary>
    /// English preschool classroom. Builds every screen in code and starts from any scene.
    /// Nursery activities only; LKG and UKG are reserved on the grown-ups screen.
    /// </summary>
    public sealed class App : MonoBehaviour
    {
        static readonly Color Paper = new Color(1f, 0.965f, 0.91f);
        static readonly Color Ink = new Color(0.23f, 0.16f, 0.13f);
        static readonly Color Soft = new Color(0.45f, 0.34f, 0.28f);
        static readonly Color Honey = new Color(1f, 0.72f, 0.19f);
        static readonly Color Leaf = new Color(0.36f, 0.62f, 0.38f);
        static readonly Color Clay = new Color(0.91f, 0.45f, 0.32f);
        static readonly Color Card = Color.white;

        Font font;
        Canvas canvas;
        RectTransform root;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Boot()
        {
            if (FindFirstObjectByType<App>() != null) return;
            new GameObject("Little Class").AddComponent<App>();
        }

        void Awake()
        {
            Application.targetFrameRate = 60;
            Screen.orientation = ScreenOrientation.Portrait;
            font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            var camGo = GameObject.FindGameObjectWithTag("MainCamera");
            var cam = camGo != null ? camGo.GetComponent<Camera>() : new GameObject("Main Camera").AddComponent<Camera>();
            cam.tag = "MainCamera";
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Paper;
            cam.orthographic = true;

            var canvasGo = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.matchWidthOrHeight = 0.5f;
            root = canvasGo.GetComponent<RectTransform>();

            if (FindFirstObjectByType<EventSystem>() == null)
            {
                var es = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
                es.transform.SetParent(transform, false);
            }

            ShowHome();
        }

        internal static void PinTop(RectTransform rt, float yFromTop, float width, float height)
        {
            rt.anchorMin = new Vector2(0.5f, 1f);
            rt.anchorMax = new Vector2(0.5f, 1f);
            rt.pivot = new Vector2(0.5f, 1f);
            rt.sizeDelta = new Vector2(width, height);
            rt.anchoredPosition = new Vector2(0f, -yFromTop);
        }

        void Clear()
        {
            for (int i = root.childCount - 1; i >= 0; i--)
                Destroy(root.GetChild(i).gameObject);
        }

        void ShowHome()
        {
            Clear();
            var page = Page();
            Title(page, "Little Class", 36);
            Body(page, "Tap a game", 140);

            var activities = new[]
            {
                ActivityId.Letters, ActivityId.Count, ActivityId.Colors, ActivityId.Animals, ActivityId.Rhyme
            };
            for (int i = 0; i < activities.Length; i++)
            {
                var id = activities[i];
                var label = Activities.Title(id);
                if (Progress.Done(id)) label += "   \u2713";
                var captured = id;
                BigButton(page, label, Activities.Room(id), 250f + i * 200f, Honey, () => ShowActivity(captured));
            }

            TextButton(page, "Grown-ups", 1560f, () => ShowParents());
        }

        void ShowParents()
        {
            Clear();
            var page = Page();
            Title(page, "Grown-ups", 36);
            Body(page, "English only for now. Nursery games are ready.\nLKG and UKG come next.\n\nFinished " + Progress.FinishedCount() + " of 5 games.", 180);
            TextButton(page, "Back to class", 1560f, ShowHome);
        }

        void ShowActivity(ActivityId id)
        {
            Clear();
            var host = new GameObject("Activity", typeof(RectTransform));
            host.transform.SetParent(root, false);
            var rt = host.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            host.AddComponent<ActivityPlay>().Begin(this, id, font);
        }

        internal void BackHome() { ShowHome(); }

        RectTransform Page()
        {
            var go = new GameObject("Page", typeof(RectTransform));
            go.transform.SetParent(root, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(48f, 48f);
            rt.offsetMax = new Vector2(-48f, -48f);
            return rt;
        }

        void Title(RectTransform page, string text, float yFromTop)
        {
            var t = Label(page, text, 84, FontStyle.Bold, Ink, TextAnchor.MiddleCenter, Vector2.zero, new Vector2(960f, 110f));
            PinTop(t.rectTransform, yFromTop, 960f, 110f);
        }

        void Body(RectTransform page, string text, float yFromTop)
        {
            var t = Label(page, text, 40, FontStyle.Normal, Soft, TextAnchor.UpperCenter, Vector2.zero, new Vector2(920f, 240f));
            PinTop(t.rectTransform, yFromTop, 920f, 240f);
        }

        void BigButton(RectTransform page, string title, string subtitle, float yFromTop, Color color, UnityEngine.Events.UnityAction onClick)
        {
            var button = Button(page, yFromTop, 960f, 180f, color, onClick);
            Label(button, title, 52, FontStyle.Bold, Ink, TextAnchor.MiddleCenter, new Vector2(0f, 16f), new Vector2(880f, 70f));
            Label(button, subtitle, 32, FontStyle.Normal, Soft, TextAnchor.MiddleCenter, new Vector2(0f, -38f), new Vector2(880f, 44f));
        }

        void TextButton(RectTransform page, string text, float yFromTop, UnityEngine.Events.UnityAction onClick)
        {
            var button = Button(page, yFromTop, 520f, 110f, new Color(1f, 1f, 1f, 0.7f), onClick);
            Label(button, text, 36, FontStyle.Bold, Ink, TextAnchor.MiddleCenter, Vector2.zero, new Vector2(480f, 80f));
        }

        internal RectTransform Button(RectTransform parent, float yFromTop, float width, float height, Color color, UnityEngine.Events.UnityAction onClick)
        {
            var go = new GameObject("Button", typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            PinTop(rt, yFromTop, width, height);
            go.GetComponent<Image>().color = color;
            go.GetComponent<Button>().onClick.AddListener(onClick);
            return rt;
        }

        internal Text Label(RectTransform parent, string text, int size, FontStyle style, Color color, TextAnchor align, Vector2 pos, Vector2 box)
        {
            var go = new GameObject("Text", typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = box;
            rt.anchoredPosition = pos;
            var t = go.GetComponent<Text>();
            t.font = font;
            t.text = text;
            t.fontSize = size;
            t.fontStyle = style;
            t.color = color;
            t.alignment = align;
            t.horizontalOverflow = HorizontalWrapMode.Wrap;
            t.verticalOverflow = VerticalWrapMode.Overflow;
            t.raycastTarget = false;
            return t;
        }

        internal static Color ColorOf(string id)
        {
            switch (id)
            {
                case "red": return new Color(0.86f, 0.27f, 0.24f);
                case "blue": return new Color(0.29f, 0.48f, 0.84f);
                case "yellow": return new Color(0.98f, 0.78f, 0.22f);
                case "green": return new Color(0.33f, 0.66f, 0.40f);
                default: return Card;
            }
        }

        internal static Color InkColor { get { return Ink; } }
        internal static Color SoftColor { get { return Soft; } }
        internal static Color LeafColor { get { return Leaf; } }
        internal static Color ClayColor { get { return Clay; } }
        internal static Color HoneyColor { get { return Honey; } }
        internal static Color CardColor { get { return Card; } }
    }

    /// <summary>Five short rounds. A wrong tap stays on the card; the right tap moves on.</summary>
    sealed class ActivityPlay : MonoBehaviour
    {
        App app;
        Font font;
        ActivityId id;
        Round[] rounds;
        int index;
        RectTransform page;
        Text prompt;
        readonly List<Button> choiceButtons = new List<Button>();

        public void Begin(App owner, ActivityId activity, Font typeface)
        {
            app = owner;
            font = typeface;
            id = activity;
            rounds = Activities.Build(id);
            var rt = GetComponent<RectTransform>();
            page = rt;
            DrawChrome();
            ShowRound();
        }

        void DrawChrome()
        {
            var title = app.Label(page, Activities.Title(id), 64, FontStyle.Bold, App.InkColor, TextAnchor.MiddleCenter,
                Vector2.zero, new Vector2(700f, 90f));
            App.PinTop(title.rectTransform, 36f, 700f, 90f);
            var back = app.Button(page, 36f, 180f, 90f, new Color(1f, 1f, 1f, 0.85f), app.BackHome);
            back.anchoredPosition = new Vector2(-400f, back.anchoredPosition.y);
            app.Label(back, "Back", 32, FontStyle.Bold, App.InkColor, TextAnchor.MiddleCenter, Vector2.zero, new Vector2(160f, 70f));
        }

        void ShowRound()
        {
            foreach (var b in choiceButtons)
                if (b != null) Destroy(b.gameObject);
            choiceButtons.Clear();
            if (prompt != null) Destroy(prompt.gameObject);

            if (index >= rounds.Length)
            {
                Progress.MarkDone(id);
                prompt = app.Label(page, "You did it!", 72, FontStyle.Bold, App.LeafColor, TextAnchor.MiddleCenter,
                    Vector2.zero, new Vector2(900f, 160f));
                App.PinTop(prompt.rectTransform, 520f, 900f, 160f);
                var again = app.Button(page, 760f, 640f, 140f, App.HoneyColor, app.BackHome);
                app.Label(again, "Back to class", 40, FontStyle.Bold, App.InkColor, TextAnchor.MiddleCenter,
                    Vector2.zero, new Vector2(600f, 100f));
                return;
            }

            var round = rounds[index];
            prompt = app.Label(page, round.Prompt, 52, FontStyle.Bold, App.InkColor, TextAnchor.UpperCenter,
                Vector2.zero, new Vector2(960f, 180f));
            App.PinTop(prompt.rectTransform, 160f, 960f, 180f);

            if (round.Count > 0)
                DrawDots(round.Count);

            float y = round.Count > 0 ? 620f : 420f;
            for (int i = 0; i < round.Choices.Length; i++)
            {
                var choice = round.Choices[i];
                var color = id == ActivityId.Colors ? App.ColorOf(choice.Id) : App.CardColor;
                var button = app.Button(page, y + i * 190f, 860f, 170f, color, () => OnPick(choice));
                var ink = id == ActivityId.Colors && choice.Id != "yellow" ? Color.white : App.InkColor;
                app.Label(button, Pretty(choice.Label), 56, FontStyle.Bold, ink, TextAnchor.MiddleCenter,
                    Vector2.zero, new Vector2(780f, 120f));
                choiceButtons.Add(button.GetComponent<Button>());
            }
        }

        void DrawDots(int count)
        {
            float gap = 120f;
            float start = -((count - 1) * gap) / 2f;
            for (int i = 0; i < count; i++)
            {
                var dot = app.Button(page, 400f, 90f, 90f, App.ClayColor, () => { });
                dot.anchoredPosition = new Vector2(start + i * gap, dot.anchoredPosition.y);
                dot.GetComponent<Button>().interactable = false;
                dot.GetComponent<Image>().raycastTarget = false;
            }
        }

        void OnPick(Choice choice)
        {
            if (index >= rounds.Length) return;
            if (!choice.Correct)
            {
                prompt.text = rounds[index].TryAgain;
                return;
            }
            prompt.text = "Yes!";
            StartCoroutine(Next());
        }

        IEnumerator Next()
        {
            foreach (var b in choiceButtons)
                if (b != null) b.interactable = false;
            yield return new WaitForSeconds(0.45f);
            index++;
            ShowRound();
        }

        static string Pretty(string label)
        {
            if (string.IsNullOrEmpty(label)) return label;
            return char.ToUpper(label[0]) + label.Substring(1);
        }
    }
}
