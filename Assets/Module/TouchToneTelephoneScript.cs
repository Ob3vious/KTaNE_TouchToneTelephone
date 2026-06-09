using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading;
using UnityEngine;

public class TouchToneTelephoneScript : MonoBehaviour
{
    private static List<KeypadRule> _ifRules = new List<KeypadRule>
    {
        new KeypadRule("A", (hist, _) => { return hist.GetCurrent().GetX() == 0; }),
        new KeypadRule("B", (hist, _) => { return hist.GetCurrent().GetX() == 1; }),
        new KeypadRule("C", (hist, _) => { return hist.GetCurrent().GetX() == 2; }),
        new KeypadRule("D", (hist, _) => { return Mathf.Abs(hist.GetCurrent().GetX() - hist.GetNext().GetX()) == Mathf.Abs(hist.GetCurrent().GetY() - hist.GetNext().GetY()); }),
        new KeypadRule("E", (hist, _) => { return hist.GetNext().GetX() > hist.GetCurrent().GetX(); }),
        new KeypadRule("F", (hist, _) => { return (hist.GetCurrent() + hist.GetNext()) % 2 == 1; }),
        new KeypadRule("G", (hist, _) => { return hist.GetCurrent().GetX() == hist.GetCurrent().GetY(); }),
        new KeypadRule("H", (hist, _) => { return hist.GetCurrent().GetY() == hist.GetNext().GetY(); }),
        new KeypadRule("I", (hist, _) => { return (hist.GetCurrent() + hist.GetNext()) % 2 == 0; }),
        new KeypadRule("J", (hist, _) => { return Mathf.Abs(hist.GetCurrent().GetX() - hist.GetNext().GetX()) <= 1 && Mathf.Abs(hist.GetCurrent().GetY() - hist.GetNext().GetY()) <= 1 && hist.GetCurrent() != hist.GetNext(); }),
        new KeypadRule("K", (hist, _) => { return hist.GetCurrent().GetX() + hist.GetCurrent().GetY() == 2; }),
        new KeypadRule("L", (hist, _) => { return Mathf.Abs(hist.GetCurrent().GetX() - hist.GetNext().GetX()) + Mathf.Abs(hist.GetCurrent().GetY() - hist.GetNext().GetY()) <= 1 && hist.GetCurrent() != hist.GetNext(); }),
        new KeypadRule("M", (_1, _2) => { return true; }),
        new KeypadRule("N", (hist, _) => { return hist.GetNext().GetY() < hist.GetCurrent().GetY(); }),
        new KeypadRule("O", (hist, _) => { return hist.GetCurrent() == 0; }),
        new KeypadRule("P", (hist, _) => { return hist.Take(hist.Count - 1).Count(x => x == hist.GetCurrent()) == 1; }),
        new KeypadRule("Q", (hist, required) => { return hist.Count > 1 && hist.Take(hist.Count - 1).Count(x => x == hist.GetCurrent()) == required[hist.GetCurrent()]; }, true),
        new KeypadRule("R", (hist, _) => { return hist.GetCurrent().GetX() == hist.GetNext().GetX() || hist.GetCurrent().GetY() == hist.GetNext().GetY(); }),
        new KeypadRule("S", (hist, _) => { return hist.GetNext().GetY() > hist.GetCurrent().GetY(); }),
        new KeypadRule("T", (hist, _) => { return hist.GetCurrent() % 2 == 0; }),
        new KeypadRule("U", (hist, _) => { return hist.GetCurrent() % 2 == 1; }),
        new KeypadRule("V", (hist, _) => { return hist.GetCurrent().GetX() == hist.GetNext().GetX(); }),
        new KeypadRule("W", (hist, _) => { return hist.GetNext().GetX() < hist.GetCurrent().GetX(); }),
        new KeypadRule("X", (hist, _) => { return hist.GetCurrent().GetY() == 0; }),
        new KeypadRule("Y", (hist, _) => { return hist.GetCurrent().GetY() == 1; }),
        new KeypadRule("Z", (hist, _) => { return hist.GetCurrent().GetY() == 2; }),
    };

