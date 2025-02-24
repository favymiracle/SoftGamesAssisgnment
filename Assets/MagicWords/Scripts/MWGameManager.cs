using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.TextCore;
using System;
using Newtonsoft.Json;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MWGameManager : MonoBehaviour
{
    [SerializeField] private ChatItem m_item;
    [SerializeField] private Transform m_content;
    [SerializeField] private string m_dataUrl;
    [SerializeField] private Button m_back;

    private DialogueData data;
    private TMP_SpriteAsset spriteAssets;
    private List<Sprite> avatars = new List<Sprite>();

    string jsonString = @"
    {
        ""dialogue"": [
            { ""name"": ""Sheldon"", ""text"": ""I admit {satisfied} the design of Cookie Crush is quite elegant in its simplicity."" },
            { ""name"": ""Leonard"", ""text"": ""That’s practically a compliment, Sheldon. {intrigued} Are you feeling okay?"" },
            { ""name"": ""Penny"", ""text"": ""Don’t worry, Leonard. He’s probably just trying to justify playing it himself."" },
            { ""name"": ""Sheldon"", ""text"": ""Incorrect. {neutral} I’m studying its mechanics. The progression system is oddly satisfying."" },
            { ""name"": ""Penny"", ""text"": ""It’s called fun, Sheldon. You should try it more often."" },
            { ""name"": ""Leonard"", ""text"": ""She’s got a point. Sometimes, a simple game can be relaxing."" },
            { ""name"": ""Neighbour"", ""text"": ""I fully agree {affirmative}"" },
            { ""name"": ""Sheldon"", ""text"": ""Relaxing? I suppose there’s merit in low-stakes gameplay to reduce cortisol levels."" },
            { ""name"": ""Penny"", ""text"": ""Translation: Sheldon likes crushing cookies but won’t admit it. {laughing}"" },
            { ""name"": ""Sheldon"", ""text"": ""Fine. I find the color-matching oddly soothing. Happy?"" },
            { ""name"": ""Leonard"", ""text"": ""Very. Now we can finally play as a team in Wordscapes."" },
            { ""name"": ""Penny"", ""text"": ""Wait, Sheldon’s doing team games now? What’s next, co-op decorating?"" },
            { ""name"": ""Sheldon"", ""text"": ""Unlikely. But if the design involves symmetry and efficiency, I may consider it."" },
            { ""name"": ""Penny"", ""text"": ""See? Casual gaming brings people together!"" },
            { ""name"": ""Leonard"", ""text"": ""Even Sheldon. That’s a win for everyone. {win}"" },
            { ""name"": ""Sheldon"", ""text"": ""Agreed. {neutral} Though I still maintain chess simulators are superior."" },
            { ""name"": ""Penny"", ""text"": ""Sure, Sheldon. {intrigued} You can play chess *after* we beat this next level."" }
        ],
        ""emojies"": [
            { ""name"": ""sad"", ""url"": ""https://api.dicebear.com:82/9.x/fun-emoji/png?seed=Sad"" },
            { ""name"": ""intrigued"", ""url"": ""https://api.dicebear.com/9.x/fun-emoji/png?seed=Sawyer"" },
            { ""name"": ""neutral"", ""url"": ""https://api.dicebear.com/9.x/fun-emoji/png?seed=Destiny"" },
            { ""name"": ""satisfied"", ""url"": ""https://api.dicebear.com/9.x/fun-emoji/png?seed=Jocelyn"" },
            { ""name"": ""laughing"", ""url"": ""https://api.dicebear.com/9.x/fun-emoji/png?seed=Sophia"" }
        ],
        ""avatars"": [
            { ""name"": ""Sheldon"", ""url"": ""https://api.dicebear.com/9.x/personas/png?body=squared&clothingColor=6dbb58&eyes=open&hair=buzzcut&hairColor=6c4545&mouth=smirk&nose=smallRound&skinColor=e5a07e"", ""position"": ""left"" },
            { ""name"": ""Penny"", ""url"": ""https://api.dicebear.com/9.x/personas/png?body=squared&clothingColor=f55d81&eyes=happy&hair=extraLong&hairColor=f29c65&mouth=smile&nose=smallRound&skinColor=e5a07e"", ""position"": ""right"" },
            { ""name"": ""Leonard"", ""url"": ""https://api.dicebear.com/9.x/personas/png?body=checkered&clothingColor=f3b63a&eyes=glasses&hair=shortCombover&hairColor=362c47&mouth=surprise&nose=mediumRound&skinColor=d78774"", ""position"": ""right"" }
        ]
    }";

    void Start()
    {
        StartCoroutine(GetContentData());

        m_back.onClick.AddListener(() => { SceneManager.LoadScene(0); });

        //data = JsonConvert.DeserializeObject<DialogueData>(jsonString);

        //FixIncorrectEmojiURLs(data.emojies);

        //string correctedJson = JsonConvert.SerializeObject(data, Formatting.Indented);

        //data = JsonConvert.DeserializeObject<DialogueData>(correctedJson);

        //StartCoroutine(CreateSpriteAsset());

        //StartCoroutine(DownloadAvatars());
    }

    private IEnumerator GetContentData() 
    {
        using (UnityWebRequest www = UnityWebRequest.Get(m_dataUrl))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                jsonString = www.downloadHandler.text;

                data = JsonConvert.DeserializeObject<DialogueData>(jsonString);

                FixIncorrectEmojiURLs(data.emojies);

                string correctedJson = JsonConvert.SerializeObject(data, Formatting.Indented);
                Debug.Log("Corrected JSON:\n" + correctedJson);

                StartCoroutine(CreateSpriteAsset());

                StartCoroutine(DownloadAvatars());
            }
            else
            {
                Debug.LogError($"Failed to download texture: {www.error}");
            }
        }
    }

    private IEnumerator CreateSpriteAsset()
    {
        // Create a new TMP Sprite Asset
        TMP_SpriteAsset spriteAsset = ScriptableObject.CreateInstance<TMP_SpriteAsset>();

        // Create lists for sprite info and sprite glyph
        List<TMP_SpriteGlyph> spriteGlyphList = new List<TMP_SpriteGlyph>();
        List<TMP_SpriteCharacter> spriteCharacterList = new List<TMP_SpriteCharacter>();

        // Download all textures
        Texture2D[] textures = new Texture2D[data.emojies.Count];
        for (int i = 0; i < data.emojies.Count; i++)
        {
            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(data.emojies[i].url))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    textures[i] = DownloadHandlerTexture.GetContent(www);
                }
                else
                {
                    Debug.LogError($"Failed to download texture: {www.error}");
                    continue;
                }
            }
        }

        // Calculate the total width and maximum height
        int totalWidth = 0;
        int maxHeight = 0;
        foreach (Texture2D tex in textures)
        {
            totalWidth += tex.width;
            maxHeight = Mathf.Max(maxHeight, tex.height);
        }

        // Create the final texture atlas
        Texture2D atlas = new Texture2D(totalWidth, maxHeight);
        atlas.filterMode = FilterMode.Bilinear;

        // Keep track of the current x position
        int currentX = 0;

        // Add each texture to the atlas
        for (int i = 0; i < textures.Length; i++)
        {
            Texture2D tex = textures[i];

            // Copy the pixels to the atlas
            atlas.SetPixels(currentX, 0, tex.width, tex.height, tex.GetPixels());

            // Create a SpriteGlyph
            TMP_SpriteGlyph spriteGlyph = new TMP_SpriteGlyph
            {
                index = (uint)i,
                metrics = new GlyphMetrics(tex.width, tex.height, 0, tex.height, tex.width), // Correct metrics
                glyphRect = new GlyphRect(currentX, 0, tex.width, tex.height), // Correct rect
                scale = 1.0f,
                sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero)
            };
            spriteGlyphList.Add(spriteGlyph);

            currentX += tex.width;
        }

        for (int i = 0; i < textures.Length; i++) 
        {
            TMP_SpriteCharacter spriteCharacter = new TMP_SpriteCharacter
            {
                unicode = 0xFFFE,
                glyphIndex = (uint)i,
                glyph = spriteGlyphList[i],
                name = data.emojies[i].name,
                scale = 1.0f
            };

            spriteCharacterList.Add(spriteCharacter);
        }

        atlas.Apply();

        spriteAsset.spriteSheet = atlas;
        for (int i = 0; i < spriteGlyphList.Count; i++)
        {
            spriteAsset.spriteGlyphTable.Add(spriteGlyphList[i]);
            spriteAsset.spriteCharacterTable.Add(spriteCharacterList[i]);
        }

        Material material = new Material(Shader.Find("TextMeshPro/Sprite"));
        material.mainTexture = atlas;
        material.name = "Emoji Material";
        spriteAsset.material = material;
        spriteAsset.name = "Custom Emoji";

        ChatItem[] chatItems = FindObjectsByType<ChatItem>(sortMode: FindObjectsSortMode.None);

        foreach (ChatItem Item in chatItems) 
        {
            Item.InitializeSpriteAssets(spriteAsset);
        }
    }

    private IEnumerator DownloadAvatars() 
    {
        for (int i = 0; i < data.avatars.Count; i++)
        {
            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(data.avatars[i].url))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    Texture2D texture = DownloadHandlerTexture.GetContent(www);
                    Sprite avatar = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                    avatars.Add(avatar);
                }
                else
                {
                    Debug.LogError($"Failed to download texture: {www.error}");
                    continue;
                }
            }
        }

        InstantiateItems();
    }

    void InstantiateItems() 
    {
        for (int i = 0; i < data.dialogue.Count; i++)
        {
            ChatItem item = Instantiate(m_item.gameObject, m_content).GetComponent<ChatItem>();

            bool isLeft = false;
            string name = data.dialogue[i].name;
            string chat = data.dialogue[i].text;
            string avatarUrl = "";

            foreach (Avatar avatar in data.avatars)
            {
                if (avatar.name == name)
                {
                    isLeft = avatar.position == "left" ? true : false;
                    avatarUrl = avatar.url;
                }
            }

            string emoji = ExtractStringBetweenBraces(chat);
            int emojiIndex = GetEmojiIndexFromString(emoji);

            chat = emojiIndex == -1 ? chat.Replace("{" + emoji + "}", "") : chat.Replace("{" + emoji + "}", "<sprite index=" + emojiIndex + ">");

            int avatarIndex = GetAvatarIndexFromName(name);

            item.Initialize(isLeft, chat, avatarIndex == -1 ? null : avatars[avatarIndex]);
        }
    }

    string ExtractStringBetweenBraces(string input)
    {
        // Find the index of the opening brace '{'
        int startIndex = input.IndexOf('{');
        // Find the index of the closing brace '}'
        int endIndex = input.IndexOf('}');

        // Check if both braces are found
        if (startIndex != -1 && endIndex != -1 && startIndex < endIndex)
        {
            // Extract the substring between the braces
            return input.Substring(startIndex + 1, endIndex - startIndex - 1);
        }

        // Return null if no valid string is found
        return null;
    }

    int GetEmojiIndexFromString(string emoji)
    {
        for (int i = 0; i < data.emojies.Count; i++) 
        {
            if (data.emojies[i].name == emoji) 
            {
                return i;
            }
        }

        return -1;
    }

    int GetAvatarIndexFromName(string name) 
    {
        for (int i = 0; i < data.avatars.Count; i++) 
        {
            if (data.avatars[i].name == name) 
            {
                return i;
            }
        }

        return -1;
    }

    void FixIncorrectEmojiURLs(List<Emoji> emojis)
    {
        foreach (var emoji in emojis)
        {
            // Use UriBuilder to handle dynamic port numbers
            if (Uri.TryCreate(emoji.url, UriKind.Absolute, out Uri uri))
            {
                // Reconstruct the URL without the port number
                var builder = new UriBuilder(uri)
                {
                    Port = -1 // Remove the port number
                };
                emoji.url = builder.Uri.ToString();
                Debug.Log($"Fixed URL for '{emoji.name}': {emoji.url}");
            }
            else
            {
                Debug.LogWarning($"Invalid URL for '{emoji.name}': {emoji.url}");
            }
        }
    }
}

[Serializable]
public class Dialogue
{
    public string name;
    public string text;
}

[Serializable]
public class Emoji
{
    public string name;
    public string url;
}

[Serializable]
public class Avatar
{
    public string name;
    public string url;
    public string position;
}

[Serializable]
public class DialogueData
{
    public List<Dialogue> dialogue;
    public List<Emoji> emojies;
    public List<Avatar> avatars;
}

public class Data
{
    public List<Emoji> emojis;
}
