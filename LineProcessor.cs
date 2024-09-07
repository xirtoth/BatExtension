using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Timers;

namespace BatExtension
{
    public class LineProcessor
    {

        public static CombatStat _combatStat = new CombatStat();
        private static readonly List<Dictionary<string, Func<Match, (Paragraph, bool)>>> PatternProcessors = new List<Dictionary<string, Func<Match, (Paragraph, bool)>>>
        {
            new Dictionary<string, Func<Match, (Paragraph, bool)>>
            {
                { @"\*+ Round (\d+) \*+", ProcessRoundPattern },
                { @"You miss\.", ProcessMissPattern },
                { @"You (pierce|incise|barely graze|slightly pierce|solidly slash|gash|lightly cut|cut|tear|shred|tear|slash|horribly shred|incisively cut|incisively tear|slit|cruelly tatter|savagely shave|rive|cruelly slash|uncontrollably slash|quickly cut|savagely rip|BRUTALLY TEAR|SAVAGELY SHRED|CRUELLY REND|BARBARICALLY REND|DISMEMBER|CRUELLY DISMEMBER) (.+?)\.", ProcessHitPattern }
            }
        };

        private static (Paragraph, bool) ProcessMissPattern(Match match)
        {
            var paragraph = new Paragraph();
            paragraph.Inlines.Add("YOU MISSED");
            _combatStat.IncreaseMisses();
            return (paragraph, combatTimer.Enabled);
        }

        private static (Paragraph, bool) ProcessHitPattern(Match match)
        {
            var hitType = match.Groups[1].Value.ToUpper();
            var enemyName = match.Groups[2].Value;
            var paragraph = new Paragraph();
            Console.WriteLine($"YOU {hitType} {enemyName}.");
            paragraph.Inlines.Add($"YOU {hitType} {enemyName}");
            _combatStat.IncreaseHits();
            _combatStat.AddHitType(hitType);
            _combatStat.EnemyName = enemyName;
            return (paragraph, combatTimer.Enabled);
        }

        private static readonly int CombatResetTime = 4000;

        private static System.Timers.Timer combatTimer;
        private static Action<bool>? setCombatState;

        static LineProcessor()
        {
            combatTimer = new System.Timers.Timer(CombatResetTime); // 5 seconds
            combatTimer.Elapsed += (sender, e) => CombatEnded();
            combatTimer.AutoReset = false;
        }

        private static void CombatEnded()
        {
            AppState.Instance.AddCombatStat(_combatStat);
            setCombatState?.Invoke(false);

            _combatStat = new CombatStat();
        }

        public static void Initialize(Action<bool> setCombatStateAction)
        {
            setCombatState = setCombatStateAction;
        }

        public static (Paragraph, bool) ProcessLine(string line)
        {
            foreach (var patternProcessor in PatternProcessors)
            {
                foreach (var pattern in patternProcessor.Keys)
                {
                    var match = Regex.Match(line, pattern);
                    if (match.Success)
                    {
                        var (paragraph, inCombat) = patternProcessor[pattern](match);
                        if (inCombat)
                        {
                            combatTimer.Stop();
                            combatTimer.Start();
                            setCombatState?.Invoke(true);
                        }
                        return (paragraph, inCombat);
                    }
                }
            }

            // Default processing for lines that do not match any pattern
            return (ProcessDefault(line), combatTimer.Enabled);
        }

        private static (Paragraph, bool) ProcessRoundPattern(Match match)
        {
            var roundNumber = match.Groups[1].Value;
            var paragraph = new Paragraph();
            bool inCombat = roundNumber == "1" || int.TryParse(roundNumber, out _);
            if (roundNumber == "1")
            {
                paragraph.Inlines.Add("COMBAT STARTED");
                _combatStat = new CombatStat();
                _combatStat.IncreaseRound();
                _combatStat.Time = DateTime.Now;
                inCombat = true;
            }
            else
            {
                paragraph.Inlines.Add($"ROUND {roundNumber}");
                inCombat = true;
                _combatStat.IncreaseRound();
            }
            return (paragraph, inCombat);
        }