    private static List<KeypadRule> _thenRules = new List<KeypadRule>
    {
        new KeypadRule("A", (hist, _) => { return hist.GetNext().GetX() == 0; }),
        new KeypadRule("B", (hist, _) => { return hist.GetNext().GetX() == 1; }),
        new KeypadRule("C", (hist, _) => { return hist.GetNext().GetX() == 2; }),
        new KeypadRule("D", (hist, _) => { return Mathf.Abs(hist.GetCurrent().GetX() - hist.GetNext().GetX()) == Mathf.Abs(hist.GetCurrent().GetY() - hist.GetNext().GetY()); }),
        new KeypadRule("E", (hist, _) => { return hist.GetNext().GetX() > hist.GetCurrent().GetX(); }),
        new KeypadRule("F", (hist, _) => { return (hist.GetCurrent() + hist.GetNext()) % 2 == 1; }),
        new KeypadRule("G", (hist, _) => { return hist.GetNext().GetX() == hist.GetNext().GetY(); }),
        new KeypadRule("H", (hist, _) => { return hist.GetCurrent().GetY() == hist.GetNext().GetY(); }),
        new KeypadRule("I", (hist, _) => { return (hist.GetCurrent() + hist.GetNext()) % 2 == 0; }),
        new KeypadRule("J", (hist, _) => { return Mathf.Abs(hist.GetCurrent().GetX() - hist.GetNext().GetX()) <= 1 && Mathf.Abs(hist.GetCurrent().GetY() - hist.GetNext().GetY()) <= 1 && hist.GetCurrent() != hist.GetNext(); }),
        new KeypadRule("K", (hist, _) => { return hist.GetNext().GetX() + hist.GetNext().GetY() == 2; }),
        new KeypadRule("L", (hist, _) => { return Mathf.Abs(hist.GetCurrent().GetX() - hist.GetNext().GetX()) + Mathf.Abs(hist.GetCurrent().GetY() - hist.GetNext().GetY()) <= 1 && hist.GetCurrent() != hist.GetNext(); }),
        new KeypadRule("M", (_1, _2) => { return false; }),
        new KeypadRule("N", (hist, _) => { return hist.GetNext().GetY() < hist.GetCurrent().GetY(); }),
        new KeypadRule("O", (hist, _) => { return hist.GetNext() == 0; }),
        new KeypadRule("P", (hist, _) => { return hist.Count(x => x == hist.GetNext()) == 1; }),
        new KeypadRule("Q", (hist, required) => { return hist.Count(x => x == hist.GetNext()) == required[hist.GetNext()]; }, true),
        new KeypadRule("R", (hist, _) => { return hist.GetCurrent().GetX() == hist.GetNext().GetX() ||  hist.GetCurrent().GetY() == hist.GetNext().GetY(); }),
        new KeypadRule("S", (hist, _) => { return hist.GetNext().GetY() > hist.GetCurrent().GetY(); }),
        new KeypadRule("T", (hist, _) => { return hist.GetNext() % 2 == 0; }),
        new KeypadRule("U", (hist, _) => { return hist.GetNext() % 2 == 1; }),
        new KeypadRule("V", (hist, _) => { return hist.GetCurrent().GetX() == hist.GetNext().GetX(); }),
        new KeypadRule("W", (hist, _) => { return hist.GetNext().GetX() < hist.GetCurrent().GetX(); }),
        new KeypadRule("X", (hist, _) => { return hist.GetNext().GetY() == 0; }),
        new KeypadRule("Y", (hist, _) => { return hist.GetNext().GetY() == 1; }),
        new KeypadRule("Z", (hist, _) => { return hist.GetNext().GetY() == 2; })
    };

    private static string _radioStation = "";

    private static string[] _keypad = new string[] { "O", "QZ", "ABC", "DEF", "GHI", "JKL", "MN", "PRS", "TUV", "WXY" };

    private static bool _threading = false;

    private List<RulePair> _rules = null;
    private List<int> _puzzle = null;

    private List<int> _secondaryPuzzle = null;
    private bool[] _showElements = null;

    public KMSelectable Phone;
    public TextMesh NoteText;
    public List<KMSelectable> Keys;
    public List<TextMesh> KeyTexts;
    public MeshRenderer CaptionOverlay;
    public List<TextMesh> CaptionTexts;

    private string _currentInput = "";
    private bool _isPickingUp = false;
    private float _pickupLerp = 0;
    private float _overlayLerp = 0;
    private bool _isSolved = false;

    private KMAudio.KMAudioRef _noise;

    private int _moduleId;
    private static int _moduleIdCounter = 1;

