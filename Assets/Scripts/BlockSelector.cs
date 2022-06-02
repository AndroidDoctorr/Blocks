using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public enum MenuMode { Blocks, Effects, Scenery }

public class BlockSelector : MonoBehaviour
{
    private bool _menuOpen = false;
    private GameObject _hand;
    private int _shapeIndex = 0;
    private int _materialIndex = 4;
    private int _effectIndex = 0;
    private int _sceneIndex = 0;
    private MenuMode _menuMode = MenuMode.Blocks;
    private AudioSource _as;

    public GameObject SelectedBlock;
    public GameObject Menu;
    public GameObject RightHand;
    public GameObject LeftHand;
    public TMP_Text MenuModeLabel;

    public SteamVR_Action_Boolean RightMenuToggle;
    public SteamVR_Action_Boolean RightTrackPadLeft;
    public SteamVR_Action_Boolean RightTrackPadRight;
    public SteamVR_Action_Boolean RightTrackPadUp;
    public SteamVR_Action_Boolean RightTrackPadDown;
    public SteamVR_Action_Boolean RightTrackPadCenter;

    public Material[] Materials;
    public GameObject[] Shapes;
    public GameObject[] Effects;
    public GameObject[] SceneryIcons;

    public delegate void OnShapeChanged(GameObject shape);
    public delegate void OnMaterialChanged(Material material);
    public delegate void OnEffectChanged(bool isEffect);
    public event OnShapeChanged onShapeChanged;
    public event OnMaterialChanged onMaterialChanged;
    public event OnEffectChanged onEffectChanged;
    public GameObject RedGhost;
    public GameObject GreenGhost;

    public static Dictionary<string, GameObject> ShapesReference = new Dictionary<string, GameObject>();
    public static Dictionary<string, Material> MaterialsReference = new Dictionary<string, Material>();

    void Start()
    {
        RightMenuToggle.onStateDown += ToggleMenuRight;
        RightTrackPadLeft.onStateDown += SelectNextLateral;
        RightTrackPadRight.onStateDown += SelectPrevLateral;
        RightTrackPadUp.onStateDown += SelectNextVertical;
        RightTrackPadDown.onStateDown += SelectPrevVertical;
        RightTrackPadCenter.onStateDown += SelectMenuMode;

        _as = GetComponent<AudioSource>();

        PopulateShapesReference();
        PopulateMaterialsReference();
    }
    void Update()
    {
        if (_menuOpen)
        {
            Quaternion handRot = _hand.transform.rotation;
            Vector3 offset = new Vector3(0, 0, 0.1f);
            Menu.transform.position = _hand.transform.position + handRot * offset;
            Menu.transform.rotation = handRot;
        }
    }

    private void ToggleMenuRight(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        _hand = RightHand;
        ToggleMenu(!_menuOpen);
    }
    private void ToggleMenu(bool isOpen)
    {
        _menuOpen = isOpen;
        Menu.SetActive(isOpen);
        if (!isOpen && RedGhost != null && GreenGhost != null)
        {
            RedGhost.SetActive(false);
            GreenGhost.SetActive(false);
        }
    }
    private void SelectMenuMode(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        if (!_menuOpen) return;
        if (_menuMode == MenuMode.Blocks)
            SetMenuMode(MenuMode.Effects);
        else if (_menuMode == MenuMode.Effects)
            SetMenuMode(MenuMode.Scenery);
        else if (_menuMode == MenuMode.Scenery)
            SetMenuMode(MenuMode.Blocks);

        PlaySound();
    }
    private void SetMenuMode(MenuMode mode)
    {
        _menuMode = mode;
        switch (_menuMode) {
            case MenuMode.Scenery:
                MenuModeLabel.text = "Scenery";
                SetScenery(_sceneIndex);
                break;
            case MenuMode.Effects:
                MenuModeLabel.text = "Effects";
                SetEffect(_effectIndex);
                break;
            case MenuMode.Blocks:
            default:
                SetShape(_shapeIndex);
                MenuModeLabel.text = "Blocks";
                SetMaterial(_materialIndex);
                break;
        }     
    }

