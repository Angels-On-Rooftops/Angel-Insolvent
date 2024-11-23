using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Emit;
using TMPro;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [SerializeField]
    public SettingsCategory[] settingsCategories;

    [SerializeField]
    AudioSource audioSource;
    AudioMixer audioMixer;

    [SerializeField]
    UnityEvent doneBehavior;

    Button categoryButtonPrefab;
    GameObject settingsPanelPrefab;
    GameObject singleSettingPrefab;

    GameObject categoriesPanel;
    GameObject settingsPanel;

    Dictionary<Button, GameObject> categoryButtonsDictionary = new Dictionary<Button, GameObject>();

    GameObject activeCategory;

    void Awake()
    {
        categoryButtonPrefab = Resources.Load<Button>("Prefabs/CoreSystems/CoreUI/Settings/SettingCategoryButton");
        settingsPanelPrefab = Resources.Load<GameObject>("Prefabs/CoreSystems/CoreUI/Settings/VerticalPanelPrefab");
        singleSettingPrefab = Resources.Load<GameObject>("Prefabs/CoreSystems/CoreUI/Settings/SingleSettingPrefab");

        categoriesPanel = this.gameObject.transform
            .GetChild(1)
            .gameObject.transform.GetChild(0)
            .gameObject;
        settingsPanel = this.gameObject.transform
            .GetChild(1)
            .gameObject.transform.GetChild(1)
            .gameObject;

        for (int j = 0; j < settingsCategories.Length; j++) //For each category
        {
            //Setup category button
            Button categoryButton = Instantiate(
                categoryButtonPrefab,
                categoriesPanel.gameObject.transform
            );
            categoryButton.onClick.AddListener(
                delegate
                {
                    SwitchSettingsCategory(categoryButton);
                }
            );

            //Setup button text
            string categoryName = settingsCategories[j].name;
            var categoryLabel = categoryButton.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            categoryLabel.text = categoryName;

            var settingsObjects = settingsCategories[j].settings;

            GameObject newSettingsPanel = Instantiate(
                settingsPanelPrefab,
                settingsPanel.gameObject.transform
            );
            for (int i = 0; i < settingsObjects.Length; i++) //for each setting in category
            {
                GameObject newSetting = Instantiate(
                    singleSettingPrefab,
                    newSettingsPanel.gameObject.transform
                );
                settingsObjects[i].CreateUIElement(newSetting, i);
            }
            newSettingsPanel.SetActive(false);

            categoryButtonsDictionary.Add(categoryButton, newSettingsPanel);
        }

        //Setup done button
        Button doneButton = Instantiate(categoryButtonPrefab, categoriesPanel.gameObject.transform);
        doneButton.onClick.AddListener(
            delegate
            {
                doneBehavior.Invoke();
                SaveSettings();
            }
        );
        var doneLabel = doneButton.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        doneLabel.text = "Done";

        activeCategory = categoryButtonsDictionary.Values.FirstOrDefault();
        categoryButtonsDictionary.Keys.First().onClick.Invoke();
        categoryButtonsDictionary.Keys.First().Select();
    }

    private void SwitchSettingsCategory(Button categoryButton)
    {
        activeCategory.SetActive(false);
        categoryButtonsDictionary[categoryButton].SetActive(true);
        activeCategory = categoryButtonsDictionary[categoryButton];
    }

    private void SaveSettings()
    {
        for(int i = 0; i < settingsCategories.Length; i++) //For each category
        {
            for(int j = 0; j < settingsCategories[i].settings.Length; j++) //For each setting in category
            {
                settingsCategories[i].settings[j].SaveSetting();
            }
        }
    }

    public void LoadSettings()
    {
        for (int i = 0; i < settingsCategories.Length; i++) //For each category
        {
            for (int j = 0; j < settingsCategories[i].settings.Length; j++) //For each setting in category
            {
                settingsCategories[i].settings[j].LoadSetting();
            }
        }
    }

    [System.Serializable]
    public class SettingsObject
    {
        public enum UIElementType
        {
            Dropdown,
            Slider,
            Toggle,
            InputBind
        };

        [System.Serializable]
        public struct UIElementConfig
        {
            public UIElementType type;
            public string label;

            public UnityEvent<MonoBehaviour> customSetup;
        }

        public UIElementConfig config;
        public GameObject prefab;

        [NonSerialized]
        public GameObject settingUiElement;

        public void CreateUIElement(GameObject parent, int listIndex)
        {
            //Create setting label
            GameObject settingLabel = new GameObject("Label");
            settingLabel.transform.SetParent(parent.gameObject.transform, false);
            settingLabel.AddComponent<RectTransform>();
            var labelText = settingLabel.AddComponent<TextMeshProUGUI>();
            labelText.text = config.label;

            //Create setting UI element
            GameObject uiElement = null;
            uiElement = Instantiate(prefab, parent.gameObject.transform);

            switch (config.type)
            {
                case UIElementType.Dropdown:
                    SetupDropdown(uiElement.GetComponent<TMPro.TMP_Dropdown>());
                    break;
                case UIElementType.Slider:
                    SetupSlider(uiElement.GetComponent<Slider>());
                    break;
                case UIElementType.Toggle:
                    SetupToggle(uiElement.GetComponent<Toggle>());
                    break;
                case UIElementType.InputBind:
                    SetupInputBindButton(uiElement.GetComponent<Button>());
                    break;
            }

            if (uiElement != null)
            {
                RectTransform newRectTransform = uiElement.GetComponent<RectTransform>();
                Vector2 newPosition = newRectTransform.anchoredPosition;
                newPosition.y -= 50 * listIndex;
                newRectTransform.anchoredPosition = newPosition;

                settingUiElement = uiElement;
            }
        }

        private void SetupDropdown(TMPro.TMP_Dropdown dropdown)
        {
            if (dropdown.GetType() == typeof(TMPro.TMP_Dropdown))
            {
                dropdown.options.Clear();
                config.customSetup?.Invoke(dropdown);
            }
            else
            {
                Debug.Log("Dropdown prefab is not of type TMPro.TMP_Dropdown on " + config.label);
            }
        }

        private void SetupSlider(Slider slider)
        {
            if (slider.GetType() == typeof(Slider))
            {
                slider.minValue = 0;
                slider.maxValue = 1;
                slider.value = 0.5f;
                config.customSetup?.Invoke(slider);
            }
            else
            {
                Debug.Log("Slider prefab is not of type Slider on " + config.label);
            }
        }

        private void SetupToggle(Toggle toggle)
        {
            if (toggle.GetType() == typeof(Toggle))
            {
                toggle.isOn = true;
                config.customSetup?.Invoke(toggle);
            }
            else
            {
                Debug.Log("Toggle prefab is not of type Toggle on " + config.label);
            }
        }

        private void SetupInputBindButton(Button bindingButton)
        {
            if (bindingButton.GetType() == typeof(Button))
            {
                var bindingLabel = bindingButton.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                bindingLabel.text = InputBindsHandler.Instance.FindBind("Walk").bindings[0].ToString();
                Debug.Log(bindingLabel.text);
                config.customSetup?.Invoke(bindingButton);
            }
            else
            {
                Debug.Log("InputBindingButton prefab is not of type Button on " + config.label);
            }
        }

        public void SaveSetting()
        {
            if(settingUiElement != null)
            {
                switch (config.type)
                {
                    case UIElementType.Dropdown:
                        PlayerPrefs.SetInt(config.label, settingUiElement.GetComponent<TMP_Dropdown>().value);
                        break;
                    case UIElementType.Slider:
                        PlayerPrefs.SetFloat(config.label, settingUiElement.GetComponent<Slider>().value);
                        break;
                    case UIElementType.Toggle:
                        PlayerPrefs.SetInt(config.label, settingUiElement.GetComponent<Toggle>().isOn ? 1 : 0);
                        break;
                }
            }
        }
        
        public void LoadSetting()
        {
            switch (config.type)
            {
                case UIElementType.Dropdown:
                    settingUiElement.GetComponent<TMP_Dropdown>().value = PlayerPrefs.GetInt(config.label);
                    break;
                case UIElementType.Slider:
                    settingUiElement.GetComponent<Slider>().value = PlayerPrefs.GetFloat(config.label);
                    break;
                case UIElementType.Toggle:
                    settingUiElement.GetComponent<Toggle>().isOn = PlayerPrefs.GetInt(config.label) != 0;
                    break;
            }
        }
    }

    [System.Serializable]
    public struct SettingsCategory
    {
        [SerializeField]
        public string name;

        [SerializeField]
        public SettingsObject[] settings;
    }
}