    void Start()
    {
        _moduleId = _moduleIdCounter++;

        if (_radioStation == "")
        {
            byte[] hash = SHA512.Create().ComputeHash(Encoding.UTF8.GetBytes(Environment.OSVersion + "\\" + Environment.MachineName + "\\" + Environment.UserName + "\\" + Environment.CurrentDirectory));
            long radioStation = 0;
            foreach (byte element in hash)
            {
                radioStation = (radioStation << 8) | element;
                radioStation %= 9000000000;
            }
            radioStation += 1000000000;

            _radioStation = (radioStation / 1000000).ToString("0000") + "-" + (radioStation % 1000000).ToString("000000");

            Log("Searching for a local radio station...");
        }
        Log("The phone number of your local radio station is {0}.", _radioStation);

        SetupVisuals();

        StartCoroutine(GeneratePuzzle());
    }

    void OnDestroy()
    {
        _threading = false;
        if (_noise != null)
        {
            _noise.StopSound();
            _noise = null;
        }
    }

    void Update()
    {
        if (_isSolved)
            _isPickingUp = false;

        float lerpTime = 0.25f;

        if (_isPickingUp && _pickupLerp < 1)
            _pickupLerp += Time.deltaTime / lerpTime;
        else if (_isPickingUp)
            _overlayLerp += Time.deltaTime / lerpTime;

        if (!_isPickingUp && _pickupLerp > 0)
        {
            _pickupLerp -= Time.deltaTime / lerpTime;
            if (_pickupLerp <= 0)
                GetComponent<KMAudio>().PlaySoundAtTransform("dropdown", Phone.transform);
        }
        if (_pickupLerp < 1 && _overlayLerp > 0)
            _overlayLerp -= Time.deltaTime / lerpTime;

        _pickupLerp = Mathf.Clamp01(_pickupLerp);
        _overlayLerp = Mathf.Clamp01(_overlayLerp);

        SetOverlayIntensity(_pickupLerp, _overlayLerp);
    }

    private void TextMessage(string phone, string data)
    {
        KMBombModule module = GetComponent<KMBombModule>();
        if (phone == "")
        {
            Log("You're trying to send a message to nowhere. At least no one had to deal with spam.");
            return;
        }
        if (phone == _radioStation.Replace("-", ""))
        {

            if (data == _secondaryPuzzle.Join(""))
            {
                Log("You have contacted your local radio station with the correct message \"{0}\". Module solved!", data);
                module.HandlePass();
                _isSolved = true;
            }
            else
            {
                Log("You have contacted your local radio station with the wrong message \"{0}\". Strike!", data);
                module.HandleStrike();
            }
            return;
        }
        if (phone == _puzzle.Join(""))
        {
            Log("You have contacted your alien friends with the message \"{0}\". Maybe don't do that if you want to stay out of trouble. Strike!", data);
            if (data == _secondaryPuzzle.Join(""))
            {
                Log("Also, they caught on to your attempt to leak the information they gave you. The phone is broken now. Have fun!");
                //solved state incapacitates the entire module
                _isSolved = true;
            }
            module.HandleStrike();

            return;
        }
        Log("You have contacted the wrong number ({0}).", phone);
        module.HandleStrike();
    }

    private bool Call(string phone)
    {
        KMBombModule module = GetComponent<KMBombModule>();
        if (phone.Contains('*'))
        {
            Log("Phone numbers usually don't contain stars, do they?");
            return false;
        }
        if (phone == "")
        {
            Log("You're trying to call no one.");
            return false;
        }
        if (phone == _radioStation.Replace("-", ""))
        {
            Log("You have called your local radio station. Please try sticking to the plan and just text them when you have something. Strike!");
            module.HandleStrike();
            return false;
        }
        if (phone == _puzzle.Join(""))
        {
            Log("You have successfully contacted your alien friends! A message will be revealed to you.");
            return true;
        }
        Log("You're calling a random stranger ({0}). It was not appreciated. Strike!", phone);
        module.HandleStrike();
        return false;
    }

