using System.Collections.Generic;

namespace LittleClass.Core
{
    public enum ActivityId
    {
        Count,
        Colors,
        Animals,
        Letters,
        Rhyme
    }

    /// <summary>One tappable answer. Id is a token the view understands (a color name, an animal, a letter).</summary>
    public sealed class Choice
    {
        public readonly string Id;
        public readonly string Label;
        public readonly bool Correct;

        public Choice(string id, string label, bool correct)
        {
            Id = id;
            Label = label;
            Correct = correct;
        }
    }

    /// <summary>One prompt. Count is how many dots to draw; zero for every other activity.</summary>
    public sealed class Round
    {
        public readonly string Prompt;
        public readonly string TryAgain;
        public readonly Choice[] Choices;
        public readonly int Count;

        public Round(string prompt, int count, params Choice[] choices)
        {
            Prompt = prompt;
            TryAgain = "Try again";
            Count = count;
            Choices = choices;
        }
    }

    public static class Activities
    {
        public static string Title(ActivityId id)
        {
            switch (id)
            {
                case ActivityId.Count: return "Counting";
                case ActivityId.Colors: return "Colors";
                case ActivityId.Animals: return "Animals";
                case ActivityId.Letters: return "Letters";
                case ActivityId.Rhyme: return "Twinkle Twinkle";
                default: return id.ToString();
            }
        }

        public static string Room(ActivityId id)
        {
            switch (id)
            {
                case ActivityId.Count:
                case ActivityId.Colors: return "Maths";
                case ActivityId.Animals: return "World";
                case ActivityId.Letters: return "English";
                default: return "Rhymes";
            }
        }

        public static Round[] Build(ActivityId id)
        {
            switch (id)
            {
                case ActivityId.Count: return CountRounds();
                case ActivityId.Colors: return ColorRounds();
                case ActivityId.Animals: return AnimalRounds();
                case ActivityId.Letters: return LetterRounds();
                default: return RhymeRounds();
            }
        }

        /// <summary>Every activity has one correct choice per round and no duplicate tokens.</summary>
        public static List<string> Validate()
        {
            var problems = new List<string>();
            foreach (ActivityId id in System.Enum.GetValues(typeof(ActivityId)))
            {
                var rounds = Build(id);
                if (rounds.Length < 4)
                    problems.Add(id + " has " + rounds.Length + " rounds");
                for (int i = 0; i < rounds.Length; i++)
                {
                    var round = rounds[i];
                    if (string.IsNullOrEmpty(round.Prompt))
                        problems.Add(id + " round " + i + " has no prompt");
                    int correct = 0;
                    var seen = new HashSet<string>();
                    foreach (var c in round.Choices)
                    {
                        if (c.Correct) correct++;
                        if (!seen.Add(c.Id))
                            problems.Add(id + " round " + i + " repeats " + c.Id);
                    }
                    if (correct != 1)
                        problems.Add(id + " round " + i + " has " + correct + " correct choices");
                    if (id == ActivityId.Count && round.Count < 1)
                        problems.Add(id + " round " + i + " draws no dots");
                }
            }
            return problems;
        }

        static Round[] CountRounds()
        {
            return new[]
            {
                Count(1),
                Count(3),
                Count(2),
                Count(5),
                Count(4)
            };
        }

        static Round Count(int n)
        {
            int lo = n == 1 ? 1 : n - 1;
            int hi = n == 5 ? 5 : n + 1;
            if (lo == n) hi = n + 1;
            var choices = new List<Choice>();
            for (int v = lo; v <= hi; v++)
                choices.Add(new Choice(v.ToString(), v.ToString(), v == n));
            string thing = n == 1 ? "dot" : "dots";
            return new Round("How many " + thing + "?", n, choices.ToArray());
        }

        static Round[] ColorRounds()
        {
            return new[]
            {
                Color("red", "blue", "yellow"),
                Color("blue", "green", "red"),
                Color("yellow", "red", "blue"),
                Color("green", "yellow", "blue")
            };
        }

        static Round Color(string correct, string a, string b)
        {
            return new Round(
                "Which one is " + correct + "?",
                0,
                new Choice(correct, correct, true),
                new Choice(a, a, false),
                new Choice(b, b, false));
        }

        static Round[] AnimalRounds()
        {
            return new[]
            {
                Animal("moo", "cow", "Cow", "dog", "Dog", "cat", "Cat"),
                Animal("woof", "dog", "Dog", "sheep", "Sheep", "duck", "Duck"),
                Animal("meow", "cat", "Cat", "cow", "Cow", "duck", "Duck"),
                Animal("quack", "duck", "Duck", "dog", "Dog", "sheep", "Sheep"),
                Animal("baa", "sheep", "Sheep", "cat", "Cat", "cow", "Cow")
            };
        }

        static Round Animal(string sound, string id, string label, string a, string al, string b, string bl)
        {
            return new Round(
                "Who says " + sound + "?",
                0,
                new Choice(id, label, true),
                new Choice(a, al, false),
                new Choice(b, bl, false));
        }

        static Round[] LetterRounds()
        {
            return new[]
            {
                Letter("A", "apple", "B", "C"),
                Letter("B", "ball", "D", "A"),
                Letter("C", "cat", "E", "B"),
                Letter("D", "dog", "A", "E"),
                Letter("E", "egg", "C", "D")
            };
        }

        static Round Letter(string letter, string word, string a, string b)
        {
            return new Round(
                letter + " is for " + word + ". Find " + letter + ".",
                0,
                new Choice(letter, letter, true),
                new Choice(a, a, false),
                new Choice(b, b, false));
        }

        static Round[] RhymeRounds()
        {
            return new[]
            {
                Rhyme("Twinkle, twinkle, little ___", "star", "Star", "shoe", "Shoe", "fish", "Fish"),
                Rhyme("How I wonder what you ___", "are", "Are", "see", "See", "eat", "Eat"),
                Rhyme("Up above the world so ___", "high", "High", "low", "Low", "wet", "Wet"),
                Rhyme("Like a diamond in the ___", "sky", "Sky", "sea", "Sea", "box", "Box")
            };
        }

        static Round Rhyme(string prompt, string id, string label, string a, string al, string b, string bl)
        {
            return new Round(
                prompt,
                0,
                new Choice(id, label, true),
                new Choice(a, al, false),
                new Choice(b, bl, false));
        }
    }
}