    private void SelectNextLateral(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        if (!_menuOpen) return;
        switch (_menuMode)
        {
            case MenuMode.Scenery:
                SetScenery(++_sceneIndex);
                break;
            case MenuMode.Effects:
                SetEffect(++_effectIndex);
                break;
            case MenuMode.Blocks:
            default:
                SetMaterial(++_materialIndex);
                break;
        }

        PlaySound();
    }
    private void SelectPrevLateral(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        if (!_menuOpen) return;
        switch (_menuMode)
        {
            case MenuMode.Scenery:
                SetScenery(--_sceneIndex);
                break;
            case MenuMode.Effects:
                SetEffect(--_effectIndex);
                break;
            case MenuMode.Blocks:
            default:
                SetMaterial(--_materialIndex);
                break;
        }

        PlaySound();
    }
    private void SelectNextVertical(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        if (!_menuOpen) return;
        switch (_menuMode)
        {
            case MenuMode.Scenery:
                // TODO: Change the weather?
                // SetWeather(++_weatherIndex);
                break;
            case MenuMode.Effects:
                // No vertical change (yet?)
                break;
            case MenuMode.Blocks:
            default:
                SetShape(++_shapeIndex);
                break;
        }

        PlaySound();
    }
    private void SelectPrevVertical(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        if (!_menuOpen) return;
        switch (_menuMode)
        {
            case MenuMode.Scenery:
                // TODO: Change the weather?
                // SetWeather(--_weatherIndex);
                break;
            case MenuMode.Effects:
                // No vertical change (yet?)
                break;
            case MenuMode.Blocks:
            default:
                SetShape(--_shapeIndex);
                break;
        }

        PlaySound();
    }
    private void SetScenery(int newIndex)
    {
        int index = newIndex;
        if (newIndex >= SceneryIcons.Length) index = 0;
        else if (newIndex < 0) index = SceneryIcons.Length - 1;

        // TODO: Switch between background scenes
    }
    private void SetEffect(int newIndex)
    {
        int index = newIndex;
        if (newIndex >= Effects.Length) index = 0;
        else if (newIndex < 0) index = Effects.Length - 1;
        _effectIndex = index;

        GameObject effect = Effects[index];
        ReplaceSelected(effect, false);
        onShapeChanged(effect);
        onEffectChanged(true);
        onMaterialChanged(null);
    }
    private void SetShape(int newIndex)
    {
        int index = newIndex;
        if (newIndex >= Shapes.Length) index = 0;
        else if (newIndex < 0) index = Shapes.Length - 1;
        _shapeIndex = index;

        GameObject shape = Shapes[index];
        ReplaceSelected(shape, true);
        onShapeChanged(shape);
        onEffectChanged(false);
    }
    private void SetMaterial(int newIndex)
    {
        int index = newIndex;
        if (newIndex >= Materials.Length) index = 0;
        else if (newIndex < 0) index = Materials.Length - 1;
        _materialIndex = index;

        Material material = Materials[index];
        GameObject selectedShape = GetSelectedItem();
        Renderer currentShapeRenderer = selectedShape.GetComponentInChildren<Renderer>();
        currentShapeRenderer.material = material;
        onMaterialChanged(Materials[_materialIndex]);
    }
    private void SetMaterial(GameObject newObject)
    {
        Material material = Materials[_materialIndex];
        Renderer currentShapeRenderer = newObject.GetComponentInChildren<Renderer>();
        currentShapeRenderer.material = material;
        onMaterialChanged(Materials[_materialIndex]);
    }
    private void ReplaceSelected(GameObject newItem, bool setMaterial)
    {
        GameObject selectedShape = GetSelectedItem();
        selectedShape.transform.parent = null;
        Destroy(selectedShape);

        GameObject newInstance = Instantiate(newItem, SelectedBlock.transform);
        newInstance.name = "SelectedItem";
        newInstance.transform.parent = SelectedBlock.transform;
        if (setMaterial) SetMaterial(newInstance);
    }
    private GameObject GetSelectedItem()
    {
        return GameObject.Find("SelectedItem");
    }
    private void PlaySound()
    {
        if (_as != null) _as.PlayDelayed(0);
    }

    private void PopulateShapesReference()
    {
        foreach (GameObject shape in Shapes)
        {
            ShapesReference.Add(shape.name, shape);
        }
        foreach (GameObject effect in Effects)
        {
            ShapesReference.Add(effect.name, effect);
        }
    }
    private void PopulateMaterialsReference()
    {
        foreach (Material material in Materials)
        {
            MaterialsReference.Add(material.name, material);
        }
    }
}