    private void SetupVisuals()
    {
        NoteText.text = "1 second\nI'm on it\n\n\n" + _radioStation;
        for (int i = 1; i < 12; i++)
        {
            Keys.Add(Instantiate(Keys[0], Keys[0].transform.parent));
            KeyTexts.AddRange(Keys.Last().GetComponentsInChildren<TextMesh>());
            Keys.Last().transform.localPosition += new Vector3(i % 3, 0, i / -3) * 0.0225f;
        }
        KMSelectable moduleSelectable = GetComponent<KMSelectable>();
        moduleSelectable.Children = moduleSelectable.Children.Concat(Keys).ToArray();
        moduleSelectable.UpdateChildren();

        KMAudio audio = GetComponent<KMAudio>();

        for (int i = 0; i < 12; i++)
        {
            KeyTexts[i * 2].text = "123456789*0#"[i].ToString();
            if (i == 9 || i == 11)
            {
                KeyTexts[i * 2].characterSize = 0.002f;
                KeyTexts[i * 2].transform.localPosition -= new Vector3(0, 0.0025f, 0);
                KeyTexts[i * 2 + 1].text = "";
            }
            else
                KeyTexts[i * 2 + 1].text = _keypad[new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, -1, 0, -1 }[i]];

            int i2 = i;
            Keys[i].OnInteract += () =>
            {
                Keys[i2].AddInteractionPunch(0.5f);
                audio.PlaySoundAtTransform("c" + (i2 % 3 + 1), transform);
                audio.PlaySoundAtTransform("r" + (i2 / 3 + 1), transform);

                if (_isSolved)
                    return false;

                switch (i2)
                {
                    case 9:
                        //star
                        if (_currentInput.Count(x => x == '*') == 2)
                        {
                            //send text message here
                            string[] contents = _currentInput.Split('*');
                            TextMessage(contents[1], contents[2]);
                            _currentInput = "";
                            break;
                        }

                        if (_currentInput.Length == 0 || _currentInput[0] != '*')
                            _currentInput = "";

                        _currentInput += "*";
                        break;
                    case 11:
                        //pound
                        _currentInput = "";
                        break;
                    default:
                        //number
                        _currentInput += new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, -1, 0, -1 }[i2];
                        break;
                }

                return false;
            };
        }

        CaptionTexts[0].text = "";
        CaptionTexts[0].GetComponent<Renderer>().material.renderQueue = 3002;
        for (int i = 0; i < _keypad.Length; i++)
        {
            CaptionTexts.Add(Instantiate(CaptionTexts[0], CaptionTexts[0].transform.parent));
            Vector3 keyPos = Keys[new int[] { 10, 0, 1, 2, 3, 4, 5, 6, 7, 8 }[i]].transform.localPosition;
            CaptionTexts.Last().transform.localPosition = new Vector3(keyPos.x * 5 + 0.012f, keyPos.z * 5, 0);
            CaptionTexts.Last().text = "-";
        }
        SetOverlayIntensity(0, 0);

        Phone.OnInteract += () =>
        {
            audio.PlaySoundAtTransform("pickup", Phone.transform);

            if (_isSolved)
                return false;

            if (_isPickingUp)
                return false;

            if (!Call(_currentInput))
            {
                _currentInput = "";
                return false;
            }
            _currentInput = "";

            _isPickingUp = true;
            _noise = audio.PlaySoundAtTransformWithRef("noise", Phone.transform);

            return false;
        };

        Phone.OnInteractEnded += () =>
        {
            _isPickingUp = false;
            if (_noise != null)
            {
                _noise.StopSound();
                _noise = null;
            }
        };
    }

    private void AddPuzzleVisuals()
    {
        List<string> rules = new List<string>();
        foreach (RulePair rule in _rules)
        {
            int[] indices = rule.Name().Select(x => Enumerable.Range(0, _keypad.Length).First(y => _keypad[y].Contains(x))).ToArray();
            rules.Add(Enumerable.Range(0, indices.Length).Select(x => Enumerable.Repeat(indices[x], _keypad[indices[x]].IndexOf(rule.Name()[x]) + 1).Join("")).Join("-"));
        }

        for (int i = 0; i < rules.Count; i++)
        {
            if (i == 0)
            {
                NoteText.text = rules[i];
                continue;
            }

            NoteText.text += (i % 2 == 0 ? "\n" : "  ") + rules[i];
        }
        NoteText.text += Enumerable.Repeat("\n", 5 - (rules.Count + 1) / 2).Join("") + _radioStation;

        bool[][] showLetters = _keypad.Select(x => x.Select(_ => true).ToArray()).ToArray();
        foreach (int digit in _puzzle)
            showLetters[digit][Enumerable.Range(0, showLetters[digit].Length).Where(x => showLetters[digit][x]).PickRandom()] &= false;
        for (int i = 0; i < _keypad.Length; i++)
            KeyTexts[new int[] { 10, 0, 1, 2, 3, 4, 5, 6, 7, 8 }[i] * 2 + 1].text = Enumerable.Range(0, showLetters[i].Length).Select(x => showLetters[i][x] ? _keypad[i][x].ToString() : ("<color='#ffffff40'>" + _keypad[i][x] + "</color>")).Join("");

        string characters = "ABCDEFGHIJKLMNOPQRSTUV".ToList().Shuffle().Take(_keypad.Length).Join("");
        for (int i = 0; i < _keypad.Length; i++)
            CaptionTexts[i + 1].text = _showElements[i] ? characters[i].ToString() : "";
        CaptionTexts[0].text = "";
        for (int i = 0; i < _secondaryPuzzle.Count; i++)
            CaptionTexts[0].text += _showElements[i + _keypad.Length] ? characters[_secondaryPuzzle[i]] : '-';
    }

    private void SetOverlayIntensity(float lerp1, float lerp2)
    {
        CaptionOverlay.material.color = new Color(0, 0, 0, lerp2 / 2);
        foreach (TextMesh text in CaptionTexts)
            text.color = new Color(1, 1, 1, lerp2);

        Phone.transform.localPosition = Vector3.Lerp(new Vector3(0, 0.015f, 0.055f), new Vector3(0.075f, 0.105f, 0.055f), lerp1);
        Phone.transform.localEulerAngles = new Vector3(0, 0, -75 * lerp1);
    }

    private IEnumerator GeneratePuzzle()
    {
        while (true)
        {
            yield return null;
            while (_threading)
                yield return null;
            _threading = true;
            //Log("Thread claimed");

            //some stuff I could also do before thread claiming but this looks better timed in logging
            _rules = new List<RulePair>();

            int ruleCount = UnityEngine.Random.Range(4, 7);
            while (ruleCount-- > 0)
                _rules.Add(new RulePair(_ifRules.PickRandom(), _thenRules.PickRandom()));

            //Log("Trying " + _rules.Select(x => x.Name()).Join(" "));

            List<List<int>> solutions = null;
            bool abort = false;
            //end of pre-thread stuff

            new Thread(() => solutions = EvaluateAllSequencesFast(_rules, 7, 9, ref abort, true)).Start();

            float t = 0;
            while (t < 15 && solutions == null)
            {
                t += Time.unscaledDeltaTime;
                yield return null;
            }

            if (solutions == null)
            {
                //Log("Reached timeout");
                abort = true;
                yield return new WaitUntil(() => solutions != null);
            }

            if (solutions.Count == 0)
            {
                //Log("Releasing thread control");
                _threading = false;
                continue;
            }

            _puzzle = solutions.PickRandom();

            abort = false;

            List<List<int>> secondarySequences = null;
            new Thread(() => secondarySequences = EvaluateAllSequencesFast(_rules, 4, 6, ref abort, false)).Start();

            yield return new WaitUntil(() => secondarySequences != null);

            if (secondarySequences.Count == 0)
            {
                //Log("Releasing thread control");
                _threading = false;
                continue;
            }

            _secondaryPuzzle = secondarySequences.PickRandom();

            List<bool[]> reductions = null;

            new Thread(() => reductions = GenerateAllInfoReductions(_secondaryPuzzle, _rules)
                .Where(x => Enumerable.Range(0, _secondaryPuzzle.Count).Count(y => !x[y + _keypad.Length] || (!x[_secondaryPuzzle[y]] && x.Take(_keypad.Length).Count(z => !z) > 1)) >= 2).ToList())
                .Start();

            yield return new WaitUntil(() => reductions != null);

            if (_threading)
            {
                //Log("Releasing thread control");
                _threading = false;
            }
            if (reductions.Count == 0)
                continue;

            int target = reductions.Select(x => x.Count(y => !y)).Max() - 1;
            _showElements = reductions.Where(x => x.Count(y => !y) >= target).PickRandom();

            Log("The rule pairs generated are: {0}.", _rules.Select(x => x.Name()).Join(", "));
            Log("The alien phone number is: {0}.", _puzzle.Join(""));
            Log("The alien message is: {0}.", _secondaryPuzzle.Join(""));
            //Log(_showElements.Select(x => x ? 1 : 0).Join(""));
            Log("The defined keypad digits are: {0}. The defined code digits are in the following positions: {1}.",
                Enumerable.Range(0, _keypad.Length).Where(x => _showElements[x]).Join(", "),
                Enumerable.Range(0, _secondaryPuzzle.Count).Where(x => _showElements[x + _keypad.Length]).Select(x => x + 1).Join(", "));

            break;
        }

        AddPuzzleVisuals();
    }

    private static List<bool[]> GenerateAllInfoReductions(List<int> code, List<RulePair> rules)
    {
        List<bool[]> options = new List<bool[]> { Enumerable.Repeat(true, 10 + code.Count).ToArray() };

        for (int i = 0; i < 10 + code.Count; i++)
            options.AddRange(options.Select(x => Enumerable.Range(0, x.Length).Select(y => y != i && x[y]).ToArray()).Where(x => GetSolution(x, code, rules) != null).ToList());

        return options;
    }

    private static List<List<int>> EvaluateAllSequencesFast(List<RulePair> rules, int minLength, int maxLength, ref bool abort, bool filterOnUseCount)
    {
        Queue<List<int>> toEvaluate = new Queue<List<int>>();
        toEvaluate.Enqueue(new List<int>());

        Dictionary<string, List<int>> legalCodes = new Dictionary<string, List<int>>();

        while (toEvaluate.Count > 0)
        {
            try
            {
                if (abort || !_threading)
                    return new List<List<int>>();

                List<int> eval = toEvaluate.Dequeue();

                if (eval.Count > minLength)
                {
                    if (filterOnUseCount)
                    {
                        int[] distribution = Enumerable.Range(0, _keypad.Length).Select(x => eval.Count(y => y == x)).ToArray();
                        if (rules.Where(x => x.CheckLate()).All(x => x.FullApplies(eval, distribution)))
                        {
                            string distributionString = distribution.Join();
                            if (legalCodes.ContainsKey(distributionString))
                                legalCodes[distributionString] = null;
                            else
                                legalCodes.Add(distributionString, eval);
                        }
                    }
                    else
                        legalCodes.Add(eval.Join(), eval);
                }

                if (eval.Count < maxLength)
                    for (int i = 0; i < 10; i++)
                    {
                        List<int> newChain = eval.Concat(new int[] { i }).ToList();

                        if (filterOnUseCount && newChain.Count(y => y == i) > _keypad[i].Length)
                            continue;

                        if (newChain.Count < 2 || rules.Where(x => !x.CheckLate()).All(x => x.Applies(newChain, null)))
                            toEvaluate.Enqueue(newChain);
                    }
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return new List<List<int>>();
            }
        }


        return legalCodes.Where(x => x.Value != null).Select(x => x.Value).ToList();
    }

    private static List<int[]> GenerateContinuations(int[] counts)
    {
        int index = Enumerable.Range(0, counts.Length).LastOrDefault(x => counts[x] > 0);
        if (counts[index] >= _keypad[index].Length)
            index++;

        return Enumerable.Range(index, counts.Length - index).Select(x => Enumerable.Range(0, counts.Length).Select(y => counts[y] + (y == x ? 1 : 0)).ToArray()).ToList();
    }

    private static List<int> GetSolution(int[] counts, List<RulePair> rules, bool mustBeUnique)
    {
        List<int> solution = null;

        Queue<List<int>> toEvaluate = new Queue<List<int>>();
        toEvaluate.Enqueue(new List<int>());

        while (toEvaluate.Count > 0)
        {
            List<int> eval = toEvaluate.Dequeue();

            bool good = eval.Count <= 1;
            if (!good)
                good = rules.All(x => x.Applies(eval, counts));

            if (!good)
                continue;

            if (eval.Count == counts.Sum())
            {
                if (solution != null)
                    return null;

                solution = eval;
                if (!mustBeUnique)
                    return solution;

                continue;
            }

            IEnumerable<int> possibleExtensions = Enumerable.Range(0, counts.Length).Where(x => eval.Count(y => y == x) < counts[x]);
            foreach (int extension in possibleExtensions)
                toEvaluate.Enqueue(eval.Concat(new int[] { extension }).ToList());
        }

        return solution;
    }

    private static List<int> GetSolution(bool[] infoReduction, List<int> code, List<RulePair> rules)
    {
        List<int> solution = null;

        Queue<List<int>> toEvaluate = new Queue<List<int>>();
        toEvaluate.Enqueue(new List<int>());

        while (toEvaluate.Count > 0)
        {
            List<int> eval = toEvaluate.Dequeue();

            bool good = eval.Count <= 1;
            if (!good)
                good = rules.Where(x => !x.CheckLate()).All(x => x.Applies(eval, null));

            if (!good)
                continue;

            if (eval.Count == code.Count)
            {
                if (!rules.Where(x => x.CheckLate()).All(x => x.Applies(eval, Enumerable.Range(0, 10).Select(y => eval.Count(z => z == y)).ToArray())))
                    continue;

                if (solution != null)
                {
                    //Debug.Log(infoReduction.Select(x => x ? 1 : 0).Join("") + ":" + solution.Join("") + "+" + eval.Join(""));
                    return null;
                }

                solution = eval;
                continue;
            }

            List<int> possibleExtensions = Enumerable.Range(0, 10).ToList();
            if (infoReduction[eval.Count + _keypad.Length])
            {
                if (infoReduction[code[eval.Count]])
                    possibleExtensions = possibleExtensions.Where(x => x == code[eval.Count]).ToList();

                for (int i = 0; i < eval.Count; i++)
                {
                    if (!infoReduction[i + _keypad.Length])
                        continue;

                    possibleExtensions = possibleExtensions.Where(x => (code[eval.Count] == code[i]) == (x == eval[i])).ToList();
                }
            }

            foreach (int extension in possibleExtensions)
                toEvaluate.Enqueue(eval.Concat(new int[] { extension }).ToList());
        }

        return solution;
    }

    private void Log(string text, params object[] args)
    {
        Debug.LogFormat("[Touch-Tone Telephone #{0}] {1}", _moduleId, string.Format(text, args));
    }

