using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public enum GhostType { None, Red, Green }

public class BlockPlacer : MonoBehaviour
{
    private BlocksRepo _repo;
    private int _xPrev = 0;
    private int _yPrev = 0;
    private int _zPrev = 0;
    private int _x = 0;
    private int _y = 0;
    private int _z = 0;
    private float _unit = 0.1f;
    private GameObject _shape;
    private Material _material;
    private bool _isEffect = false;
    private bool _canPlace = false;
    private AudioSource _as;

    public SteamVR_Action_Boolean Trigger;
    public SteamVR_Action_Boolean Squeeze;
    public GameObject Ghosts;
    public GameObject GreenGhost;
    public GameObject RedGhost;
    public BlockSelector Selector;

    public GameObject StartShape;
    public Material StartMaterial;

    void Start()
    {
        _repo = new BlocksRepo();
        // TODO: Seed block repo with saved user data?

        // Subscribe to trigger and squeezer events
        Trigger.onStateDown += TriggerPress;
        Squeeze.onStateDown += SqueezePress;
        Selector.onShapeChanged += SelectShape;
        Selector.onMaterialChanged += SelectMaterial;
        Selector.onEffectChanged += (isEffect) => _isEffect = isEffect;

        _as = GetComponent<AudioSource>();
        _shape = StartShape;
        _material = StartMaterial;
    }

    void Update()
    {
        // Get closest coordinates
        _x = Mathf.RoundToInt(transform.position.x / _unit);
        _y = Mathf.RoundToInt((transform.position.y - 0.5f * _unit) / _unit);
        _z = Mathf.RoundToInt(transform.position.z / _unit);

        if (_x != _xPrev || _y != _yPrev || _z != _zPrev)
        {
            GhostType ghostType = IsPositionValidAt(_x, _y, _z);
            _xPrev = _x;
            _yPrev = _y;
            _zPrev = _z;
            _canPlace = ghostType == GhostType.Green;
            ShowGhost(ghostType);
        }
    }
    private void TriggerPress(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        if (_canPlace)
        {
            var rot = GetClosestRotation();
            var go = Instantiate(_shape, GetPositionFromCoords(_x, _y, _z), rot);
            if (!_isEffect)
            {
                Renderer renderer = go.GetComponentInChildren<Renderer>();
                renderer.material = _material;
            }
            _repo.PlaceBlock(
                go, _shape.name,
                _material ? _material.name : "",
                _x, _y, _z,
                rot.w, rot.x, rot.y, rot.z);
        }
        else
        {
            if (_as != null) _as.PlayDelayed(0);
        }
    }
    private void SqueezePress(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        string coordStr = $"{_x},{_y},{_z}";
        if (_repo.HasOccupantAt(coordStr))
        {
            _repo.RemoveBlock(coordStr);
        }
    }
    private void SelectShape(GameObject shape)
    {
        _shape = shape;
    }
    private void SelectMaterial(Material material)
    {
        _material = material;
    }
    private void ShowGhost(GhostType ghostType)
    {
        GreenGhost.SetActive(ghostType == GhostType.Green);
        RedGhost.SetActive(ghostType == GhostType.Red);
        Vector3 position = GetPositionFromCoords(_x, _y, _z);
        Ghosts.transform.position = position;
    }
    private GhostType IsPositionValidAt(int x, int y, int z)
    {
        string coordStr = $"{x},{y},{z}";
        // If the spot is occupied, it's invalid (but can be deleted)
        if (_repo.HasOccupantAt(coordStr)) return GhostType.Red;
        // If the spot is on the ground, it's valid
        if (y == 0) return GhostType.Green;
        // If the spot is open but has a neighbor, it's valid
        if (HasNeighbor(x, y, z)) return GhostType.Green;
        // Otherwise, it's not valid
        return GhostType.None;
    }
    private bool HasNeighbor(int x, int y, int z)
    {
        if (_repo.HasOccupantAt($"{x + 1},{y},{z}")) return true;
        if (_repo.HasOccupantAt($"{x - 1},{y},{z}")) return true;
        if (_repo.HasOccupantAt($"{x},{y + 1},{z}")) return true;
        if (_repo.HasOccupantAt($"{x},{y - 1},{z}")) return true;
        if (_repo.HasOccupantAt($"{x},{y},{z + 1}")) return true;
        if (_repo.HasOccupantAt($"{x},{y},{z - 1}")) return true;
        return false;
    }
    private Vector3 GetPositionFromCoords(int x, int y, int z)
    {
        return new Vector3(
            x * _unit,
            y * _unit,
            z * _unit
        );
    }
    private Quaternion GetClosestRotation()
    {
        return new Quaternion(
            Mathf.RoundToInt(transform.rotation.x),
            Mathf.RoundToInt(transform.rotation.y),
            Mathf.RoundToInt(transform.rotation.z),
            Mathf.RoundToInt(transform.rotation.w)
        );
    }
}