        private static Paragraph ProcessDefault(string line)
        {
            var paragraph = new Paragraph();
            var parts = line.Split(new[] { "[" }, StringSplitOptions.None);
            foreach (var part in parts)
            {
                if (string.IsNullOrEmpty(part)) continue;

                var textRun = new Run();
                var codeEndIndex = part.IndexOf('m');
                if (codeEndIndex != -1)
                {
                    var code = part.Substring(0, codeEndIndex);
                    var text = part.Substring(codeEndIndex + 1);

                    switch (code)
                    {
                        case "0": // Reset
                            textRun.Text = text;
                            textRun.ClearValue(TextElement.FontWeightProperty);
                            textRun.ClearValue(TextElement.ForegroundProperty);
                            break;

                        case "1": // Bold
                            textRun.Text = text;
                            textRun.FontWeight = FontWeights.Bold;
                            break;

                        case "30": // Black
                            textRun.Text = text;
                            textRun.Foreground = new SolidColorBrush(Colors.Black);
                            break;

                        case "31": // Red
                            textRun.Text = text;
                            textRun.Foreground = new SolidColorBrush(Colors.Red);
                            break;

                        case "32": // Green
                            textRun.Text = text;
                            textRun.Foreground = new SolidColorBrush(Colors.Green);
                            break;

                        case "33": // Yellow
                            textRun.Text = text;
                            textRun.Foreground = new SolidColorBrush(Colors.Yellow);
                            break;

                        case "34": // Blue
                            textRun.Text = text;
                            textRun.Foreground = new SolidColorBrush(Colors.Blue);
                            break;

                        case "35": // Magenta
                            textRun.Text = text;
                            textRun.Foreground = new SolidColorBrush(Colors.Magenta);
                            break;

                        case "36": // Cyan
                            textRun.Text = text;
                            textRun.Foreground = new SolidColorBrush(Colors.Cyan);
                            break;

                        case "37": // White
                            textRun.Text = text;
                            textRun.Foreground = new SolidColorBrush(Colors.White);
                            break;

                        case "1;30": // Bold Black
                            textRun.Text = text;
                            textRun.FontWeight = FontWeights.Bold;
                            textRun.Foreground = new SolidColorBrush(Colors.Black);
                            break;

                        case "1;31": // Bold Red
                            textRun.Text = text;
                            textRun.FontWeight = FontWeights.Bold;
                            textRun.Foreground = new SolidColorBrush(Colors.Red);
                            break;

                        case "1;32": // Bold Green
                            textRun.Text = text;
                            textRun.FontWeight = FontWeights.Bold;
                            textRun.Foreground = new SolidColorBrush(Colors.Green);
                            break;

                        case "1;33": // Bold Yellow
                            textRun.Text = text;
                            textRun.FontWeight = FontWeights.Bold;
                            textRun.Foreground = new SolidColorBrush(Colors.Yellow);
                            break;

                        case "1;34": // Bold Blue
                            textRun.Text = text;
                            textRun.FontWeight = FontWeights.Bold;
                            textRun.Foreground = new SolidColorBrush(Colors.Blue);
                            break;

                        case "1;35": // Bold Magenta
                            textRun.Text = text;
                            textRun.FontWeight = FontWeights.Bold;
                            textRun.Foreground = new SolidColorBrush(Colors.Magenta);
                            break;

                        case "1;36": // Bold Cyan
                            textRun.Text = text;
                            textRun.FontWeight = FontWeights.Bold;
                            textRun.Foreground = new SolidColorBrush(Colors.Cyan);
                            break;

                        case "1;37": // Bold White
                            textRun.Text = text;
                            textRun.FontWeight = FontWeights.Bold;
                            textRun.Foreground = new SolidColorBrush(Colors.White);
                            break;

                        default:
                            textRun.Text = part;
                            break;
                    }
                }
                else
                {
                    textRun.Text = part;
                }

                paragraph.Inlines.Add(textRun);
            }

            return paragraph;
        }
    }
}