#pragma warning disable 414
    private string TwitchHelpMessage = "'!{0} call 381654729' to call phone number 381654729. '!{0} hang up' to hang up. '!{0} text 381654729 69420' to text the number 69420 to the phone number 381654729. Use digits only.";
#pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
        yield return null;

        command = command.ToLowerInvariant();
        string[] commands = command.Split(' ');
        if (command == "hang up")
        {
            if (!_isPickingUp)
            {
                yield return "sendtochaterror You can't hang up if there is no ongoing call.";
                yield break;
            }
            Phone.OnInteractEnded();
        }
        else if (commands.Length == 2 && commands[0] == "call" && commands[1].RegexMatch(@"^\d+$"))
        {
            if (_isPickingUp)
                Phone.OnInteractEnded();

            if (_currentInput != "")
            {
                Keys[11].OnInteract();
                yield return new WaitForSeconds(0.25f);
            }
            foreach (char c in commands[1])
            {
                Keys["123456789*0#".IndexOf(c)].OnInteract();
                yield return new WaitForSeconds(0.25f);
            }
            Phone.OnInteract();
        }
        else if (commands.Length == 3 && commands[0] == "text" && commands[1].RegexMatch(@"^\d+$") && commands[2].RegexMatch(@"^\d+$"))
        {
            if (_isPickingUp)
                Phone.OnInteractEnded();

            if (_currentInput != "")
            {
                Keys[11].OnInteract();
                yield return new WaitForSeconds(0.25f);
            }
            Keys[9].OnInteract();
            yield return new WaitForSeconds(0.25f);
            foreach (char c in commands[1])
            {
                Keys["123456789*0#".IndexOf(c)].OnInteract();
                yield return new WaitForSeconds(0.25f);
            }
            Keys[9].OnInteract();
            yield return new WaitForSeconds(0.25f);
            foreach (char c in commands[2])
            {
                Keys["123456789*0#".IndexOf(c)].OnInteract();
                yield return new WaitForSeconds(0.25f);
            }
            Keys[9].OnInteract();
        }
        else
        {
            yield return "sendtochaterror Invalid command.";
            yield break;
        }
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        yield return null;

        while (_showElements == null)
            yield return true;

        //for the sake of regexmatch use '!'
        string solution = "*" + _radioStation.Replace("-", "") + "*" + _secondaryPuzzle.Join("") + "*";
        while (!_isSolved)
        {
            if (solution.Substring(0, _currentInput.Length) != _currentInput)
            {
                Keys[11].OnInteract();
                yield return new WaitForSeconds(0.25f);
            }
            Keys["123456789*0#".IndexOf(solution[_currentInput.Length])].OnInteract();
            yield return new WaitForSeconds(0.25f);
        }
    }
}
