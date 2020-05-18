using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;

public class AlphabetizeScript : MonoBehaviour
{

    public KMAudio Audio;
    public KMBombInfo Bomb;
    public KMBombModule Module;
	
	public AudioClip[] SFX;

    public KMSelectable LeftQuad;
    public KMSelectable RightQuad;
	
	public TextMesh TheLetter;
	public TextMesh TheFiber;
	public TextMesh TheTetra;
	
	private bool Playable = false;
	
	private int Computer = 0;

    private int TheCopperValue = 0;
	private int TheFoil = 0;
	private int Supper = 0;

	private int[] SilverLine;
	
	private string[] Alphabreak = {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"};

	
	private string[][] TheSequence = new string[16][]{
		new string[26] {"W", "L", "U", "A", "Z", "V", "H", "E", "J", "D", "N", "Q", "S", "Y", "F", "P", "G", "M", "B", "O", "I", "R", "C", "X", "T", "K"},
		new string[26] {"M", "V", "F", "B", "X", "J", "Q", "N", "H", "W", "Z", "T", "A", "K", "P", "E", "O", "C", "U", "R", "I", "S", "D", "Y", "L", "G"},
		new string[26] {"D", "E", "Y", "Q", "L", "F", "Z", "O", "U", "R", "N", "P", "H", "V", "K", "I", "M", "S", "J", "A", "C", "G", "X", "T", "W", "B"},
		new string[26] {"L", "B", "M", "C", "H", "N", "A", "K", "V", "F", "J", "O", "S", "G", "D", "Q", "E", "P", "U", "X", "R", "W", "I", "Z", "T", "Y"},
		new string[26] {"B", "V", "U", "H", "F", "M", "A", "L", "P", "I", "N", "Y", "S", "G", "J", "T", "R", "Q", "K", "E", "O", "X", "W", "D", "C", "Z"},
		new string[26] {"O", "X", "F", "G", "N", "S", "K", "U", "V", "P", "H", "Q", "J", "I", "Z", "W", "C", "T", "R", "Y", "A", "L", "E", "M", "D", "B"},
		new string[26] {"E", "S", "F", "K", "U", "Y", "Z", "O", "P", "A", "W", "V", "R", "J", "B", "M", "I", "X", "G", "D", "Q", "N", "T", "H", "C", "L"},
		new string[26] {"K", "H", "L", "C", "I", "S", "Q", "N", "P", "O", "M", "B", "G", "J", "Z", "W", "R", "A", "Y", "V", "X", "T", "F", "D", "E", "U"},
		new string[26] {"G", "W", "Y", "U", "S", "D", "N", "Z", "Q", "F", "V", "A", "L", "R", "E", "P", "K", "M", "T", "O", "H", "X", "B", "C", "I", "J"},
		new string[26] {"J", "E", "X", "U", "S", "L", "C", "Q", "Y", "N", "O", "H", "K", "Z", "A", "D", "F", "W", "P", "R", "B", "I", "T", "M", "V", "G"},
		new string[26] {"I", "G", "Y", "X", "K", "Z", "M", "E", "U", "L", "P", "N", "A", "Q", "O", "V", "B", "J", "T", "W", "D", "S", "H", "F", "C", "R"},
		new string[26] {"R", "Z", "L", "A", "J", "B", "F", "V", "X", "T", "K", "Y", "N", "O", "D", "G", "W", "E", "H", "C", "Q", "M", "P", "U", "I", "S"},
		new string[26] {"H", "T", "G", "M", "S", "N", "P", "X", "C", "V", "Y", "R", "W", "Q", "Z", "J", "F", "U", "O", "D", "E", "I", "A", "K", "B", "L"},
		new string[26] {"Z", "F", "X", "B", "J", "K", "R", "N", "O", "Q", "C", "P", "U", "T", "V", "E", "H", "S", "M", "I", "A", "D", "L", "Y", "W", "G"},
		new string[26] {"X", "S", "C", "O", "V", "H", "Z", "Q", "P", "Y", "F", "A", "B", "I", "E", "T", "G", "K", "D", "J", "U", "R", "M", "L", "W", "N"},
		new string[26] {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"}
		};

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool ModuleSolved;

    void Awake()
    {
        moduleId = moduleIdCounter++;
        LeftQuad.OnInteract += delegate () { PressLeftQuad(); return false; };
        RightQuad.OnInteract += delegate () { PressRightQuad(); return false; };
    }

    void Start()
    {
        Module.OnActivate += ActivateModule;
		Module.OnActivate += Randomizer;
		Playable = true;
    }

    void ActivateModule()
    {
		TheFiber.text = "";
        List<int> NumericalValue = new List<int>();
        for (int i = 0; i < 1; i++) 
		{
			if (Bomb.GetIndicators().Count() == 2)
			{
				if (i < 1)
				{
					NumericalValue.Add(0);
					Debug.LogFormat("[Alphabetize #{0}] Table 1 is true.", moduleId);
				}
			}

			if (Bomb.GetPortCount() == 3)
			{
				if (i < 1)
				{
					NumericalValue.Add(1);
					Debug.LogFormat("[Alphabetize #{0}] Table 2 is true.", moduleId);
				}
			}

			if (Bomb.IsIndicatorPresent("CAR"))
			{
				if (i < 1)
				{
					NumericalValue.Add(2);
					Debug.LogFormat("[Alphabetize #{0}] Table 3 is true.", moduleId);
				}
			}

			if (Bomb.GetStrikes() % 2 == 0)
			{
				if (i < 1)
				{
					NumericalValue.Add(3);
					Debug.LogFormat("[Alphabetize #{0}] Table 4 is true.", moduleId);
				}
			}

			if (Bomb.GetBatteryCount() % 2 == 0)
			{
				if (i < 1)
				{
					NumericalValue.Add(4);
					Debug.LogFormat("[Alphabetize #{0}] Table 5 is true.", moduleId);
				}
			}

			if (Bomb.IsIndicatorPresent("MSA"))
			{
				if (i < 1)
				{
					NumericalValue.Add(5);
					Debug.LogFormat("[Alphabetize #{0}] Table 6 is true.", moduleId);
				}
			}

			if (Bomb.GetSerialNumberLetters().First() == 'B' || Bomb.GetSerialNumberLetters().First() == 'C' || Bomb.GetSerialNumberLetters().First() == 'D' || Bomb.GetSerialNumberLetters().First() == 'F' || Bomb.GetSerialNumberLetters().First() == 'G' || Bomb.GetSerialNumberLetters().First() == 'H' || Bomb.GetSerialNumberLetters().First() == 'J' || Bomb.GetSerialNumberLetters().First() == 'K' || Bomb.GetSerialNumberLetters().First() == 'L' || Bomb.GetSerialNumberLetters().First() == 'M' || Bomb.GetSerialNumberLetters().First() == 'N' || Bomb.GetSerialNumberLetters().First() == 'P' || Bomb.GetSerialNumberLetters().First() == 'Q' || Bomb.GetSerialNumberLetters().First() == 'R' || Bomb.GetSerialNumberLetters().First() == 'S' || Bomb.GetSerialNumberLetters().First() == 'T' || Bomb.GetSerialNumberLetters().First() == 'V' || Bomb.GetSerialNumberLetters().First() == 'W' || Bomb.GetSerialNumberLetters().First() == 'X' || Bomb.GetSerialNumberLetters().First() == 'Y' || Bomb.GetSerialNumberLetters().First() == 'Z')
			{
				if (i < 1)
				{
					NumericalValue.Add(6);
					Debug.LogFormat("[Alphabetize #{0}] Table 7 is true.", moduleId);
				}
			}

			if (Bomb.GetPortPlates().Count() == 2)
			{
				if (i < 1)
				{
					NumericalValue.Add(7);
					Debug.LogFormat("[Alphabetize #{0}] Table 8 is true.", moduleId);
				}
			}

			if (Bomb.GetBatteryCount() % 2 == 1)
			{
				if (i < 1)
				{
					NumericalValue.Add(8);
					Debug.LogFormat("[Alphabetize #{0}] Table 9 is true.", moduleId);
				}
			}

			if (Bomb.GetSerialNumberNumbers().Count() >= 3)
			{
				if (i < 1)
				{
					NumericalValue.Add(9);
					Debug.LogFormat("[Alphabetize #{0}] Table 10 is true.", moduleId);
				}
			}

			if (Bomb.GetIndicators().Count() > 2)
			{
				if (i < 1)
				{
					NumericalValue.Add(10);
					Debug.LogFormat("[Alphabetize #{0}] Table 11 is true.", moduleId);
				}
			}

			if (Bomb.GetSolvedModuleNames().Count() > 3)
			{
				if (i < 1)
				{
					NumericalValue.Add(11);
					Debug.LogFormat("[Alphabetize #{0}] Table 12 is true.", moduleId);
				}
			}

			if ((Bomb.GetSerialNumber().Contains("A") || Bomb.GetSerialNumber().Contains("E") || Bomb.GetSerialNumber().Contains("I") || Bomb.GetSerialNumber().Contains("O") || Bomb.GetSerialNumber().Contains("U")) != true)
			{
				if (i < 1)
				{
					NumericalValue.Add(12);
					Debug.LogFormat("[Alphabetize #{0}] Table 13 is true.", moduleId);
				}
			}

			if (Bomb.GetModuleNames().Count() > 30)
			{
				if (i < 1)
				{
					NumericalValue.Add(13);
					Debug.LogFormat("[Alphabetize #{0}] Table 14 is true.", moduleId);
				}
			}

			if (Bomb.GetIndicators().Count() < 4)
			{
				if (i < 1)
				{
					NumericalValue.Add(14);
					Debug.LogFormat("[Alphabetize #{0}] Table 15 is true.", moduleId);
				}
			}
		}
		
		SilverLine = NumericalValue.Distinct().ToArray();
		
		if ((SilverLine.Count() % 2) == 0)
		{
			TheCopperValue = 15;
			Debug.LogFormat("[Alphabetize #{0}] -----------------------------", moduleId);
			Debug.LogFormat("[Alphabetize #{0}] The string used is the English Alphabet.", moduleId);
			Debug.LogFormat("[Alphabetize #{0}] -----------------------------", moduleId);
		}
		
		else if ((SilverLine.Count() % 2) == 1)
		{
			if (SilverLine.Count() == 1)
			{
				TheCopperValue = 0;
			}
			
			else if (SilverLine.Count() == 3)
			{
				TheCopperValue = 1;
			}
			
			else if (SilverLine.Count() == 5)
			{
				TheCopperValue = 2;
			}
			
			else if (SilverLine.Count() == 7)
			{
				TheCopperValue = 3;
			}
			
			else if (SilverLine.Count() == 9)
			{
				TheCopperValue = 4;
			}
			
			else if (SilverLine.Count() == 11)
			{
				TheCopperValue = 5;
			}
			
			else if (SilverLine.Count() == 13)
			{
				TheCopperValue = 6;
			}
			
			else if (SilverLine.Count() == 15)
			{
				TheCopperValue = 7;
			}
			
			Debug.LogFormat("[Alphabetize #{0}] -----------------------------", moduleId);
			Debug.LogFormat("[Alphabetize #{0}] The string used is in Table " + (SilverLine[TheCopperValue] + 1).ToString(), moduleId);
			Debug.LogFormat("[Alphabetize #{0}] -----------------------------", moduleId);
		}
	}
	
	void Randomizer()
	{
		Audio.PlaySoundAtTransform(SFX[1].name, transform);
		if (TheFoil == 26)
		{
			StartCoroutine(TheSolved());
		}
		
		else if (TheFoil != 26)
		{
			int Choco = 0;
			Choco = UnityEngine.Random.Range(0,3);
			
			if (Choco != 0)
			{
				Supper = UnityEngine.Random.Range(0, Alphabreak.Count());
				TheLetter.text = Alphabreak[Supper];
			}
			
			else if (Choco == 0)
			{
				if (TheCopperValue != 15)
				{
					TheLetter.text = TheSequence[SilverLine[TheCopperValue]][TheFoil];
				}
				
				else if (TheCopperValue == 15)
				{
					TheLetter.text = TheSequence[TheCopperValue][TheFoil];
				}
			}
			
			if ((TheCopperValue != 15 && TheLetter.text == TheSequence[SilverLine[TheCopperValue]][TheFoil]) || (TheCopperValue == 15 && TheLetter.text == TheSequence[TheCopperValue][TheFoil]))
			{
				Debug.LogFormat("[Alphabetize #{0}] Letter " + TheLetter.text.ToString() + " matches the letter in the current stage.", moduleId);
			}
			
			else
			{
				Debug.LogFormat("[Alphabetize #{0}] Letter " + TheLetter.text.ToString() + " does not match the letter in the current stage.", moduleId);
			}
		}
	}

	void PressLeftQuad()
	{
		if (Computer == 0 && Playable == true)
		{
			LeftQuad.AddInteractionPunch(0.2f);
			if (TheCopperValue != 15)
			{
				if (TheSequence[SilverLine[TheCopperValue]][TheFoil] != TheLetter.text)
				{
					Debug.LogFormat("[Alphabetize #{0}] You pressed left. Correct.", moduleId);
					Debug.LogFormat("[Alphabetize #{0}] -----------------------------", moduleId);
					TheFoil = TheFoil + 1;
					Randomizer();
				}
				
				else
				{
					StartCoroutine(Again());
					Debug.LogFormat("[Alphabetize #{0}] You pressed left. Incorrect.", moduleId);
					Debug.LogFormat("[Alphabetize #{0}] -----------------------------", moduleId);
				}
			}
			
			else if (TheCopperValue == 15)
			{
				if (TheSequence[TheCopperValue][TheFoil] != TheLetter.text)
				{
					Debug.LogFormat("[Alphabetize #{0}] You pressed left. Correct.", moduleId);
					Debug.LogFormat("[Alphabetize #{0}] -----------------------------", moduleId);
					TheFoil = TheFoil + 1;
					Randomizer();
				}
				
				else
				{
					StartCoroutine(Again());
					Debug.LogFormat("[Alphabetize #{0}] You pressed left. Incorrect.", moduleId);
					Debug.LogFormat("[Alphabetize #{0}] -----------------------------", moduleId);
				}
			}
		}
	}

	void PressRightQuad()
	{
		if (Computer == 0 && Playable == true)
		{
			RightQuad.AddInteractionPunch(0.2f);
			if (TheCopperValue != 15)
			{
				if (TheSequence[SilverLine[TheCopperValue]][TheFoil] == TheLetter.text)
				{
					Debug.LogFormat("[Alphabetize #{0}] You pressed right. Correct.", moduleId);
					Debug.LogFormat("[Alphabetize #{0}] -----------------------------", moduleId);
					TheFoil = TheFoil + 1;
					Randomizer();
				}
				
				else
				{
					StartCoroutine(Again());
					Debug.LogFormat("[Alphabetize #{0}] You pressed right. Incorrect.", moduleId);
					Debug.LogFormat("[Alphabetize #{0}] -----------------------------", moduleId);
				}
			}
			
			else if (TheCopperValue == 15)
			{
				if (TheSequence[TheCopperValue][TheFoil] == TheLetter.text)
				{
					Debug.LogFormat("[Alphabetize #{0}] You pressed right. Correct.", moduleId);
					Debug.LogFormat("[Alphabetize #{0}] -----------------------------", moduleId);
					TheFoil = TheFoil + 1;
					Randomizer();
				}
				
				else
				{
					StartCoroutine(Again());
					Debug.LogFormat("[Alphabetize #{0}] You pressed right. Incorrect.", moduleId);
					Debug.LogFormat("[Alphabetize #{0}] -----------------------------", moduleId);
				}
			}
		}
	}
		
	IEnumerator TheSolved()
	{
		StrikingBool = true;
		Computer = 1;
		TheLetter.text = "";
		TheTetra.text = "D";
		Audio.PlaySoundAtTransform(SFX[1].name, transform);
		yield return new WaitForSeconds(0.2f);
		TheTetra.text = "DO";
		Audio.PlaySoundAtTransform(SFX[1].name, transform);
		yield return new WaitForSeconds(0.2f);
		TheTetra.text = "DON";
		Audio.PlaySoundAtTransform(SFX[1].name, transform);
		yield return new WaitForSeconds(0.2f);
		TheTetra.text = "DONE";
		Audio.PlaySoundAtTransform(SFX[1].name, transform);
		yield return new WaitForSeconds(0.2f);
		TheTetra.text = "DONE!";
		Audio.PlaySoundAtTransform(SFX[1].name, transform);
		yield return new WaitForSeconds(0.5f);
		Module.HandlePass();
		Audio.PlaySoundAtTransform(SFX[0].name, transform);
		Debug.LogFormat("[Alphabetize #{0}] Module is done.", moduleId);
	}
		
	IEnumerator Again()
	{
		StrikingBool = true;
		Computer = 1;
		TheLetter.text = "";
		TheTetra.text = "N";
		Audio.PlaySoundAtTransform(SFX[1].name, transform);
		yield return new WaitForSeconds(0.2f);
		TheTetra.text = "NO";
		Audio.PlaySoundAtTransform(SFX[1].name, transform);
		yield return new WaitForSeconds(0.2f);
		TheTetra.text = "NOP";
		Audio.PlaySoundAtTransform(SFX[1].name, transform);
		yield return new WaitForSeconds(0.2f);
		TheTetra.text = "NOPE";
		Audio.PlaySoundAtTransform(SFX[1].name, transform);
		yield return new WaitForSeconds(0.2f);
		TheTetra.text = "NOPE!";
		Audio.PlaySoundAtTransform(SFX[1].name, transform);
		yield return new WaitForSeconds(1f);
		TheTetra.text = "";
		Module.HandleStrike();
		Reset();
		ActivateModule();
		Randomizer();
		Computer = 0;
		StrikingBool = false;
	}
		
	void Reset()
	{
		TheCopperValue = 0;
		TheFoil = 0;
		Supper = 0;
	}
	
	IEnumerator UpdateVariables()
	{
		StrikingBool = true;
		TheLetter.text = "U";
		yield return new WaitForSeconds(0.1f);
		TheLetter.text = "P";
		yield return new WaitForSeconds(0.1f);
		TheLetter.text = "D";
		yield return new WaitForSeconds(0.1f);
		TheLetter.text = "A";
		yield return new WaitForSeconds(0.1f);
		TheLetter.text = "T";
		yield return new WaitForSeconds(0.1f);
		TheLetter.text = "E";
		yield return new WaitForSeconds(0.1f);
		TheLetter.text = "D";
		yield return new WaitForSeconds(0.1f);
		TheLetter.text = "!";
		yield return new WaitForSeconds(0.5f);
		Reset();
		Debug.LogFormat("[Alphabetize #{0}] The module is updated due to a command", moduleId);
		Debug.LogFormat("[Alphabetize #{0}] -----------------------------", moduleId);
		ActivateModule();
		Randomizer();
		StrikingBool = false;
	}
	
	//twitch plays
    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"Use the command !{0} left/right to press the corresponding side on the module. You can use the command !{0} update to update the rule list being applied in the module (This however resets all inputs in the module)";
    #pragma warning restore 414
	
	bool StrikingBool = false;
	
	IEnumerator ProcessTwitchCommand(string command)
	{
		if (Regex.IsMatch(command, @"^\s*right\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant) || Regex.IsMatch(command, @"^\s*r\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
		{
			yield return null;
			if (StrikingBool == true)
			{
				yield return "sendtochaterror The module is not showing a letter. The command was not processed.";
				yield break;
			}
			yield return "solve";
			yield return "strike";
			RightQuad.OnInteract();
		}
		
		if (Regex.IsMatch(command, @"^\s*left\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant) || Regex.IsMatch(command, @"^\s*l\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
		{
			yield return null;
			if (StrikingBool == true)
			{
				yield return "sendtochaterror The module is not showing a letter. The command was not processed.";
				yield break;
			}
			yield return "solve";
			yield return "strike";
			LeftQuad.OnInteract();
		}
		
		if (Regex.IsMatch(command, @"^\s*update\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
		{
			yield return null;
			if (StrikingBool == true)
			{
				yield return "sendtochaterror The module is not showing a letter. The command was not processed.";
				yield break;
			}
			StartCoroutine(UpdateVariables());
			yield return "sendtochat The rules are now updated.";
		}

	}
}
