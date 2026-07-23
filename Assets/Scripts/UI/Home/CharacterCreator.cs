using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class CharacterCreator : MonoBehaviour
{
    public TextMeshProUGUI old_tmp;
    public TextMeshProUGUI lifeExpectancy_tmp;
    public TextMeshProUGUI linhCan_tmp;
    public TextMeshProUGUI thienPhu_tmp;
    public TMP_InputField nameInputField;
    GenerationStats newCharacter;

    string[] THIEN_PHU = { "Bùa", "Trồng trọt", "Luyện đan" };
    //string[] LINH_CAN =  { "Kim", "Mộc", "Thủy", "Hỏa", "Thổ" };
    List<string> LINH_CAN = new List<string>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadData();
        ressetCharacter();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LoadData()
    {
        TextAsset csv = Resources.Load<TextAsset>("Store/Database/linh_can");
        string[] rows = csv.text.Split('\n');

        foreach(string row in rows)
        {
            string[] cols = row.Split(',');
            string linhCan = cols[2]
                                .Replace("\n", "")
                                .Replace("\r", "")
                                .Trim();
            LINH_CAN.Add(linhCan);

        }
    }

    void ressetCharacter()
    {
        int tuoi = Random.Range(10, 15);
        int lifeExpectancy = tuoi + Random.Range(60, 81);
        int linhCanCount = Random.Range(1, 4);
        List<string> linhCan = LINH_CAN.OrderBy(_ => Random.value).Take(linhCanCount).ToList();
        string thienPhu = THIEN_PHU[Random.Range(0, THIEN_PHU.Length)];
        string characterName = nameInputField != null ? nameInputField.text : string.Empty;

        newCharacter = new GenerationStats();
        newCharacter.name = characterName;
        newCharacter.age = tuoi;
        newCharacter.lifeExpectancy = lifeExpectancy;
        newCharacter.linhCan = linhCan;
        newCharacter.thienPhu = thienPhu;

        old_tmp.text = tuoi.ToString();
        lifeExpectancy_tmp.text = lifeExpectancy.ToString();
        linhCan_tmp.text = string.Join(", ", linhCan);
        thienPhu_tmp.text = thienPhu;

        Debug.Log(string.Join(", ", linhCan));
        // Reset character creation fields
    }


    public void OnResetClick()
    {
       ressetCharacter();
    }

    public void OnStartGame(){
        CharactorStats.Current = new CharactorStats {
            baseStats = new BaseStats(),
            derivedStats = new DerivedStats(),
            generationStats = newCharacter
        };
        GameManager.Instance.setCharactorStats(CharactorStats.Current);
        GameManager.Instance.newGame();
        UnityEngine.SceneManagement.SceneManager.LoadScene("LoadStartGame");
    }


}
